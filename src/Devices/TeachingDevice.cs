/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

using System;

namespace DeviceManagement.Devices
{
    /// <summary>
    /// Abstract intermediate class representing teaching devices eligible for short-term lending.
    /// </summary>
    public abstract class TeachingDevice : Device
    {
        // ====================================================================
        // Fields
        // ====================================================================
        private int _maxBorrowPeriods;
        private int _maxBorrowDays;

        // ====================================================================
        // Properties
        // ====================================================================

        /// Maximum allowable lending duration (periods)
        public int MaxBorrowPeriods
        {
            get => _maxBorrowPeriods;
            protected set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Max borrow periods must be strictly greater than zero.");
                }
                _maxBorrowPeriods = value;
            }
        }

        // Maximum allowable lending duration (days)
        public int MaxBorrowDays
        {
            get => _maxBorrowDays;
            protected set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Max borrow days must be strictly greater than zero.");
                }
                _maxBorrowDays = value;
            }
        }

        // ====================================================================
        // Constructors
        // ====================================================================

        protected TeachingDevice(
            string deviceId,
            string deviceName,
            int yearOfUsage,
            decimal purchasePrice,
            DeviceStatus status,
            int maxBorrowPeriods,
            int maxBorrowDays)
            : base(deviceId, deviceName, yearOfUsage, purchasePrice, status)
        {
            MaxBorrowPeriods = maxBorrowPeriods;    // setter
            MaxBorrowDays = maxBorrowDays;          // setter
        }

        // ====================================================================
        // Circulation State Methods
        // ====================================================================

        // Marks the device as borrowed
        // Permitted only when current status is Available
        public void MarkAsBorrowed()
        {
            if (Status != DeviceStatus.Available)
            {
                throw new InvalidOperationException($"Cannot loan device '{DeviceId}'. Expected status: Available, actual: {Status}.");
            }
            Status = DeviceStatus.Borrowed;
        }
    }
}