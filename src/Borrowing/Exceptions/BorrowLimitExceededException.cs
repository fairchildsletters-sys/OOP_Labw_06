/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

namespace DeviceManagement.Borrowing.Exceptions
{
    // Thrown when a borrower attempts to loan more items than their permitted quota.
    public class BorrowLimitExceededException : BorrowingException
    {
        public string BorrowerId { get; }
        public int MaximumDevices { get; }

        public BorrowLimitExceededException(string borrowerId, int maximumDevices)
            : base($"Borrower '{borrowerId}' has reached the maximum lending limit of {maximumDevices} device(s).")
        {
            BorrowerId = borrowerId;
            MaximumDevices = maximumDevices;
        }
    }
}