using Leave.Contracts.Requests;
using Leave.Contracts.Response;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PayrollAPI.Data;
using PayrollAPI.DataModel.HRM;
using PayrollAPI.Interfaces.HRM;
using PayrollAPI.Models;
using PayrollAPI.Models.HRM;
using static Mysqlx.Notice.Warning.Types;

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

        public async Task<IEnumerable<Supervisor>> GetAllSupervisors()
        {
            return await Task.FromResult(_context.Supervisor.AsEnumerable());
        }

        public async Task<Supervisor?> GetSupervisor(int id)
        {
            var _supervisor = _context.Supervisor.SingleOrDefault(x => x.id == id);
            return await Task.FromResult(_supervisor);
        }

        public async Task<bool> CreateSupervisor(Supervisor supervisor)
        {
            _context.Supervisor.Add(supervisor);
            await _context.SaveChangesAsync();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateSupervisor(int id, Supervisor supervisor)
        {
            var existingSupervisor = await _context.Supervisor.FindAsync(id);

            if (existingSupervisor is null)
            {
                return await Task.FromResult(false);
            }
            else
            {
                existingSupervisor.isActive = supervisor.isActive;
                existingSupervisor.lastUpdateBy = supervisor.lastUpdateBy;
                existingSupervisor.lastUpdateDate = supervisor.lastUpdateDate;
                existingSupervisor.lastUpdateTime = supervisor.lastUpdateTime;

                await _context.SaveChangesAsync();

                return await Task.FromResult(true);
            }
        }

        public async Task<ApprovalWorkflowResponse> GetApprovalWorkflow()
        {
         var empApprovals =   _context.EmpApprovals
        .Include(e => e.approvalWorkflowsId)
            .ThenInclude(w => w.approvalLevels)
        .Include(e => e.approvalWorkflowsId)
            .ThenInclude(w => w.approverId)
        .ToList();

            var result = empApprovals.Select(e => new ApprovalWorkflowDto
            {
                id = e.id,
                epf = e.epf,
                approvalLevel = e.level.HasValue ? $"Level {e.level.Value}" : null,
                SupervisorList = e.approvalWorkflowsId.Select(w => new SupervisorDto
                {
                    Level = w.approvalLevels != null ? w.approvalLevels.levelName : null,
                    Epf = w.approverId != null ? int.Parse(w.approverId.epf) : 0
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
            var _empApprovals = _context.EmpApprovals.Where(x => x.epf == Convert.ToInt32(epf))
                .Include(x=> x.approvalWorkflowsId)
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

        public async Task<bool> RequestLeave(RequestLeaveRequest request)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction()) 
            {
                try
                {
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
                    _leaveRequest.actingDelegate = request.actDelegate;
                    _leaveRequest.actingDelegateApprovalStatus = ApprovalStatus.Pending;
                    _leaveRequest.requestStatus = ApprovalStatus.Pending;
                    _leaveRequest.actionedBy = request.requestBy;
                    _leaveRequest.reason = request.reason;

                    _context.LeaveRequest.Add(_leaveRequest);

                    await _context.SaveChangesAsync();

                    var empApproval = _context.EmpApprovals.Where(x => x.epf == Convert.ToInt32(request.epf))
                       .Include(e => e.approvalWorkflowsId)
                    .ThenInclude(w => w.approvalLevels)
                .Include(e => e.approvalWorkflowsId)
                    .ThenInclude(w => w.approverId).FirstOrDefault();

                    Notification notification = new Notification
                    {
                        epf = Convert.ToInt32(_leaveRequest.actingDelegate),
                        target = request.epf,
                        description = "has request you to become an acting Delegate",
                        createdDate = DateTime.Now,
                        markAsRead = false,
                        type = 0
                    };

                    _context.Notification.Add(notification);

                    foreach (var item in empApproval.approvalWorkflowsId)
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
                            epf = Convert.ToInt32(item.approverId.epf),
                            target = request.epf,
                            description = "has send a leave request",
                            createdDate = DateTime.Now,
                            markAsRead = false,
                            type = 0
                        };

                        _context.Notification.Add(notifications);
                        _context.LeaveApproval.Add(_approval);
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return await Task.FromResult(true);
                }
                catch (Exception ex) 
                {
                    transaction.Rollback();
                    return await Task.FromResult(false);
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

            if (request.isDelegate && request.status == "Approved") 
            {
                leaveRequest.actingDelegateApprovalStatus = ApprovalStatus.Approved;
                await _context.SaveChangesAsync();
                return await Task.FromResult(false);
            }
            else  if(request.isDelegate && request.status == "Rejected")
            {
                leaveRequest.actingDelegateApprovalStatus = ApprovalStatus.Rejected;
                await _context.SaveChangesAsync();
                return await Task.FromResult(false);
            }
            else
            {
                List<LeaveApproval> leaveApprovals = _context.LeaveApproval
                    .Include(x=>x.level)
                    .Include(x=>x.approver_id)
                    .Where(x=> x.requestId.leaveRequestId == request.requestId).ToList();

                LeaveApproval? leaveApproval = leaveApprovals.Find(x => x.approver_id.epf == request.approver);

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
                }

                leaveApproval.comments = request.comment;
                leaveRequest.currentLevel = leaveApproval.level.id;

                if (leaveRequest.currentLevel == leaveApprovals.Count)
                {
                    leaveRequest.finalStatus = finalStatus;
                    leaveRequest.requestStatus = approvalStatus;
                }

                leaveRequest.lastUpdateBy = request.approver;
                leaveRequest.lastUpdateDate = DateTime.Now.Date;
                leaveRequest.lastUpdateTime = DateTime.Now;

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
                //leaveApproval.lastUpdateDate
            }

            leaveRequest.requestStatus = ApprovalStatus.Cancelled;
            leaveRequest.finalStatus = FinalStatus.Cancelled;
            leaveRequest.lastUpdateBy = request.cancelBy;
            leaveRequest.lastUpdateDate = DateTime.Now.Date;
            leaveRequest.lastUpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return await Task.FromResult(true);
        }
        public async Task<IEnumerable<LeaveApproval?>> GetLeaveApprovals(int id)
        {
            return await Task.FromResult(_context.LeaveApproval.Where(x => x.requestId.leaveRequestId == id)
                .Include(x => x.level)
                .Include(x => x.approver_id)
                .AsEnumerable());
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

        public async Task<bool> RequestAdvancePayment(AdvancePayment advancePayment)
        {
            var _previousRequest = _context.AdvancePayment.Where(x => x.period == advancePayment.period).FirstOrDefault();
            if (_previousRequest == null)
            {
                _context.AdvancePayment.Add(advancePayment);
                await _context.SaveChangesAsync();

                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
    }
}
