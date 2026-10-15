/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

using System;

namespace DeviceManagement.Borrowing.Borrowers
{
    /// <summary>
    /// Represents a teaching assistant holding an allocation quota of 2 devices per slip.
    /// </summary>
    public class TeachingAssistant : IBorrower
    {
        // ====================================================================
        // Properties
        // ====================================================================

        public string BorrowerId { get; }
        public string FullName { get; }

        // ====================================================================
        // Constructors
        // ====================================================================

        public TeachingAssistant(string borrowerId, string fullName)
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

        // Quota rule: Teaching assistants are permitted to loan at most 2 devices per slip.
        public int GetMaximumDevices()
        {
            return 2;
        }

        public override string ToString()
        {
            return $"[Teaching Assistant] {FullName} (ID: {BorrowerId})";
        }
    }
}
