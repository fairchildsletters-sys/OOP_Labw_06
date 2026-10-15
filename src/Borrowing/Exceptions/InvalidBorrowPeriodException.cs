/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

namespace DeviceManagement.Borrowing.Exceptions
{
    // Thrown when requested loan days are non-positive or exceed the device's allowable limit.
    public class InvalidBorrowPeriodException : BorrowingException
    {
        public string DeviceId { get; }
        public int RequestedDays { get; }
        public int MaxAllowedDays { get; }

        public InvalidBorrowPeriodException(string deviceId, int requestedDays, int maxAllowedDays)
            : base($"Invalid loan period of {requestedDays} day(s) for device '{deviceId}'. Maximum allowable: {maxAllowedDays} day(s).")
        {
            DeviceId = deviceId;
            RequestedDays = requestedDays;
            MaxAllowedDays = maxAllowedDays;
        }
    }
}