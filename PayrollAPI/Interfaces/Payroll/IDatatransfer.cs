using PayrollAPI.DataModel;
using PayrollAPI.Models.Payroll;
using System.Data;

namespace PayrollAPI.Interfaces.Payroll
{
    public interface IDatatransfer
    {
        public Task<MsgDto> PayCodeCheck(int companyCode, int period);
        public Task<MsgDto> ConfirmDataTransfer(ApprovalDto approvalDto);

        public Task<MsgDto> PreparePayrun(ApprovalDto approvalDto);

        public Task<MsgDto> DataTransfer(string json);

        public Task<MsgDto> RollBackTempData(ApprovalDto approvalDto);
        public ICollection<Temp_Employee> GetTempEmployeeList(int companyCode, int period);

        public MsgDto GetDataTransferStatistics(int companyCode, int period);

        Task<MsgDto> OtherPaymentDataTransfer(string json);
        IDbTransaction BeginTransaction();
    }
}
