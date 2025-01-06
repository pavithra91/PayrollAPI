using Leave.Contracts.Requests;
using PayrollAPI.Models.Reservation;

namespace PayrollAPI.Interfaces.Reservation
{
    public interface IReservation
    {
        Task<IEnumerable<Bungalow>> GetAllBungalows();
        Task<Bungalow> GetBungalowById(int id);
        Task<bool> CreateBungalow(Bungalow bungalow);
        Task<bool> UpdateBungalow(int id, Bungalow bungalow);


        Task<IEnumerable<BungalowRates>> GetBungalowRates(int id);
        Task<bool> UpdateBungalowRates(int id, UpdateBungalowRatesRequest request);


        Task<IEnumerable<ReservationCategory>> GetAllReservationCategories();
        Task<ReservationCategory> GetReservationCategoryById(int id);


        Task<IEnumerable<Bungalow_Reservation>> GetAllReservations();
        Task<IEnumerable<Bungalow_Reservation>> GetAllReservations(string epf);
        Task<Bungalow_Reservation> GetReservationById(int id);
        Task<string> CreateReservation(Bungalow_Reservation reservation);
        Task<bool> UpdateReservation(int id, Bungalow_Reservation reservation);
        Task<bool> CancelReservation(ReservationCancellationRequest request);
        Task<List<object>> GetRestrictedDates();
        Task<List<object>> GetRestrictedDates(int id);


        Task<List<Reservation_Payments_View>> GetReservationsPayments(ReservationPaymentRequest request);

        Task<bool> RaffelDraw();
        Task<bool> WinnerConfirmation();
    }
}
