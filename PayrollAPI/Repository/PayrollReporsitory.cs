using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using System.Data;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using org.matheval;
using Expression = org.matheval.Expression;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using Newtonsoft.Json;
using PayrollAPI.Controllers;
using Newtonsoft.Json.Linq;

namespace PayrollAPI.Repository
{
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

        public async Task<MsgDto> ProcessPayroll(ApprovalDto approvalDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();

            try
            {
                ICollection<Employee_Data> _emp = _context.Employee_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).OrderBy(o => o.epf).ToList();
                ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).ToList();
                ICollection<Calculation> _calculation = _context.Calculation.Where(o => o.companyCode == approvalDto.companyCode).ToList();
                ICollection<Tax_Calculation> _taxCalculation = _context.Tax_Calculation.Where(o => o.companyCode == approvalDto.companyCode).ToList();
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
                        decimal _taxTot = _empPayrollData.Where(o => o.taxConRate > 0 && o.paytype != 'A').Sum(w => w.taxContribution);

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
                        ePF_ETF.deductionGross = _grossDed;
                        ePF_ETF.emp_contribution = _empPayrollData.Where(x => x.calCode == "EPFEM").Select(x => x.amount).FirstOrDefault(0);
                        ePF_ETF.comp_contribution = _empPayrollData.Where(x => x.calCode == "EPFCO").Select(x => x.amount).FirstOrDefault(0);
                        ePF_ETF.etf = _empPayrollData.Where(x => x.calCode == "ETFCO").Select(x => x.amount).FirstOrDefault(0);
                        ePF_ETF.tax = _empPayrollData.Where(x => x.calCode == "APTAX").Select(x => x.amount).FirstOrDefault(0);

                        await _context.EPF_ETF.AddAsync(ePF_ETF);
                    };

                   _payRun.payrunBy = approvalDto.approvedBy;
                   _payRun.payrunStatus = "EPF/TAX Calculated";
                   _payRun.payrunDate = DateTime.Now;
                   _payRun.payrunTime = DateTime.Now;

                    await _context.SaveChangesAsync();

                    transaction.Commit();


                    _msg.MsgCode = 'S';
                    _msg.Message = "EPF/TAX Calculated Successfully";
                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "EPF/TAX Already Ccalculated. Operation Failed!";
                    return _msg;
                }
            }
            catch(Exception ex)
            {
                transaction.Rollback();
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

            ICollection<Employee_Data> _emp = _context.Employee_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).OrderBy(o => o.epf).ToList();
            ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).ToList();
            ICollection<Unrecovered> _unRecoveredList = _context.Unrecovered.Where(o => o.companyCode == approvalDto.companyCode).ToList();
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
                                _unRecoveredObj.amount = deductionItem.amount;
                                _context.Unrecovered.Add(_unRecoveredObj);


                                Payroll_Data _payItem = _empPayrollData.Where(o => o.calCode == deductionItem.calCode).FirstOrDefault();
                                _payItem.amount = deductionItem.amount + _grossTot;
                                _context.Attach(_payItem);
                                _context.Entry(_payItem).Property(p => p.amount).IsModified = true;
                                _grossTot = 0;
                            }
                        }
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
                _msg.Message = "Unrecovered File Already Ccalculated. Operation Failed!";
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
                ICollection<Tax_Calculation> _taxCalculation = _context.Tax_Calculation.Where(o => o.companyCode == companyCode).ToList();
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

                                Payroll_Data _payItem = _context.Payroll_Data.Where(o=>o.id == deductionItem.id).FirstOrDefault();
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
            ICollection<EPF_ETF> ePF_ETFs = await _context.EPF_ETF.Where(o => o.period == period && o.companyCode == companyCode).OrderBy(o => o.epf).ToListAsync();
            
            MsgDto _msg = new MsgDto();

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
            _logger.LogInformation($"Get PaySheet for Emp {epf}");
            MsgDto _msg = new MsgDto();
            try
            {
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
                                           select new
                                           {
                                               id = defaultVal.id,
                                               name = defaultVal.description,
                                               payCode = payData.payCode,
                                               amount = payData.amount,
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

                DataTable dt = new DataTable();
                dt.Columns.Add("empData");
                dt.Columns.Add("salData");
                dt.Columns.Add("earningData");
                dt.Columns.Add("deductionData");
                dt.Columns.Add("unRecoveredData");
                dt.Rows.Add(JsonConvert.SerializeObject(_empData), JsonConvert.SerializeObject(_salData), JsonConvert.SerializeObject(_earningDataResult), JsonConvert.SerializeObject(_deductionDataResult), JsonConvert.SerializeObject(_unrecovered));

                _msg.Data = JsonConvert.SerializeObject(dt).Replace('/', ' ');
                _msg.MsgCode = 'S';
                _msg.Message = "Success";
                return _msg;
            }
            catch(Exception ex)
            {
                _logger.LogError("Error");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> PrintPaySheets(int companyCode, int period)
        {
            //_logger.LogInformation($"Get PaySheet for Emp {epf}");
            MsgDto _msg = new MsgDto();
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

                DataTable dt = new DataTable();
                dt.Columns.Add("empData");
                dt.Columns.Add("salData");
                dt.Columns.Add("earningData");
                dt.Columns.Add("deductionData");
                dt.Columns.Add("unRecoveredData");

                foreach (Employee_Data emp in _empData)
                {
                    var _earningDataResult = from payData in _earningData
                                             join payCode in _payCodes on payData.payCode equals payCode.payCode
                                             into Earnings
                                             where payData.epf == emp.epf
                                             from defaultVal in Earnings.DefaultIfEmpty()
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
                                               select new
                                               {
                                                   name = defaultVal.description,
                                                   payCode = payData.payCode,
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
                   });

                    var _unRecData = _unrecoveredData.
                    Where(o => o.period == period && o.epf == emp.epf).
                    Select(e => new
                    {
                        e?.payCode,
                        e?.calCode,
                        e?.amount
                    });


                    dt.Rows.Add(JsonConvert.SerializeObject(_empDisplayData), JsonConvert.SerializeObject(_salData), JsonConvert.SerializeObject(_earningDataResult), JsonConvert.SerializeObject(_deductionDataResult), JsonConvert.SerializeObject(_unRecData));
                }

                _msg.Data = JsonConvert.SerializeObject(dt).Replace('/', ' ');
                _msg.MsgCode = 'S';
                _msg.Message = "Success";
                return _msg;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error");
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
                    OrderByDescending(o => o.id).ToListAsync();

                _msg.MsgCode = 'S';
                _msg.Data = JsonConvert.SerializeObject(_payRun);
                _msg.Message = "Success";
                return _msg;
            }
            catch (Exception ex)
            {
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
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> Writeback(int period, int companyCode)
        {
            _logger.LogInformation("Process Started");
            MsgDto _msg = new MsgDto();
            try
            {
                ICollection<Payroll_Data> _payData = await _context.Payroll_Data.Where(o => o.period == period && o.companyCode == companyCode).
                    ToListAsync();
                ICollection<Unrecovered> _unrecovered = await _context.Unrecovered.Where(o => o.period == period && o.companyCode == companyCode).AsSplitQuery().
                    ToListAsync();
                _logger.LogInformation("Get the Database data");
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

                _logger.LogInformation("Process Success");
                _msg.MsgCode = 'S';
                _msg.Data = JsonConvert.SerializeObject(_payData.OrderBy(o => o.epf));
                _msg.Message = "Success";
                return _msg;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
    }
}
