using System.Data;
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
            var voucherPaymentList = await _context.OtherPayment.Where(x => x.voucherNo == voucherNo).ToListAsync();

            if(voucherPaymentList.Count == 0)
            {
                return await Task.FromResult(false);
            }

            _context.OtherPayment.Where(x => x.voucherNo == voucherNo).UpdateFromQuery(x => new OtherPayment { paymentStatus = PaymentStatus.Processed, bankTransferDate = processingDate, lastUpdateBy = processBy, lastUpdateDate = com.GetTimeZone().Date, lastUpdateTime = com.GetTimeZone() });

            await _context.SaveChangesAsync();



            transaction.Commit();

            return await Task.FromResult(true);
        }
    }
}
