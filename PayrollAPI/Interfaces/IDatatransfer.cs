using PayrollAPI.DataModel;
using System.Data;

namespace PayrollAPI.Interfaces
{
    public interface IDatatransfer
    {
        public Task<MsgDto> ConfirmDataTransfer(ApprovalDto approvalDto);

        IDbTransaction BeginTransaction();
    }
}
