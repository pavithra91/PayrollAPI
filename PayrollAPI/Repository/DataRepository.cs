using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces;
using PayrollAPI.Models;
using System.Collections;
using System.Data;
using System.ServiceModel.Channels;
using Z.BulkOperations;
using static LinqToDB.Common.Configuration;
using static System.Runtime.CompilerServices.RuntimeHelpers;

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

        public async Task<MsgDto> DataTransfer(string json)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                IList<Temp_Employee> _tempEmpList = new List<Temp_Employee>();
                IList<Temp_Payroll> _tempPayList = new List<Temp_Payroll>();
                IList<SAPTotPayCode> _tempSAPTot = new List<SAPTotPayCode>();

                DataSet _masterDataTable = JsonConvert.DeserializeObject<DataSet>(json);

                foreach (DataRow _dataRow in _masterDataTable.Tables[0].AsEnumerable())
                {
                    _tempEmpList.Add(new Temp_Employee()
                    {
                        companyCode = Convert.ToInt32(_dataRow["company"]),
                        location = Convert.ToInt32(_dataRow["plant"]),
                        epf = _dataRow["epf"].ToString().Substring(3),
                        period = Convert.ToInt32(_dataRow["period"]),
                        empName = _dataRow["empName"].ToString(),
                        costCenter = _dataRow["costCenter"].ToString(),
                        empGrade = _dataRow["empGrade"].ToString(),
                        gradeCode = Convert.ToInt32(_dataRow["gradeCode"]),
                        paymentType = Convert.ToInt32(_dataRow["paymentType"]),
                        bankCode = Convert.ToInt32(_dataRow["bankCode"]),
                        branchCode = Convert.ToInt32(_dataRow["branchCode"]),
                        accountNo = _dataRow["accountNo"].ToString(),
                        createdBy = _dataRow["createdBy"].ToString(),
                        createdDate = DateTime.Now,
                    });
                }

                foreach (DataRow _dataRow in _masterDataTable.Tables[1].AsEnumerable())
                {
                    _tempPayList.Add(new Temp_Payroll()
                    {
                        companyCode = Convert.ToInt32(_dataRow["company"]),
                        location = Convert.ToInt32(_dataRow["plant"]),
                        epf = _dataRow["epf"].ToString().Substring(3),
                        period = Convert.ToInt32(_dataRow["period"]),
                        othours = (float)Convert.ToDouble(_dataRow["othours"].ToString()),
                        costCenter = _dataRow["costcenter"].ToString(),
                        payCode = Convert.ToInt32(_dataRow["payCode"].ToString()),
                        calCode = "",
                        payCategory = _dataRow["payCategory"].ToString(),
                        payCodeType = _dataRow["payCodeType"].ToString(),
                        paytype = string.IsNullOrEmpty(_dataRow["paytype"].ToString()) ? ' ' : _dataRow["paytype"].ToString()[0],
                        amount = Convert.ToDecimal(_dataRow["amount"].ToString()),
                        balanceamount = Convert.ToDecimal(_dataRow["balanceamount"].ToString()),
                        epfConRate = Convert.ToInt32(_dataRow["epfConRate"]),
                        taxConRate = Convert.ToInt32(_dataRow["taxConRate"]),
                        createdDate = DateTime.Now,
                    });
                }

                foreach (DataRow _dataRow in _masterDataTable.Tables[2].AsEnumerable())
                {
                    _tempSAPTot.Add(new SAPTotPayCode()
                    {
                        companyCode = Convert.ToInt32(_dataRow["company"]),
                        period = Convert.ToInt32(_dataRow["period"]),
                        payCode = Convert.ToInt32(_dataRow["payCode"]),
                        payType = _dataRow["payType"].ToString(),
                        calCode = "",
                        totalAmount = Convert.ToDecimal(_dataRow["totAmout"].ToString()),
                        totCount = Convert.ToInt32(_dataRow["totCount"].ToString()),
                        createdDate = DateTime.Now,
                    });
                }
                /* Parallel.ForEach(_masterDataTable.AsEnumerable(), _dataRow => {
                     _tempEmpList.Add(new Temp_Employee()
                     {
                         company = Convert.ToInt32(_dataRow["company"]),
                         plant = Convert.ToInt32(_dataRow["plant"]),
                         epf = _dataRow["epf"].ToString(),
                         period = Convert.ToInt32(_dataRow["period"]),
                         empName = _dataRow["empName"].ToString(),
                         costCenter = _dataRow["costCenter"].ToString(),
                         empGrade = _dataRow["empGrade"].ToString(),
                         gradeCode = Convert.ToInt32(_dataRow["gradeCode"]),
                         paymentType = Convert.ToInt32(_dataRow["paymentType"]),
                         bankCode = Convert.ToInt32(_dataRow["bankCode"]),
                         branchCode = Convert.ToInt32(_dataRow["branchCode"]),
                         accountNo = _dataRow["accountNo"].ToString(),
                     });
                 }); */


                _context.BulkCopy(_tempEmpList);
                _context.BulkCopy(_tempPayList);
                _context.BulkCopy(_tempSAPTot);

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

        public async Task<MsgDto> RollBackTempData(ApprovalDto approvalDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                _context.Employee_Data.Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode).
                    DeleteFromQuery();
                _context.Payroll_Data.Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode).
                    DeleteFromQuery();

                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "Data Rollback Operation Completed Successfully";
                return _msg;
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

        public async Task<MsgDto> ConfirmDataTransfer(ApprovalDto approvalDto)
        {
            using var transaction = BeginTransaction();
            MsgDto _msg = new MsgDto();
            try
            {
                _context.Temp_Employee
                .Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode)
                .InsertFromQuery("Employee_Data", x => new { 
                x.companyCode, 
                x.location, 
                x.epf,
                x.period,
                x.empName,
                x.costCenter,
                x.empGrade,
                x.gradeCode,
                x.paymentType,
                x.bankCode,
                x.branchCode,
                x.accountNo
                });

                _context.Temp_Payroll
                .Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode)
                .InsertFromQuery("Payroll_Data", x => new {
                    x.companyCode,
                    x.location,
                    x.epf,
                    x.period,
                    x.costCenter,
                    x.othours,
                    x.payCode,
                    x.calCode,
                    x.paytype,
                    x.payCodeType,
                    x.payCategory,
                    x.amount,
                    x.balanceamount,
                    x.epfConRate,
                    x.taxConRate,
                });

                Payrun _objPay = new Payrun();
                _objPay.companyCode = approvalDto.companyCode;
                _objPay.period = approvalDto.period;
                _objPay.approvedBy = approvalDto.approvedBy;
                _objPay.approvedDate = DateTime.Now;
                _objPay.approvedTime = DateTime.Now;
                _objPay.noOfEmployees = _context.Temp_Employee.Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode).Count();
                _objPay.noOfRecords = _context.Temp_Payroll.Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode).Count();
                _objPay.payrunStatus = "Approved";

                _context.Payrun.Add(_objPay);

                _context.Payroll_Data.UpdateFromQuery(x => new Payroll_Data { epfContribution = x.amount * (decimal)x.epfConRate, taxContribution = x.amount * (decimal)x.taxConRate, calCode = "_" + x.payCode });

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

        public MsgDto GetDataTransferStatistics(int companyCode, int period)
        {
            MsgDto _msg = new MsgDto();
            try
            {
                int _empCount = _context.Temp_Employee.Where(o => o.companyCode == companyCode && o.period == period).ToList().Count();

                IList _paySummaryList = _context.Temp_Payroll.Where(o => o.companyCode == companyCode && o.period == period).
                    GroupBy(p => new { p.payCode }).
                    Select(g => new {
                        PayCode = g.Key.payCode,
                        Amount = g.Sum(o => o.amount),
                        Line_Item_Count = g.Count()
                    }).OrderBy(p => p.PayCode).ToList();

                IList _paySAPSummaryList = _context.SAPTotPayCode.Where(o => o.companyCode == companyCode && o.period == period).
                    GroupBy(p => new { p.payCode }).
                    Select(g => new {
                        PayCode = g.Key.payCode,
                        Amount = g.Sum(o => o.totalAmount),
                        Line_Item_Count = g.Sum(o => o.totCount)
                    }).OrderBy(p => p.PayCode).ToList();

                var result = new List<object>();
                result.Add(new { empCount = _empCount, nonSAPPayData = _paySummaryList, SAPPayData = _paySAPSummaryList });

                _msg.Data = JsonConvert.SerializeObject(result);
                _msg.MsgCode = 'S';
                _msg.Message = "";
                
                return _msg;
            }
            catch(Exception ex)
            {
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public ICollection<Temp_Employee> GetTempEmployeeList(int companyCode, int period)
        {
            ICollection<Temp_Employee> _tempEmpList = _context.Temp_Employee.Where(o=> o.companyCode == companyCode && o.period == period).ToList();
            return _tempEmpList;
        }
    }
}
