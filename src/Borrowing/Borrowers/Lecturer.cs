/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

using System;

namespace DeviceManagement.Borrowing.Borrowers
{
    /// <summary>
    /// Represents an academic lecturer holding (quota of 3 devices per slip)
    /// </summary>
    public class Lecturer : IBorrower
    {
        // ====================================================================
        // Properties
        // ====================================================================

        public string BorrowerId { get; }
        public string FullName { get; }

        // ====================================================================
        // Constructors
        // ====================================================================

        public Lecturer(string borrowerId, string fullName)
        {
            if (string.IsNullOrWhiteSpace(borrowerId))
            {
                throw new ArgumentException("Borrower ID cannot be null or empty.", nameof(borrowerId));
            }
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Full name cannot be null or empty.", nameof(fullName));
            }

            BorrowerId = borrowerId.Trim();
            FullName = fullName.Trim();
        }

        // ====================================================================
        // Interface Implementations
        // ====================================================================

        // Quota rule: Lecturers are permitted to loan at most 3 devices per slip.
        public int GetMaximumDevices()
        {
            return 3;
        }

        public override string ToString()
        {
            return $"[Lecturer] {FullName} (ID: {BorrowerId})";
        }
    }
}