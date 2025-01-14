using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data;
using Expression = org.matheval.Expression;
using PdfSharp.Diagnostics;
using PdfSharp.Pdf.Security;
using PdfSharp.Drawing.Layout;
using PayrollAPI.Services;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using LinqToDB.Tools;
using PayrollAPI.Interfaces.Payroll;
using PayrollAPI.Models.Payroll;

namespace PayrollAPI.Repository.Payroll
{
    public class PayrollReporsitory : IPayroll
    {
        private readonly DBConnect _context;
        private readonly ILogger _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        Common com = new Common();
        public PayrollReporsitory(DBConnect db, ILogger<PayrollReporsitory> logger, IBackgroundTaskQueue taskQueue)
        {
            _context = db;
            _taskQueue = taskQueue;
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


                DateTime currentDate = com.GetTimeZone().Date;
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
                        List<string> variables = expression.getVariables();
                        foreach (string variable in variables)
                        {
                            var _val = calCodeList.Where(o => o.Key == variable).FirstOrDefault();
                            if (_val.Key != null)
                            {
                                expression.Bind(variable, _val.Value);
                            }
                        }

                        decimal _result = expression.Eval<decimal>();

                        calCodeList.Add(cal.calCode, _result);
                    };

                    calCodeList.Add("EMPCount", _emp.Count);

                    Dictionary<string, string> result = new Dictionary<string, string>();

                    double epfco = (_summaryList[0].EPFCOM - Convert.ToDouble(calCodeList.Where(o => o.Key == "EPFCO").FirstOrDefault().Value)) / _summaryList[0].EPFCOM * 100;
                    double gorss = (_summaryList[0].Gross - Convert.ToDouble(calCodeList.Where(o => o.Key == "TGROS").FirstOrDefault().Value)) / _summaryList[0].Gross * 100;
                    double epfem = (_summaryList[0].EPFEMP - Convert.ToDouble(calCodeList.Where(o => o.Key == "EPFEM").FirstOrDefault().Value)) / _summaryList[0].EPFEMP * 100;
                    double etf = (_summaryList[0].ETF - Convert.ToDouble(calCodeList.Where(o => o.Key == "ETFCO").FirstOrDefault().Value)) / _summaryList[0].ETF * 100;

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
                    _msg.Message = "Simulation Ready";
                    return _msg;

                }
                else
                {
                    _msg.MsgCode = 'S';
                    _msg.Message = "EPF/TAX Calculated Successfully";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(0, $"Error in Simulate Payroll : " + ex.Message);
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

                    foreach (Employee_Data emp in _emp)
                    {
                        ICollection<Payroll_Data> _empPayrollData = _payrollData.Where(o => o.epf == emp.epf).OrderBy(o => o.payCode).ToList();
                        var _lumpsumPayCodes = _context.Sys_Properties.Where(o => o.variable_name == "Lump_Sum_PayCode").Select(s => Convert.ToInt32(s.variable_value)).ToList();

                        decimal _grossTot = _empPayrollData.Where(o => o.payCategory == "0").Sum(w => w.amount);
                        decimal _epfTot = _empPayrollData.Where(o => o.epfConRate > 0).Sum(w => w.epfContribution);
                        decimal _taxTot = Math.Floor(_empPayrollData.Where(o => o.taxConRate > 0 && o.paytype != 'A' && !_lumpsumPayCodes.Contains(o.payCode)).Sum(w => w.taxContribution));

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
                            List<string> variables = expression.getVariables();
                            foreach (string variable in variables)
                            {
                                var _val = _empPayrollData.Where(o => o.calCode == variable).FirstOrDefault();
                                if (_val != null)
                                {
                                    expression.Bind(variable, _val.amount);
                                }
                                // Console.WriteLine(variable);
                            }

                            decimal _result = expression.Eval<decimal>();

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
                            decimal _taxResult = expression.Eval<decimal>();

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

                        decimal _grossDed = _empPayrollData.Where(o => o.payCategory == "1" && o.payCodeType != "T" && o.payCodeType != "C").Sum(w => w.amount);

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
                    _payRun.payrunDate = com.GetTimeZone().Date;
                    _payRun.payrunTime = com.GetTimeZone();

                    await _context.SaveChangesAsync();


                    int _response = Calculate_lumpSumTax(approvalDto.companyCode, approvalDto.period, _payrollData, out string _responseMessage);

                    if (_response < 0)
                    {
                        transaction.Rollback();
                        _msg.MsgCode = 'E';
                        _msg.Message = _responseMessage;
                        return _msg;
                    }
                    else if (_response == 0)
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
                        decimal _grossDed = _empPayrollData.Where(o => o.payCategory == "1" && o.payCodeType != "T" && o.payCodeType != "C").Sum(w => w.amount);

                        if (_grossDed > _grossTot)
                        {
                            ICollection<Payroll_Data> _empDeductions = _empPayrollData.Where(o => o.payCategory == "1" && o.payCodeType != "T" && o.payCodeType != "C").OrderBy(o => o.payCode).ToList();
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
                    _payRun.payrunDate = com.GetTimeZone().Date;
                    _payRun.payrunTime = com.GetTimeZone();

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
            
            DataTable dt = new DataTable();

            try
            {
                Employee_Data? _selectedEmpData = _context.Employee_Data.
        Where(o => o.period == period && o.epf == epf).FirstOrDefault();

                if (_selectedEmpData == null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Employee Found";

                    return _msg;
                }

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
                                             defaultVal.id,
                                             name = defaultVal.description,
                                             payData.payCode,
                                             payData.amount,
                                             payData.calCode,
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
                                               payData.id,
                                               name = defaultVal.description,
                                               payData.payCode,
                                               payData.paytype,
                                               payData.amount,
                                               payData.balanceAmount,
                                               payData.calCode,
                                               payData.payCodeType,
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
                                          payData.id,
                                          name = defaultVal.description,
                                          payData.payCode,
                                          payData.paytype,
                                          payData.balanceAmount,
                                          payData.amount,
                                          payData.calCode,
                                      };


                dt.Columns.Add("empData");
                dt.Columns.Add("salData");
                dt.Columns.Add("earningData");
                dt.Columns.Add("deductionData");
                dt.Columns.Add("unRecoveredData");
                dt.Columns.Add("loanData");
                dt.Rows.Add(JsonConvert.SerializeObject(_empData), JsonConvert.SerializeObject(_salData), JsonConvert.SerializeObject(_earningDataResult), JsonConvert.SerializeObject(_deductionDataResult), JsonConvert.SerializeObject(_unrecovered), JsonConvert.SerializeObject(_loanDataResult));

                _msg.Data = JsonConvert.SerializeObject(dt).Replace('/', ' ');
                _msg.MsgCode = 'S';
                _msg.Message = "Success";

                return _msg;
            }
            catch (Exception ex)
            {
                _logger.LogError($"get-paysheet : {ex.Message}");
                _logger.LogError($"get-paysheet : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> UploadPaysheet(string epf, int period)
        {
            MsgDto _msg = new MsgDto();
            int _paysheetCounter = 1000000;
            Random random = new Random();
            int randomNumber = random.Next(1000000, 9999999);
            DataTable dt = new DataTable();

            try
            {
                PaySheet_Log _paysheetLog = _context.PaySheet_Log.Where(o => o.epf == epf && o.period == period).FirstOrDefault();

                Employee_Data? _selectedEmpData = _context.Employee_Data.
        Where(o => o.period == period && o.epf == epf).FirstOrDefault();

                if (_selectedEmpData == null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Employee Found";

                    return _msg;
                }

                if (_paysheetLog == null)
                {
                    _paysheetLog = new PaySheet_Log();
                    _paysheetLog.epf = epf;
                    _paysheetLog.period = period;
                    _paysheetLog.companyCode = _selectedEmpData.companyCode;
                    _paysheetLog.changeDate = com.GetTimeZone();
                    _context.PaySheet_Log.Add(_paysheetLog);
                    _context.SaveChanges();
                }

                Sys_Properties? _sendSMSSettings = _context.Sys_Properties.Where(o => o.variable_name == "Send_SMS_PaySheet_View").FirstOrDefault();
                Sys_Properties? _S3UploadSettings = _context.Sys_Properties.Where(o => o.variable_name == "S3_Upload_PaySheet_View").FirstOrDefault();
                Sys_Properties smsBody = _context.Sys_Properties.Where(o => o.variable_name == "SMS_Body").FirstOrDefault();

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
                                             defaultVal.id,
                                             name = defaultVal.description,
                                             payData.payCode,
                                             payData.amount,
                                             payData.calCode,
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
                                               payData.id,
                                               name = defaultVal.description,
                                               payData.payCode,
                                               payData.paytype,
                                               payData.amount,
                                               payData.balanceAmount,
                                               payData.calCode,
                                               payData.payCodeType,
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
                                          payData.id,
                                          name = defaultVal.description,
                                          payData.payCode,
                                          payData.paytype,
                                          payData.balanceAmount,
                                          payData.amount,
                                          payData.calCode,
                                      };


                dt.Columns.Add("empData");
                dt.Columns.Add("salData");
                dt.Columns.Add("earningData");
                dt.Columns.Add("deductionData");
                dt.Columns.Add("unRecoveredData");
                dt.Columns.Add("loanData");
                dt.Rows.Add(JsonConvert.SerializeObject(_empData), JsonConvert.SerializeObject(_salData), JsonConvert.SerializeObject(_earningDataResult), JsonConvert.SerializeObject(_deductionDataResult), JsonConvert.SerializeObject(_unrecovered), JsonConvert.SerializeObject(_loanDataResult));

                _msg.Data = JsonConvert.SerializeObject(dt).Replace('/', ' ');

                EPF_ETF? _epfData = await _context.EPF_ETF.
                    Where(o => o.period == period && o.epf == epf).FirstOrDefaultAsync();

                if (_S3UploadSettings.variable_value == "True")
                {
                    _paysheetCounter += randomNumber;

                    if (!_paysheetLog.isPaysheetUploaded)
                    {
                        var pdfData = GeneratePayslipsForEmployee(_payData, _selectedEmpData, _epfData, period);

                        var fileName = $"{_paysheetCounter}.pdf";
                        await UploadPdfToS3(pdfData, fileName, period.ToString());
                        _paysheetLog.paysheetID = fileName;
                        _paysheetLog.isPaysheetUploaded = true;
                    }
                }

                string _endPoint = "https://cpstl-poc-main-s3.s3.ap-southeast-1.amazonaws.com/public/" + period + "/" + _paysheetLog.paysheetID;

                if (_sendSMSSettings?.variable_value == "True")
                {
                    if (!_paysheetLog.isSMSSend)
                    {
                        if (_selectedEmpData?.phoneNo != null)
                        {
                            SMSSender sms = new SMSSender(_selectedEmpData.phoneNo, string.Format(smsBody.variable_value.Replace("{break}", "\n"), period, _endPoint));
                            bool respose = await sms.sendSMS(sms);

                            if (respose)
                            {
                                _paysheetLog.isSMSSend = true;
                            }
                        }
                        else
                        {
                            _paysheetLog.message = "Employee Phone Number Not found";
                        }
                    }
                }

                if (_paysheetLog != null)
                {
                    _context.Entry(_paysheetLog).State = EntityState.Modified;
                }

                _context.SaveChangesAsync();

                _msg.MsgCode = 'S';
                _msg.Message = "Success";

                return _msg;
            }
            catch (Exception ex)
            {
                SysLog sys = new SysLog();
                sys.application = "API: Print Payslip main thread";
                sys.level = "1";
                sys.message = ex.Message;
                _context.SysLog.Add(sys);
                _context.SaveChanges();

                _logger.LogError($"get-paysheet : {ex.Message}");
                _logger.LogError($"get-paysheet : {ex.InnerException}");

                if (dt.Rows.Count > 0)
                {
                    _msg.Data = JsonConvert.SerializeObject(dt).Replace('/', ' ');
                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";

                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "Error : " + ex.Message;
                    _msg.Description = "Inner Expection : " + ex.InnerException;
                    return _msg;
                }
            }
        }

        public async Task<MsgDto> PrintPaySheets(int companyCode, int period)
        {
            MsgDto _msg = new MsgDto();
            var objectsToSave = new List<PaySheet_Log>();

            Sys_Properties sys_Properties = _context.Sys_Properties.Where(o => o.variable_name == "Send_SMS_PaySheet_View").FirstOrDefault();
            Sys_Properties smsBody = _context.Sys_Properties.Where(o => o.variable_name == "SMS_Body").FirstOrDefault();
            int count = 0;
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

                var paysheetLogEpfs = _paysheetLog.Select(p => p.epf).ToHashSet();
                _empData = _empData.Where(e => !paysheetLogEpfs.Contains(e.epf)).ToList();

                int _paysheetCounter = 3000;
                Random random = new Random();
                int randomNumber = random.Next(1, 11);

                var tasks = _empData.Select(async emp =>
                {
                    _paysheetCounter += randomNumber;
                    ICollection<Payroll_Data> _SelectedPayData = _payData.Where(o => o.epf == emp.epf).ToList();
                    EPF_ETF _selectedEPFData = _epfData.Where(o => o.epf == emp.epf).FirstOrDefault();

                    var _objLog = new PaySheet_Log
                    {
                        epf = emp.epf,
                        period = period,
                        companyCode = companyCode,
                    };

                    var pdfData = GeneratePayslipsForEmployee(_SelectedPayData, emp, _selectedEPFData, period);
                    var fileName = $"{_paysheetCounter}.pdf";

                    string _endPoint = "https://cpstl-poc-main-s3.s3.ap-southeast-1.amazonaws.com/public/" + period + "/" + _paysheetCounter + ".pdf";

                    await UploadPdfToS3(pdfData, fileName, period.ToString());

                    //Task.Delay(10);

                    if (sys_Properties?.variable_value == "True")
                    {
                        if (emp.phoneNo != null)
                        {
                            SMSSender sms = new SMSSender(emp.phoneNo, string.Format(smsBody.variable_value.Replace("{break}", "\n"), period, _endPoint));
                            bool respose = await sms.sendSMS(sms);
                            _objLog.isSMSSend = respose;
                        }
                        else
                        {
                            _objLog.isSMSSend = false;
                            _objLog.message = "Employee Phone Number not found";
                        }
                    }

                    count++;

                    _objLog.paysheetID = fileName;
                    _objLog.isPaysheetUploaded = true;
                    objectsToSave.Add(_objLog);

                    // TO DO: Used to only upload 3 paysheets to server
                    /*if (count >= 3)
                    {
                        _context.PaySheet_Log.AddRange(objectsToSave);
                        _context.SaveChanges();
                        _msg.MsgCode = 'S';
                        _msg.Message = "Success";
                        return _msg;
                    }*/
                });

                //foreach (Employee_Data emp in _empData)
                //{

                //}

                //_msg.Data = JsonConvert.SerializeObject(dt).Replace('/', ' ');

                await Task.WhenAll(tasks);

                _context.PaySheet_Log.AddRange(objectsToSave);
                _context.SaveChanges();
                _msg.MsgCode = 'S';
                _msg.Message = "Success";

                return _msg;
            }
            catch (Exception ex)
            {
                int c = count;
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

        public async Task<MsgDto> CreateBankFile(ApprovalDto approvalDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                Payrun? _payRun = _context.Payrun.Where(o => o.companyCode == approvalDto.companyCode && o.period == approvalDto.period).FirstOrDefault();

                if (_payRun?.payrunStatus == "Unrec File Created")
                {
                    var workItem = new PaysheetBGParams { companyCode = approvalDto.companyCode, period = approvalDto.period, approvedBy = approvalDto.approvedBy };
                    _taskQueue.BackgroundServiceQueue(workItem);

                    _msg.MsgCode = 'S';
                    _msg.Message = "Success";
                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "Payrun Should be in Unrec File Created Status";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError($"CreateBankFile : {ex.Message}");
                _logger.LogError($"CreateBankFile : {ex.InnerException}");
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


                if (period - taxYearFrom < 0)
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

                    decimal _arriesSum = _payItem.Where(o => o.epf == item.EPF).Sum(s => s.amount);
                    int count = _epfetf.Where(o => o.epf == item.EPF).Count();
                    decimal _pTaxableGross = Convert.ToDecimal(_epfetf.Where(o => o.epf == item.EPF).Sum(s => s.taxableGross));
                    decimal _pTax = Convert.ToDecimal(_epfetf.Where(o => o.epf == item.EPF).Sum(s => s.tax));
                    decimal _pLumsumpTax = Convert.ToDecimal(_epfetf.Where(o => o.epf == item.EPF).Sum(s => s.lumpsumTax));
                    decimal _previousLumpsumGross = Convert.ToDecimal(_epfetf.Where(o => o.epf == item.EPF).Sum(s => s.lumpSumGross));

                    if (count > 0)
                    {
                        _pTaxableGross = _pTaxableGross / count;
                        _pTax = _pTax / count;
                    }

                    decimal A = _pTax * 12;

                    decimal D = _pTaxableGross * 12;
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
                        decimal _taxResult = expression.Eval<decimal>();

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
            catch (Exception ex)
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
                tf.DrawString("\nThis message contains sensitive information that is intended solely for the recipient named. \nThis is an automatically generated payslip. Please direct any questions or concerns to the IS Function department. \n-Nilakshi Madanayake-: 864 \n-Pavithra Bhagya -: 865 \n-Gayani Jayasinghe-: 862 \n-Finance Section-: 270,271,483", normalFont, XBrushes.Black,
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
                SysLog sys = new SysLog();
                sys.application = "API: Generate Payslip";
                sys.level = "1";
                sys.message = ex.Message;
                _context.SysLog.Add(sys);
                _context.SaveChanges();

                return null;
            }
        }

        private XGraphics DrawHeader(XGraphics gfx, double x, double y, Employee_Data _empData, XFont normalFont, XPen pen, int period)
        {
            try
            {
                var imagePath = Path.Combine("/app", "logo.jpg");
                //var imagePath = Path.Combine("C:\\Users\\17532\\source\\repos\\pavithra91\\PayrollAPI\\PayrollAPI\\logo.jpg");

                //var watermarkImagePath = Path.Combine("C:\\Users\\17532\\source\\repos\\pavithra91\\PayrollAPI\\PayrollAPI\\Draft.png");
                var watermarkImagePath = Path.Combine("/app", "Draft.png");
                XImage watermarkImage = XImage.FromFile(watermarkImagePath);
                gfx.DrawImage(watermarkImage, 50, 130, 500, 500);

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
                gfx.DrawString(GetPeriod(period.ToString()), normalFont, XBrushes.Black,
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
                SysLog sys = new SysLog();
                sys.application = "API: Header";
                sys.level = "1";
                sys.message = ex.Message;
                _context.SysLog.Add(sys);
                _context.SaveChanges();

                return gfx;
            }
        }
        public async Task UploadPdfToS3(byte[] pdfData, string fileName, string period)
        {
            try
            {
                var uploader = new S3Uploader("AKIAV3CJE2DCBB7UZJDM", "oPvNVvN3U5e+MZwtmRK8/X+5kLDxNzXsCubr1XbT", "cpstl-poc-main-s3", Amazon.RegionEndpoint.APSoutheast1);
                await uploader.UploadFileAsync(pdfData, "public/" + period + "/" + fileName);
            }
            catch (Exception ex)
            {
                SysLog sys = new SysLog();
                sys.application = "API: Upload to S3";
                sys.level = "1";
                sys.message = ex.Message;
                _context.SysLog.Add(sys);
                _context.SaveChanges();
            }
        }

        [AllowAnonymous]
        public async void CheckLogger()
        {
            _logger.LogError(0, "Error while processing request from {Address}");
        }


        #region Unused Methods
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
                        List<string> variables = expression.getVariables();
                        foreach (string variable in variables)
                        {
                            var _val = _empPayrollData.Where(o => o.calCode == variable).FirstOrDefault();
                            if (_val != null)
                            {
                                expression.Bind(variable, _val.amount);
                            }
                            // Console.WriteLine(variable);
                        }

                        decimal _result = expression.Eval<decimal>();

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
                        decimal _taxResult = expression.Eval<decimal>();

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

                    decimal _grossDed = _empPayrollData.Where(o => o.payCategory == "1" && o.payCodeType != "T" && o.payCodeType != "C").Sum(w => w.amount);

                    if (_grossDed > _grossTot)
                    {
                        ICollection<Payroll_Data> _empDeductions = _empPayrollData.Where(o => o.payCategory == "1" && o.payCodeType != "T" && o.payCodeType != "C").OrderBy(o => o.payCode).ToList();

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
        #endregion
    }
}
