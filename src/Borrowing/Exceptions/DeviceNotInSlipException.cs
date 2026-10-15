/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

namespace DeviceManagement.Borrowing.Exceptions
{
    // Thrown when attempting to process a return for a device identifier absent from the slip.
    public class DeviceNotInSlipException : BorrowingException
    {
        public string DeviceId { get; }

        public DeviceNotInSlipException(string deviceId)
            : base($"Device '{deviceId}' does not belong to this borrowing slip.")
        {
            DeviceId = deviceId;
        }
    }
}