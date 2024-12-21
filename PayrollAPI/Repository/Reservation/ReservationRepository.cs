using Microsoft.EntityFrameworkCore;
using PayrollAPI.Data;
using PayrollAPI.Interfaces.Reservation;
using PayrollAPI.Models.HRM;
using PayrollAPI.Models.Reservation;
using PayrollAPI.Services;

namespace PayrollAPI.Repository.Reservation
{
    public class ReservationRepository: IReservation
    {
        private readonly HRMDBConnect _context;
        Common com = new Common();
        public ReservationRepository(HRMDBConnect db)
        {
            _context = db;
        }

        public async Task<IEnumerable<Bungalow>> GetAllBungalows()
        {
            return await Task.FromResult(_context.Bungalow
                .Include(x => x.rates)
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
                existingBungalow.noOfRooms = bungalow.noOfRooms;
                existingBungalow.reopenDate = bungalow.reopenDate;

                existingBungalow.lastUpdateBy = bungalow.lastUpdateBy;
                existingBungalow.lastUpdateDate = bungalow.lastUpdateDate;
                existingBungalow.lastUpdateTime = bungalow.lastUpdateTime;

                await _context.SaveChangesAsync();

                return await Task.FromResult(true);
            }
        }
    }
}
