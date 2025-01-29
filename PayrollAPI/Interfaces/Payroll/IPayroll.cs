using PayrollAPI.DataModel;
using System.Data;

namespace PayrollAPI.Interfaces.Payroll
{
    public interface IPayroll
    {
        public Task<MsgDto> SimulatePayroll(ApprovalDto approvalDto);
        public Task<MsgDto> ProcessPayroll(ApprovalDto approvalDto);
        public Task<MsgDto> CreateUnrecoveredFile(ApprovalDto approvalDto);
        public Task<MsgDto> ProcessPayrollbyEPF(string epf, int period, int companyCode);

        public Task<MsgDto> GetPayrollSummary(int period, int companyCode);

        public Task<MsgDto> GetPaySheet(string epf, int period);

        public Task<MsgDto> PrintPaySheets(int companyCode, int period);

        public Task<MsgDto> GetPayrunDetails();

        public Task<MsgDto> GetPayrunDetails(int period, int CompanyCode);

        public Task<MsgDto> Writeback(int period, int companyCode);

        public Task<MsgDto> GetEmployeePaySheet(ApprovalDto approvalDto);

        public Task<MsgDto> ResendPaySheets(ApprovalDto approvalDto);

        public Task<MsgDto> CreateBankFile(ApprovalDto approvalDto);
        public IDbTransaction BeginTransaction();

        public void CheckLogger();
    }
}
