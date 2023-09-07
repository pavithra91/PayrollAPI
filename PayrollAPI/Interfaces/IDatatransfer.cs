using PayrollAPI.DataModel;

namespace PayrollAPI.Interfaces
{
    public interface IDatatransfer
    {
        public Task<MsgDto> ConfirmDataTransfer(ApprovalDto approvalDto);
    }
}
