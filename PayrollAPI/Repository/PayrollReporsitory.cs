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
            ICollection<Employee_Data> _emp = _context.Employee_Data.Where(o => o.period == approvalDto.period).OrderBy(o=>o.epf).ToList();
            ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == approvalDto.period).ToList();          
            ICollection<Calculation> _calculation = _context.Calculation.Where(o => o.companyCode == approvalDto.companyCode).ToList();
            ICollection<Unrecovered> _unRecoveredList = _context.Unrecovered.Where(o => o.companyCode == approvalDto.companyCode).ToList();

            // Calculate EPF and Tax
            using var transaction = BeginTransaction();

            Parallel.ForEach(_emp, emp =>
            {
                ICollection<Payroll_Data> _empPayrollData = _payrollData.Where(o => o.epf == emp.epf).OrderBy(o => o.payCode).ToList();

                decimal _epfTot = _empPayrollData.Where(o => o.epfContribution > 0).Sum(w => w.epfContribution);
                decimal _taxTot = _empPayrollData.Where(o => o.taxContribution > 0).Sum(w => w.taxContribution);

                Payroll_Data _tempEPFTOT = new Payroll_Data();
                _tempEPFTOT.period = approvalDto.period;
                _tempEPFTOT.epf = emp.epf;
                _tempEPFTOT.othours = 0;
                _tempEPFTOT.payCategory = "";
                _tempEPFTOT.payCode = 0;
                _tempEPFTOT.calCode = "EPFTO";
                _tempEPFTOT.paytype = null;
                _tempEPFTOT.costcenter = emp.costCenter;
                _tempEPFTOT.payCodeType = "T";
                _tempEPFTOT.amount = _epfTot;
                _tempEPFTOT.balanceamount = 0;
                _tempEPFTOT.displayOnPaySheet = false;
                _tempEPFTOT.epfConRate = 0;
                _tempEPFTOT.epfContribution = 0;
                _tempEPFTOT.taxConRate = 0;
                _tempEPFTOT.taxContribution = 0;

                _empPayrollData.Add(_tempEPFTOT);
               // _context.Payroll_Data.Add(_objEPFTOT);
                /*
                Payroll_Data _objTAXTOT = new Payroll_Data();
                _objTAXTOT.period = approvalDto.period;
                _objTAXTOT.epf = emp.epf;
                _objTAXTOT.othours = 0;
                _objTAXTOT.payCategory = "";
                _objTAXTOT.payCode = 0;
                _objTAXTOT.calCode = "EPFTO";
                _objTAXTOT.paytype = 0;
                _objTAXTOT.costcenter = emp.costCenter;
                _objTAXTOT.payCodeType = "T";
                _objTAXTOT.amount = _epfTot;
                _objTAXTOT.balanceamount = 0;
                _objTAXTOT.displayOnPaySheet = false;
                _objTAXTOT.epfConRate = 0;
                _objTAXTOT.epfContribution = 0;
                _objTAXTOT.taxConRate = 0;
                _objTAXTOT.taxContribution = 0;

                _payrollData.Add(_objTAXTOT);
                */

                foreach(Calculation cal in _calculation)
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
                        Console.WriteLine(variable);
                    }

                    Decimal _result = expression.Eval<Decimal>();

                    Payroll_Data _objEPFTOT = new Payroll_Data();
                    _objEPFTOT.period = approvalDto.period;
                    _objEPFTOT.epf = emp.epf;
                    _objEPFTOT.othours = 0;
                    _objEPFTOT.payCategory = "T";
                    _objEPFTOT.payCode = cal.payCode;
                    _objEPFTOT.calCode = cal.calCode;
                    _objEPFTOT.paytype = null;
                    _objEPFTOT.costcenter = emp.costCenter;
                    _objEPFTOT.payCodeType = "S";
                    _objEPFTOT.amount = _result;
                    _objEPFTOT.balanceamount = 0;
                    _objEPFTOT.displayOnPaySheet = true;
                    _objEPFTOT.epfConRate = 0;
                    _objEPFTOT.epfContribution = 0;
                    _objEPFTOT.taxConRate = 0;
                    _objEPFTOT.taxContribution = 0;

                    _empPayrollData.Add(_objEPFTOT);
                    _context.Payroll_Data.Add(_objEPFTOT);
                };

                decimal _grossTot = _empPayrollData.Where(o => o.payCategory == "0").Sum(w => w.amount);
                decimal _grossDed = _empPayrollData.Where(o => o.payCategory == "1").Sum(w => w.amount);

                if(_grossDed > _grossTot)
                {
                    ICollection<Payroll_Data> _empDeductions = _empPayrollData.Where(o=>o.payCategory == "1").OrderBy(o => o.payCode).ToList();
                    
                    foreach(Payroll_Data deductionItem in _empDeductions)
                    {
                        _grossTot -= deductionItem.amount;

                        if(_grossTot < 0)
                        {
                            // Add to unrecovered List
                            Unrecovered _unRecoveredObj = new Unrecovered();
                            //_unRecoveredObj.companyCode = 2323;
                            _unRecoveredObj.period = deductionItem.period;
                            _unRecoveredObj.epf = deductionItem.epf;
                            _unRecoveredObj.payCategory = deductionItem.payCategory;
                            _unRecoveredObj.payCode = deductionItem.payCode;
                            _unRecoveredObj.calCode = deductionItem.calCode;
                            _unRecoveredObj.costcenter = deductionItem.costcenter;
                            _unRecoveredObj.amount = deductionItem.amount;
                        }
                    }
                }

            });

            transaction.Commit();

            MsgDto _msg = new MsgDto();
            _msg.MsgCode = 'S';
            _msg.Message = "Data Transered Successfully";
            return _msg;
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();

            return transaction.GetDbTransaction();
        }
    }
}
