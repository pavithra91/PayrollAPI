namespace PayrollAPI.Data.EntityMapping
{
    public static class StatusMapper
    {
        public enum BookingStatus
        {
            Pending = 0,
            Confirmed = 1,
            Rejected = 2,
            Cancelled = 3,
            Raffle_Winner = 4,
            Raffle_Drawn = 5
        }

        public enum BookingPriority
        {
            HighPriority = 1,
            LowPriority = 0
        }
        public enum BookingType
        {
            CPSTL_Employee = 0,
            CPC_Employee = 1,
            Retired_Employee = 2,
            External = 3
        }

        public enum PaymentStatus
        {
            Transferred = 0,
            Processed = 1,
            ReOpened = 2,
        }

        public enum NotificationType
        {
            Leave = 0,
            Reservation = 1,
            System = 2,
        }

        public enum Cancellation_Policy
        {
            No_Cancellation = 0,
            Half_Cancellation = 1,
            Full_Cancellation = 2,
        }
    }
}
