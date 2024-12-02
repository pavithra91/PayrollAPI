using Leave.Contracts.Requests;
using Leave.Contracts.Response;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.DataModel.HRM
{
    public static class ContractMapping
    {
        #region Leave Types
        public static LeaveType MapToLeaveType(this LeaveTypeRequest request)
        {
            return new LeaveType
            {
                leaveTypeName = request.leaveTypeName,
                description = request.description,
                maxDays = request.maxDays,
                carryForwardAllowed = request.carryForwardAllowed,
                createdBy = request.createdBy,
            };
        }

        public static LeaveType MapToLeaveType(this UpdateLeaveTypeRequest request, int id)
        {
            return new LeaveType
            {
                leaveTypeId = id,
                leaveTypeName = request.leaveTypeName,
                description = request.description,
                maxDays = request.maxDays,
                carryForwardAllowed = request.carryForwardAllowed,
                lastUpdateBy = request.createdBy,
                lastUpdateDate = DateTime.Now.Date,
                lastUpdateTime = DateTime.Now,
            };
        }

        public static LeaveTypeResponse MapToResponse(this LeaveType leaveType)
        {
            return new LeaveTypeResponse
            {
                leaveTypeId = leaveType.leaveTypeId,
                leaveTypeName = leaveType.leaveTypeName,
                description = leaveType.description,
                maxDays = leaveType.maxDays,
                carryForwardAllowed = leaveType.carryForwardAllowed,
                createdBy = leaveType.createdBy,
            };
        }

        public static LeaveTypesResponse MapToResponse(this IEnumerable<LeaveType> leaveTypes)
        {
            return new LeaveTypesResponse
            {
                Items = leaveTypes.Select(MapToResponse)
            };
        }

        #endregion

        #region Supervisor
        public static Supervisor MapToSupervisor(this SupervisorRequest request)
        {
            return new Supervisor
            {
                userId = request.userId,
                epf = request.epf,
                isActive = true,
                createdBy = request.createdBy,
            };
        }

        public static Supervisor MapToSupervisor(this UpdateSupervisorRequest request, int id)
        {
            return new Supervisor
            {
                id = id,
                isActive = request.isActive,
                lastUpdateBy = request.createdBy,
                lastUpdateDate = DateTime.Now.Date,
                lastUpdateTime = DateTime.Now,
            };
        }

        public static SupervisorResponse MapToResponse(this Supervisor supervisor)
        {
            return new SupervisorResponse
            {
                id = supervisor.id,
                userId = supervisor.userId,
                epf = supervisor.epf,
                createdBy = supervisor.createdBy,
            };
        }

        public static SupervisorsResponse MapToResponse(this IEnumerable<Supervisor> supervisors)
        {
            return new SupervisorsResponse
            {
                Items = supervisors.Select(MapToResponse)
            };
        }

        #endregion


        public static ApprovalWorkflowResponse MapToApprovalWorkflow(this IEnumerable<EmpApprovals> request, IEnumerable<EmpApprovalWorkflow> workflow)
        {
            //List<ApprovalWorkflowResponse> list = new List<ApprovalWorkflowResponse>();
            //foreach (EmpApprovals empApprovals in request)
            //{
            //    ApprovalWorkflowResponse approval = new ApprovalWorkflowResponse();
            //    approval.epf = empApprovals.epf;
            //    approval.supervisorList = workflow.Where(x=>x.empApprovalId.id == empApprovals.id).ToList();
            //}
            return null;
        }

        public static LeaveApprovalResponse MapToResponse(this LeaveApproval leaveApproval)
        {
            return new LeaveApprovalResponse
            {
                approver = leaveApproval.approver_id.epf,
                levelName = leaveApproval.level.levelName,
                status = leaveApproval.status.ToString(),
            };
        }


        public static LeaveRequestResponse MapToResponse(this LeaveRequest leaveRequest)
        {
            return new LeaveRequestResponse
            {
                requestId = leaveRequest.leaveRequestId,
                epf = Convert.ToString(leaveRequest.epf),
                leaveTypeName = leaveRequest.leaveType.leaveTypeName,
                startDate = leaveRequest.startDate,
                endDate = leaveRequest.endDate,
                lieuLeaveDate = leaveRequest.lieuLeaveDate,
                isHalfDay = leaveRequest.isHalfDay,
                reason = leaveRequest.reason,
                currentLevel = leaveRequest.currentLevel,
                requestStatus = leaveRequest.requestStatus.ToString(),
                actingDelegate = leaveRequest.actingDelegate,
                actingDelegateApprovalStatus = leaveRequest.actingDelegateApprovalStatus.ToString(),
            };
        }

        public static LeaveHistoryResponse MapToResponse(this IEnumerable<LeaveRequest> leaveRequests)
        {
            return new LeaveHistoryResponse
            {
                Items = leaveRequests.Select(MapToResponse)
            };
        }

        public static LeaveRequestApprovalResponse MapToResponse(this LeaveRequest leaveRequest, IEnumerable<LeaveApproval>  leaveApprovals)
        {
            return new LeaveRequestApprovalResponse
            {
                leaveRequest = leaveRequest.MapToResponse(),
                Approvals = leaveApprovals.Select(MapToResponse)
            };
        }

        public static LeaveRequestApprovalListResponses MapToLeaveApprovalList(this IEnumerable<LeaveApproval> leaveApprovals)
        {
            List<LeaveRequestApprovalListResponse> list = new List<LeaveRequestApprovalListResponse>();

            foreach (var items in leaveApprovals)
            {
                LeaveRequestApprovalListResponse leaveRequest = new LeaveRequestApprovalListResponse
                {
                    epf = items.epf,
                    //leaveTypeName = items.requestId.leaveType.leaveTypeName,
                    startDate = items.requestId.startDate,
                    endDate = items.requestId.endDate,
                    actingDelegate = items.requestId.actingDelegate,
                    actingDelegateApprovalStatus = items.requestId.actingDelegateApprovalStatus.ToString(),
                    currentLevel = items.requestId.currentLevel,
                    halfDayType = items.requestId.halfDayType.ToString(),
                    reason = items.requestId.reason,
                    isHalfDay = items.requestId.isHalfDay,
                    lieuLeaveDate = items.requestId.lieuLeaveDate,
                    requestId = items.requestId.leaveRequestId,
                    requestStatus = items.requestId.requestStatus.ToString(),
                    approverStatus = items.status.ToString(),
                    noOfDays = items.requestId.noOfDays,

                };

                list.Add(leaveRequest);
            }

            return new LeaveRequestApprovalListResponses
            {
                items = list
            };
        }



        public static LeaveBalanceResponse MapToResponse(this LeaveBalance leaveBalance)
        {
            return new LeaveBalanceResponse
            {
                epf = Convert.ToString(leaveBalance.epf),
                year = leaveBalance.year,
                leaveTypeName = leaveBalance.leaveType.leaveTypeName,                
                allocatedLeave = leaveBalance.allocatedLeaves,
                usedLeave = leaveBalance.usedLeaves,
                remainingLeave = leaveBalance.remainingLeaves,
                carrForwardLeave = leaveBalance.carryForwardLeaves,
            };
        }

        public static LeaveDashboardResponse MapToResponse(this IEnumerable<LeaveRequest> leaveRequests, IEnumerable<LeaveBalance> leaveBalances)
        {
            return new LeaveDashboardResponse
            {
                LeaveHistory = leaveRequests.Select(MapToResponse),
                LeaveBalance = leaveBalances.Select(MapToResponse)
            };
        }

        public static NotificationResponse MapToResponse(this Notification notification)
        {
            return new NotificationResponse
            {
                id = notification.id,
                epf = notification.epf,
                description = notification.description,
                reference = notification.reference,
                target = notification.target,
                type = notification.type,
                date = notification.createdDate,
                status = notification.status,
                readed = notification.markAsRead,
            };
        }

        public static NotificationsResponse MapToResponse(this IEnumerable<Notification> notifications)
        {
            return new NotificationsResponse
            {
                Items = notifications.Select(MapToResponse)
            };
        }

        public static AdvancePayment MapToAdvancePayment(this AdvancePaymentRequest request)
        {
            return new AdvancePayment
            {
                epf = request.epf,
                period = Convert.ToInt32(DateTime.Now.Year + "" + DateTime.Now.Month),
                isFullAmount = request.isFullAmount,
                amount = request.isFullAmount ? 0 : request.amount,
                status = ApprovalStatus.Pending,
                createdBy = request.createdBy,
            };
        }


        #region Employee
        public static EmployeeResponse MapToResponse(this Employee employee)
        {
            return new EmployeeResponse
            {
                id = employee.id,
                userID = employee.userID,
                epf = employee.epf,
                companyCode = employee.companyCode,
                costCenter = employee.costCenter,
                empName = employee.empName,
                grade = employee.empGrade.gradeCode,
                isActive = employee.status,
                role = employee.role,
            };
        }

        public static EmployeesResponse MapToResponse(this IEnumerable<Employee> employees)
        {
            return new EmployeesResponse
            {
                Items = employees.Select(MapToResponse)
            };
        }
        #endregion
    }
}
