using Leave.Contracts.Requests;
using Leave.Contracts.Response;
using Microsoft.AspNetCore.Mvc;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Interfaces.HRM
{
    public interface ILeave
    {
        Task<IEnumerable<LeaveType>> GetAllLeaveTypes();
        Task<LeaveType?> GetLeaveType(int id);
        Task<bool> CreateLeaveType(LeaveType leaveType);
        Task<bool> UpdateLeaveType(int id, LeaveType leaveType);
        Task<IEnumerable<LeaveType>> GetAvailableLeaveTypes(string epf);


        Task<IEnumerable<Supervisor>> GetAllSupervisors();
        Task<Supervisor?> GetSupervisor(int id);
        Task<bool> CreateSupervisor(Supervisor supervisor);
        Task<bool> UpdateSupervisor(int id, Supervisor supervisor);
        Task<IEnumerable<Supervisor>> GetMySupervisors(int epf);


        Task<ApprovalWorkflowResponse> GetApprovalWorkflow();

        Task<bool> AssignSupervisor(string epf, int approvalLevel, List<int> approverNames, string updateBy);
        //Task<bool> CheckLeaveBalance(RequestLeaveRequest request);
        Task<(bool Success, string Message)> RequestLeave(RequestLeaveRequest request);
        Task<bool> ApproveLeave(ApproveLeaveRequest request);
        Task<bool> CancelLeave(CancelLeaveRequest request);

        Task<IEnumerable<LeaveApproval?>> GetLeaveApproval(int id);
        Task<IEnumerable<LeaveApproval?>> GetLeaveApprovals(int id);
        Task<LeaveRequest?> GetLeaveRequest(int id);

        Task<IEnumerable<LeaveRequest>> GetLeaveRequestHistory(int epf);

        Task<IEnumerable<LeaveBalance>> GetLeaveBalance(int epf); 
        Task<IEnumerable<Notification>> GetNotifications(int epf);
        Task<bool> ReadNotification(int notificationId);
    }
}
