using PayrollAPI.DataModel;
using System.Data;

namespace PayrollAPI.Interfaces
{
    public interface IPayroll
    {
        public Task<MsgDto> ProcessPayroll(ApprovalDto approvalDto);

        public Task<MsgDto> ProcessPayrollbyEPF(string epf, int period, int companyCode);

        public Task<MsgDto> GetPayrollSummary(int period, int companyCode);

        public IDbTransaction BeginTransaction();
    }
}
