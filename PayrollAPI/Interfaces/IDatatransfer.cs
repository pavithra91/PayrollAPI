using PayrollAPI.DataModel;
using System.Data;

namespace PayrollAPI.Interfaces
{
    public interface IDatatransfer
    {
        public Task<MsgDto> ConfirmDataTransfer(ApprovalDto approvalDto);

        public Task<MsgDto> PreparePayrun(ApprovalDto approvalDto);

        public Task<MsgDto> DataTransfer(string json);

        IDbTransaction BeginTransaction();
    }
}
