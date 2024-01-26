using PayrollAPI.DataModel;
using PayrollAPI.Models;
using System.Collections;
using System.Data;

namespace PayrollAPI.Interfaces
{
    public interface IDatatransfer
    {
        public Task<MsgDto> ConfirmDataTransfer(ApprovalDto approvalDto);

        public Task<MsgDto> PreparePayrun(ApprovalDto approvalDto);

        public Task<MsgDto> DataTransfer(string json);

        public Task<MsgDto> RollBackTempData(ApprovalDto approvalDto);
        public ICollection<Temp_Employee> GetTempEmployeeList(int companyCode, int period);

        public MsgDto GetDataTransferStatistics(int companyCode, int period);
        IDbTransaction BeginTransaction();
    }
}
