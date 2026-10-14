/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

using System;

namespace DeviceManagement.Devices
{
    /// <summary>
    /// Defining the foundational state and contractual behaviours of all devices.
    /// </summary>
    public abstract class Device
    {
        // ====================================================================
        // Fields (Backing fields)
        // ====================================================================

        private string _deviceName;
        private int _yearOfUsage;
        private decimal _purchasePrice;

        // ====================================================================
        // Properties
        // ====================================================================

        // ID: Immutable post-initialisation.
        public string DeviceId { get; }

        // Device Name: non-null, non-empty, trimmed of whitespace.
        public string DeviceName
        {
            get => _deviceName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Device name cannot be null or empty.", nameof(value));
                }
                _deviceName = value.Trim();
            }
        }

        // Year of Usage: must not exceed the current calendar year.
        public int YearOfUsage
        {
            get => _yearOfUsage;
            set
            {
                if (value > DateTime.Now.Year)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Year of usage cannot exceed the current calendar year.");
                }
                _yearOfUsage = value;
            }
        }

        // Purchase Price: must be strictly greater than zero.
        public decimal PurchasePrice
        {
            get => _purchasePrice;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Purchase price must be strictly greater than zero.");
                }
                _purchasePrice = value;
            }
        }

        // Current state: read-only publicly; state mutations occur via explicit methods.
        public DeviceStatus Status { get; protected set; }

        // ====================================================================
        // Constructors
        // ====================================================================

        protected Device(string deviceId, string deviceName, int yearOfUsage, decimal purchasePrice, DeviceStatus status)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException("Device ID cannot be null or empty.", nameof(deviceId));
            }
            DeviceId = deviceId.Trim();

            DeviceName = deviceName;       // setter
            YearOfUsage = yearOfUsage;     // setter
            PurchasePrice = purchasePrice; // setter
            Status = status;
        }

        // ====================================================================
        // Abstract & Virtual Methods
        // ====================================================================

        // Computes the annual maintenance expenditure (polymorphic)
        public abstract decimal CalculateAnnualMaintenanceCost();

        // Returns a formatted textual display of the device.
        public override string ToString()
        {
            return $"[{DeviceId}] {DeviceName} | Year: {YearOfUsage} | Price: {PurchasePrice:C0} | Status: {Status}";
        }

        // ====================================================================
        // State Transition Methods (Lifecycle behaviours)
        // ====================================================================

        public void MarkAsAvailable()
        {
            if (Status == DeviceStatus.Retired)
            {
                throw new InvalidOperationException($"Cannot restore device '{DeviceId}' to Available because it is permanently retired.");
            }

            Status = DeviceStatus.Available;
        }

        public void SendToMaintenance()
        {
            if (Status == DeviceStatus.Borrowed)
            {
                throw new InvalidOperationException($"Cannot send device '{DeviceId}' to maintenance whilst it is currently borrowed.");
            }

            if (Status == DeviceStatus.Retired)
            {
                throw new InvalidOperationException($"Cannot send device '{DeviceId}' to maintenance because it has been decommissioned.");
            }

            Status = DeviceStatus.UnderMaintenance;
        }

        public void Retire()
        {
            if (Status == DeviceStatus.Borrowed)
            {
                throw new InvalidOperationException($"Cannot decommission device '{DeviceId}' whilst it is currently on loan.");
            }

            Status = DeviceStatus.Retired;
        }
    }
}