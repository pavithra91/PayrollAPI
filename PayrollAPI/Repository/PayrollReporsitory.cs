using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data;
using Expression = org.matheval.Expression;
using Org.BouncyCastle.Ocsp;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Diagnostics;
using PdfSharp.Pdf.Security;
using PdfSharp.UniversalAccessibility.Drawing;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Drawing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Text;
using static LinqToDB.Common.Configuration;

namespace PayrollAPI.Repository
{
    public class Employee
    {
        //Members declaration
        public string epf { get; set; }
        public string empName { get; set; }
        public string companyCode { get; set; }
        public string location { get; set; }
        public string costCenter { get; set; }
        public string empGrade { get; set; }
        public string gradeCode { get; set; }
    }
    public class PayrollReporsitory : IPayroll
    {
        private readonly DBConnect _context;
        private readonly ILogger _logger;
        public PayrollReporsitory(DBConnect db, ILogger<PayrollReporsitory> logger)
        {
            _context = db;
            _logger = logger;
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();

            return transaction.GetDbTransaction();
        }

        public async Task<MsgDto> SimulatePayroll(ApprovalDto approvalDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();

            try
            {
                ICollection<Employee_Data> _emp = _context.Employee_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).OrderBy(o => o.epf).ToList();
                ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).ToList();
                ICollection<Calculation> _calculation = _context.Calculation.Where(o => o.companyCode == approvalDto.companyCode).ToList();

                // int _previousPeriod = GetPreviousPeriod(approvalDto.period.ToString());
                int _previousPeriod = GetPreviousPeriod(approvalDto.period.ToString());


                DateTime currentDate = DateTime.Now;
                DateTime previousMonth = currentDate.AddMonths(-1);

                _previousPeriod = Convert.ToInt32(previousMonth.ToString("yyyyMM"));


                var _summaryList = _context.GetSummaryDetails.FromSqlRaw("SELECT * FROM payrolldb.Payroll_Summary_View WHERE period= " + _previousPeriod + ";").ToList();

                Payrun? _payRun = _context.Payrun.Where(o => o.companyCode == approvalDto.companyCode && o.period == approvalDto.period).FirstOrDefault();

                if (_payRun == null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = $"No Data available for period - {approvalDto.period}. Payrun operation failed.";
                    return _msg;
                }
                else if (_payRun.payrunStatus == "Confirmed")
                {
                    decimal _grossTot = _payrollData.Where(o => o.payCategory == "0").Sum(w => w.amount);
                    decimal _epfTot = _payrollData.Where(o => o.epfConRate > 0).Sum(w => w.epfContribution);
                    decimal _taxTot = _payrollData.Where(o => o.taxConRate > 0 && o.paytype != 'A').Sum(w => w.taxContribution);

                    Dictionary<string, decimal> calCodeList = new Dictionary<string, decimal>();
                    calCodeList.Add("EPFTO", _epfTot);
                    calCodeList.Add("TGROS", _grossTot);

                    foreach (Calculation cal in _calculation)
                    {
                        Expression expression = new Expression();
                        expression.SetFomular(cal.calFormula);
                        List<String> variables = expression.getVariables();
                        foreach (String variable in variables)
                        {
                            var _val = calCodeList.Where(o => o.Key == variable).FirstOrDefault();
                            if (_val.Key != null)
                            {
                                expression.Bind(variable, _val.Value);
                            }
                            // Console.WriteLine(variable);
                        }

                        Decimal _result = expression.Eval<Decimal>();

                        calCodeList.Add(cal.calCode, _result);
                    };

                    calCodeList.Add("EMPCount", _emp.Count);

                    Dictionary<string, string> result = new Dictionary<string, string>();

                    double epfco = ((_summaryList[0].EPFCOM - Convert.ToDouble(calCodeList.Where(o => o.Key == "EPFCO").FirstOrDefault().Value)) / _summaryList[0].EPFCOM) * 100;
                    double gorss = ((_summaryList[0].Gross - Convert.ToDouble(calCodeList.Where(o => o.Key == "TGROS").FirstOrDefault().Value)) / _summaryList[0].Gross) * 100;
                    double epfem = ((_summaryList[0].EPFEMP - Convert.ToDouble(calCodeList.Where(o => o.Key == "EPFEM").FirstOrDefault().Value)) / _summaryList[0].EPFEMP) * 100;
                    double etf = ((_summaryList[0].ETF - Convert.ToDouble(calCodeList.Where(o => o.Key == "ETFCO").FirstOrDefault().Value)) / _summaryList[0].ETF) * 100;

                    DataTable dt = new DataTable();
                    dt.Columns.Add("ResultType");
                    dt.Columns.Add("Percentage");
                    dt.Columns.Add("CurrentValue");

                    dt.Rows.Add("Gross Amount", gorss, Convert.ToDouble(calCodeList.Where(o => o.Key == "TGROS").FirstOrDefault().Value));
                    dt.Rows.Add("EPF Employee Contribution", epfem, Convert.ToDouble(calCodeList.Where(o => o.Key == "EPFEM").FirstOrDefault().Value));
                    dt.Rows.Add("EPF Company Contribution", epfco, Convert.ToDouble(calCodeList.Where(o => o.Key == "EPFCO").FirstOrDefault().Value));
                    dt.Rows.Add("ETF Company Contribution", etf, Convert.ToDouble(calCodeList.Where(o => o.Key == "ETFCO").FirstOrDefault().Value));


                    _msg.MsgCode = 'S';
                    _msg.Data = JsonConvert.SerializeObject(dt);
                    _msg.Message = "Simulate Ready";
                    return _msg;

                }
                else
                {
                    _msg.MsgCode = 'S';
                    _msg.Message = "EPF/TAX Calculated Successfully";
                    return _msg;
                }
            }
            catch
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error";
                return _msg;
            }
        }
        public async Task<MsgDto> ProcessPayroll(ApprovalDto approvalDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();

            try
            {
                ICollection<Employee_Data> _emp = _context.Employee_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).OrderBy(o => o.epf).ToList();
                ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).ToList();
                ICollection<Calculation> _calculation = _context.Calculation.Where(o => o.companyCode == approvalDto.companyCode && o.status == true).ToList();
                ICollection<Tax_Calculation> _taxCalculation = _context.Tax_Calculation.Where(o => o.companyCode == approvalDto.companyCode && o.status == true && o.taxCategory == "IT").ToList();
                ICollection<Unrecovered> _unRecoveredList = _context.Unrecovered.Where(o => o.companyCode == approvalDto.companyCode).ToList();
                Payrun? _payRun = _context.Payrun.Where(o => o.companyCode == approvalDto.companyCode && o.period == approvalDto.period).FirstOrDefault();

                if (_payRun == null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = $"No Data available for period - {approvalDto.period}. Payrun operation failed.";
                    return _msg;
                }
                else if (_payRun.payrunStatus == "Confirmed")
                {
                    // Calculate EPF and Tax

                    //Parallel.ForEach(_emp, emp =>
                    foreach (Employee_Data emp in _emp)
                    {
                        ICollection<Payroll_Data> _empPayrollData = _payrollData.Where(o => o.epf == emp.epf).OrderBy(o => o.payCode).ToList();

                        decimal _grossTot = _empPayrollData.Where(o => o.payCategory == "0").Sum(w => w.amount);
                        decimal _epfTot = _empPayrollData.Where(o => o.epfConRate > 0).Sum(w => w.epfContribution);
                        decimal _taxTot = Math.Floor(_empPayrollData.Where(o => o.taxConRate > 0 && o.paytype != 'A').Sum(w => w.taxContribution));

                        Payroll_Data _tempEPFTOT = new Payroll_Data();
                        _tempEPFTOT.companyCode = emp.companyCode;
                        _tempEPFTOT.location = emp.location;
                        _tempEPFTOT.period = approvalDto.period;
                        _tempEPFTOT.epf = emp.epf;
                        _tempEPFTOT.othours = 0;
                        _tempEPFTOT.payCategory = "T";
                        _tempEPFTOT.payCode = 0;
                        _tempEPFTOT.calCode = "EPFTO";
                        _tempEPFTOT.paytype = null;
                        _tempEPFTOT.costCenter = emp.costCenter;
                        _tempEPFTOT.payCodeType = "T";
                        _tempEPFTOT.amount = _epfTot;
                        _tempEPFTOT.balanceAmount = 0;
                        _tempEPFTOT.displayOnPaySheet = false;
                        _tempEPFTOT.epfConRate = 0;
                        _tempEPFTOT.epfContribution = 0;
                        _tempEPFTOT.taxConRate = 0;
                        _tempEPFTOT.taxContribution = 0;

                        _empPayrollData.Add(_tempEPFTOT);
                        _context.Payroll_Data.Add(_tempEPFTOT);

                        Payroll_Data _tempGROSS = new Payroll_Data();
                        _tempGROSS.companyCode = emp.companyCode;
                        _tempGROSS.location = emp.location;
                        _tempGROSS.period = approvalDto.period;
                        _tempGROSS.epf = emp.epf;
                        _tempGROSS.othours = 0;
                        _tempGROSS.payCategory = "T";
                        _tempGROSS.payCode = 0;
                        _tempGROSS.calCode = "TGROS";
                        _tempGROSS.paytype = null;
                        _tempGROSS.costCenter = emp.costCenter;
                        _tempGROSS.payCodeType = "T";
                        _tempGROSS.amount = _grossTot;
                        _tempGROSS.balanceAmount = 0;
                        _tempGROSS.displayOnPaySheet = false;
                        _tempGROSS.epfConRate = 0;
                        _tempGROSS.epfContribution = 0;
                        _tempGROSS.taxConRate = 0;
                        _tempGROSS.taxContribution = 0;

                        _empPayrollData.Add(_tempGROSS);
                        //_context.Payroll_Data.Add(_tempEPFTOT);

                        Payroll_Data _objTAXGRO = new Payroll_Data();
                        _objTAXGRO.companyCode = emp.companyCode;
                        _objTAXGRO.location = emp.location;
                        _objTAXGRO.period = approvalDto.period;
                        _objTAXGRO.epf = emp.epf;
                        _objTAXGRO.othours = 0;
                        _objTAXGRO.payCategory = "T";
                        _objTAXGRO.payCode = 0;
                        _objTAXGRO.calCode = "TAXGR";
                        _objTAXGRO.paytype = null;
                        _objTAXGRO.costCenter = emp.costCenter;
                        _objTAXGRO.payCodeType = "T";
                        _objTAXGRO.amount = _taxTot;
                        _objTAXGRO.balanceAmount = 0;
                        _objTAXGRO.displayOnPaySheet = false;
                        _objTAXGRO.epfConRate = 0;
                        _objTAXGRO.epfContribution = 0;
                        _objTAXGRO.taxConRate = 0;
                        _objTAXGRO.taxContribution = 0;

                        _empPayrollData.Add(_objTAXGRO);
                        _context.Payroll_Data.Add(_objTAXGRO);

                        // Calculations
                        foreach (Calculation cal in _calculation)
                        {
                            Expression expression = new Expression();
                            expression.SetFomular(cal.calFormula);
                            List<String> variables = expression.getVariables();
                            foreach (String variable in variables)
                            {
                                var _val = _empPayrollData.Where(o => o.calCode == variable).FirstOrDefault();
                                if (_val != null)
                                {
                                    expression.Bind(variable, _val.amount);
                                }
                                // Console.WriteLine(variable);
                            }

                            Decimal _result = expression.Eval<Decimal>();

                            Payroll_Data _objEPFTOT = new Payroll_Data();
                            _objEPFTOT.companyCode = emp.companyCode;
                            _objEPFTOT.location = emp.location;
                            _objEPFTOT.period = approvalDto.period;
                            _objEPFTOT.epf = emp.epf;
                            _objEPFTOT.othours = 0;
                            _objEPFTOT.payCategory = cal.payCategory;
                            _objEPFTOT.payCode = cal.payCode;
                            _objEPFTOT.calCode = cal.calCode;
                            _objEPFTOT.paytype = null;
                            _objEPFTOT.costCenter = emp.costCenter;
                            _objEPFTOT.payCodeType = cal.contributor;
                            _objEPFTOT.amount = _result;
                            _objEPFTOT.balanceAmount = 0;
                            _objEPFTOT.displayOnPaySheet = true;
                            _objEPFTOT.epfConRate = 0;
                            _objEPFTOT.epfContribution = 0;
                            _objEPFTOT.taxConRate = 0;
                            _objEPFTOT.taxContribution = 0;

                            _empPayrollData.Add(_objEPFTOT);
                            _context.Payroll_Data.Add(_objEPFTOT);
                        };

                        // Tax Calculation
                        foreach (Tax_Calculation cal in _taxCalculation)
                        {
                            if (_taxTot > cal.range)
                            {
                                continue;
                            }
                            Expression expression = new Expression();
                            expression.SetFomular(cal.calFormula);
                            expression.Bind("TGROSS", _taxTot);
                            Decimal _taxResult = expression.Eval<Decimal>();

                            Payroll_Data _objTAXTOT = new Payroll_Data();
                            _objTAXTOT.companyCode = emp.companyCode;
                            _objTAXTOT.location = emp.location;
                            _objTAXTOT.period = approvalDto.period;
                            _objTAXTOT.epf = emp.epf;
                            _objTAXTOT.othours = 0;
                            _objTAXTOT.payCategory = "1";
                            _objTAXTOT.payCode = 328;
                            _objTAXTOT.calCode = "APTAX";
                            _objTAXTOT.paytype = null;
                            _objTAXTOT.costCenter = emp.costCenter;
                            _objTAXTOT.payCodeType = cal.contributor;
                            _objTAXTOT.amount = _taxResult;
                            _objTAXTOT.balanceAmount = 0;
                            _objTAXTOT.displayOnPaySheet = true;
                            _objTAXTOT.epfConRate = 0;
                            _objTAXTOT.epfContribution = 0;
                            _objTAXTOT.taxConRate = 0;
                            _objTAXTOT.taxContribution = 0;

                            _empPayrollData.Add(_objTAXTOT);
                            _context.Payroll_Data.Add(_objTAXTOT);
                            break;
                        }

                        decimal _grossDed = _empPayrollData.Where(o => o.payCategory == "1" && (o.payCodeType != "T" && o.payCodeType != "C")).Sum(w => w.amount);

                        EPF_ETF ePF_ETF = new EPF_ETF();
                        ePF_ETF.epf = emp.epf;
                        ePF_ETF.period = emp.period;
                        ePF_ETF.companyCode = emp.companyCode;
                        ePF_ETF.location = emp.location;
                        ePF_ETF.empName = emp.empName;
                        ePF_ETF.grade = emp.empGrade;
                        ePF_ETF.epfGross = _epfTot;
                        ePF_ETF.taxableGross = _taxTot;
                        ePF_ETF.grossAmount = _grossTot;
                        ePF_ETF.netAmount = _grossTot - _grossDed;
                        ePF_ETF.deductionGross = _grossDed;
                        ePF_ETF.emp_contribution = _empPayrollData.Where(x => x.calCode == "EPFEM").Select(x => x.amount).FirstOrDefault(0);
                        ePF_ETF.comp_contribution = _empPayrollData.Where(x => x.calCode == "EPFCO").Select(x => x.amount).FirstOrDefault(0);
                        ePF_ETF.etf = _empPayrollData.Where(x => x.calCode == "ETFCO").Select(x => x.amount).FirstOrDefault(0);
                        ePF_ETF.tax = _empPayrollData.Where(x => x.calCode == "APTAX").Select(x => x.amount).FirstOrDefault(0);
                        ePF_ETF.createdBy = approvalDto.approvedBy;

                        await _context.EPF_ETF.AddAsync(ePF_ETF);
                    };

                    _payRun.payrunBy = approvalDto.approvedBy;
                    _payRun.payrunStatus = "EPF/TAX Calculated";
                    _payRun.payrunDate = DateTime.Now;
                    _payRun.payrunTime = DateTime.Now;

                    await _context.SaveChangesAsync();


                    int _response = Calculate_lumpSumTax(approvalDto.companyCode, approvalDto.period, _payrollData, out string _responseMessage);

                    if(_response < 0)
                    {
                        transaction.Rollback();
                        _msg.MsgCode = 'E';
                        _msg.Message = _responseMessage;
                        return _msg;
                    } 
                    else if(_response == 0)
                    {
                        transaction.Rollback();
                        _msg.MsgCode = 'E';
                        _msg.Message = _responseMessage;
                        return _msg;
                    }

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    _msg.MsgCode = 'S';
                    _msg.Message = "EPF/TAX Calculated Successfully";
                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "EPF/TAX Already Calculated. Operation Failed!";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError($"process-payroll : {ex.Message}");
                _logger.LogError($"process-payroll : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> CreateUnrecoveredFile(ApprovalDto approvalDto)
        {
            // TODO : Check _payrun Status

            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                ICollection<Employee_Data> _emp = _context.Employee_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).OrderBy(o => o.epf).ToList();
                ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).ToList();
                ICollection<Unrecovered> _unRecoveredList = _context.Unrecovered.Where(o => o.companyCode == approvalDto.companyCode).ToList();
                ICollection<EPF_ETF> _epfETF = _context.EPF_ETF.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).ToList();
                Payrun? _payRun = _context.Payrun.Where(o => o.companyCode == approvalDto.companyCode && o.period == approvalDto.period).FirstOrDefault();

                if (_payRun == null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = $"No Data available for period - {approvalDto.period}. Payrun operation failed.";
                    return _msg;
                }
                else if (_payRun.payrunStatus == "EPF/TAX Calculated")
                {
                    foreach (Employee_Data emp in _emp)
                    {
                        ICollection<Payroll_Data> _empPayrollData = _payrollData.Where(o => o.epf == emp.epf).OrderBy(o => o.payCode).ToList();

                        decimal _grossTot = _empPayrollData.Where(o => o.payCategory == "0").Sum(w => w.amount);
                        decimal _grossDed = _empPayrollData.Where(o => o.payCategory == "1" && (o.payCodeType != "T" && o.payCodeType != "C")).Sum(w => w.amount);

                        if (_grossDed > _grossTot)
                        {
                            ICollection<Payroll_Data> _empDeductions = _empPayrollData.Where(o => o.payCategory == "1" && (o.payCodeType != "T" && o.payCodeType != "C")).OrderBy(o => o.payCode).ToList();
                            decimal unRecTotal = 0;
                            foreach (Payroll_Data deductionItem in _empDeductions)
                            {
                                decimal previousGross = _grossTot;

                                _grossTot -= deductionItem.amount;

                                if (_grossTot < 0)
                                {
                                    // Add to unrecovered List
                                    _grossTot = _grossTot * -1;
                                    Unrecovered _unRecoveredObj = new Unrecovered();
                                    _unRecoveredObj.companyCode = deductionItem.companyCode;
                                    _unRecoveredObj.location = deductionItem.location;
                                    _unRecoveredObj.period = deductionItem.period;
                                    _unRecoveredObj.epf = deductionItem.epf;
                                    _unRecoveredObj.payCategory = deductionItem.payCategory;
                                    _unRecoveredObj.payCode = deductionItem.payCode;
                                    _unRecoveredObj.calCode = deductionItem.calCode;
                                    _unRecoveredObj.costCenter = deductionItem.costCenter;
                                    _unRecoveredObj.amount = _grossTot;
                                    _context.Unrecovered.Add(_unRecoveredObj);

                                    unRecTotal += _grossTot;

                                    Payroll_Data _payItem = _empPayrollData.Where(o => o.id == deductionItem.id).FirstOrDefault();
                                    if (previousGross == 0)
                                    {
                                        previousGross = deductionItem.amount;
                                    }
                                    _payItem.amount = previousGross;
                                    _payItem.paytype = 'U';
                                    _context.Entry(_payItem).State = EntityState.Modified;
                                    _grossTot = 0;
                                }
                            }

                            EPF_ETF ePF_ETF = _epfETF.Where(o => o.epf == emp.epf).FirstOrDefault();
                            ePF_ETF.unRecoveredTotal = unRecTotal;
                            unRecTotal = 0;
                        }
                    }

                    _payRun.payrunBy = approvalDto.approvedBy;
                    _payRun.payrunStatus = "Unrec File Created";
                    _payRun.payrunDate = DateTime.Now;
                    _payRun.payrunTime = DateTime.Now;

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    _msg.MsgCode = 'S';
                    _msg.Message = "Unrecovered File Created Successfully";
                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "Unrecovered File Already Created. Operation Failed!";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError($"create-unrecovered : {ex.Message}");
                _logger.LogError($"create-unrecovered : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> ProcessPayrollbyEPF(string epf, int period, int companyCode)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                ICollection<Employee_Data> _emp = _context.Employee_Data.Where(o => o.period == period && o.companyCode == companyCode && o.epf == epf).OrderBy(o => o.epf).ToList();
                ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == period && o.companyCode == companyCode && o.epf == epf).ToList();
                ICollection<Calculation> _calculation = _context.Calculation.Where(o => o.companyCode == companyCode).ToList();
                ICollection<Tax_Calculation> _taxCalculation = _context.Tax_Calculation.Where(o => o.companyCode == companyCode && o.status == true && o.taxCategory == "IT").ToList();
                ICollection<Unrecovered> _unRecoveredList = _context.Unrecovered.Where(o => o.companyCode == companyCode).ToList();

                // Calculate EPF and Tax

                //Parallel.ForEach(_emp, emp =>
                foreach (Employee_Data emp in _emp)
                {
                    ICollection<Payroll_Data> _empPayrollData = _payrollData.Where(o => o.epf == emp.epf).OrderBy(o => o.payCode).ToList();

                    decimal _grossTot = _empPayrollData.Where(o => o.payCategory == "0").Sum(w => w.amount);
                    decimal _epfTot = _empPayrollData.Where(o => o.epfConRate > 0).Sum(w => w.epfContribution);
                    decimal _taxTot = _empPayrollData.Where(o => o.taxConRate > 0 && o.paytype != 'A').Sum(w => w.taxContribution);

                    Payroll_Data _tempEPFTOT = new Payroll_Data();
                    _tempEPFTOT.companyCode = emp.companyCode;
                    _tempEPFTOT.location = emp.location;
                    _tempEPFTOT.period = period;
                    _tempEPFTOT.epf = emp.epf;
                    _tempEPFTOT.othours = 0;
                    _tempEPFTOT.payCategory = "T";
                    _tempEPFTOT.payCode = 0;
                    _tempEPFTOT.calCode = "EPFTO";
                    _tempEPFTOT.paytype = null;
                    _tempEPFTOT.costCenter = emp.costCenter;
                    _tempEPFTOT.payCodeType = "T";
                    _tempEPFTOT.amount = _epfTot;
                    _tempEPFTOT.balanceAmount = 0;
                    _tempEPFTOT.displayOnPaySheet = false;
                    _tempEPFTOT.epfConRate = 0;
                    _tempEPFTOT.epfContribution = 0;
                    _tempEPFTOT.taxConRate = 0;
                    _tempEPFTOT.taxContribution = 0;

                    _empPayrollData.Add(_tempEPFTOT);
                    _context.Payroll_Data.Add(_tempEPFTOT);

                    Payroll_Data _tempGROSS = new Payroll_Data();
                    _tempGROSS.companyCode = emp.companyCode;
                    _tempGROSS.location = emp.location;
                    _tempGROSS.period = period;
                    _tempGROSS.epf = emp.epf;
                    _tempGROSS.othours = 0;
                    _tempGROSS.payCategory = "T";
                    _tempGROSS.payCode = 0;
                    _tempGROSS.calCode = "TGROS";
                    _tempGROSS.paytype = null;
                    _tempGROSS.costCenter = emp.costCenter;
                    _tempGROSS.payCodeType = "T";
                    _tempGROSS.amount = _grossTot;
                    _tempGROSS.balanceAmount = 0;
                    _tempGROSS.displayOnPaySheet = false;
                    _tempGROSS.epfConRate = 0;
                    _tempGROSS.epfContribution = 0;
                    _tempGROSS.taxConRate = 0;
                    _tempGROSS.taxContribution = 0;

                    _empPayrollData.Add(_tempGROSS);
                    //_context.Payroll_Data.Add(_tempEPFTOT);

                    Payroll_Data _objTAXGRO = new Payroll_Data();
                    _objTAXGRO.companyCode = emp.companyCode;
                    _objTAXGRO.location = emp.location;
                    _objTAXGRO.period = period;
                    _objTAXGRO.epf = emp.epf;
                    _objTAXGRO.othours = 0;
                    _objTAXGRO.payCategory = "T";
                    _objTAXGRO.payCode = 0;
                    _objTAXGRO.calCode = "TAXGR";
                    _objTAXGRO.paytype = null;
                    _objTAXGRO.costCenter = emp.costCenter;
                    _objTAXGRO.payCodeType = "T";
                    _objTAXGRO.amount = _taxTot;
                    _objTAXGRO.balanceAmount = 0;
                    _objTAXGRO.displayOnPaySheet = false;
                    _objTAXGRO.epfConRate = 0;
                    _objTAXGRO.epfContribution = 0;
                    _objTAXGRO.taxConRate = 0;
                    _objTAXGRO.taxContribution = 0;

                    _empPayrollData.Add(_objTAXGRO);
                    _context.Payroll_Data.Add(_objTAXGRO);

                    // Calculations
                    foreach (Calculation cal in _calculation)
                    {
                        Expression expression = new Expression();
                        expression.SetFomular(cal.calFormula);
                        List<String> variables = expression.getVariables();
                        foreach (String variable in variables)
                        {
                            var _val = _empPayrollData.Where(o => o.calCode == variable).FirstOrDefault();
                            if (_val != null)
                            {
                                expression.Bind(variable, _val.amount);
                            }
                            // Console.WriteLine(variable);
                        }

                        Decimal _result = expression.Eval<Decimal>();

                        Payroll_Data _objEPFTOT = new Payroll_Data();
                        _objEPFTOT.companyCode = emp.companyCode;
                        _objEPFTOT.location = emp.location;
                        _objEPFTOT.period = period;
                        _objEPFTOT.epf = emp.epf;
                        _objEPFTOT.othours = 0;
                        _objEPFTOT.payCategory = cal.payCategory;
                        _objEPFTOT.payCode = cal.payCode;
                        _objEPFTOT.calCode = cal.calCode;
                        _objEPFTOT.paytype = null;
                        _objEPFTOT.costCenter = emp.costCenter;
                        _objEPFTOT.payCodeType = cal.contributor;
                        _objEPFTOT.amount = _result;
                        _objEPFTOT.balanceAmount = 0;
                        _objEPFTOT.displayOnPaySheet = true;
                        _objEPFTOT.epfConRate = 0;
                        _objEPFTOT.epfContribution = 0;
                        _objEPFTOT.taxConRate = 0;
                        _objEPFTOT.taxContribution = 0;

                        _empPayrollData.Add(_objEPFTOT);
                        _context.Payroll_Data.Add(_objEPFTOT);
                    };

                    // Tax Calculation
                    foreach (Tax_Calculation cal in _taxCalculation)
                    {
                        if (_taxTot > cal.range)
                        {
                            continue;
                        }
                        Expression expression = new Expression();
                        expression.SetFomular(cal.calFormula);
                        expression.Bind("TGROSS", _taxTot);
                        Decimal _taxResult = expression.Eval<Decimal>();

                        Payroll_Data _objTAXTOT = new Payroll_Data();
                        _objTAXTOT.companyCode = emp.companyCode;
                        _objTAXTOT.location = emp.location;
                        _objTAXTOT.period = period;
                        _objTAXTOT.epf = emp.epf;
                        _objTAXTOT.othours = 0;
                        _objTAXTOT.payCategory = "1";
                        _objTAXTOT.payCode = 328;
                        _objTAXTOT.calCode = "APTAX";
                        _objTAXTOT.paytype = null;
                        _objTAXTOT.costCenter = emp.costCenter;
                        _objTAXTOT.payCodeType = cal.contributor;
                        _objTAXTOT.amount = _taxResult;
                        _objTAXTOT.balanceAmount = 0;
                        _objTAXTOT.displayOnPaySheet = true;
                        _objTAXTOT.epfConRate = 0;
                        _objTAXTOT.epfContribution = 0;
                        _objTAXTOT.taxConRate = 0;
                        _objTAXTOT.taxContribution = 0;

                        _empPayrollData.Add(_objTAXTOT);
                        _context.Payroll_Data.Add(_objTAXTOT);
                        break;
                    }

                    EPF_ETF ePF_ETF = new EPF_ETF();
                    ePF_ETF.epf = emp.epf;
                    ePF_ETF.period = emp.period;
                    ePF_ETF.companyCode = emp.companyCode;
                    ePF_ETF.location = emp.location;
                    ePF_ETF.empName = emp.empName;
                    ePF_ETF.grade = emp.empGrade;
                    ePF_ETF.epfGross = _epfTot;
                    ePF_ETF.taxableGross = _taxTot;
                    ePF_ETF.emp_contribution = _empPayrollData.Where(x => x.calCode == "EPFEM").Select(x => x.amount).FirstOrDefault(0);
                    ePF_ETF.comp_contribution = _empPayrollData.Where(x => x.calCode == "EPFCO").Select(x => x.amount).FirstOrDefault(0);
                    ePF_ETF.etf = _empPayrollData.Where(x => x.calCode == "ETFCO").Select(x => x.amount).FirstOrDefault(0);
                    ePF_ETF.tax = _empPayrollData.Where(x => x.calCode == "APTAX").Select(x => x.amount).FirstOrDefault(0);

                    await _context.EPF_ETF.AddAsync(ePF_ETF);

                    decimal _grossDed = _empPayrollData.Where(o => o.payCategory == "1" && (o.payCodeType != "T" && o.payCodeType != "C")).Sum(w => w.amount);

                    if (_grossDed > _grossTot)
                    {
                        ICollection<Payroll_Data> _empDeductions = _empPayrollData.Where(o => o.payCategory == "1" && (o.payCodeType != "T" && o.payCodeType != "C")).OrderBy(o => o.payCode).ToList();

                        foreach (Payroll_Data deductionItem in _empDeductions)
                        {
                            _grossTot -= deductionItem.amount;

                            if (_grossTot < 0)
                            {
                                // Add to unrecovered List
                                Unrecovered _unRecoveredObj = new Unrecovered();
                                _unRecoveredObj.companyCode = deductionItem.companyCode;
                                _unRecoveredObj.location = deductionItem.location;
                                _unRecoveredObj.period = deductionItem.period;
                                _unRecoveredObj.epf = deductionItem.epf;
                                _unRecoveredObj.payCategory = deductionItem.payCategory;
                                _unRecoveredObj.payCode = deductionItem.payCode;
                                _unRecoveredObj.calCode = deductionItem.calCode;
                                _unRecoveredObj.costCenter = deductionItem.costCenter;
                                _unRecoveredObj.amount = _grossTot;
                                _context.Unrecovered.Add(_unRecoveredObj);

                                Payroll_Data _payItem = _context.Payroll_Data.Where(o => o.id == deductionItem.id).FirstOrDefault();
                                _payItem.amount = deductionItem.amount + _grossTot;
                                _context.Attach(_payItem);
                                _context.Entry(_payItem).Property(p => p.amount).IsModified = true;
                                _grossTot = 0;
                            }
                        }
                    }
                };

                await _context.SaveChangesAsync();

                transaction.Commit();


                _msg.MsgCode = 'S';
                _msg.Message = "fv";
                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
        public async Task<MsgDto> GetPayrollSummary(int period, int companyCode)
        {
            MsgDto _msg = new MsgDto();
            var ePF_ETFs = await _context.EPF_ETF.Where(o => o.period == period && o.companyCode == companyCode).OrderBy(o => o.epf).Select(o => new
            {
                o.companyCode,
                o.location,
                o.period,
                o.epf,
                o.empName,
                o.grade,
                o.emp_contribution,
                o.comp_contribution,
                o.etf,
                o.epfGross,
                o.taxableGross,
                o.tax,
                o.lumpsumTax,
            }).ToListAsync();

            if (ePF_ETFs.Count > 0)
            {
                _msg.Data = JsonConvert.SerializeObject(ePF_ETFs);
                _msg.MsgCode = 'S';
                _msg.Message = "Payroll Summary Available";
                return _msg;
            }
            else
            {
                _msg.MsgCode = 'E';
                _msg.Message = "No Data Available";
                return _msg;
            }
        }

        public async Task<MsgDto> GetPaySheet(string epf, int period)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                PaySheet_Log _paysheetLog = _context.PaySheet_Log.Where(o => o.epf == epf && period == period).FirstOrDefault();
                
                ICollection<Payroll_Data> _payData = await _context.Payroll_Data.
                    Where(o => o.period == period && o.epf == epf).
                    OrderBy(o => o.epf).ToListAsync();

                ICollection<PayCode> _payCodes = await _context.PayCode.ToListAsync();

                ICollection<Payroll_Data> _earningData = _payData.Where(o => o.displayOnPaySheet == true && o.payCategory == "0").OrderBy(o => o.epf).ToList();

                var _earningDataResult = from payData in _earningData
                                         join payCode in _payCodes on payData.payCode equals payCode.payCode
                                         into Earnings
                                         where payData.epf == epf
                                         from defaultVal in Earnings.DefaultIfEmpty()
                                         orderby payData.payCode
                                         select new
                                         {
                                             id = defaultVal.id,
                                             name = defaultVal.description,
                                             payCode = payData.payCode,
                                             amount = payData.amount,
                                             calCode = payData.calCode,
                                         };

                ICollection<Payroll_Data> _deductionData = _payData.Where(o => o.displayOnPaySheet == true && o.payCategory == "1").OrderBy(o => o.epf).ToList();

                var _deductionDataResult = from payData in _deductionData
                                           join payCode in _payCodes on payData.payCode equals payCode.payCode
                                         into Deductions
                                           where payData.epf == epf && payData.payCode > 0
                                           from defaultVal in Deductions.DefaultIfEmpty()
                                           orderby payData.payCode
                                           select new
                                           {
                                               id = payData.id,
                                               name = defaultVal.description,
                                               payCode = payData.payCode,
                                               paytype = payData.paytype,
                                               amount = payData.amount,
                                               balanceAmount = payData.balanceAmount,
                                               calCode = payData.calCode,
                                           };

                var _empData = await _context.Employee_Data.
                               Where(o => o.period == period && o.epf == epf).
                               Select(e => new
                               {
                                   e.epf,
                                   e.empName,
                                   e.companyCode,
                                   e.location,
                                   e.costCenter,
                                   e.empGrade,
                                   e.gradeCode,
                               }).ToListAsync();

                var _salData = await _context.EPF_ETF.
                   Where(o => o.period == period && o.epf == epf).
                   Select(e => new
                   {
                       e.id,
                       e.epfGross,
                       e.taxableGross,
                       e.tax,
                       e.emp_contribution,
                       e.comp_contribution,
                       e.etf,
                       e.deductionGross,
                       e.unRecoveredTotal,
                       e.grossAmount,
                       e.netAmount,
                       e.lumpsumTax,
                   }).ToListAsync();

                var _unrecovered = await _context.Unrecovered.
                    Where(o => o.period == period && o.epf == epf).
                    Select(e => new
                    {
                        e.id,
                        e.payCode,
                        e.calCode,
                        e.amount
                    }).ToListAsync();

                var _loanDataResult = from payData in _deductionData
                                      join payCode in _payCodes on payData.payCode equals payCode.payCode
                                    into Deductions
                                      where payData.epf == epf && payData.payCode > 0 && payData.balanceAmount > 0
                                      from defaultVal in Deductions.DefaultIfEmpty()
                                      orderby payData.payCode
                                      select new
                                      {
                                          id = payData.id,
                                          name = defaultVal.description,
                                          payCode = payData.payCode,
                                          paytype = payData.paytype,
                                          balanceAmount = payData.balanceAmount,
                                          amount = payData.amount,
                                          calCode = payData.calCode,
                                      };

                DataTable dt = new DataTable();
                dt.Columns.Add("empData");
                dt.Columns.Add("salData");
                dt.Columns.Add("earningData");
                dt.Columns.Add("deductionData");
                dt.Columns.Add("unRecoveredData");
                dt.Columns.Add("loanData");
                dt.Rows.Add(JsonConvert.SerializeObject(_empData), JsonConvert.SerializeObject(_salData), JsonConvert.SerializeObject(_earningDataResult), JsonConvert.SerializeObject(_deductionDataResult), JsonConvert.SerializeObject(_unrecovered), JsonConvert.SerializeObject(_loanDataResult));

                _msg.Data = JsonConvert.SerializeObject(dt).Replace('/', ' ');

                EPF_ETF _epfData = await _context.EPF_ETF.
                    Where(o => o.period == period && o.epf == epf).FirstOrDefaultAsync();

                Employee_Data _selectedEmpData = _context.Employee_Data.
                        Where(o => o.period == period && o.epf == epf).FirstOrDefault();

                if(_paysheetLog != null && !_paysheetLog.isPaysheetUploaded)
                {
                    var pdfData = GeneratePayslipsForEmployee(_payData, _selectedEmpData, _epfData, period);

                    var fileName = $"{epf}.pdf";
                    await UploadPdfToS3(pdfData, fileName, period.ToString());
                }

                if (_paysheetLog != null && !_paysheetLog.isSMSSend)
                {
                    Sys_Properties sys_Properties = _context.Sys_Properties.Where(o => o.variable_name == "Send_SMS_PaySheet_View").FirstOrDefault();
                    Sys_Properties smsBody = _context.Sys_Properties.Where(o => o.variable_name == "SMS_Body").FirstOrDefault();
                    string _endPoint = "https://cpstl-poc-main-s3.s3.ap-southeast-1.amazonaws.com/public/" + period + "/" + _selectedEmpData.epf + ".pdf";

                    if (sys_Properties.variable_value == "True")
                    {
                        SMSSender sms = new SMSSender(_selectedEmpData.phoneNo, string.Format(smsBody.variable_value.Replace("{break}", "\n"), _selectedEmpData.epf, period) + _endPoint);
                        sms.sendSMS(sms);
                    }

                }

                _msg.MsgCode = 'S';
                _msg.Message = "Success";

                return _msg;
            }
            catch (Exception ex)
            {
                _logger.LogError($"get-paysheet : {ex.Message}");
                _logger.LogError($"get-paysheet : {ex.InnerException}");
                _msg.MsgCode = 'E';
                //_msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        private byte[] GeneratePayslipsForEmployee(ICollection<Payroll_Data> _payData, Employee_Data _empData, EPF_ETF _epfData, int period)
        {
            //Employee_Data _empData = _empData2.FirstOrDefault();

            /*ICollection<Payroll_Data> _payData = _context.Payroll_Data.
                    Where(o => o.period == period && o.epf == epf).
                    OrderBy(o => o.epf).ToList();*/

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
                                         id = defaultVal.id,
                                         name = defaultVal.description,
                                         payCode = payData.payCode,
                                         amount = payData.amount,
                                         othours = payData.othours,
                                         calCode = payData.calCode,
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
                                           id = payData.id,
                                           name = defaultVal.description,
                                           payCode = payData.payCode,
                                           paytype = payData.paytype,
                                           amount = payData.amount,
                                           balanceAmount = payData.balanceAmount,
                                           othours = payData.othours,
                                           calCode = payData.calCode,
                                       };


            PdfDocument document= new PdfDocument();
            PdfPage page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;
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
                if(item.othours > 0)
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

            foreach (var item in _deductionDataResult)
            {
                if (item.othours > 0)
                {
                    gfx.DrawString(item.othours.ToString("N"), normalFont, XBrushes.Black,
                        new XRect(50, y, 50, 0), XStringFormats.BaseLineRight);
                }

                if (item.balanceAmount > 0)
                {
                    gfx.DrawString(item.balanceAmount.ToString("N"), normalFont, XBrushes.Black,
                        new XRect(50, y, 50, 0), XStringFormats.BaseLineRight);
                    gfx.DrawString((item.balanceAmount - item.amount).ToString("N"), normalFont, XBrushes.Black,
                        new XRect(520, y, 50, 0), XStringFormats.BaseLineRight);
                }

                gfx.DrawString(item.payCode.ToString(), normalFont, XBrushes.Black,
                    new XRect(145, y, 50, 0));
                gfx.DrawString(item.name, normalFont, XBrushes.Black,
                    new XRect(175, y, 50, 0));

                if(item.paytype == 'U')
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
                new XRect(145, y + 8, 50, 0));
            gfx.DrawString(_epfData.deductionGross.ToString(), normalFont, XBrushes.Black,
                new XRect(400, y + 8, 50, 0));

            gfx.DrawLine(lineGrey, 130, lineY, 130, y + 15);
            gfx.DrawLine(lineGrey, 367, lineY, 367, y + 15);
            gfx.DrawLine(lineGrey, 490, lineY, 490, y + 15);

            gfx.DrawLine(lineGrey, 130, y, 490, y);
            gfx.DrawLine(lineGrey, 130, y+15, 490, y+15);

            y += 30;

            if(y > 700)
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
                new XRect(50, y + 8, 50, 0));
            gfx.DrawString(_epfData.netAmount.ToString("N"), normalFont, XBrushes.Black,
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
            document.SecuritySettings.UserPassword = _empData.epf;
            var securityHandler = document.SecurityHandler ?? NRT.ThrowOnNull<PdfStandardSecurityHandler>();
            securityHandler.SetEncryptionToV5();


            MemoryStream stream = new MemoryStream();
            document.Save(stream, true);
            //document.Save(_empData.epf + ".pdf");     

            return stream.ToArray();
        }

        private XGraphics DrawHeader(XGraphics gfx, double x, double y, Employee_Data _empData, XFont normalFont, XPen pen, int period)
        {
            //var imagePath = Path.Combine("/app", "logo.jpg");
            var imagePath = Path.Combine("C:\\Users\\17532\\source\\repos\\pavithra91\\PayrollAPI\\PayrollAPI\\logo.jpg");

            XImage image = XImage.FromFile(imagePath);

            gfx.DrawImage(image, 90, 10, 50, 50);

            XFont headerFont = new XFont("courier", 14);

            gfx.DrawString("Ceylon Petroleum Storage Terminals Limited", headerFont, XBrushes.Black,
                new XRect(150, 30, 150, 0));

            gfx.DrawRectangle(pen, new XRect(40, 88, 540, 20));

            gfx.DrawString(_empData.epf, normalFont, XBrushes.Black,
                new XRect(x + 50, y, 50, 0));
            gfx.DrawString(GetPeriod(period.ToString()), normalFont, XBrushes.Black,
                new XRect(x + 100, y, 50, 0));
            gfx.DrawString(_empData.empName, normalFont, XBrushes.Black,
                new XRect(x + 200, y, 150, 0));

            string _paymentType = "BANK";
            if (_empData.paymentType == 1)
            {
                _paymentType = "CASH";
            }

            gfx.DrawString(_paymentType, normalFont, XBrushes.Black,
                new XRect(x + 450, y, 50, 0));

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
        public async Task UploadPdfToS3(byte[] pdfData, string fileName, string period)
        {
            var uploader = new S3Uploader("AKIAV3CJE2DCBB7UZJDM", "oPvNVvN3U5e+MZwtmRK8/X+5kLDxNzXsCubr1XbT", "cpstl-poc-main-s3", Amazon.RegionEndpoint.APSoutheast1);
            await uploader.UploadFileAsync(pdfData, "public/" + period + "/" + fileName);
        }

        public async Task<MsgDto> PrintPaySheets(int companyCode, int period)
        {
            MsgDto _msg = new MsgDto();
            var objectsToSave = new List<PaySheet_Log>();

            Sys_Properties sys_Properties = _context.Sys_Properties.Where(o => o.variable_name == "Send_SMS_PaySheet_View").FirstOrDefault();
            Sys_Properties smsBody = _context.Sys_Properties.Where(o => o.variable_name == "SMS_Body").FirstOrDefault();

            try
            {
                ICollection<Payroll_Data> _payData = await _context.Payroll_Data.
                    Where(o => o.period == period && o.companyCode == companyCode).
                    OrderBy(o => o.epf).ToListAsync();

                ICollection<Employee_Data> _empData = await _context.Employee_Data.
                    Where(o => o.period == period && o.companyCode == companyCode).
                    OrderBy(o => o.epf).ToListAsync();

                ICollection<EPF_ETF> _epfData = await _context.EPF_ETF.
                    Where(o => o.period == period && o.companyCode == companyCode).
                    OrderBy(o => o.epf).ToListAsync();

                ICollection<Unrecovered> _unrecoveredData = await _context.Unrecovered.
                    Where(o => o.period == period && o.companyCode == companyCode).
                    OrderBy(o => o.epf).ToListAsync();


                ICollection<PayCode> _payCodes = await _context.PayCode.ToListAsync();

                ICollection<Payroll_Data> _earningData = _payData.Where(o => o.displayOnPaySheet == true && o.payCategory == "0").OrderBy(o => o.epf).ToList();
                ICollection<Payroll_Data> _deductionData = _payData.Where(o => o.displayOnPaySheet == true && o.payCategory == "1").OrderBy(o => o.epf).ToList();

                ICollection<PaySheet_Log> _paysheetLog = _context.PaySheet_Log.Where(o => o.companyCode == companyCode && o.period == period).OrderBy(o => o.epf).ToList();

                //DataTable dt = new DataTable();
                //dt.Columns.Add("empData");
                //dt.Columns.Add("salData");
                //dt.Columns.Add("earningData");
                //dt.Columns.Add("deductionData");
                //dt.Columns.Add("unRecoveredData");
                //dt.Columns.Add("loanData");


                var paysheetLogEpfs = _paysheetLog.Select(p => p.epf).ToHashSet();
                _empData = _empData.Where(e => !paysheetLogEpfs.Contains(e.epf)).ToList();

                int count = 0;


                foreach (Employee_Data emp in _empData)
                {

                    ICollection<Payroll_Data> _payData2 = _payData.Where(o => o.epf == emp.epf).ToList();
                    EPF_ETF _epfData2 = _epfData.Where(o => o.epf == emp.epf).FirstOrDefault();

                    /*var _earningDataResult = from payData in _earningData
                                             join payCode in _payCodes on payData.payCode equals payCode.payCode
                                             into Earnings
                                             where payData.epf == emp.epf
                                             from defaultVal in Earnings.DefaultIfEmpty()
                                             orderby payData.payCode
                                             select new
                                             {
                                                 name = defaultVal.description,
                                                 payCode = payData.payCode,
                                                 amount = payData.amount,
                                                 calCode = payData.calCode,
                                             };

                    var _deductionDataResult = from payData in _deductionData
                                               join payCode in _payCodes on payData.payCode equals payCode.payCode
                                             into Deductions
                                               where payData.epf == emp.epf && payData.payCode > 0
                                               from defaultVal in Deductions.DefaultIfEmpty()
                                               orderby payData.payCode
                                               select new
                                               {
                                                   name = defaultVal.description,
                                                   payCode = payData.payCode,
                                                   paytype = payData.paytype,
                                                   amount = payData.amount,
                                                   balanceAmount = payData.balanceAmount,
                                                   calCode = payData.calCode,
                                               };

                    var _loanData = from payData in _deductionData
                                    join payCode in _payCodes on payData.payCode equals payCode.payCode
                                  into Loans
                                    where payData.epf == emp.epf && payData.payCode > 0 && payData.balanceAmount > 0
                                    from defaultVal in Loans.DefaultIfEmpty()
                                    orderby payData.payCode
                                    select new
                                    {
                                        name = defaultVal.description,
                                        payCode = payData.payCode,
                                        balance = payData.balanceAmount,
                                        amount = payData.amount,
                                        calCode = payData.calCode,
                                    };

                    var _empDisplayData = _empData.
                               Where(o => o.period == period && o.epf == emp.epf).
                               Select(e => new
                               {
                                   e.epf,
                                   e.empName,
                                   e.companyCode,
                                   e.location,
                                   e.costCenter,
                                   e.empGrade,
                                   e.gradeCode,
                               });


                    var _salData = _epfData.
                   Where(o => o.period == period && o.epf == emp.epf).
                   Select(e => new
                   {
                       e.epfGross,
                       e.taxableGross,
                       e.tax,
                       e.emp_contribution,
                       e.comp_contribution,
                       e.etf,
                       e.deductionGross,
                       e.unRecoveredTotal,
                       e.grossAmount,
                       e.netAmount,
                       e.lumpsumTax,
                   });

                    var _unRecData = _unrecoveredData.
                    Where(o => o.period == period && o.epf == emp.epf).
                    Select(e => new
                    {
                        e?.payCode,
                        e?.calCode,
                        e?.amount
                    });


                    dt.Rows.Add(JsonConvert.SerializeObject(_empDisplayData), JsonConvert.SerializeObject(_salData), JsonConvert.SerializeObject(_earningDataResult), JsonConvert.SerializeObject(_deductionDataResult), JsonConvert.SerializeObject(_unRecData), JsonConvert.SerializeObject(_loanData));
                    */

                    var _objLog = new PaySheet_Log
                    {
                        epf = emp.epf,
                        period = period,
                        companyCode = companyCode,        
                    };

                    var pdfData = GeneratePayslipsForEmployee(_payData2, emp, _epfData2, period);
                    var fileName = $"{emp.epf}.pdf";
                    await UploadPdfToS3(pdfData, fileName, period.ToString());

                    string _endPoint = "https://cpstl-poc-main-s3.s3.ap-southeast-1.amazonaws.com/public/" + period + "/" + emp.epf + ".pdf";
                    if (sys_Properties.variable_value == "True")
                    {
                        if (emp.phoneNo != null)
                        {
                            SMSSender sms = new SMSSender(emp.phoneNo, string.Format(smsBody.variable_value.Replace("{break}", "\n"), emp.epf, period) + _endPoint);
                            sms.sendSMS(sms);
                            _objLog.isSMSSend = true;
                        }
                        else
                        {
                            _objLog.isSMSSend= false;
                            _objLog.message = "Employee Phone Number not found";
                        }
                    }

                    count++;

                    _objLog.isPaysheetUploaded = true;
                    objectsToSave.Add(_objLog);

                    if (count >= 3)
                    {
                        //_msg.Data = JsonConvert.SerializeObject(dt).Replace('/', ' ');
                        _context.PaySheet_Log.AddRange(objectsToSave);
                        _context.SaveChanges();
                        _msg.MsgCode = 'S';
                        _msg.Message = "Success";
                        return _msg;
                    }
                }

                //_msg.Data = JsonConvert.SerializeObject(dt).Replace('/', ' ');
                _context.PaySheet_Log.AddRange(objectsToSave);
                _context.SaveChanges();
                _msg.MsgCode = 'S';
                _msg.Message = "Success";
                return _msg;
            }
            catch (Exception ex)
            {
                _context.PaySheet_Log.AddRange(objectsToSave);
                _context.SaveChanges();
                _logger.LogError($"print-paysheet : {ex.Message}");
                _logger.LogError($"print-paysheet : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetPayrunDetails()
        {
            MsgDto _msg = new MsgDto();
            try
            {
                ICollection<Payrun> _payRun = await _context.Payrun.
                    OrderByDescending(o => o.period).ToListAsync();

                _msg.MsgCode = 'S';
                _msg.Data = JsonConvert.SerializeObject(_payRun);
                _msg.Message = "Success";
                return _msg;
            }
            catch (Exception ex)
            {
                _logger.LogError($"get-payrun : {ex.Message}");
                _logger.LogError($"get-payrun : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> GetPayrunDetails(int period, int CompanyCode)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                ICollection<Payrun> _payRun = await _context.Payrun.
                    Where(o => o.period == period && o.companyCode == CompanyCode).
                    ToListAsync();

                _msg.MsgCode = 'S';
                _msg.Data = JsonConvert.SerializeObject(_payRun);
                _msg.Message = "Success";
                return _msg;
            }
            catch (Exception ex)
            {
                _logger.LogError($"get-payrun-by-period : {ex.Message}");
                _logger.LogError($"get-payrun-by-period : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> Writeback(int period, int companyCode)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                ICollection<Payroll_Data> _payData = await _context.Payroll_Data.Where(o => o.period == period && o.companyCode == companyCode).
                    ToListAsync();
                ICollection<Unrecovered> _unrecovered = await _context.Unrecovered.Where(o => o.period == period && o.companyCode == companyCode).
                    ToListAsync();

                foreach (var item in _unrecovered)
                {
                    _payData.Add(new Payroll_Data
                    {
                        companyCode = item.companyCode,
                        location = item.location,
                        period = item.period,
                        epf = item.epf,
                        othours = 0,
                        payCategory = item.payCategory,
                        payCode = item.payCode,
                        calCode = item.calCode,
                        costCenter = item.costCenter,
                        payCodeType = "U",
                        amount = item.amount,
                        balanceAmount = 0
                    });
                }

                _msg.MsgCode = 'S';
                _msg.Data = JsonConvert.SerializeObject(_payData.OrderBy(o => o.epf));
                _msg.Message = "Success";
                return _msg;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Writeback : {ex.Message}");
                _logger.LogError($"Writeback : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> CreateBankFile(int period, int companyCode)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                ICollection<EPF_ETF> _payData = _context.EPF_ETF.Where(o=>o.companyCode == companyCode && o.period == period).ToList();
                ICollection<Employee_Data> _empData = _context.Employee_Data.Where(o => o.companyCode == companyCode && o.period == period).ToList();
                ICollection<Sys_Properties> _sysProperties = _context.Sys_Properties.Where(o => o.category_name == "System_Variable").ToList();

                var itemList = from epf_etf in _payData
                               join empData in _empData on epf_etf.epf equals empData.epf
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
                string _comp_BankCode = _sysProperties.Where(o => o.variable_name == "Bank_Code").FirstOrDefault().variable_value;
                string _comp_Branch_Code = _sysProperties.Where(o => o.variable_name == "Branch_Code").FirstOrDefault().variable_value;
                string _comp_Account_No = _sysProperties.Where(o => o.variable_name == "Account_No").FirstOrDefault().variable_value;
                string _comp_Account_Name = _sysProperties.Where(o => o.variable_name == "Account_Name").FirstOrDefault().variable_value;

                foreach (var item in itemList)
                {
                    string amount = item.netSalary.ToString().Replace(".", "");
                    amount.Count();

                    DateTime now = DateTime.Now;
                    string date = now.ToString("yyMMdd");

                    formattedString += string.Format(
                    "{0,4}{1,4}{2:3}{3,-12}{4,-20}23{5,21}SLR{6,4}{7:3}{8, -12}{9, -50}{10, 6}{11,6}\n",
                    "0000", item.bankCode, item.branchCode, item.accountNo.PadLeft(12,'0'), item.name.Trim(), amount.PadLeft(21, '0'), _comp_BankCode, _comp_Branch_Code, _comp_Account_No.PadLeft(12, '0'), _comp_Account_Name, date, "000000");
                }

                File.WriteAllText("output.txt", formattedString);

                _msg.MsgCode = 'S';
               // _msg.Data = JsonConvert.SerializeObject(_payData.OrderBy(o => o.epf));
                _msg.Message = "Success";
                return _msg;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Writeback : {ex.Message}");
                _logger.LogError($"Writeback : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        private int GetPreviousPeriod(string period)
        {
            string month = period[^2..];
            string year = period[0..4];
            int _month = Convert.ToInt32(month);
            int _year = Convert.ToInt32(year);

            if (_month == 1)
            {
                _month = 12;
                _year -= 1;
                return Convert.ToInt32(_year.ToString() + _month.ToString());
            }
            else
            {
                _month -= 1;
                return Convert.ToInt32(_year.ToString() + _month.ToString());
            }
        }

        private int Calculate_lumpSumTax(int companyCode, int period, ICollection<Payroll_Data> _payrollData, out string _responseMessage)
        {
            _responseMessage = "";
            string _epf = "";

            try
            {
                ICollection<Tax_Calculation> _taxCalculation = _context.Tax_Calculation.Where(o => o.companyCode == companyCode && o.status == true && o.taxCategory == "LT").ToList();
                //ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.companyCode == companyCode && o.period == period).ToList();
                ICollection<EPF_ETF> _epfETF = _context.EPF_ETF.Where(o => o.period == period && o.companyCode == companyCode).ToList();
                var _taxYear = _context.Sys_Properties.Where(o => o.category_name == "System_Variable").Select(s => new { s.variable_name, s.variable_value }).ToList();
                var _lumpsumPayCodes = _context.Sys_Properties.Where(o => o.variable_name == "Lump_Sum_PayCode").Select(s => new { s.variable_value }).ToList();

                List<int> allowedPayCodes = new List<int>();
                int taxYearFrom = Convert.ToInt32(_taxYear.Where(o => o.variable_name == "YearFrom").Select(s => s.variable_value).FirstOrDefault());
                int taxYearTo = Convert.ToInt32(_taxYear.Where(o => o.variable_name == "YearTo").Select(s => s.variable_value).FirstOrDefault());


                if ((period - taxYearFrom) < 0)
                {
                    _responseMessage = "Tax Year has already closed";
                    return -1;
                }

                foreach (var item in _lumpsumPayCodes)
                {
                    allowedPayCodes.Add(Convert.ToInt32(item.variable_value));
                }

                var _payItem = _payrollData.Where(o => o.paytype == 'A').ToList();

                var _records = _payrollData.Where(o => allowedPayCodes.Contains(o.payCode) && o.paytype != 'A')
                                           .GroupBy(employee => employee.epf)
                                           .Select(group => new { EPF = group.Key, Amount = group.Sum(e => e.amount) })
                                           .ToList();

                var _epfetf = _context.EPF_ETF.Where(o => o.period >= taxYearFrom && o.period <= taxYearTo).Select(o => new
                {
                    o.epf,
                    o.taxableGross,
                    o.tax,
                    o.lumpsumTax,
                    o.lumpSumGross,
                }).ToList();

                foreach (var item in _records)
                {
                    _epf = item.EPF;
                    if(_epf == "16715")
                    {

                    }
                    decimal _arriesSum = _payItem.Where(o => o.epf == item.EPF).Sum(s => s.amount);
                    int count = _epfetf.Where(o => o.epf == item.EPF).Count();
                    decimal _pTaxableGross = Convert.ToDecimal(_epfetf.Where(o => o.epf == item.EPF).Sum(s => s.taxableGross));
                    decimal _pTax = Convert.ToDecimal(_epfetf.Where(o => o.epf == item.EPF).Sum(s => s.tax));
                    decimal _pLumsumpTax = Convert.ToDecimal(_epfetf.Where(o => o.epf == item.EPF).Sum(s => s.lumpsumTax));
                    decimal _previousLumpsumGross = Convert.ToDecimal(_epfetf.Where(o => o.epf == item.EPF).Sum(s => s.lumpSumGross));

                    if(count > 0)
                    {
                        _pTaxableGross = (_pTaxableGross) / count;
                        _pTax = (_pTax) / count;
                    }

                    decimal A = (decimal)(_pTax) * 12;

                    decimal D = (decimal)(_pTaxableGross) * 12;
                    decimal _lumpSumGross = Math.Floor(item.Amount) + Math.Floor(_arriesSum);
                    D = D + _lumpSumGross + _previousLumpsumGross;
                    A = A + _pLumsumpTax;

                    foreach (Tax_Calculation cal in _taxCalculation)
                    {
                        if (D > cal.range)
                        {
                            continue;
                        }
                        Expression expression = new Expression();
                        expression.SetFomular(cal.calFormula);
                        expression.Bind("D", D);
                        expression.Bind("A", A);
                        Decimal _taxResult = expression.Eval<Decimal>();

                        if (_taxResult < 0)
                        {
                            _lumpSumGross = 0;
                            _taxResult = 0;
                        }

                        EPF_ETF ePF_ETF = _epfETF.Where(o => o.epf == item.EPF && o.period == period && o.companyCode == companyCode).FirstOrDefault();
                        ePF_ETF.lumpsumTax = _taxResult;
                        ePF_ETF.netAmount = ePF_ETF.netAmount - _taxResult;
                        ePF_ETF.lumpSumGross = _lumpSumGross;
                        ePF_ETF.deductionGross = ePF_ETF.deductionGross + _taxResult;

                        Payroll_Data emp = _payrollData.Where(o => o.epf == item.EPF).FirstOrDefault();

                        Payroll_Data _objTAXTOT = new Payroll_Data();
                        _objTAXTOT.companyCode = companyCode;
                        _objTAXTOT.location = emp.location;
                        _objTAXTOT.period = period;
                        _objTAXTOT.epf = emp.epf;
                        _objTAXTOT.othours = 0;
                        _objTAXTOT.payCategory = "1";
                        _objTAXTOT.payCode = 328;
                        _objTAXTOT.calCode = "LUMTX";
                        _objTAXTOT.paytype = null;
                        _objTAXTOT.costCenter = emp.costCenter;
                        _objTAXTOT.payCodeType = cal.contributor;
                        _objTAXTOT.amount = _taxResult;
                        _objTAXTOT.balanceAmount = 0;
                        _objTAXTOT.displayOnPaySheet = true;
                        _objTAXTOT.epfConRate = 0;
                        _objTAXTOT.epfContribution = 0;
                        _objTAXTOT.taxConRate = 0;
                        _objTAXTOT.taxContribution = 0;

                        _context.Payroll_Data.Add(_objTAXTOT);

                        break;
                    }
                }
                _responseMessage = "Lump-Sum Tax Calculated Successfully";
                return 1;
            }
            catch(Exception ex)
            {
                _responseMessage = $"{ex.Message} : Error record {_epf}";
                return 0;
            }
        }
    
        private string GetPeriod(string period)
        {
            int year = int.Parse(period.Substring(0, 4));
            int month = int.Parse(period.Substring(4, 2));
            DateTime date = new DateTime(year, month, 1);

            // Format the DateTime as "YYYY MMMM"
            return date.ToString("MMM yyyy").ToUpper();
        }
    }
}
