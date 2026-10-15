/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

using System;
using DeviceManagement.Devices;
using DeviceManagement.Borrowing.Exceptions;

namespace DeviceManagement.Borrowing.Slips
{
    /// <summary>
    /// Represents a line-item record of a specific teaching device on loan within a slip.
    /// </summary>
    public class BorrowItem
    {

        // ====================================================================
        // Fields
        // ====================================================================

        private int _fromPeriod;
        private int _toPeriod;

        // ====================================================================
        // Properties
        // ====================================================================

        // Reference to the physical teaching asset on loan.
        public TeachingDevice Device { get; }

        public DateTime BorrowedDate { get; }

        public DateTime DueDate { get; }

        // Academic period trackers (supplementary timetable parameters)
        public int FromPeriod
        {
            get => _fromPeriod;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "FromPeriod must be strictly positive.");
                }
                if (_toPeriod > 0 && value > _toPeriod)
                {
                    throw new ArgumentException("FromPeriod cannot be greater than ToPeriod.");
                }
                _fromPeriod = value;
            }
        }

        public int ToPeriod
        {
            get => _toPeriod;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "ToPeriod must be strictly positive.");
                }
                if (value < _fromPeriod)
                {
                    throw new ArgumentException("ToPeriod cannot be less than FromPeriod.");
                }
                _toPeriod = value;
            }
        }

        // Actual return timestamp; initialised to null until formal return processing
        public DateTime? ReturnedDate { get; private set; }

        // Computed flag determining whether the item has been returned
        public bool IsReturned => ReturnedDate.HasValue;

        // ====================================================================
        // Constructors
        // ====================================================================

        public BorrowItem(TeachingDevice device, DateTime borrowedDate, DateTime dueDate)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device), "Teaching device cannot be null.");
            }

            if (dueDate.Date < borrowedDate.Date)
            {
                throw new ArgumentException("Due date cannot be earlier than borrowed date.");
            }

            Device = device;
            BorrowedDate = borrowedDate;
            DueDate = dueDate;
            ReturnedDate = null;
        }

        // ====================================================================
        // Operational Methods
        // ====================================================================

        // Records the return of the item, validating temporal invariants
        public void Return(DateTime returnedDate)
        {
            if (IsReturned)
            {
                throw new DeviceAlreadyReturnedException(Device.DeviceId);
            }

            if (returnedDate.Date < BorrowedDate.Date)
            {
                throw new ArgumentException($"Return date ({returnedDate:yyyy-MM-dd}) cannot be prior to borrowed date ({BorrowedDate:yyyy-MM-dd}).");
            }

            ReturnedDate = returnedDate;
        }
    }
}