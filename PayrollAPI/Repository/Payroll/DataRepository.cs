using EntityFramework.Exceptions.Common;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using PayrollAPI.Data;
using PayrollAPI.DataModel;
using PayrollAPI.Interfaces.Payroll;
using PayrollAPI.Models.Payroll;
using System.Collections;
using System.Data;

namespace PayrollAPI.Repository.Payroll
{
    public class DataRepository : IDatatransfer
    {
        private readonly DBConnect _context;
        private readonly ILogger _logger;
        public DataRepository(DBConnect db, ILogger<DataRepository> logger)
        {
            _context = db;
            _logger = logger;
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();

            return transaction.GetDbTransaction();
        }

        public async Task<MsgDto> DataTransfer(string json)
        {
            int count = 0;
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                IList<Temp_Employee> _tempEmpList = new List<Temp_Employee>();
                IList<Temp_Payroll> _tempPayList = new List<Temp_Payroll>();
                IList<SAPTotPayCode> _tempSAPTot = new List<SAPTotPayCode>();

                DataSet _masterDataTable = JsonConvert.DeserializeObject<DataSet>(json);

                if (_masterDataTable == null || _masterDataTable.Tables.Count == 0)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Found";
                    return _msg;
                }

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
                        phoneNo = _dataRow["phoneNo"].ToString(),
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

                _context.BulkCopy(_tempEmpList);
                _context.BulkCopy(_tempPayList);
                _context.BulkCopy(_tempSAPTot);

                foreach (DataRow _dataRow in _masterDataTable.Tables[3].AsEnumerable())
                {
                    Payrun _objPay = new Payrun();
                    _objPay.companyCode = Convert.ToInt32(_dataRow["company"]);
                    _objPay.period = Convert.ToInt32(_dataRow["period"]);
                    _objPay.dataTransferredBy = _dataRow["transferedBy"].ToString();
                    _objPay.dataTransferredDate = DateTime.Now;
                    _objPay.dataTransferredTime = DateTime.Now;
                    _objPay.noOfEmployees = _masterDataTable.Tables[0].Rows.Count;
                    _objPay.noOfRecords = _masterDataTable.Tables[1].Rows.Count;
                    _objPay.payrunStatus = "Transfer Complete";

                    _context.Payrun.Add(_objPay);

                }
                await _context.SaveChangesAsync();
                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "Data Transered Successfully";
                return _msg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(count);
                transaction.Rollback();
                _logger.LogError($"DataTransfer : {ex.Message}");
                _logger.LogError($"DataTransfer : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Data = ex.Data.ToString();
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> PayCodeCheck(int companyCode, int period)
        {
            MsgDto _msg = new MsgDto();

            var missingPayCodes = await _context.Temp_Payroll.Where(e1 => !_context.PayCode.Any(e2 => e1.payCode == e2.payCode)).Select(e2 => e2.payCode).Distinct().ToListAsync();

            if (missingPayCodes.Count > 0)
            {
                _msg.MsgCode = 'S';
                _msg.Data = JsonConvert.SerializeObject(missingPayCodes);
                _msg.Message = $"Paycode Mismatch. The provided paycode does not match any records in the system. Please verify and update the correct paycode";
                return _msg;
            }
            else
            {
                _msg.MsgCode = 'S';
                _msg.Data = JsonConvert.SerializeObject("[]");
                _msg.Message = $"Bad Request";
                return _msg;
            }
        }

        public async Task<MsgDto> ConfirmDataTransfer(ApprovalDto approvalDto)
        {
            using var transaction = BeginTransaction();
            MsgDto _msg = new MsgDto();
            try
            {
                if (approvalDto.companyCode == 0 || approvalDto.period == 0 || approvalDto.approvedBy == null || approvalDto.approvedBy == "")
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = $"Bad Request";
                    return _msg;
                }

                Payrun? _payRun = _context.Payrun.Where(x => x.companyCode == approvalDto.companyCode
                        && x.period == approvalDto.period).FirstOrDefault();

                if (_payRun == null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = $"No Data available for period - {approvalDto.period}. Operation failed.";
                    return _msg;
                }
                else if (_payRun.payrunStatus == "Transfer Complete")
                {

                    IEnumerable<PayCode> _paycode = _context.PayCode.Where(o => o.rate < 1).ToList();

                    foreach (PayCode paycode in _paycode)
                    {
                        _context.Temp_Payroll.Where(x => x.payCode == paycode.payCode).UpdateFromQuery(x => new Temp_Payroll { taxConRate = (float)paycode.rate });
                    }


                    _context.Temp_Employee
                .Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode)
                .InsertFromQuery("Employee_Data", x => new
                {
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
                    x.accountNo,
                    x.phoneNo
                });


                    _context.Temp_Payroll
                    .Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode)
                    .InsertFromQuery("Payroll_Data", x => new
                    {
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

                    _context.Payroll_Data.UpdateFromQuery(x => new Payroll_Data { epfContribution = x.amount * (decimal)x.epfConRate, taxContribution = x.amount * (decimal)x.taxConRate, calCode = "_" + x.payCode });

                    _payRun.approvedBy = approvalDto.approvedBy;
                    _payRun.approvedDate = DateTime.Today;
                    _payRun.approvedTime = DateTime.Now;
                    _payRun.payrunStatus = "Confirmed";
                    _context.Attach(_payRun);

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    _msg.MsgCode = 'S';
                    _msg.Message = "Data Transered Confirmed";
                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = $"Payrun already Confirmed for period - {approvalDto.period}.";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError($"confirm-data-transfer : {ex.Message}");
                _logger.LogError($"confirm-data-transfer : {ex.InnerException}");
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
                if (approvalDto.companyCode == 0 || approvalDto.period == 0 || approvalDto.approvedBy == null || approvalDto.approvedBy == "")
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = $"Bad Request";
                    return _msg;
                }

                Payrun? _payRun = _context.Payrun.Where(x => x.companyCode == approvalDto.companyCode
                && x.period == approvalDto.period).FirstOrDefault();

                if (_payRun == null)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = $"No Data available for period - {approvalDto.period}. Rollback operation failed.";
                    return _msg;
                }

                if (_payRun.payrunStatus == "Transfer Complete")
                {
                    _context.Temp_Employee.Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode).
                        DeleteFromQuery();
                    _context.Temp_Payroll.Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode).
                        DeleteFromQuery();
                    _context.Payrun.Where(x => x.period == approvalDto.period && x.companyCode == approvalDto.companyCode).
                        DeleteFromQuery();

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    _msg.MsgCode = 'S';
                    _msg.Message = "Data Rollback Operation Completed Successfully";
                    return _msg;
                }
                else
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = $"Payrun status - {_payRun.payrunStatus} - cannot be rollback.";
                    return _msg;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError($"temp-data-rollback : {ex.Message}");
                _logger.LogError($"temp-data-rollback : {ex.InnerException}");
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
                if (companyCode == 0 || period == 0)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = $"Bad Request";
                    return _msg;
                }

                int _empCount = _context.Temp_Employee.Where(o => o.companyCode == companyCode && o.period == period).ToList().Count();

                IList _paySummaryList = _context.Temp_Payroll.Where(o => o.companyCode == companyCode && o.period == period).
                    GroupBy(p => new { p.payCode }).
                    Select(g => new
                    {
                        PayCode = g.Key.payCode,
                        Amount = g.Sum(o => o.amount),
                        Line_Item_Count = g.Count()
                    }).OrderBy(p => p.PayCode).ToList();

                IList _paySAPSummaryList = _context.SAPTotPayCode.Where(o => o.companyCode == companyCode && o.period == period).
                    GroupBy(p => new { p.payCode }).
                    Select(g => new
                    {
                        PayCode = g.Key.payCode,
                        Amount = g.Sum(o => o.totalAmount),
                        Line_Item_Count = g.Sum(o => o.totCount)
                    }).OrderBy(p => p.PayCode).ToList();

                if (_empCount == 0 && _paySummaryList.Count == 0 || _paySAPSummaryList.Count == 0)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Found";
                    return _msg;
                }

                var result = new List<object>();
                result.Add(new { empCount = _empCount, nonSAPPayData = _paySummaryList, SAPPayData = _paySAPSummaryList });

                _msg.Data = JsonConvert.SerializeObject(result);
                _msg.MsgCode = 'S';
                _msg.Message = "Request executed Successfully";

                return _msg;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetDataTransferStatistics : {ex.Message}");
                _logger.LogError($"GetDataTransferStatistics : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> PreparePayrun(ApprovalDto approvalDto)
        {
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
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
                transaction.Rollback();
                _logger.LogError($"PreparePayrun : {ex.Message}");
                _logger.LogError($"PreparePayrun : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }

        public async Task<MsgDto> OtherPaymentDataTransfer(string json)
        {
            int count = 0;
            MsgDto _msg = new MsgDto();
            using var transaction = BeginTransaction();
            try
            {
                IList<OtherPayment> _tempPaymentList = new List<OtherPayment>();

                DataSet _paymentDataTable = JsonConvert.DeserializeObject<DataSet>(json);

                if (_paymentDataTable == null || _paymentDataTable.Tables.Count == 0)
                {
                    _msg.MsgCode = 'E';
                    _msg.Message = "No Data Found";
                    return _msg;
                }
                foreach (DataRow _dataRow in _paymentDataTable.Tables[0].AsEnumerable())
                {
                    _tempPaymentList.Add(new OtherPayment()
                    {
                        epf = _dataRow["epf"].ToString().Substring(3),
                        paymentStatus = Data.EntityMapping.StatusMapper.PaymentStatus.Transferred,
                        empName = _dataRow["empName"].ToString(),
                        paymentCategory = _dataRow["empGrade"].ToString(),
                        amount = Convert.ToDecimal(_dataRow["gradeCode"]),
                        voucherNo = _dataRow["paymentType"].ToString(),
                        bankCode = _dataRow["bankCode"].ToString(),
                        accountNo = _dataRow["accountNo"].ToString(),
                        createdBy = _dataRow["createdBy"].ToString(),
                        createdDate = DateTime.Now,
                    });
                }

                _context.BulkCopy(_tempPaymentList);
                await _context.SaveChangesAsync();
                transaction.Commit();

                _msg.MsgCode = 'S';
                _msg.Message = "Data Transered Successfully";
                return _msg;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError($"DataTransfer : {ex.Message}");
                _logger.LogError($"DataTransfer : {ex.InnerException}");
                _msg.MsgCode = 'E';
                _msg.Message = "Error : " + ex.Message;
                _msg.Data = ex.Data.ToString();
                _msg.Description = "Inner Expection : " + ex.InnerException;
                return _msg;
            }
        }
                public ICollection<Temp_Employee> GetTempEmployeeList(int companyCode, int period)
        {
            ICollection<Temp_Employee> _tempEmpList = _context.Temp_Employee.Where(o => o.companyCode == companyCode && o.period == period).ToList();
            return _tempEmpList;
        }
    }
}
