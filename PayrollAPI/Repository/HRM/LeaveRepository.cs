using Leave.Contracts.Requests;
using Leave.Contracts.Response;
using LinqToDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PayrollAPI.Data;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Models.HRM;

namespace PayrollAPI.Repository.HRM
{
    public class LeaveRepository : ILeave
    {
        private readonly HRMDBConnect _context;
        public LeaveRepository(HRMDBConnect db)
        {
            _context = db;
         }

        public async Task<IEnumerable<LeaveType>> GetAllLeaveTypes()
        {
            return await Task.FromResult(_context.LeaveType.AsEnumerable());
        }

        public async Task<LeaveType?> GetLeaveType(int id)
        {
            var _leaveType = _context.LeaveType.SingleOrDefault(x=>x.leaveTypeId == id);
            return await Task.FromResult(_leaveType);
        }

        public async Task<bool> CreateLeaveType(LeaveType leaveType)
        {
            _context.LeaveType.Add(leaveType);
            await _context.SaveChangesAsync();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateLeaveType(int id, LeaveType leavetype)
        {
            var existingLeaveType = await _context.LeaveType.FindAsync(id);

            if (existingLeaveType is null)
            {
                return await Task.FromResult(false);
            }
            else
            {
                existingLeaveType.leaveTypeName = leavetype.leaveTypeName;
                existingLeaveType.description = leavetype.description;
                existingLeaveType.maxDays = leavetype.maxDays;
                existingLeaveType.carryForwardAllowed = leavetype.carryForwardAllowed;
                existingLeaveType.lastUpdateBy = leavetype.lastUpdateBy;
                existingLeaveType.lastUpdateDate = leavetype.lastUpdateDate;
                existingLeaveType.lastUpdateTime = leavetype.lastUpdateTime;

                await _context.SaveChangesAsync();

                return await Task.FromResult(true);
            }
        }

        public async Task<IEnumerable<LeaveType>> GetAvailableLeaveTypes(string epf)
        {
            return await Task.FromResult(_context.LeaveBalance.Include(x=>x.leaveType)
                .Where(x=>x.epf == Convert.ToInt32(epf) && x.remainingLeaves > 0).Select(x=>x.leaveType).AsEnumerable());
        }

        public async Task<IEnumerable<Supervisor>> GetAllSupervisors()
        {
            return await Task.FromResult(_context.Supervisor
                .Include(x=>x.epf)
                .Include(x=>x.epf.empGrade)
                .AsEnumerable());
        }

        public async Task<Supervisor?> GetSupervisor(int id)
        {
            var _supervisor = _context.Supervisor.SingleOrDefault(x => x.id == id);
            return await Task.FromResult(_supervisor);
        }

        public async Task<IEnumerable<Supervisor>> GetMySupervisors(int epf)
        {
            Employee? employee = _context.Employee.Where(x=>x.epf == epf.ToString()).FirstOrDefault();
            return await Task.FromResult(_context.Supervisor
                .Include(x=>x.epf)
                .Include(x=>x.epf.empGrade)
                .Where(x=>x.epf.costCenter == employee.costCenter && x.isActive == true)
                .AsEnumerable());
        }

        public async Task<ApprovalWorkflowResponse> GetApprovalWorkflow()
        {
         var empApprovals =   _context.EmpApprovals
        .Include(e=>e.employee)
            .ThenInclude(e=>e.workflowLevel)
        .Include(e => e.employee)
            .ThenInclude(e => e.empGrade)
        .Include(e => e.approvalWorkflowsId)
            .ThenInclude(w => w.approvalLevels)
        .Include(e => e.approvalWorkflowsId)
            .ThenInclude(w => w.approverId).ThenInclude(x=>x.epf)
        .ToList();

            var result = empApprovals.Select(e => new ApprovalWorkflowDto
            {
                id = e.id,
                epf = e.employee.epf,
                empName = e.employee.empName,
                empGrade = e.employee.empGrade.gradeCode,
                approvalLevel = e.employee.workflowLevel.levelName,
                SupervisorList = e.approvalWorkflowsId.Select(w => new SupervisorDto
                {
                    Level = w.approvalLevels != null ? w.approvalLevels.levelName : null,
                    Epf = w.approverId != null ? int.Parse(w.approverId.epf.epf) : 0,
                    empName = w.approverId != null ? w.approverId.epf.empName : ""
                }).ToList(),
            });

            var response = new ApprovalWorkflowResponse
            {
                total = result.Count(),
                data = result
            };

            return await Task.FromResult(response);
        }


        public async Task<bool> AssignSupervisor(string epf, int approvalLevel, List<int> approverNames, string updateBy)
        {
            try
            {
                var _empApprovals = _context.EmpApprovals
                    .Include(x => x.employee)
                    .Include(x => x.approvalWorkflowsId)
                    .Where(x => x.employee.epf == epf)
                    .FirstOrDefault();
                if (_empApprovals != null)
                {
                    _empApprovals.level = approvalLevel;
                    _empApprovals.lastUpdateBy = updateBy;
                    _empApprovals.lastUpdateDate = DateTime.Now.Date;
                    _empApprovals.lastUpdateTime = DateTime.Now;

                    var relatedWorkflows = _context.EmpApprovalWorkflow
                    .Where(w => w.empApprovalId.id == _empApprovals.id);

                    _context.EmpApprovalWorkflow.RemoveRange(relatedWorkflows);

                    var supervisors = _context.Supervisor.ToList();
                    var levels = _context.WorkflowTypes.ToList();
                    int count = 1;

                    foreach (var workflow in approverNames)
                    {
                        EmpApprovalWorkflow empApprovalWorkflow = new EmpApprovalWorkflow
                        {
                            empApprovalId = _empApprovals,
                            approvalLevels = levels.Find(x => x.id == count),
                            approverId = supervisors.Find(x => x.id == workflow)
                        };

                        _context.EmpApprovalWorkflow.Add(empApprovalWorkflow);
                        count++;
                    }

                    await _context.SaveChangesAsync();

                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
            catch(Exception ex)
            {
                return await Task.FromResult(false);
            }
        }

        //public async Task<bool> CheckLeaveBalance(RequestLeaveRequest request)
        //{
        //    LeaveBalance leaveBalance = _context.LeaveBalance.Where(x => x.leaveType.leaveTypeId == Convert.ToInt32(request.leaveType)).FirstOrDefault();

        //    if (leaveBalance == null || leaveBalance.remainingLeaves == 0)
        //    {
        //        return await Task.FromResult(false);
        //    }
        //    else if ((leaveBalance.remainingLeaves - request.noOfDays) < 0)
        //    {
        //        return await Task.FromResult(false);
        //    }

        //    return await Task.FromResult(true);
        //}
        public async Task<(bool Success, string Message)> RequestLeave(RequestLeaveRequest request)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction()) 
            {
                try
                {
                    LeaveBalance? leaveBalance = _context.LeaveBalance
                        .Include(x=>x.leaveType)
                        .Where(x => x.leaveType.leaveTypeId == Convert.ToInt32(request.leaveType) && x.epf == Convert.ToInt32(request.epf)).FirstOrDefault();

                    if (leaveBalance == null || leaveBalance.remainingLeaves == 0)
                    {
                        return await Task.FromResult((false, "You don't have assinged leave"));
                    }
                    else if ((leaveBalance.remainingLeaves - request.noOfDays) < 0)
                    {
                        return await Task.FromResult((false, $"You don't have {leaveBalance.leaveType.description} left"));
                    }

                    LeaveRequest _leaveRequest = new LeaveRequest();
                    _leaveRequest.epf = Convert.ToInt32(request.epf);
                    _leaveRequest.leaveType = _context.LeaveType.Where(x=>x.leaveTypeId == Convert.ToInt32(request.leaveType)).FirstOrDefault();
                    _leaveRequest.startDate = request.startDate;
                    _leaveRequest.endDate = request.endDate;
                    _leaveRequest.isHalfDay = request.isHalfDay;
                    _leaveRequest.noOfDays = request.noOfDays;

                    if (request.isHalfDay)
                    {
                        _leaveRequest.halfDayType = request.halfDayType == "1" ? 
                            HalfDayType.Morning : 
                            HalfDayType.Evening;
                    }
                    if (request.lieuLeaveDate != null)
                    {
                        _leaveRequest.lieuLeaveDate = _leaveRequest.lieuLeaveDate;
                    }

                    if(request.actDelegate != "None")
                    {
                        _leaveRequest.actingDelegate = request.actDelegate;
                        _leaveRequest.actingDelegateApprovalStatus = ApprovalStatus.Pending;
                    }
                    else
                    {
                        _leaveRequest.actingDelegate = "None";
                    }

                    _leaveRequest.requestStatus = ApprovalStatus.Pending;
                    _leaveRequest.actionedBy = request.requestBy;
                    _leaveRequest.reason = request.reason;

                    _context.LeaveRequest.Add(_leaveRequest);

                    await _context.SaveChangesAsync();

                    Employee emp = _context.Employee.Where(x => x.epf == request.epf).FirstOrDefault();

                    var empApproval = _context.EmpApprovals
                        .Include(e=>e.employee)
                        .Where(x => x.employee == emp)
                       .Include(e => e.approvalWorkflowsId)
                       .ThenInclude(w => w.approvalLevels)
                       .Include(e => e.approvalWorkflowsId)
                       .ThenInclude(w => w.approverId)
                       .ThenInclude(w=>w.epf).FirstOrDefault();

                    var query = _context.EmpApprovals
                        .Include(e => e.employee)
                        .Where(x => x.employee == emp)
                       .Include(e => e.approvalWorkflowsId)
                       .ThenInclude(w => w.approvalLevels)
                       .Include(e => e.approvalWorkflowsId)
                       .ThenInclude(w => w.approverId)
                       .ThenInclude(w => w.epf).ToQueryString();

                    if (request.actDelegate != "None")
                    {
                        Notification notification = new Notification
                        {
                            epf = Convert.ToInt32(_leaveRequest.actingDelegate),
                            target = request.epf,
                            description = "has request you to become an acting Delegate",
                            createdDate = DateTime.Now,
                            markAsRead = false,
                            type = 0,
                            reference = _leaveRequest.leaveRequestId.ToString()
                        };

                        _context.Notification.Add(notification);
                    }

                    foreach (var item in empApproval.approvalWorkflowsId)
                    {
                        var x = item.approvalLevels.levelName;
                        var y = item.approverId.epf;
                        var z = item.empApprovalId.level;

                        if("Level " + empApproval.approvalWorkflowsId.Count == item.approvalLevels.levelName)
                        {
                            LeaveRequest? _managerLeave = _context.LeaveRequest
                                .Where(x=>x.epf == Convert.ToInt32(item.approverId.epf.epf) && 
                                x.startDate.Date <= DateTime.Today && x.endDate.Date >= DateTime.Today)
                                .FirstOrDefault();

                            if (_managerLeave != null)
                            {
                                var _employee = _context.Employee.Where(x => x.epf == _managerLeave.actingDelegate).FirstOrDefault();
                                var _supervisor = _context.Supervisor.Where(x => x.epf == _employee).FirstOrDefault();
                                if (_supervisor == null) 
                                {
                                    Supervisor tempSupervisor = new Supervisor
                                    {
                                        isActive = true,
                                        isTempSupervisor = true,
                                        epf = _employee,
                                        createdBy = "System",
                                        createdDate= DateTime.Now,
                                        createdTime = DateTime.Now,
                                        expireDate = _managerLeave.endDate,
                                        isManager = true,
                                        userId = _managerLeave.epf.ToString()
                                    };

                                    _context.Supervisor.Add(tempSupervisor);
                                    await _context.SaveChangesAsync();

                                    _supervisor = tempSupervisor;
                                }

                                LeaveApproval _forwardApproval = new LeaveApproval
                                {
                                    requestId = _leaveRequest,
                                    epf = request.epf,
                                    approver_id = _supervisor,
                                    level = item.approvalLevels,
                                    tempApproval = true,
                                    status = ApprovalStatus.Pending,
                                };

                                Notification _forwardNotifications = new Notification
                                {
                                    epf = Convert.ToInt32(_supervisor.epf.epf),
                                    target = request.epf,
                                    description = "has send a leave request",
                                    createdDate = DateTime.Now,
                                    markAsRead = false,
                                    type = 0,
                                    reference = _leaveRequest.leaveRequestId.ToString()
                                };

                                _context.LeaveApproval.Add(_forwardApproval);
                                _context.Notification.Add(_forwardNotifications);
                            }
                            else
                            {
                                LeaveApproval _approval = new LeaveApproval
                                {
                                    requestId = _leaveRequest,
                                    epf = request.epf,
                                    approver_id = item.approverId,
                                    level = item.approvalLevels,
                                    status = ApprovalStatus.Pending,
                                };

                                Notification notifications = new Notification
                                {
                                    epf = Convert.ToInt32(item.approverId.epf.epf),
                                    target = request.epf,
                                    description = "has send a leave request",
                                    createdDate = DateTime.Now,
                                    markAsRead = false,
                                    type = 0,
                                    reference = _leaveRequest.leaveRequestId.ToString()
                                };

                                _context.Notification.Add(notifications);
                                _context.LeaveApproval.Add(_approval);
                            }
                        }
                        else
                        {
                            LeaveApproval _approval = new LeaveApproval
                            {
                                requestId = _leaveRequest,
                                epf = request.epf,
                                approver_id = item.approverId,
                                level = item.approvalLevels,
                                status = ApprovalStatus.Pending,
                            };

                            Notification notifications = new Notification
                            {
                                epf = Convert.ToInt32(item.approverId.epf.epf),
                                target = request.epf,
                                description = "has send a leave request",
                                createdDate = DateTime.Now,
                                markAsRead = false,
                                type = 0,
                                reference = _leaveRequest.leaveRequestId.ToString()
                            };

                            _context.Notification.Add(notifications);
                            _context.LeaveApproval.Add(_approval);
                        }
                    }

                    leaveBalance.remainingLeaves = leaveBalance.remainingLeaves - request.noOfDays;
                    leaveBalance.usedLeaves = leaveBalance.usedLeaves + request.noOfDays;

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return await Task.FromResult((true, "You request has been send for approval"));
                }
                catch (Exception ex) 
                {
                    transaction.Rollback();
                    return await Task.FromResult((false, $"Error Ouccered: {ex.Message}"));
                }
            }                
        }

        public async Task<bool> ApproveLeave(ApproveLeaveRequest request)
        {
            LeaveRequest? leaveRequest = _context.LeaveRequest.FirstOrDefault(x => x.leaveRequestId == request.requestId);
            if (leaveRequest == null || leaveRequest.finalStatus != null) {
                return await Task.FromResult(false);
            }

            if(leaveRequest.actingDelegateApprovalStatus != ApprovalStatus.Pending && request.isDelegate)
            {
                return await Task.FromResult(false);
            }

            Notification notification = _context.Notification
                    .Where(x => x.epf == Convert.ToInt32(request.approver) && x.reference == leaveRequest.leaveRequestId.ToString()).FirstOrDefault();

            if (request.isDelegate && request.status == "Approved") 
            {
                leaveRequest.actingDelegateApprovalStatus = ApprovalStatus.Approved;
                leaveRequest.actingDelegateApprovedDate = DateTime.Now.Date;
                leaveRequest.actingDelegateApprovedTime = DateTime.Now;
 
                notification.markAsRead = true;


                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            else  if(request.isDelegate && request.status == "Rejected")
            {
                leaveRequest.actingDelegateApprovalStatus = ApprovalStatus.Rejected;
                leaveRequest.actingDelegateApprovedDate = DateTime.Now.Date;
                leaveRequest.actingDelegateApprovedTime = DateTime.Now;

                notification.markAsRead = true;

                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            else
            {
                List<LeaveApproval> leaveApprovals = _context.LeaveApproval
                    .Include(x=>x.level)
                    .Include(x=>x.approver_id)
                    .ThenInclude(x=>x.epf)
                    .Where(x=> x.requestId.leaveRequestId == request.requestId).ToList();

                LeaveApproval? leaveApproval = leaveApprovals.Find(x => x.approver_id.epf.epf == request.approver);

                FinalStatus finalStatus = FinalStatus.Cancelled;
                ApprovalStatus approvalStatus = ApprovalStatus.Pending;

                if (request.status == "Approved")
                {
                    leaveApproval.status = ApprovalStatus.Approved;
                    finalStatus = FinalStatus.Approved;
                    approvalStatus = ApprovalStatus.Approved;
                }
                else
                {
                    leaveApproval.status = ApprovalStatus.Rejected;
                    finalStatus = FinalStatus.Rejected;
                    approvalStatus = ApprovalStatus.Rejected;

                    LeaveBalance? leaveBalance = _context.LeaveBalance
                        .Include(x => x.leaveType)
                        .Where(x => x.leaveType.leaveTypeId == leaveRequest.leaveType.leaveTypeId && x.epf == Convert.ToInt32(leaveRequest.epf)).FirstOrDefault();

                    leaveBalance.remainingLeaves = leaveBalance.remainingLeaves + leaveRequest.noOfDays.GetValueOrDefault();
                    leaveBalance.usedLeaves = leaveBalance.usedLeaves - leaveRequest.noOfDays.GetValueOrDefault();
                }

                leaveApproval.comments = request.comment;
                leaveApproval.lastUpdateBy = request.approver;
                leaveApproval.lastUpdateDate = DateTime.Now.Date;
                leaveApproval.lastUpdateTime = DateTime.Now.TimeOfDay;

                leaveRequest.currentLevel = leaveApproval.level.id;

                if (leaveRequest.currentLevel == leaveApprovals.Count)
                {
                    leaveRequest.finalStatus = finalStatus;
                    leaveRequest.requestStatus = approvalStatus;

                    // Final approval notification
                    Notification notifications = new Notification
                    {
                        epf = leaveRequest.epf,                        
                        description = $"Your leave request has been {finalStatus}.",
                        createdDate = DateTime.Now,
                        markAsRead = false,
                        status = finalStatus.ToString(),
                        type = 2,
                        reference = request.requestId.ToString(),
                    };

                    _context.Notification.Add(notifications);
                }

                leaveRequest.lastUpdateBy = request.approver;
                leaveRequest.lastUpdateDate = DateTime.Now.Date;
                leaveRequest.lastUpdateTime = DateTime.Now;

                notification.markAsRead = true;

                await _context.SaveChangesAsync();
                return await Task.FromResult(true);
            }
        }

        public async Task<bool> CancelLeave(CancelLeaveRequest request)
        {
            LeaveRequest? leaveRequest = _context.LeaveRequest
                .FirstOrDefault(x=>x.leaveRequestId == request.leaveRequestId);
            if (leaveRequest == null || leaveRequest.finalStatus != null || leaveRequest.currentLevel != 0)
            {
                return await Task.FromResult(false);
            }

            List<LeaveApproval> leaveApprovals = _context.LeaveApproval
                    .Include(x => x.level)
                    .Include(x => x.approver_id)
                    .Where(x => x.requestId.leaveRequestId == request.leaveRequestId).ToList();

            foreach(LeaveApproval leaveApproval in leaveApprovals) 
            {
                leaveApproval.status = ApprovalStatus.Cancelled;
                leaveApproval.lastUpdateBy = request.cancelBy;
                leaveApproval.lastUpdateDate = DateTime.Now.Date;
                leaveApproval.lastUpdateTime = DateTime.Now.TimeOfDay;
            }

            leaveRequest.requestStatus = ApprovalStatus.Cancelled;
            leaveRequest.finalStatus = FinalStatus.Cancelled;
            leaveRequest.lastUpdateBy = request.cancelBy;
            leaveRequest.lastUpdateDate = DateTime.Now.Date;
            leaveRequest.lastUpdateTime = DateTime.Now;

            LeaveBalance? leaveBalance = _context.LeaveBalance
                        .Include(x => x.leaveType)
                        .Where(x => x.leaveType.leaveTypeId == leaveRequest.leaveType.leaveTypeId && x.epf == Convert.ToInt32(leaveRequest.epf)).FirstOrDefault();

            leaveBalance.remainingLeaves = leaveBalance.remainingLeaves + leaveRequest.noOfDays.GetValueOrDefault();
            leaveBalance.usedLeaves = leaveBalance.usedLeaves - leaveRequest.noOfDays.GetValueOrDefault();

            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        public async Task<IEnumerable<LeaveApproval?>> GetLeaveApproval(int id)
        {
            return await Task.FromResult(_context.LeaveApproval.Where(x => x.requestId.leaveRequestId == id)
                .Include(x => x.level)
                .Include(x => x.approver_id)
                .ThenInclude(x=>x.epf)
                .AsEnumerable());
        }

        public async Task<IEnumerable<LeaveApproval?>> GetLeaveApprovals(int id)
        {
            return await Task.FromResult(_context.LeaveApproval.Where(x => x.approver_id.epf.epf == id.ToString())
                .Include(x=> x.requestId)
                .Include(x => x.level)
                .Include(x => x.approver_id)
                .OrderByDescending(x=>x.id)
                .AsEnumerable());
        }

        // Backgroud Job
        public async Task<bool> ClearTempApprovals()
        {
            try
            {
                var leaveApprovals = _context.LeaveApproval.Where(x => x.tempApproval == true && x.status == ApprovalStatus.Pending)
                    .Include(x => x.requestId)
                    .Include(x => x.approver_id)
                    .ToList();

                if(leaveApprovals.Count == 0)
                {
                    return await Task.FromResult(true);
                }

                foreach (var item in leaveApprovals)
                {
                    if (item.approver_id.expireDate == DateTime.Today)
                    {
                        if (item.approver_id.isTempSupervisor == true)
                        {
                            _context.Supervisor.Remove(item.approver_id);
                        }

                        var _notification = _context.Notification.Where(x => x.reference == item.requestId.leaveRequestId.ToString() && x.epf == Convert.ToInt32(item.approver_id.epf.epf))
                            .FirstOrDefault();
                        _context.Notification.Remove(_notification);
                        _context.LeaveApproval.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                }

                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<LeaveRequest?> GetLeaveRequest(int id)
        {
            var _leaveRequest = _context.LeaveRequest
                .Include(x=>x.leaveType)
                .SingleOrDefault(x => x.leaveRequestId == id);
            return await Task.FromResult(_leaveRequest);
        }
        public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestHistory(int epf)
        {
            return await Task.FromResult(_context.LeaveRequest.Where(x=>x.epf == epf)
                .Include(x=>x.leaveType)
                .OrderByDescending(x=>x.leaveRequestId)
                .AsEnumerable());
        }

        public async Task<IEnumerable<LeaveBalance>> GetLeaveBalance(int epf)
        {
            return await Task.FromResult(_context.LeaveBalance.Where(x => x.epf == epf)
                .Include(x => x.leaveType)
                .OrderBy(x => x.leaveType)
                .AsEnumerable());
        }

        public async Task<IEnumerable<Notification>> GetNotifications(int epf)
        {
            return await Task.FromResult(_context.Notification.Where(x => x.epf == epf)
                .OrderByDescending(x => x.id)
                .Take(10)
                .AsEnumerable());
        }

        public async Task<bool> ReadNotification(int notificationId)
        {
            Notification? notification = _context.Notification.Where(x => x.id == notificationId).FirstOrDefault();
            if(notification != null)
            {
                notification.markAsRead = true;
                await _context.SaveChangesAsync();
            }
            return await Task.FromResult(true);
        }
    }
}
