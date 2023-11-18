using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using System.Data;

namespace PayrollAPI.Repository
{
    public class DataRepository : IDatatransfer
    {
        private readonly DBConnect _context;
        public DataRepository(DBConnect db)
        {
            _context = db;
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();

            return transaction.GetDbTransaction();
        }

        public async Task<MsgDto> ConfirmDataTransfer(ApprovalDto approvalDto)
        {
            using var transaction = BeginTransaction();
            MsgDto _msg = new MsgDto();
            try
            {
                ICollection<Temp_Employee> _employeeList = _context.Temp_Employee.Where(o => o.period == approvalDto.period).ToList();

                IList<Employee_Data> _newEmpList = new List<Employee_Data>();

                Parallel.ForEach(_employeeList, emp =>
                {
                    _newEmpList.Add(new Employee_Data()
                    {
                        epf = emp.epf,
                        period = emp.period,
                        empName = emp.empName,
                        costCenter = emp.costCenter,
                        empGrade = emp.empGrade,
                        gradeCode = emp.gradeCode,
                        paymentType = emp.paymentType,
                        bankCode = emp.bankCode,
                        branchCode = emp.branchCode,
                        accountNo = emp.branchCode,
                        status = true
                    });
                });

                await _context.BulkInsertAsync(_newEmpList);

                ICollection<Temp_Payroll> _payItemList = _context.Temp_Payroll.Where(o => o.period == approvalDto.period).ToList();
                ICollection<PayCode> _payCode = _context.PayCode.ToList();

                IList<Payroll_Data> _newPayrollData = new List<Payroll_Data>();

                foreach (PayCode payCode in _payCode)
                {
                    ICollection<Temp_Payroll> _tempList = _payItemList.Where(w => w.payCode == payCode.payCode).ToList();

                    Parallel.ForEach(_tempList, payItem => {
                        _newPayrollData.Add(new Payroll_Data()
                        {
                            epf = payItem.epf,
                            period = payItem.period,
                            othours = payItem.othours,
                            payCategory = payItem.payCategory,
                            payCode = payItem.payCode,
                            calCode = payCode.calCode,
                            paytype = payItem.paytype,
                            costcenter = payItem.costcenter,
                            payCodeType = payItem.payCodeType,
                            amount = payItem.amount,
                            balanceamount = payItem.balanceamount,
                            epfConRate = payItem.epfConRate,
                            epfContribution = (payItem.amount * (decimal)payItem.epfConRate),
                            taxConRate = payItem.taxConRate,
                            taxContribution = (payItem.amount * (decimal)payItem.taxConRate),
                        });
                    });
                }

                await _context.BulkInsertAsync(_newPayrollData);

                Payrun _objPay = new Payrun();
                _objPay.period = approvalDto.period;
                _objPay.approvedBy = approvalDto.approvedBy;
                _objPay.approvedDate = DateTime.Now;
                _objPay.noOfEmployees = _newEmpList.Count;
                _objPay.noOfRecords = _newPayrollData.Count;
                _objPay.payrunStatus = "Approved";

                _context.Add(_objPay);

                await _context.SaveChangesAsync();
                transaction.Commit();


                _msg.MsgCode = 'S';
                _msg.Message = "Data Transered Successfully";
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

        public async Task<MsgDto> PreparePayrun(ApprovalDto approvalDto)
        {
            try
            {
                using var transaction = BeginTransaction();

                MsgDto _msg = new MsgDto();
                ICollection<EmpSpecialRate> _empSpecialRates = _context.EmpSpecialRate.Where(o => o.status == true).ToList();
                ICollection<Payroll_Data> _payrollData = _context.Payroll_Data.Where(o => o.period == approvalDto.period).ToList();

                Parallel.ForEach(_empSpecialRates, payItem =>
                {
                    Payroll_Data _data = new Payroll_Data();

                    _data = _payrollData.Where(o => o.epf == payItem.epf && o.payCode == payItem.payCode).FirstOrDefault();
                    if (_data != null)
                    {
                        _data.amount = payItem.rate;
                        _data.taxContribution = payItem.rate * (decimal)_data.taxConRate;
                        _data.epfContribution = payItem.rate * (decimal)_data.epfConRate;
                    }
                    // _context.SaveChanges();
                });

                await _context.SaveChangesAsync();
                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "Data Preparation Successfully Complete";
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
        public async Task<MsgDto> DataTransfer(string json)
        {
            MsgDto _msg = new MsgDto();

            try
            {
                using var transaction = BeginTransaction();

                DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
                _msg.MsgCode = 'S';
                _msg.Message = "Data Transered Successfully";
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
    }
}
