using PayrollAPI.Models.Payroll;
using PayrollAPI.Models.Reservation;

namespace PayrollAPI.Interfaces.Payment
{
    public interface IPayment
    {
        Task<IEnumerable<OtherPayment>> GetVoucherPayments(string voucherNo);
        Task<bool> ProcessVoucher(string voucherNo, DateTime processingDate, string processBy);
    }
}
