/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

using System;

namespace DeviceManagement.Borrowing.Exceptions
{
    // Base exception for all business domain errors occurring within the lending subsystem.
    public class BorrowingException : Exception
    {
        public BorrowingException(string message) : base(message)
        {
        }

        public BorrowingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}