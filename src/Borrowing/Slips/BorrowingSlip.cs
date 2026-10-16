/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

using System;
using System.Collections.Generic;
using System.Linq;
using DeviceManagement.Devices;
using DeviceManagement.Borrowing.Borrowers;
using DeviceManagement.Borrowing.Exceptions;

namespace DeviceManagement.Borrowing.Slips
{
    /// <summary>
    /// Aggregate root managing the checkout and check-in lifecycle of borrowed devices.
    /// </summary>
    public class BorrowingSlip
    {
        // ====================================================================
        // Fields
        // ====================================================================

        private readonly List<BorrowItem> _items;
        
        // Daily fine tariff applied per overdue line item (20,000/day)
        private const decimal DailyLateFeeRate = 20000m;

        // ====================================================================
        // Properties
        // ====================================================================

        public string SlipId { get; }
        public IBorrower Borrower { get; }
        public string TeachingRoom { get; }
        public DateTime BorrowedDate { get; }

        // Exposes an immutable view of internal line items (Composition)
        public IReadOnlyList<BorrowItem> Items => _items.AsReadOnly();

        // ====================================================================
        // Constructors
        // ====================================================================

        public BorrowingSlip(string slipId, IBorrower borrower, string teachingRoom, DateTime borrowedDate)
        {
            if (string.IsNullOrWhiteSpace(slipId))
            {
                throw new ArgumentException("Slip ID cannot be null or empty.", nameof(slipId));
            }

            if (borrower == null)
            {
                throw new ArgumentNullException(nameof(borrower), "Borrower cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(teachingRoom))
            {
                throw new ArgumentException("Teaching room cannot be null or empty.", nameof(teachingRoom));
            }

            SlipId = slipId.Trim();
            Borrower = borrower;
            TeachingRoom = teachingRoom.Trim();
            BorrowedDate = borrowedDate;
            _items = new List<BorrowItem>();
        }

        // ====================================================================
        // Methods - Lending Operations
        // ====================================================================

        // Appends a device to the slip, ensuring transaction integrity
        public void AddDevice(TeachingDevice device, int borrowDays)
        {
            // 1. Invariant: Null validation
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device), "Device cannot be null.");
            }

            // 2. Invariant: Duration constraints
            if (borrowDays <= 0 || borrowDays > device.MaxBorrowDays)
            {
                throw new InvalidBorrowPeriodException(device.DeviceId, borrowDays, device.MaxBorrowDays);
            }

            // 3. Invariant: No duplicate items within the same slip
            if (_items.Any(item => item.Device.DeviceId.Equals(device.DeviceId, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateDeviceException(device.DeviceId);
            }

            // 4. Invariant: Borrower quota limit
            if (_items.Count >= Borrower.GetMaximumDevices())
            {
                throw new BorrowLimitExceededException(Borrower.BorrowerId, Borrower.GetMaximumDevices());
            }

            // 5. Invariant: Operational availability
            if (device.Status != DeviceStatus.Available)
            {
                throw new DeviceUnavailableException(device.DeviceId, device.Status);
            }

            // State mutation: Only executed after all pre-conditions are fully satisfied
            DateTime dueDate = BorrowedDate.AddDays(borrowDays);
            var newItem = new BorrowItem(device, BorrowedDate, dueDate);

            device.MarkAsBorrowed();
            _items.Add(newItem);
        }

        // ====================================================================
        // Methods - Return Operations
        // ====================================================================

        // Processes the return of a borrowed device, returning the calculated overdue penalty fee.
        public decimal ReturnDevice(string deviceId, DateTime returnedDate)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException("Device ID cannot be null or empty.", nameof(deviceId));
            }

            // 1. Locate item on slip
            BorrowItem? item = _items.FirstOrDefault(i => i.Device.DeviceId.Equals(deviceId.Trim(), StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                throw new DeviceNotInSlipException(deviceId.Trim());
            }

            // 2. Invariant: Item cannot already be returned
            if (item.IsReturned)
            {
                throw new DeviceAlreadyReturnedException(deviceId.Trim());
            }

            // 3. Process return on line item
            item.Return(returnedDate);

            // 4. Restore physical asset status back to Available
            item.Device.MarkAsAvailable();

            // 5. Compute fine if returned past due date
            if (returnedDate.Date > item.DueDate.Date)
            {
                int overdueDays = (returnedDate.Date - item.DueDate.Date).Days;
                return overdueDays * DailyLateFeeRate;
            }

            return 0m;
        }

        // Calculates total overdue penalty fees across all returned items in this slip.
        public decimal CalculateTotalLateFee()
        {
            decimal totalLateFee = 0m;

            foreach (BorrowItem item in _items)
            {
                if (item.IsReturned && item.ReturnedDate!.Value.Date > item.DueDate.Date)
                {
                    int overdueDays = (item.ReturnedDate.Value.Date - item.DueDate.Date).Days;
                    totalLateFee += overdueDays * DailyLateFeeRate;
                }
            }

            return totalLateFee;
        }
    }
}