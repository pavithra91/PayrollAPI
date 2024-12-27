using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Leave.Contracts.Requests;
using Leave.Contracts.Response;
using PayrollAPI.Models.HRM;
using PayrollAPI.Models.Payroll;
using PayrollAPI.Models.Reservation;
using PayrollAPI.Models.Services;
using static Leave.Contracts.Response.PaymentResponse;
using static PayrollAPI.Data.EntityMapping.StatusMapper;

namespace PayrollAPI.DataModel.HRM
{
    public static class ContractMapping
    {
        public static DateTime GetTimeZone()
        {
            string userTimeZoneId = "Asia/Colombo";
            TimeZoneInfo userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZoneId);

            DateTime utcDate = DateTime.UtcNow;
            DateTime userLocalDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, userTimeZone);

            return userLocalDate;
        }
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
                createdDate = GetTimeZone().Date,
                createdTime = GetTimeZone(),
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
                lastUpdateDate = GetTimeZone().Date,
                lastUpdateTime = GetTimeZone(),
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
        public static Supervisor MapToSupervisor(this SupervisorRequest request, Employee emp)
        {
            return new Supervisor
            {
                userId = request.userId,
                epf = emp,
                isActive = true,
                isManager = request.isManager,
                createdBy = request.createdBy,
                createdDate = GetTimeZone().Date,
                createdTime = GetTimeZone(),
            };
        }

        public static Supervisor MapToSupervisor(this UpdateSupervisorRequest request, int id)
        {
            return new Supervisor
            {
                id = id,
                isActive = request.isActive,
                isManager = request.isManager,
                lastUpdateBy = request.createdBy,
                lastUpdateDate = GetTimeZone().Date,
                lastUpdateTime = GetTimeZone(),
            };
        }

        public static SupervisorResponse MapToResponse(this Supervisor supervisor)
        {
            return new SupervisorResponse
            {
                id = supervisor.id,
                userId = supervisor.userId,
                epf = supervisor.epf.epf,
                grade = supervisor.epf.empGrade.gradeCode,
                empName = supervisor.epf.empName,
                isManager = supervisor.isManager,
                isActive = supervisor.isActive,
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
                approver = leaveApproval.approver_id.epf.epf,
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
                notificationType = notification.notificationType.ToString(),
            };
        }

        public static NotificationsResponse MapToResponse(this IEnumerable<Notification> notifications)
        {
            return new NotificationsResponse
            {
                Items = notifications.Select(MapToResponse)
            };
        }

        public static AdvancePayment MapToAdvancePayment(this AdvancePaymentRequest request, Employee employee)
        {
            return new AdvancePayment
            {
                employee = employee,
                period = Convert.ToInt32(GetTimeZone().Year + "" + GetTimeZone().Month),
                isFullAmount = request.isFullAmount,
                amount = request.isFullAmount ? 0 : request.amount,
                status = ApprovalStatus.Pending,
                createdBy = request.createdBy,
                createdDate = GetTimeZone().Date,
                createdTime = GetTimeZone(),
            };
        }


        public static AdvancePaymentDisplayResponse MapToResponse(this AdvancePayment advancePayment)
        {
            return new AdvancePaymentDisplayResponse
            {
                id = advancePayment.id,
                epf = advancePayment.employee.epf,
                empName = advancePayment.employee.empName,
                costCenter = advancePayment.employee.costCenter,
                amount = advancePayment.amount,
                isFullAmount = advancePayment.isFullAmount,
                period = advancePayment.period,
                status = advancePayment.status.ToString(),
                createdBy = advancePayment.createdBy,
            };
        }

        public static AdvancePaymentsDisplayResponse MapToResponse(this IEnumerable<AdvancePayment> advancePayments)
        {
            return new AdvancePaymentsDisplayResponse
            {
                Items = advancePayments.Select(MapToResponse)
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


        #region Schedule Jobs
        public static JobSchedule MapToJobSchedule(this ScheduleJobRequest request)
        {
            return new JobSchedule
            {
                jobName = request.jobName,
                groupName = request.groupName,
                cronExpression = request.cronExpression,
                isActive = true,
                createdBy = request.createdBy,
                createdDate = GetTimeZone().Date,
                createdTime = GetTimeZone(),
            };
        }

        //public static JobSchedule MapToJobSchedule(this UpdateLeaveTypeRequest request, int id)
        //{
        //    return new JobSchedule
        //    {
        //        leaveTypeId = id,
        //        leaveTypeName = request.leaveTypeName,
        //        description = request.description,
        //        maxDays = request.maxDays,
        //        carryForwardAllowed = request.carryForwardAllowed,
        //        lastUpdateBy = request.createdBy,
        //        lastUpdateDate = DateTime.Now.Date,
        //        lastUpdateTime = DateTime.Now,
        //    };
        //}

        public static ScheduleJobResponse MapToResponse(this JobSchedule jobSchedule)
        {
            return new ScheduleJobResponse
            {
                id = jobSchedule.id,
                jobName = jobSchedule.jobName,
                groupName = jobSchedule.groupName,
                cronExpression = jobSchedule.cronExpression,
                isActive = jobSchedule.isActive,
                createdBy = jobSchedule.createdBy,
            };
        }

        public static ScheduleJobsResponse MapToResponse(this IEnumerable<JobSchedule> jobSchedules)
        {
            return new ScheduleJobsResponse
            {
                Items = jobSchedules.Select(MapToResponse)
            };
        }
        #endregion


        #region Bungalows
        public static Bungalow MapToBungalow(this BungalowRequest request)
        {
            return new Bungalow
            {
                companyCode = request.companyCode,
                bungalowName = request.bungalowName,
                description = request.description,
                address = request.address,
                contactNumber = request.contactNumber,
                isCloded = request.isCloded,
                mainImg = request.mainImg,
                maxBookingPeriod = request.maxBookingPeriod,
                noOfRooms = request.noOfRooms,
                reopenDate = request.reopenDate,
                createdBy = request.createdBy,
            };
        }

        public static Bungalow MapToBungalow(this UpdateBungalowRequest request, int id)
        {
            return new Bungalow
            {
                id = id,
                bungalowName = request.bungalowName,
                description = request.description,
                address = request.address,
                contactNumber = request.contactNumber,
                isCloded = request.isCloded,
                mainImg = request.mainImg,
                maxBookingPeriod = request.maxBookingPeriod,
                noOfRooms = request.noOfRooms,
                reopenDate = request.reopenDate,
                lastUpdateBy = request.lastUpdateBy,
                lastUpdateDate = GetTimeZone().Date,
                lastUpdateTime = GetTimeZone(),
            };
        }

        public static RateResponse MapToResponse(this BungalowRates bungalow)
        {
            return new RateResponse
            {
                rateId = bungalow.id,
                categoryName = bungalow.category.categoryName,
                amount = bungalow.perDayCost,
            };
        }

        public static RatesResponse MapToResponse(this IEnumerable<BungalowRates> bungalows)
        {
            return new RatesResponse
            {
                rates = bungalows.Select(MapToResponse)
            };
        }

        public static BungalowResponse MapToResponse(this Bungalow bungalow)
        {
            return new BungalowResponse
            {
                id = bungalow.id,
                companyCode = bungalow.companyCode,
                bungalowName = bungalow.bungalowName,
                description = bungalow.description,
                address = bungalow.address,
                contactNumber = bungalow.contactNumber,
                isCloded = bungalow.isCloded,
                mainImg = bungalow.mainImg,
                maxBookingPeriod = bungalow.maxBookingPeriod,
                noOfRooms = bungalow.noOfRooms,
                reopenDate = bungalow.reopenDate,
                createdBy = bungalow.createdBy,
                maxOccupancy = bungalow.maxOccupancy,
                bungalowRates = bungalow.rates.MapToResponse()
            };
        }

        public static BungalowsResponse MapToResponse(this IEnumerable<Bungalow> bungalows)
        {
            return new BungalowsResponse
            {
                Items = bungalows.Select(MapToResponse)
            };
        }
        #endregion

                public static ReservationCategoryResponse MapToResponse(this ReservationCategory category)
        {
            return new ReservationCategoryResponse
            {
                id = category.id,
                categoryName = category.categoryName,
                createdBy = category.createdBy,
            };
        }

        public static ReservationCategoriesResponse MapToResponse(this IEnumerable<ReservationCategory> categories)
        {
            return new ReservationCategoriesResponse
            {
                Items = categories.Select(MapToResponse)
            };
        }


        #region Reservation
        public static Bungalow_Reservation MapToReservation(this ReservationRequest request, Employee emp, Bungalow bungalow, ReservationCategory category)
        {
            BookingStatus bookingStatus = BookingStatus.Pending;
            if (category.id == 5)
            {
                bookingStatus = BookingStatus.Confirmed;
            }
            return new Bungalow_Reservation
            {
                companyCode = request.companyCode,
                employee = emp,
                bungalow = bungalow,
                reservationCategory = category,
                bookingStatus = bookingStatus,
                checkInDate = request.checkInDate,
                checkOutDate = request.checkOutDate,
                noOfAdults = request.noOfAdults,
                noOfChildren = request.noOfChildren,
                totalPax = request.totalPax,
                contactNumber_1 = request.contactNumber_1,
                contactNumber_2 = request.contactNumber_2,
                nicNo = request.nicNo,
                comments = request.comments,
                createdBy = request.createdBy,
                createdDate = GetTimeZone().Date,
                createdTime = GetTimeZone(),
            };
        }

        public static Bungalow_Reservation MapToReservation(this UpdateReservationRequest request, int id, Employee emp, Bungalow bungalow)
        {
            return new Bungalow_Reservation
            {
                id = id,
                employee = emp,
                bungalow = bungalow,
                checkInDate = request.checkInDate,
                checkOutDate = request.checkOutDate,
                noOfAdults = request.noOfAdults,
                noOfChildren = request.noOfChildren,
                totalPax = request.totalPax,
                contactNumber_1 = request.contactNumber_1,
                contactNumber_2 = request.contactNumber_2,
                nicNo = request.nicNo,
                comments = request.comments,
                lastUpdateBy = request.lastUpdateBy,
                lastUpdateDate = GetTimeZone().Date,
                lastUpdateTime = GetTimeZone(),
            };
        }

        public static ReservationResponse MapToResponse(this Bungalow_Reservation reservation)
        {
            return new ReservationResponse
            {
                id = reservation.id,
                companyCode = reservation.companyCode,
                empId = reservation.employee.id,
                epf = reservation.employee.epf,
                bungalowId = reservation.bungalow.id,
                bungalowName = reservation.bungalow.bungalowName,
                checkInDate = reservation.checkInDate,
                checkOutDate = reservation.checkOutDate,
                noOfAdults = reservation.noOfAdults,
                noOfChildren = reservation.noOfChildren,
                totalPax = reservation.totalPax,
                contactNumber_1 = reservation.contactNumber_1,
                contactNumber_2 = reservation.contactNumber_2,
                reservationCategory = reservation.reservationCategory.categoryName,
                nicNo = reservation.nicNo,
                comments = reservation.comments,
                createdBy = reservation.createdBy,
                bookingStatus = reservation.bookingStatus.ToString(),
                reservationCost = reservation.reservationCost,
            };
        }

        public static ReservationsResponse MapToResponse(this IEnumerable<Bungalow_Reservation> reservations)
        {
            return new ReservationsResponse
            {
                Items = reservations.Select(MapToResponse)
            };
        }
        #endregion


        #region Voucher Payments
        public static PaymentResponse MapToResponse(this OtherPayment payment)
        {
            return new PaymentResponse
            {
                id = payment.id,
                epf = payment.epf,
                empName = payment.empName,
                bankCode = payment.bankCode,
                accountNo = payment.accountNo,
                amount = payment.amount,
                paymentCategory = payment.paymentCategory,
                bankTransferDate = payment.bankTransferDate,
                voucherNo = payment.voucherNo,
                status = payment.paymentStatus.ToString(),
                createdBy = payment.createdBy,
            };
        }

        public static PaymentsResponse MapToResponse(this IEnumerable<OtherPayment> payments)
        {
            return new PaymentsResponse
            {
                Items = payments.Select(MapToResponse)
            };
        }
        #endregion
    }
}
