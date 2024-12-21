namespace PayrollAPI.Data.EntityMapping
{
    public static class StatusMapper
    {
        public enum BookingStatus
        {
            Pending = 0,
            Approved = 1,
            Rejected = 2,
            Cancelled = 3
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
    }
}
