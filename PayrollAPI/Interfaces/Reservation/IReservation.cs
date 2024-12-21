using PayrollAPI.Models.Reservation;

namespace PayrollAPI.Interfaces.Reservation
{
    public interface IReservation
    {
        Task<IEnumerable<Bungalow>> GetAllBungalows();
        Task<Bungalow> GetBungalowById(int id);
        Task<bool> CreateBungalow(Bungalow bungalow);
        Task<bool> UpdateBungalow(int id, Bungalow bungalow);
    }
}
