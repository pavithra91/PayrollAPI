using Microsoft.EntityFrameworkCore;
using PayrollAPI.DataModel;
using PayrollAPI.Models;
using PdfSharp.Diagnostics;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Pdf.Security;
using PdfSharp.Pdf;
using PayrollAPI.Data;
using PayrollAPI.Models.Payroll;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using Org.BouncyCastle.Ocsp;
using System.Linq;

namespace PayrollAPI.Services
{
    public class PaysheetPrint
    {
        private readonly DBConnect _context;
        
        public PaysheetPrint(DBConnect context)
        {
            _context = context;
        }

        public async Task<MsgDto> PrintPaySheets(int companyCode, int period, string approvedBy)
        {
            MsgDto _msg = new MsgDto();
            Common com = new Common();
            BackgroudJobs bj = new BackgroudJobs
            {
                companyCode = companyCode,
                period = period,
                createdBy = approvedBy,
                createdDate = com.GetTimeZone().Date,
                createdTime = com.GetTimeZone(),
                backgroudJobStatus = "Backgroud Job Started"
            };

            var objectsToSave = new List<PaySheet_Log>();


            Sys_Properties sys_Properties = _context.Sys_Properties.Where(o => o.variable_name == "Send_SMS_PaySheet_View").FirstOrDefault();
            IEnumerable<Sys_Properties> sms_configurations = _context.Sys_Properties.Where(o => o.groupName == "SMS").ToList();
            IEnumerable<Sys_Properties> companyBankDetails = _context.Sys_Properties.Where(o => o.groupName == "Company_Bank_Details").ToList();
            Sys_Properties smsBody = _context.Sys_Properties.Where(o => o.variable_name == "SMS_Body").FirstOrDefault();
            Payrun? _payRun = _context.Payrun.Where(o => o.companyCode == companyCode && o.period == period).FirstOrDefault();

            try
            {
                ICollection<Payroll_Data> _payData = _context.Payroll_Data.
                    Where(o => o.period == period && o.companyCode == companyCode).
                    OrderBy(o => o.epf).ToList();

                ICollection<Employee_Data> _empData = _context.Employee_Data.
                    Where(o => o.period == period && o.companyCode == companyCode && o.status == true).
                    OrderBy(o => o.epf).ToList();

                ICollection<EPF_ETF> _epfData = _context.EPF_ETF.
                    Where(o => o.period == period && o.companyCode == companyCode).
                    OrderBy(o => o.epf).ToList();

                ICollection<Unrecovered> _unrecoveredData = _context.Unrecovered.
                    Where(o => o.period == period && o.companyCode == companyCode).
                    OrderBy(o => o.epf).ToList();


                ICollection<PayCode> _payCodes = _context.PayCode.ToList();

                ICollection<Payroll_Data> _earningData = _payData.Where(o => o.displayOnPaySheet == true && o.payCategory == "0").OrderBy(o => o.epf).ToList();
                ICollection<Payroll_Data> _deductionData = _payData.Where(o => o.displayOnPaySheet == true && o.payCategory == "1").OrderBy(o => o.epf).ToList();

                ICollection<PaySheet_Log> _paysheetLog = _context.PaySheet_Log.Where(o => o.companyCode == companyCode && o.period == period).OrderBy(o => o.epf).ToList();

                var paysheetLogEpfs = _paysheetLog.Select(p => p.epf).ToHashSet();
                _empData = _empData.Where(e => !paysheetLogEpfs.Contains(e.epf)).ToList();

                int _paysheetCounter = 10000;
                Random random = new Random();
                int randomNumber = random.Next(1, 11);

                int count = 0;

                foreach (var emp in _empData)
                {
                    count++;
                    var _objLog = new PaySheet_Log
                    {
                        epf = emp.epf,
                        period = period,
                        companyCode = companyCode,
                    };

                    try
                    {
                        _paysheetCounter += randomNumber;
                        _objLog.paysheetID = _paysheetCounter.ToString();

                        ICollection<Payroll_Data> _SelectedPayData = _payData.Where(o => o.epf == emp.epf).ToList();
                        EPF_ETF _selectedEPFData = _epfData.Where(o => o.epf == emp.epf).FirstOrDefault();

                        var pdfData = GeneratePayslipsForEmployee(_SelectedPayData, emp, _selectedEPFData, period);

                        var fileName = $"{_paysheetCounter}.pdf";

                        string _endPoint = "https://cpstl-poc-main-s3.s3.ap-southeast-1.amazonaws.com/public/" + period + "/" + _paysheetCounter + ".pdf";

                        com.UploadPdfToS3(pdfData, fileName, period.ToString());

                        if (sys_Properties?.variable_value == "True")
                        {
                            if (emp.phoneNo != null)
                            {
                                SMSSender sms = new SMSSender(emp.phoneNo, string.Format(smsBody.variable_value.Replace("{break}", "\n"), period, _endPoint));
                                bool respose = await sms.sendSMS(sms);
                                Thread.Sleep(300);
                                _objLog.isSMSSend = respose;
                            }
                            else
                            {
                                _objLog.isSMSSend = false;
                                _objLog.message = "Employee Phone Number not found";
                            }
                        }

                        _objLog.paysheetID = fileName;
                        _objLog.isPaysheetUploaded = true;
                        objectsToSave.Add(_objLog);
                    }
                    catch (Exception ex)
                    {
                        _objLog.message = ex.Message;
                        objectsToSave.Add(_objLog);
                        continue;
                    }
                }

                _context.PaySheet_Log.AddRange(objectsToSave);

                if (_payRun != null)
                {
                    _payRun.bankFileCreatedBy = approvedBy;
                    _payRun.bankFileCreatedDate = com.GetTimeZone().Date;
                    _payRun.bankFileCreatedTime = com.GetTimeZone();

                    _context.Entry(_payRun).State = EntityState.Modified;
                }

                _context.SaveChanges();


                var itemList = from epf_etf in _epfData
                               join empData in _empData on epf_etf.epf equals empData.epf
                               where empData.status == true
                               orderby epf_etf.epf
                               select new
                               {
                                   epf = epf_etf.epf,
                                   name = epf_etf.empName,
                                   netSalary = epf_etf.netAmount.ToString(),
                                   bankCode = empData.bankCode.ToString(),
                                   branchCode = empData.branchCode.ToString(),
                                   accountNo = empData.accountNo.ToString(),
                               };
                string formattedString = "";

                string _comp_BankCode = companyBankDetails.Where(o => o.variable_name == "Bank_Code").FirstOrDefault().variable_value;
                string _comp_Branch_Code = companyBankDetails.Where(o => o.variable_name == "Branch_Code").FirstOrDefault().variable_value;
                string _comp_Account_No = companyBankDetails.Where(o => o.variable_name == "Account_No").FirstOrDefault().variable_value;
                string _comp_Account_Name = companyBankDetails.Where(o => o.variable_name == "Account_Name").FirstOrDefault().variable_value;
                
                foreach (var item in itemList)
                {
                    string amount = item.netSalary.ToString().Replace(".", "");
                    amount.Count();
                    DateTime now = DateTime.Now;
                    string date = now.ToString("yyMMdd");
                    formattedString += string.Format(
                    "{0,4}{1,4}{2:3}{3,-12}{4,-20}23{5,21}SLR{6,4}{7:3}{8, -12}{9, -50}{10, 6}{11,6}\n",
                    "0000", item.bankCode, item.branchCode, item.accountNo.PadLeft(12, '0'), item.name.Trim(), amount.PadLeft(21, '0'), _comp_BankCode, _comp_Branch_Code, _comp_Account_No.PadLeft(12, '0'), _comp_Account_Name, date, "000000");
                }

                byte[] array = Encoding.UTF8.GetBytes(formattedString);
                using var memoryStream = new MemoryStream(array);
                memoryStream.Seek(0, SeekOrigin.Begin);

                IEnumerable<Sys_Properties> email_configurations = _context.Sys_Properties.Where(o => o.groupName == "Email" || o.groupName == "Email_Config").ToList();

                EmailSender email = new EmailSender();
                bool status = await email.SendEmail(email_configurations, memoryStream.ToArray(), period + "-CPSTL", "txt");

                if (status)
                {
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "Fail to send Email";
                }

                bj.backgroudJobStatus = "Task Completed";
                bj.finishedTime = com.GetTimeZone();

                _context.BackgroudJobs.Add(bj);
                _context.SaveChangesAsync();

                return _msg;
            }
            catch (Exception ex)
            {
                _context.BackgroudJobs.Add(bj);
                _context.SaveChangesAsync();

                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> ResendPaySheets(int companyCode, int period, string approvedBy)
        {
            MsgDto _msg = new MsgDto();
            Common com = new Common();

            Payrun? _payRun = _context.Payrun.Where(o => o.companyCode == companyCode && o.period == period).FirstOrDefault();

            if (_payRun.payrunStatus != "Bank TR Compelete") {
                _msg.MsgCode = 'E';
                _msg.Message = "Payrun status not in Bank TR Compelete";
                return _msg;
            }
            else
            {
                BackgroudJobs bj = new BackgroudJobs
                {
                    companyCode = companyCode,
                    period = period,
                    createdBy = approvedBy,
                    createdDate = com.GetTimeZone().Date,
                    createdTime = com.GetTimeZone(),
                    backgroudJobStatus = "Paysheet Resend Backgroud Job Started"
                };

                Sys_Properties sys_Properties = _context.Sys_Properties.Where(o => o.variable_name == "Send_SMS_PaySheet_View").FirstOrDefault();
                IEnumerable<Sys_Properties> sms_configurations = _context.Sys_Properties.Where(o => o.groupName == "SMS").ToList();
                IEnumerable<Sys_Properties> companyBankDetails = _context.Sys_Properties.Where(o => o.groupName == "Company_Bank_Details").ToList();
                Sys_Properties smsBody = _context.Sys_Properties.Where(o => o.variable_name == "SMS_Body").FirstOrDefault();

                IEnumerable<PaySheet_Log> resendList = _context.PaySheet_Log.Where(x => x.isSMSSend == false && x.message == null).ToList();

                foreach (var paySheet_Log in resendList)
                {
                    var employee_Data = _context.Employee_Data.Where(x => x.epf == paySheet_Log.epf).FirstOrDefault();

                    if(employee_Data != null)
                    {

                        string _endPoint = "https://cpstl-poc-main-s3.s3.ap-southeast-1.amazonaws.com/public/" + period + "/" + paySheet_Log.paysheetID;

                        if (employee_Data.phoneNo != null)
                        {
                            SMSSender sms = new SMSSender(employee_Data.phoneNo, string.Format(smsBody.variable_value.Replace("{break}", "\n"), period, _endPoint));
                            bool respose = await sms.sendSMS(sms);
                            //Thread.Sleep(300);
                            paySheet_Log.isSMSSend = respose;
                        }
                    }
                }

                bj.backgroudJobStatus = "Task Completed";
                bj.finishedTime = com.GetTimeZone();

                _context.BackgroudJobs.Add(bj);

                _context.SaveChanges();

                _msg.MsgCode = 'S';
                _msg.Message = "Success";
                return _msg;
            }
        }

        private byte[] GeneratePayslipsForEmployee(ICollection<Payroll_Data> _payData, Employee_Data _empData, EPF_ETF _epfData, int period)
        {
            try
            {
                ICollection<PayCode> _payCodes = _context.PayCode.ToList();

                ICollection<Payroll_Data> _earningData = _payData.Where(o => o.displayOnPaySheet == true && o.payCategory == "0").OrderBy(o => o.epf).ToList();

                var _earningDataResult = from payData in _earningData
                                         join payCode in _payCodes on payData.payCode equals payCode.payCode
                                         into Earnings
                                         where payData.epf == _empData.epf
                                         from defaultVal in Earnings.DefaultIfEmpty()
                                         orderby payData.payCode
                                         select new
                                         {
                                             defaultVal.id,
                                             name = defaultVal.description,
                                             payData.payCode,
                                             payData.amount,
                                             payData.othours,
                                             payData.calCode,
                                         };

                ICollection<Payroll_Data> _deductionData = _payData.Where(o => o.displayOnPaySheet == true && o.payCategory == "1").OrderBy(o => o.epf).ToList();

                var _deductionDataResult = from payData in _deductionData
                                           join payCode in _payCodes on payData.payCode equals payCode.payCode
                                         into Deductions
                                           where payData.epf == _empData.epf && payData.payCode > 0
                                           from defaultVal in Deductions.DefaultIfEmpty()
                                           orderby payData.payCode
                                           select new
                                           {
                                               payData.id,
                                               name = defaultVal.description,
                                               payData.payCode,
                                               payData.paytype,
                                               payData.amount,
                                               payData.balanceAmount,
                                               payData.othours,
                                               payData.calCode,
                                               payData.payCodeType,
                                           };

                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;

                //XGraphics watermark = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Prepend);

                XGraphics gfx = XGraphics.FromPdfPage(page);
                XPen pen = new XPen(XColors.Black);
                XPen lineGrey = new XPen(XColors.Gray);

                var width = page.Width.Millimeter;
                var height = page.Height.Millimeter;

                XFont normalFont = new XFont("courier", 10);

                double y = 100;
                double x = 0;
                double lineY = 120;
                double lineX = 140;

                gfx = DrawHeader(gfx, x, y, _empData, normalFont, pen, period);

                y = 150;

                foreach (var item in _earningDataResult)
                {
                    if (item.othours > 0)
                    {
                        gfx.DrawString(item.othours.ToString("N") + "*", normalFont, XBrushes.Black,
                            new XRect(50, y, 50, 0), XStringFormats.BaseLineRight);
                    }

                    gfx.DrawString(item.payCode.ToString(), normalFont, XBrushes.Black,
                        new XRect(145, y, 50, 0));
                    gfx.DrawString(item.name, normalFont, XBrushes.Black,
                        new XRect(175, y, 50, 0));

                    gfx.DrawString(item.amount.ToString("N"), normalFont, XBrushes.Black,
                        new XRect(400, y, 50, 0), XStringFormats.BaseLineRight);

                    y += 15;
                }

                gfx.DrawString("GROSS PAY", normalFont, XBrushes.Black,
                    new XRect(145, y + 8, 50, 0));
                gfx.DrawString(_epfData.grossAmount.ToString("N"), normalFont, XBrushes.Black,
                    new XRect(400, y + 8, 50, 0));

                y += 30;

                int count = 0;

                foreach (var item in _deductionDataResult)
                {
                    if (count == 15)
                    {

                    }
                    count++;

                    if (item.othours > 0)
                    {
                        gfx.DrawString(item.othours.ToString("N"), normalFont, XBrushes.Black,
                            new XRect(50, y, 50, 0), XStringFormats.BaseLineRight);
                    }

                    if (item.balanceAmount > 0)
                    {
                        gfx.DrawString(item.balanceAmount.ToString("N"), normalFont, XBrushes.Black,
                            new XRect(50, y, 50, 0), XStringFormats.BaseLineRight);
                        if (item.payCodeType == "P")
                        {
                            gfx.DrawString((item.balanceAmount + item.amount).ToString("N"), normalFont, XBrushes.Black,
                                new XRect(520, y, 50, 0), XStringFormats.BaseLineRight);
                        }
                        else
                        {
                            gfx.DrawString((item.balanceAmount - item.amount).ToString("N"), normalFont, XBrushes.Black,
                                new XRect(520, y, 50, 0), XStringFormats.BaseLineRight);
                        }
                    }

                    gfx.DrawString(item.payCode.ToString(), normalFont, XBrushes.Black,
                        new XRect(145, y, 50, 0));
                    gfx.DrawString(item.name, normalFont, XBrushes.Black,
                        new XRect(175, y, 50, 0));

                    if (item.paytype == 'U')
                    {
                        gfx.DrawString(item.amount.ToString("N"), normalFont, XBrushes.Red,
                            new XRect(400, y, 50, 0), XStringFormats.BaseLineRight);
                    }
                    else
                    {
                        gfx.DrawString(item.amount.ToString("N"), normalFont, XBrushes.Black,
                            new XRect(400, y, 50, 0), XStringFormats.BaseLineRight);
                    }

                    y += 15;

                    if (y > 700)
                    {
                        gfx.DrawLine(lineGrey, 130, lineY, 130, y + 15);
                        gfx.DrawLine(lineGrey, 367, lineY, 367, y + 15);
                        gfx.DrawLine(lineGrey, 490, lineY, 490, y + 15);

                        //gfx.DrawLine(lineGrey, 130, y, 490, y);
                        gfx.DrawLine(lineGrey, 130, y + 15, 490, y + 15);

                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        gfx = DrawHeader(gfx, 0, 100, _empData, normalFont, pen, period);

                        y = 150;
                    }
                }

                gfx.DrawString("DEDUCTIONS", normalFont, XBrushes.Black,
                    new XRect(145, y + 10, 50, 0));
                gfx.DrawString(_epfData.deductionGross.GetValueOrDefault().ToString("N"), normalFont, XBrushes.Black,
                    new XRect(400, y + 10, 50, 0));

                gfx.DrawLine(lineGrey, 130, lineY, 130, y + 15);
                gfx.DrawLine(lineGrey, 367, lineY, 367, y + 15);
                gfx.DrawLine(lineGrey, 490, lineY, 490, y + 15);

                gfx.DrawLine(lineGrey, 130, y, 490, y);
                gfx.DrawLine(lineGrey, 130, y + 15, 490, y + 15);

                y += 30;

                if (y > 700)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    gfx = DrawHeader(gfx, 0, 100, _empData, normalFont, pen, period);
                    y = 150;
                }
                // else
                // {
                //      y = 750;
                // }

                gfx.DrawRectangle(lineGrey, new XRect(130, y, 100, 30));
                gfx.DrawString("NET PAY", normalFont, XBrushes.Black,
                    new XRect(50, y + 10, 50, 0));

                decimal netPay = _epfData.netAmount;

                if (netPay < 0)
                {
                    netPay = 0;
                }

                gfx.DrawString(netPay.ToString("N"), normalFont, XBrushes.Black,
                    new XRect(145, y + 15, 50, 0));


                gfx.DrawRectangle(lineGrey, new XRect(240, y, 250, 30));
                gfx.DrawString("EPF CORP.", normalFont, XBrushes.Black,
                    new XRect(245, y + 12, 50, 0));
                gfx.DrawString("CONTRIBUTION", normalFont, XBrushes.Black,
                    new XRect(245, y + 24, 50, 0));

                gfx.DrawString(_epfData.comp_contribution.ToString("N"), normalFont, XBrushes.Black,
                    new XRect(250, y + 46, 50, 0));

                gfx.DrawLine(lineGrey, 325, y, 325, y + 60);

                gfx.DrawString("EPF TOTAL", normalFont, XBrushes.Black,
                    new XRect(330, y + 12, 50, 0));
                gfx.DrawString("CONTRIBUTION", normalFont, XBrushes.Black,
                    new XRect(330, y + 24, 50, 0));

                gfx.DrawString((_epfData.emp_contribution + _epfData.comp_contribution).ToString("N"), normalFont, XBrushes.Black,
                    new XRect(335, y + 46, 50, 0));

                gfx.DrawLine(lineGrey, 410, y, 410, y + 60);

                gfx.DrawString("ETF", normalFont, XBrushes.Black,
                    new XRect(415, y + 12, 50, 0));
                gfx.DrawString("CONTRIBUTION", normalFont, XBrushes.Black,
                    new XRect(415, y + 24, 50, 0));

                gfx.DrawString(_epfData.etf.ToString("N"), normalFont, XBrushes.Black,
                    new XRect(420, y + 46, 50, 0));

                gfx.DrawRectangle(lineGrey, new XRect(240, y + 30, 250, 30));

                y += 76;
                double tempY = y + 60;

                if (y > 700)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    gfx = DrawHeader(gfx, 0, 100, _empData, normalFont, pen, period);
                    y = 150;
                }
                else if (tempY > 725)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    gfx = DrawHeader(gfx, 0, 100, _empData, normalFont, pen, period);
                    y = 150;
                }

                XTextFormatter tf = new XTextFormatter(gfx);
                tf.Alignment = XParagraphAlignment.Left;

                tf.DrawString("DISCLAIMER", normalFont, XBrushes.Black,
                    new XRect(94, y, 380, 50), XStringFormats.TopLeft);
                tf.DrawString("\nThis Document contains sensitive information that is intended solely for the recipient named. \nThis is an automatically generated payslip.", normalFont, XBrushes.Black,
                    new XRect(100, y, 380, 120), XStringFormats.TopLeft);

                tf.DrawString("\n*\n\n*", normalFont, XBrushes.Black,
                    new XRect(94, y, 10, 50), XStringFormats.TopLeft);

                //  gfx.DrawString("width : " + width + " height : " + height, normalFont, XBrushes.Black,
                //       new XRect(200, 650, 50, 0));

                //  y = 0;
                //  for(int i= 0; i <=11; i++)
                //   {
                //     gfx.DrawString((i+1).ToString(), normalFont, XBrushes.Black,
                //     new XRect(y, 800, 10, 0));

                //      y += 50;
                //   }

                // Encryption         
                if (_empData.accountNo != null && _empData.accountNo != "")
                {
                    string last4Digits = _empData.accountNo.Trim().Substring(_empData.accountNo.Length - 4);
                    document.SecuritySettings.UserPassword = _empData.epf.ToString() + last4Digits;
                }
                else
                {
                    document.SecuritySettings.UserPassword = _empData.epf;
                }
                document.SecuritySettings.OwnerPassword = "manthan";
                var securityHandler = document.SecurityHandler ?? NRT.ThrowOnNull<PdfStandardSecurityHandler>();
                securityHandler.SetEncryptionToV5();


                MemoryStream stream = new MemoryStream();
                document.Save(stream, true);
                //document.Save("202407/" + _empData.epf + ".pdf");     

                return stream.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private XGraphics DrawHeader(XGraphics gfx, double x, double y, Employee_Data _empData, XFont normalFont, XPen pen, int period)
        {
            try
            {
                Common com = new Common();
                //var imagePath = Path.Combine("/app", "logo.jpg");
                var imagePath = Path.Combine("C:\\Users\\17532\\source\\repos\\pavithra91\\PayrollAPI\\PayrollAPI\\logo.jpg");

                //var watermarkImagePath = Path.Combine("C:\\Users\\Pavithra\\source\\repos\\pavithra91\\PayrollAPI\\PayrollAPI\\Draft.png");
                //var watermarkImagePath = Path.Combine("/app", "Draft.png");
                //XImage watermarkImage = XImage.FromFile(watermarkImagePath);
                //gfx.DrawImage(watermarkImage, 50, 130, 500, 500);

                XImage image = XImage.FromFile(imagePath);

                gfx.DrawImage(image, 90, 10, 50, 50);

                XFont headerFont = new XFont("courier", 14);

                gfx.DrawString("CEYLON PETROLEUM STORAGE TERMINALS LIMITED", headerFont, XBrushes.Black,
                    new XRect(150, 30, 150, 0));

                gfx.DrawString("EMPLOYEE PAY SHEET", headerFont, XBrushes.Black,
                    new XRect(250, 50, 150, 0));

                gfx.DrawRectangle(pen, new XRect(40, 88, 540, 20));

                gfx.DrawString("EPF : ", normalFont, XBrushes.Black,
                    new XRect(x + 50, y, 50, 0));
                gfx.DrawString(_empData.epf, normalFont, XBrushes.Black,
                    new XRect(x + 85, y, 50, 0));
                gfx.DrawString(com.GetPeriod(period.ToString()), normalFont, XBrushes.Black,
                    new XRect(x + 150, y, 50, 0));
                gfx.DrawString(_empData.empName, normalFont, XBrushes.Black,
                    new XRect(x + 250, y, 150, 0));

                string _paymentType = "BANK";
                if (_empData.paymentType == 1)
                {
                    _paymentType = "CASH";
                }

                gfx.DrawString(_paymentType, normalFont, XBrushes.Black,
                    new XRect(x + 430, y, 50, 0));

                gfx.DrawString("GRADE :", normalFont, XBrushes.Black,
                    new XRect(x + 495, y, 50, 0));

                gfx.DrawString(_empData.empGrade.ToString(), normalFont, XBrushes.Black,
                    new XRect(x + 550, y, 50, 0));

                gfx.DrawString("Opening Balance", normalFont, XBrushes.Black,
                    new XRect(x + 30, y + 30, 50, 0));
                gfx.DrawString("Pay Code", normalFont, XBrushes.Black,
                    new XRect(x + 150, y + 30, 50, 0));
                gfx.DrawString("Earnings/Deductions", normalFont, XBrushes.Black,
                    new XRect(x + 370, y + 30, 50, 0));
                gfx.DrawString("Closing Balance", normalFont, XBrushes.Black,
                    new XRect(x + 495, y + 30, 50, 0));

                return gfx;
            }
            catch (Exception ex)
            {
                return gfx;
            }
        }

    }
}
