using Leave.Contracts.Requests;
using Microsoft.EntityFrameworkCore;
using PayrollAPI.Data;
using PayrollAPI.Interfaces.Payroll;
using PayrollAPI.Interfaces.Reservation;
using PayrollAPI.Models.HRM;
using PayrollAPI.Models.Reservation;
using PayrollAPI.Services;
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
            return await Task.FromResult(_context.Reservation.Where(x => x.id == id)
                .Include(x => x.employee)
                .Include(x => x.bungalow)
                .Include(x => x.reservationCategory)
                .FirstOrDefault());
        }
        public async Task<bool> CreateReservation(Bungalow_Reservation reservation)
        {
            try
            {
                var sys_Properties = await _admin.GetSystemProperties("Bungalow_Reservation");
                var propertyName  = Convert.ToInt32(sys_Properties.Where(x => x.variable_name == "Reservation_Gap").FirstOrDefault().variable_value) * -1;

                DateTime reservationGap = reservation.checkInDate.AddMonths(propertyName);

                var previousReservations = _context.Reservation.Where(x => x.checkInDate > reservationGap && 
                x.createdBy == reservation.createdBy && x.reservationCategory.id != 5).ToList();

                if (previousReservations.Count > 0)
                {
                    reservation.bookingPriority = BookingPriority.LowPriority;
                }

                _context.Reservation.Add(reservation);
                await _context.SaveChangesAsync();

                if (reservation.reservationCategory.id == 5)
                {
                    RaffleDraw raffleDraw = new RaffleDraw()
                    {
                        bookingStatus = Data.EntityMapping.StatusMapper.BookingStatus.Confirmed,
                        rank = 1,
                        reservationID = reservation.id,
                        createdDate = com.GetTimeZone().Date,
                        createdTime = com.GetTimeZone(),
                        bungalow_Reservation = reservation,
                    };

                    Bungalow? bungalow = _context.Bungalow.Where(x => x.id == reservation.bungalow.id).FirstOrDefault();
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
                            type = 2,
                            description = "You have won the Raffel Draw. Please confirm the Reservation",
                            markAsRead = false,
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
                                    type = 2,
                                    description = "You have won the Raffel Draw. Please confirm the Reservation",
                                    markAsRead = false,
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
                TimeSpan difference = now - item.createdDate;

                if(difference.TotalDays > 4)
                {

                }
            }

            return await Task.FromResult(true);
        }
    }
}
