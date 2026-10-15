/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

using DeviceManagement.Devices;

namespace DeviceManagement.Borrowing.Exceptions
{
    // Thrown when an allocation attempt is made on a device not in the Available state.
    public class DeviceUnavailableException : BorrowingException
    {
        public string DeviceId { get; }
        public DeviceStatus CurrentStatus { get; }

        public DeviceUnavailableException(string deviceId, DeviceStatus currentStatus)
            : base($"Device '{deviceId}' is unavailable for borrowing (Current state: {currentStatus}).")
        {
            DeviceId = deviceId;
            CurrentStatus = currentStatus;
        }
    }
}