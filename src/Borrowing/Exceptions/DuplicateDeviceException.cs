/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

namespace DeviceManagement.Borrowing.Exceptions
{
    // Thrown when attempting to add the same physical device twice to a single borrowing slip.
    public class DuplicateDeviceException : BorrowingException
    {
        public string DeviceId { get; }

        public DuplicateDeviceException(string deviceId)
            : base($"Device '{deviceId}' has already been added to this borrowing slip.")
        {
            DeviceId = deviceId;
        }
    }
}