using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using Leave.Contracts.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PayrollAPI.Data;
using PayrollAPI.Interfaces.Payroll;
using PayrollAPI.Interfaces.Reservation;
using PayrollAPI.Models.HRM;
using PayrollAPI.Models.Reservation;
using PayrollAPI.Services;
using System.Linq;
using static PayrollAPI.Data.EntityMapping.StatusMapper;

namespace PayrollAPI.Repository.Reservation
{
    public class ReservationRepository: IReservation
    {
        private readonly HRMDBConnect _context;
        private readonly IAdmin _admin;
        Common com = new Common();
        public ReservationRepository(HRMDBConnect db, IAdmin admin)
        {
            _context = db;
            _admin = admin;
        }

        public async Task<IEnumerable<Bungalow>> GetAllBungalows()
        {
            return await Task.FromResult(_context.Bungalow
                .Include(x => x.rates)
                .ThenInclude(x=>x.category)
                .AsEnumerable());
        }
        public async Task<Bungalow> GetBungalowById(int id)
        {
            return await Task.FromResult(_context.Bungalow.Where(x => x.id == id)
                .Include(x=>x.rates)
                .ThenInclude(x => x.category)
                .FirstOrDefault());
        }
        public async Task<bool> CreateBungalow(Bungalow bungalow)
        {
            _context.Bungalow.Add(bungalow);
            await _context.SaveChangesAsync();

            var category = _context.ReservationCategory.ToList();

            foreach (var item in category) 
            {
                BungalowRates bungalowRates = new BungalowRates
                {
                    bungalow = bungalow,
                    category = item,
                    perDayCost = 0,
                    createdBy = bungalow.createdBy,
                };

                _context.Bungalow.Add(bungalow);
            }

            await _context.SaveChangesAsync();

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateBungalow(int id, Bungalow bungalow)
        {
            var existingBungalow = await _context.Bungalow.FindAsync(id);

            if (existingBungalow is null)
            {
                return await Task.FromResult(false);
            }
            else
            {
                existingBungalow.bungalowName = bungalow.bungalowName;
                existingBungalow.address = bungalow.address;
                existingBungalow.description = bungalow.description;
                existingBungalow.contactNumber = bungalow.contactNumber;
                existingBungalow.isCloded = bungalow.isCloded;
                existingBungalow.mainImg = bungalow.mainImg;
                existingBungalow.maxBookingPeriod = bungalow.maxBookingPeriod;
                existingBungalow.maxOccupancy = bungalow.maxOccupancy;
                existingBungalow.noOfRooms = bungalow.noOfRooms;
                existingBungalow.reopenDate = bungalow.reopenDate;

                existingBungalow.lastUpdateBy = bungalow.lastUpdateBy;
                existingBungalow.lastUpdateDate = bungalow.lastUpdateDate;
                existingBungalow.lastUpdateTime = bungalow.lastUpdateTime;

                await _context.SaveChangesAsync();

                return await Task.FromResult(true);
            }
        }


        public async Task<IEnumerable<BungalowRates>> GetBungalowRates(int id)
        {
            return await Task.FromResult(_context.BungalowRates
                .Include(x => x.bungalow)
                .Include(x=>x.category)
                .Where(x=>x.bungalow.id == id)
                .AsEnumerable());
        }

        public async Task<bool> UpdateBungalowRates(int id, UpdateBungalowRatesRequest request)
        {
            var rates = _context.BungalowRates.Where(x => x.bungalow.id == id).ToList();

            foreach (var rate in request.rates) 
            {
                var updated = rates.Find(x => x.id == rate.rateId);
                updated.perDayCost = rate.amount;
                updated.lastUpdateBy = request.lastupdateBy;
                updated.lastUpdateDate = com.GetTimeZone().Date;
                updated.lastUpdateTime = com.GetTimeZone();
            }

            await _context.SaveChangesAsync();

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<ReservationCategory>> GetAllReservationCategories()
        {
            return await Task.FromResult(_context.ReservationCategory
                .AsEnumerable());
        }
        public async Task<ReservationCategory> GetReservationCategoryById(int id)
        {
            return await Task.FromResult(_context.ReservationCategory.Where(x => x.id == id)
                .FirstOrDefault());
        }

        public async Task<IEnumerable<Bungalow_Reservation>> GetAllReservations()
        {
            return await Task.FromResult(_context.Reservation
                .Include(x => x.employee)
                .Include(x => x.bungalow)
                .Include(x => x.reservationCategory)
                .AsEnumerable());
        }

        public async Task<IEnumerable<Bungalow_Reservation>> GetAllReservations(string epf)
        {
            return await Task.FromResult(_context.Reservation
                .Where(x=>x.employee.epf == epf)
                .Include(x => x.employee)
                .Include(x => x.bungalow)
                .Include(x => x.reservationCategory)
                .OrderByDescending(x=>x.id)
                .AsEnumerable());
        }
        public async Task<Bungalow_Reservation> GetReservationById(int id)
        {
            Bungalow_Reservation? bungalow_Reservation = _context.Reservation.Where(x => x.id == id)
                .Include(x => x.employee)
                .Include(x => x.bungalow)
                .Include(x => x.reservationCategory)
                .FirstOrDefault();

            if(bungalow_Reservation != null)
            {
                Notification? notification = _context.Notification.Where(x => x.reference == id.ToString()
                    && x.epf == Convert.ToInt32(bungalow_Reservation.employee.epf)).FirstOrDefault();

                if (notification != null)
                {
                    notification.markAsRead = true;
                    _context.SaveChanges();
                }
            }         

            return await Task.FromResult(bungalow_Reservation);
        }
        public async Task<bool> CreateReservation(Bungalow_Reservation reservation)
        {
            try
            {
                Bungalow? bungalow = _context.Bungalow
                    .Where(x => x.id == reservation.bungalow.id).FirstOrDefault();
                var sys_Properties = await _admin.GetSystemProperties("Bungalow_Reservation");

                if (bungalow.nextRaffelDrawDate > reservation.checkOutDate && reservation.checkInDate > com.GetTimeZone().Date)
                {

                    BungalowRates? rate = _context.BungalowRates.Where(x => x.bungalow == reservation.bungalow && x.category == reservation.reservationCategory)
                        .FirstOrDefault();

                    if (rate == null) {
                        return await Task.FromResult(false);
                    }

                    TimeSpan difference = reservation.checkOutDate - reservation.checkInDate;

                    reservation.reservationCost = rate.perDayCost * Convert.ToDecimal(difference.TotalDays);

                    _context.Reservation.Add(reservation);
                    await _context.SaveChangesAsync();

                    Notification notification = new Notification
                    {
                        epf = Convert.ToInt32(reservation.employee.epf),
                        notificationType = NotificationType.Reservation,
                        createdDate = com.GetTimeZone(),
                        type = 4,
                        description = "Please confirm your Reservation",
                        markAsRead = false,
                        reference = reservation.id.ToString(),
                    };

                    _context.Notification.Add(notification);



                    await _context.SaveChangesAsync();
                    return await Task.FromResult(true);
                }


                var overlappingReservations = _context.Reservation
                    .Where(x => x.checkInDate <= reservation.checkOutDate && // Existing check-in is before or on new check-out
                    x.checkOutDate >= reservation.checkInDate && x.bungalow == bungalow) // Existing check-out is after or on new check-in
                    .ToList();

                if(overlappingReservations.Count > 0 )
                {
                    return await Task.FromResult(false);
                }


                var propertyName  = Convert.ToInt32(sys_Properties.Where(x => x.variable_name == "Reservation_Gap").FirstOrDefault().variable_value) * -1;

                DateTime reservationGap = reservation.checkInDate.AddMonths(propertyName);

                var previousReservations = _context.Reservation.Where(x => x.checkInDate > reservationGap && 
                x.createdBy == reservation.createdBy && x.reservationCategory.id != 5 && x.bookingStatus == BookingStatus.Confirmed).ToList();

                if (previousReservations.Count > 0)
                {
                    reservation.bookingPriority = BookingPriority.LowPriority;
                }

                _context.Reservation.Add(reservation);
                await _context.SaveChangesAsync();

                // Official Booking
                if (reservation.reservationCategory.id == 5)
                {
                    RaffleDraw raffleDraw = new RaffleDraw()
                    {
                        bookingStatus = Data.EntityMapping.StatusMapper.BookingStatus.Confirmed,
                        rank = 1,
                        reservationID = reservation.id,
                        createdDate = com.GetTimeZone().Date,
                        createdTime = com.GetTimeZone(),
                        lastUpdateDate = com.GetTimeZone().Date,
                        lastUpdateTime = com.GetTimeZone(),
                        bungalow_Reservation = reservation,
                    };

                    if (bungalow != null)
                    {
                        bungalow.nextRaffelDrawDate = reservation.checkOutDate;
                    }
                }

                await _context.SaveChangesAsync();

                return await Task.FromResult(true);
            }
            catch(Exception ex)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> UpdateReservation(int id, Bungalow_Reservation reservation)
        {
            var existingReservation = await _context.Reservation.FindAsync(id);

            if (existingReservation is null)
            {
                return await Task.FromResult(false);
            }
            else
            {
                //existingBungalow.bungalowName = bungalow.bungalowName;
                //existingBungalow.address = bungalow.address;
                //existingBungalow.description = bungalow.description;
                //existingBungalow.contactNumber = bungalow.contactNumber;
                //existingBungalow.isCloded = bungalow.isCloded;
                //existingBungalow.mainImg = bungalow.mainImg;
                //existingBungalow.maxBookingPeriod = bungalow.maxBookingPeriod;
                //existingBungalow.maxOccupancy = bungalow.maxOccupancy;
                //existingBungalow.noOfRooms = bungalow.noOfRooms;
                //existingBungalow.reopenDate = bungalow.reopenDate;

                //existingBungalow.lastUpdateBy = bungalow.lastUpdateBy;
                //existingBungalow.lastUpdateDate = bungalow.lastUpdateDate;
                //existingBungalow.lastUpdateTime = bungalow.lastUpdateTime;

                await _context.SaveChangesAsync();

                return await Task.FromResult(true);
            }
        }


        public async Task<bool> CancelReservation(ReservationCancellationRequest request)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                     var reservation = _context.Reservation.Where(x => x.id == request.id)
                        .Include(x => x.bungalow)
                        .Include(e => e.employee)
                        .Include(r => r.raffleDraw)
                        .FirstOrDefault();

                    DateTime now = com.GetTimeZone().Date;
                    TimeSpan difference = reservation.checkInDate - now;

                    decimal amount = 0;

                    if (reservation.reservationCost != null)
                    {
                        amount = reservation.reservationCost.Value;
                    }

                    if (difference.TotalDays <= 7)
                    {
                        CancellationCharges cancellationCharges = new CancellationCharges()
                        {
                            cancellation_Policy = Cancellation_Policy.Full_Cancellation,
                            reservation = reservation,
                            reservationID = reservation.id,
                            cancellationFees = amount,
                            createdDate = com.GetTimeZone().Date,
                            createdTime = com.GetTimeZone(),
                        };

                        _context.CancellationCharges.Add(cancellationCharges);
                    }
                    else if (difference.TotalDays > 7 && difference.TotalDays < 30)
                    {
                        CancellationCharges cancellationCharges = new CancellationCharges()
                        {
                            cancellation_Policy = Cancellation_Policy.Full_Cancellation,
                            reservation = reservation,
                            reservationID = reservation.id,
                            cancellationFees = amount / 2,
                            createdDate = com.GetTimeZone().Date,
                            createdTime = com.GetTimeZone(),
                        };

                        _context.CancellationCharges.Add(cancellationCharges);
                    }

                    reservation.bookingStatus = BookingStatus.Cancelled;

                    reservation.lastUpdateBy = request.lastUpdateBy;
                    reservation.lastUpdateDate = com.GetTimeZone().Date;
                    reservation.lastUpdateTime = com.GetTimeZone();

                    Notification notification = new Notification
                    {
                        epf = Convert.ToInt32(reservation.employee.epf),
                        notificationType = NotificationType.Reservation,
                        createdDate = com.GetTimeZone(),
                        type = 5,
                        description = "Your Reservation is Cancelled",
                        markAsRead = false,
                        reference = reservation.id.ToString(),
                    };

                    _context.Notification.Add(notification);

                    if (reservation.raffleDraw != null)
                    {
                        reservation.raffleDraw.bookingStatus = BookingStatus.Cancelled;

                        var nextRaffelDrawPosition = _context.RaffleDraw.Where(x => x.reservationID == request.id && x.rank == (reservation.raffleDraw.rank + 1))
                        .Include(x => x.bungalow_Reservation)
                        .ThenInclude(x => x.reservationCategory)
                        .Include(e => e.bungalow_Reservation)
                        .ThenInclude(e => e.employee)
                        .FirstOrDefault();

                        if (nextRaffelDrawPosition != null)
                        {
                            nextRaffelDrawPosition.bookingStatus = BookingStatus.Raffle_Winner;
                            nextRaffelDrawPosition.bungalow_Reservation.bookingStatus = BookingStatus.Raffle_Winner;

                            var reservationCost = _context.BungalowRates.Where(x => x.bungalow == reservation.bungalow
                                && x.category == nextRaffelDrawPosition.bungalow_Reservation.reservationCategory)
                                .FirstOrDefault();

                            DateTime newCheckInDate = nextRaffelDrawPosition.bungalow_Reservation.checkInDate;
                            TimeSpan noOfDays = nextRaffelDrawPosition.bungalow_Reservation.checkOutDate - newCheckInDate;

                            nextRaffelDrawPosition.bungalow_Reservation.reservationCost = reservationCost.perDayCost * Convert.ToDecimal(noOfDays.TotalDays);

                            Notification nextRaffelDrawPositionNotification = new Notification
                            {
                                epf = Convert.ToInt32(nextRaffelDrawPosition.bungalow_Reservation.employee.epf),
                                notificationType = NotificationType.Reservation,
                                createdDate = com.GetTimeZone(),
                                type = 3,
                                description = "You have won the Raffel Draw. Please confirm the Reservation",
                                markAsRead = false,
                                reference = nextRaffelDrawPosition.bungalow_Reservation.id.ToString(),
                            };

                            _context.Notification.Add(nextRaffelDrawPositionNotification);
                        }
                    }
                    
                    _context.SaveChanges();
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

        public async Task<List<object>> GetRestrictedDates()
        {
            var reservations = _context.Reservation
                    .Where(x => x.reservationCategory.id == 5 && x.checkInDate >= DateTime.Now)
                    .Select(x => new
                    {
                        x.checkInDate,
                        x.checkOutDate
                    })
                    .ToList();

            var bungalowList = _context.Bungalow.ToList();

            foreach (var bungalow in bungalowList)
            {
                var reservation = _context.Reservation
                    .Where(x => x.checkInDate < bungalow.nextRaffelDrawDate && x.checkInDate > com.GetTimeZone().Date 
                    && x.checkOutDate < bungalow.nextRaffelDrawDate)
                    .Select(x => new
                    {
                        x.checkInDate,
                        x.checkOutDate
                    })
                    .ToList();

                if(reservation != null)
                {
                    reservations.AddRange(reservation);
                }
            }

            List<object> formattedDates = new List<object>();

            foreach (var reservation in reservations)
            {
                DateTime current = reservation.checkInDate;
                while (current <= reservation.checkOutDate)
                {
                    formattedDates.Add(new
                    {
                        day = current.Day,
                        month = current.Month - 1
                    });
                    current = current.AddDays(1);
                }
            }

            return await Task.FromResult(formattedDates);
        }

        public async Task<List<object>> GetRestrictedDates(int id)
        {
            var bungalow = _context.Bungalow.Where(x => x.id == id).FirstOrDefault();

            var reservations = _context.Reservation
                    .Where(x => x.reservationCategory.id == 5 && x.checkInDate >= DateTime.Now)
                    .Select(x => new
                    {
                        x.checkInDate,
                        x.checkOutDate
                    })
                    .ToList();

                var reservation = _context.Reservation
                    .Where(x => x.bungalow == bungalow && x.checkInDate < bungalow.nextRaffelDrawDate && x.checkInDate > com.GetTimeZone().Date
                    && x.checkOutDate < bungalow.nextRaffelDrawDate && (x.bookingStatus != BookingStatus.Pending || x.bookingStatus != BookingStatus.Cancelled))
                    .Select(x => new
                    {
                        x.checkInDate,
                        x.checkOutDate
                    })
                    .ToList();

                if (reservation != null)
                {
                    reservations.AddRange(reservation);
                }

            List<object> formattedDates = new List<object>();

            foreach (var res in reservations)
            {
                DateTime current = res.checkInDate;
                while (current <= res.checkOutDate)
                {
                    formattedDates.Add(new
                    {
                        day = current.Day,
                        month = current.Month - 1
                    });
                    current = current.AddDays(1);
                }
            }

            return await Task.FromResult(formattedDates);
        }


        public async Task<List<Reservation_Payments_View>> GetReservationsPayments(ReservationPaymentRequest request)
        {
            var formattedList = _context.ReservationPayments.Where(x => x.categoryId == request.category 
            && x.checkInDate >= request.fromDate && x.checkInDate <= request.toDate).ToList();

            return await Task.FromResult(formattedList);
        }

        public async Task<bool> RaffelDraw()
        {
            try
            {
                int dayCount = 31;
                //DateTime nextReffelDrawDate = DateTime.Now;
                DateTime reffelDate = DateTime.Now.AddDays(dayCount);

                var bungalowList = _context.Bungalow.Where(x => x.isCloded == false)
                    .Include(r=>r.rates).ToList();

                var sys_Properties = await _admin.GetSystemProperties("Bungalow_Reservation");
                int next_Raffle_ID = Convert.ToInt32(sys_Properties.Where(x => x.variable_name == "Next_Raffle_ID").FirstOrDefault().variable_value);

                foreach (var bungalow in bungalowList)
                {
                    if (bungalow.nextRaffelDrawDate != reffelDate.Date) 
                    {
                        continue;
                    }

                    var allCompetitors = _context.Reservation.Where(x => x.checkInDate.Date == reffelDate.Date && x.reservationCategory.id != 5 &&
                        x.bungalow == bungalow && x.bookingStatus == BookingStatus.Pending)
                        .Include(b => b.bungalow)
                        .Include(e => e.employee)
                        .Include(r => r.reservationCategory)
                        .ToList();

                    if (allCompetitors.Count == 0)
                    {
                        continue;
                    }
                    else if (allCompetitors.Count == 1)
                    {
                        RaffleDraw raffleDraw = new RaffleDraw
                        {
                            bungalow_Reservation = allCompetitors[0],
                            bookingStatus = BookingStatus.Pending,
                            createdDate = com.GetTimeZone().Date,
                            createdTime = com.GetTimeZone(),
                            lastUpdateDate = com.GetTimeZone().Date,
                            lastUpdateTime = com.GetTimeZone(),
                            reservationID = allCompetitors[0].id,
                            rank = 1,
                            RaffleDrawId = next_Raffle_ID,
                        };

                        _context.RaffleDraw.Add(raffleDraw);

                        allCompetitors[0].raffleDraw = raffleDraw;

                        Notification notification = new Notification
                        {
                            epf = Convert.ToInt32(allCompetitors[0].employee.epf),
                            notificationType = NotificationType.Reservation,
                            createdDate = com.GetTimeZone(),
                            type = 3,
                            description = "You have won the Raffel Draw. Please confirm the Reservation",
                            markAsRead = false,
                            reference = allCompetitors[0].id.ToString(),
                        };

                        _context.Notification.Add(notification);

                        _context.SaveChanges();

                        return await Task.FromResult(true);
                    }
                    else
                    {
                        var rng = new Random();

                        List<Bungalow_Reservation> highPriorityCPSTL = new List<Bungalow_Reservation>();
                        List<Bungalow_Reservation> lowPriorityCPSTL = new List<Bungalow_Reservation>();
                        List<Bungalow_Reservation> highPriorityCPC = new List<Bungalow_Reservation>();
                        List<Bungalow_Reservation> lowPriorityCPC = new List<Bungalow_Reservation>();
                        List<Bungalow_Reservation> highPriorityRetired = new List<Bungalow_Reservation>();
                        List<Bungalow_Reservation> lowPriorityRetired = new List<Bungalow_Reservation>();
                        //List<Bungalow_Reservation> lowPriorityRetiredCPSTL = new List<Bungalow_Reservation>();
                        //List<Bungalow_Reservation> highPriorityRetiredCPC = new List<Bungalow_Reservation>();
                        //List<Bungalow_Reservation> lowPriorityRetiredCPC = new List<Bungalow_Reservation>();
                        List<Bungalow_Reservation> external = new List<Bungalow_Reservation>();

                        foreach (var competitor in allCompetitors)
                        {
                            if (competitor.reservationCategory.id == 1 && competitor.bookingPriority == Data.EntityMapping.StatusMapper.BookingPriority.HighPriority)
                            {
                                highPriorityCPSTL.Add(competitor);
                            }
                            else if (competitor.reservationCategory.id == 1 && competitor.bookingPriority == Data.EntityMapping.StatusMapper.BookingPriority.LowPriority)
                            {
                                lowPriorityCPSTL.Add(competitor);
                            }
                            else if (competitor.reservationCategory.id == 2 && competitor.bookingPriority == Data.EntityMapping.StatusMapper.BookingPriority.HighPriority)
                            {
                                highPriorityCPC.Add(competitor);
                            }
                            else if (competitor.reservationCategory.id == 2 && competitor.bookingPriority == Data.EntityMapping.StatusMapper.BookingPriority.LowPriority)
                            {
                                lowPriorityCPC.Add(competitor);
                            }
                            else if (competitor.reservationCategory.id == 3 && competitor.bookingPriority == Data.EntityMapping.StatusMapper.BookingPriority.HighPriority)
                            {
                                highPriorityRetired.Add(competitor);
                            }
                            else if (competitor.reservationCategory.id == 3 && competitor.bookingPriority == Data.EntityMapping.StatusMapper.BookingPriority.LowPriority)
                            {
                                lowPriorityRetired.Add(competitor);
                            }
                            else
                            {
                                external.Add(competitor);
                            }
                        }

                        highPriorityCPSTL = highPriorityCPSTL.OrderBy(u => rng.Next()).ToList();
                        lowPriorityCPSTL = lowPriorityCPSTL.OrderBy(u => rng.Next()).ToList();
                        highPriorityCPC = highPriorityCPC.OrderBy(u => rng.Next()).ToList();
                        lowPriorityCPC = lowPriorityCPC.OrderBy(u => rng.Next()).ToList();
                        highPriorityRetired = highPriorityRetired.OrderBy(u => rng.Next()).ToList();
                        lowPriorityRetired = lowPriorityRetired.OrderBy(u => rng.Next()).ToList();
                        external = external.OrderBy(u => rng.Next()).ToList();

                        var raffelDrawOrder = highPriorityCPSTL.Concat(highPriorityCPC).ToList();
                        raffelDrawOrder.Concat(highPriorityRetired).ToList();
                        raffelDrawOrder.Concat(lowPriorityCPSTL).ToList();
                        raffelDrawOrder.Concat(lowPriorityCPC).ToList();
                        raffelDrawOrder.Concat(lowPriorityRetired).ToList();
                        raffelDrawOrder.Concat(external).ToList();

                        int rank = 1;
                        foreach (var item in raffelDrawOrder)
                        {
                            RaffleDraw raffleDraw = new RaffleDraw
                            {
                                bookingStatus = BookingStatus.Pending,
                                bungalow_Reservation = item,
                                createdDate = com.GetTimeZone().Date,
                                createdTime = com.GetTimeZone(),
                                lastUpdateDate = com.GetTimeZone().Date,
                                lastUpdateTime = com.GetTimeZone(),
                                rank = rank,
                                reservationID = item.id,
                                RaffleDrawId = next_Raffle_ID,
                            };

                            _context.RaffleDraw.Add(raffleDraw);

                            if (rank == 1)
                            {
                                var rate = bungalow.rates.Where(x => x.category.id == item.reservationCategory.id).FirstOrDefault();
                                TimeSpan difference = item.checkOutDate - item.checkInDate;

                                item.bookingStatus = BookingStatus.Raffle_Winner;
                                item.reservationCost = rate.perDayCost * Convert.ToDecimal(difference.TotalDays);

                                Notification notification = new Notification
                                {
                                    epf = Convert.ToInt32(allCompetitors[0].employee.epf),
                                    notificationType = NotificationType.Reservation,
                                    createdDate = com.GetTimeZone(),
                                    type = 3,
                                    description = "You have won the Raffel Draw. Please confirm the Reservation",
                                    markAsRead = false,
                                    reference = allCompetitors[0].id.ToString(),
                                };

                                _context.Notification.Add(notification);
                            }
                            else
                            {
                                item.bookingStatus = BookingStatus.Raffle_Drawn;
                            }
                            item.raffleDraw = raffleDraw;

                            rank++;
                        }

                        bungalow.nextRaffelDrawDate = raffelDrawOrder[0].checkOutDate;
                        _context.SaveChanges();
                    }

                    next_Raffle_ID++;
                }

                await _admin.SetSystemProperties("Bungalow_Reservation", "Next_Raffle_ID", next_Raffle_ID.ToString());
                return await Task.FromResult(true);
            }
            catch (Exception ex) 
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> WinnerConfirmation()
        {
            var WinnerList = _context.RaffleDraw.Where(x => x.bookingStatus == BookingStatus.Raffle_Winner)
                .Include(x=>x.bungalow_Reservation);

            foreach (var item in WinnerList) 
            {
                DateTime now = com.GetTimeZone().Date;
                TimeSpan difference = now - item.lastUpdateDate;

                if(difference.TotalDays > 4)
                {
                    item.bungalow_Reservation.bookingStatus = BookingStatus.Rejected;

                    item.bookingStatus = BookingStatus.Rejected;

                    Notification cancellationNotification = new Notification
                    {
                        epf = Convert.ToInt32(item.bungalow_Reservation.employee.epf),
                        notificationType = NotificationType.Reservation,
                        createdDate = com.GetTimeZone(),
                        type = 1,
                        description = "Your reservation was canceled due to non-confirmation.",
                        markAsRead = false,
                        reference = item.reservationID.ToString(),
                    };

                    _context.Notification.Add(cancellationNotification);

                    int newRank = item.rank + 1;

                    var nextWinner = _context.RaffleDraw.Where(x => x.rank == newRank && x.RaffleDrawId == item.RaffleDrawId)
                        .Include(x => x.bungalow_Reservation)
                        .ThenInclude(e=>e.employee)
                        .FirstOrDefault();

                    if (nextWinner != null)
                    {
                        nextWinner.bungalow_Reservation.bookingStatus = BookingStatus.Raffle_Winner;
                        nextWinner.bookingStatus = BookingStatus.Raffle_Winner;
                        nextWinner.lastUpdateDate = com.GetTimeZone().Date;
                        nextWinner.lastUpdateTime = com.GetTimeZone();

                        Notification notification = new Notification
                        {
                            epf = Convert.ToInt32(nextWinner.bungalow_Reservation.employee.epf),
                            notificationType = NotificationType.Reservation,
                            createdDate = com.GetTimeZone(),
                            type = 3,
                            description = "You have won the Raffel Draw. Please confirm the Reservation",
                            markAsRead = false,
                            reference = nextWinner.reservationID.ToString(),
                        };

                        _context.Notification.Add(notification);
                    }

                    _context.SaveChanges();
                }
            }

            return await Task.FromResult(true);
        }
    }
}
