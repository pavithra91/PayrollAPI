using System.Data;
using System.Text;
using LinqToDB;
using Microsoft.EntityFrameworkCore.Storage;
using PayrollAPI.Data;
using PayrollAPI.Interfaces.Payment;
using PayrollAPI.Models.Payroll;
using PayrollAPI.Models.Reservation;
using PayrollAPI.Services;
using static PayrollAPI.Data.EntityMapping.StatusMapper;

namespace PayrollAPI.Repository.Payment
{
    public class PaymentRepository: IPayment
    {
        private readonly DBConnect _context;
        Common com = new Common();
        public PaymentRepository(DBConnect db)
        {
            _context = db;
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();

            return transaction.GetDbTransaction();
        }

        public async Task<IEnumerable<OtherPayment>> GetVoucherPayments(string voucherNo)
        {
            return await Task.FromResult(_context.OtherPayment.Where(x => x.voucherNo == voucherNo)
                .AsEnumerable());
        }

        public async Task<bool> ProcessVoucher(string voucherNo, DateTime processingDate, string processBy)
        {
            using var transaction = BeginTransaction();
            try
            {
                var voucherPaymentList = await _context.OtherPayment.Where(x => x.voucherNo == voucherNo).ToListAsync();

                if (voucherPaymentList.Count == 0)
                {
                    return await Task.FromResult(false);
                }

                _context.OtherPayment.Where(x => x.voucherNo == voucherNo).UpdateFromQuery(x => new OtherPayment { paymentStatus = PaymentStatus.Processed, bankTransferDate = processingDate, lastUpdateBy = processBy, lastUpdateDate = com.GetTimeZone().Date, lastUpdateTime = com.GetTimeZone() });

                await _context.SaveChangesAsync();

                ICollection<Sys_Properties> _sysProperties = _context.Sys_Properties.Where(o => o.groupName == "Company_Bank_Details").ToList();

                string formattedString = "";
                string _comp_BankCode = _sysProperties.Where(o => o.variable_name == "Bank_Code").FirstOrDefault().variable_value;
                string _comp_Branch_Code = _sysProperties.Where(o => o.variable_name == "Branch_Code").FirstOrDefault().variable_value;
                string _comp_Account_No = _sysProperties.Where(o => o.variable_name == "Account_No").FirstOrDefault().variable_value;
                string _comp_Account_Name = _sysProperties.Where(o => o.variable_name == "Account_Name").FirstOrDefault().variable_value;

                foreach (var item in voucherPaymentList)
                {
                    string amount = item.amount.ToString().Replace(".", "");
                    amount.Count();
                    DateTime now = DateTime.Now;

                    string date = now.ToString("yyMMdd");
                    formattedString += string.Format(
                    "{0,4}{1,7}{2,-12}{3,-20}23{4,21}SLR{5,4}{6:3}{7, -12}{8, -50}{9, 6}{10,6}\n",
                    "0000", 
                    item.bankCode, 
                    item.accountNo.PadLeft(12, '0'), 
                    item.empName.Trim(), amount.PadLeft(21, '0'), 
                    _comp_BankCode, 
                    _comp_Branch_Code, 
                    _comp_Account_No.PadLeft(12, '0'), 
                    _comp_Account_Name, 
                    date, 
                    "000000");
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(formattedString);

                ICollection<Sys_Properties> properties = _context.Sys_Properties.Where(o => o.groupName == "Email_Config" || o.groupName == "Email").ToList();

                EmailSender emailSender = new EmailSender();
                emailSender.SendEmail(properties, byteArray, voucherNo + ".txt", "text/plain");

                transaction.Commit();

                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return await Task.FromResult(false);
            }
        }
    }
}
