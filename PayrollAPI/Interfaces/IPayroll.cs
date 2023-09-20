using PayrollAPI.DataModel;
using System.Data;

namespace PayrollAPI.Interfaces
{
    public interface IPayroll
    {
        public Task<MsgDto> ProcessPayroll(ApprovalDto approvalDto);

        public IDbTransaction BeginTransaction();
    }
}
