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

namespace PayrollAPI.Repository
{
    public class PayrollReporsitory : IPayroll
    {
        private readonly DBConnect _context;

        public PayrollReporsitory(DBConnect db)
        {
            _context = db;
        }

        public async Task<MsgDto> ProcessPayroll(ApprovalDto approvalDto)
        {
            try
            {
                ICollection<Employee_Data> _emp = _context.Employee_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).OrderBy(o => o.epf).ToList();
                ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == approvalDto.period && o.companyCode == approvalDto.companyCode).ToList();
                ICollection<Calculation> _calculation = _context.Calculation.Where(o => o.companyCode == approvalDto.companyCode).ToList();
                ICollection<Tax_Calculation> _taxCalculation = _context.Tax_Calculation.Where(o => o.companyCode == approvalDto.companyCode).ToList();
                ICollection<Unrecovered> _unRecoveredList = _context.Unrecovered.Where(o => o.companyCode == approvalDto.companyCode).ToList();

                // Calculate EPF and Tax
                using var transaction = BeginTransaction();

                //Parallel.ForEach(_emp, emp =>
                foreach(Employee_Data emp in _emp)
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
                        _objEPFTOT.payCodeType = "P";
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
                        if(_taxTot > cal.range) 
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
                        _objTAXTOT.payCodeType = "P";
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

                    EPF_ETF ePF_ETF= new EPF_ETF();
                    ePF_ETF.epf = emp.epf;
                    ePF_ETF.companyCode = emp.companyCode;
                    ePF_ETF.location = emp.location;
                    ePF_ETF.grade = emp.empGrade;
                    ePF_ETF.epfGross = _epfTot;
                    ePF_ETF.taxableGross = _taxTot;
                    ePF_ETF.emp_contribution = _empPayrollData.Where(x=>x.calCode == "EPFEM").Select(x=>x.amount).FirstOrDefault(0);
                    ePF_ETF.comp_contribution = _empPayrollData.Where(x => x.calCode == "EPFCO").Select(x => x.amount).FirstOrDefault(0);
                    ePF_ETF.etf = _empPayrollData.Where(x => x.calCode == "ETFCO").Select(x => x.amount).FirstOrDefault(0);
                    ePF_ETF.tax = _empPayrollData.Where(x => x.calCode == "APTAX").Select(x => x.amount).FirstOrDefault(0);

                    decimal _grossDed = _empPayrollData.Where(o => o.payCategory == "1").Sum(w => w.amount);

                    if (_grossDed > _grossTot)
                    {
                        ICollection<Payroll_Data> _empDeductions = _empPayrollData.Where(o => o.payCategory == "1").OrderBy(o => o.payCode).ToList();

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
                            }
                        }
                    }

                };

                await _context.SaveChangesAsync();

                transaction.Commit();

                MsgDto _msg = new MsgDto();
                _msg.MsgCode = 'S';
                _msg.Message = "Data Transered Successfully";
                return _msg;
            }
            catch(Exception ex)
            {
                MsgDto _msg = new MsgDto();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();

            return transaction.GetDbTransaction();
        }

        public async Task<MsgDto> ProcessPayrollbyEPF(string epf, int period, int companyCode)
        {
            try
            {
                ICollection<Employee_Data> _emp = _context.Employee_Data.Where(o => o.period == period && o.companyCode == companyCode && o.epf == epf).OrderBy(o => o.epf).ToList();
                ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == period && o.companyCode == companyCode && o.epf == epf).ToList();
                ICollection<Calculation> _calculation = _context.Calculation.Where(o => o.companyCode == companyCode).ToList();
                ICollection<Tax_Calculation> _taxCalculation = _context.Tax_Calculation.Where(o => o.companyCode == companyCode).ToList();
                ICollection<Unrecovered> _unRecoveredList = _context.Unrecovered.Where(o => o.companyCode == companyCode).ToList();

                // Calculate EPF and Tax
                using var transaction = BeginTransaction();

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
                        _objEPFTOT.payCodeType = "P";
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
                        _objTAXTOT.payCodeType = "P";
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
                    ePF_ETF.companyCode = emp.companyCode;
                    ePF_ETF.location = emp.location;
                    ePF_ETF.grade = emp.empGrade;
                    ePF_ETF.epfGross = _epfTot;
                    ePF_ETF.taxableGross = _taxTot;
                    ePF_ETF.emp_contribution = _empPayrollData.Where(x => x.calCode == "EPFEM").Select(x => x.amount).FirstOrDefault(0);
                    ePF_ETF.comp_contribution = _empPayrollData.Where(x => x.calCode == "EPFCO").Select(x => x.amount).FirstOrDefault(0);
                    ePF_ETF.etf = _empPayrollData.Where(x => x.calCode == "ETFCO").Select(x => x.amount).FirstOrDefault(0);
                    ePF_ETF.tax = _empPayrollData.Where(x => x.calCode == "APTAX").Select(x => x.amount).FirstOrDefault(0);

                    decimal _grossDed = _empPayrollData.Where(o => o.payCategory == "1").Sum(w => w.amount);

                    if (_grossDed > _grossTot)
                    {
                        ICollection<Payroll_Data> _empDeductions = _empPayrollData.Where(o => o.payCategory == "1").OrderBy(o => o.payCode).ToList();

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
                            }
                        }
                    }

                };

                await _context.SaveChangesAsync();

                transaction.Commit();

                MsgDto _msg = new MsgDto();
                _msg.MsgCode = 'S';
                _msg.Message = "Data Transered Successfully";
                return _msg;
            }
            catch (Exception ex)
            {
                MsgDto _msg = new MsgDto();
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
    }
}
