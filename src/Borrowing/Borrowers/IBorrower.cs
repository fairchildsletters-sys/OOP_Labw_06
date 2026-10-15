/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

namespace DeviceManagement.Borrowing.Borrowers
{
    // Contract defining behavioural specifications for eligible academic borrowers
    public interface IBorrower
    {
        string BorrowerId { get; }
        string FullName { get; }

        // Returns the maximum number of devices permitted on a single borrowing slip
        int GetMaximumDevices();
    }
}