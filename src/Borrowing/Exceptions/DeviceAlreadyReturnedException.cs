/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

namespace DeviceManagement.Borrowing.Exceptions
{
    // Thrown when an item on the slip is submitted for return more than once.
    public class DeviceAlreadyReturnedException : BorrowingException
    {
        public string DeviceId { get; }

        public DeviceAlreadyReturnedException(string deviceId)
            : base($"Device '{deviceId}' on this slip has already been returned.")
        {
            DeviceId = deviceId;
        }
    }
}