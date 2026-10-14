/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

namespace DeviceManagement.Devices
{
    /// <summary>
    /// Concrete class representing a wireless or wired teaching microphone.
    /// </summary>
    public class Microphone : TeachingDevice
    {
        // ====================================================================
        // Constructors
        // ====================================================================

        public Microphone(
            string deviceId,
            string deviceName,
            int yearOfUsage,
            decimal purchasePrice,
            DeviceStatus status,
            int maxBorrowPeriods,
            int maxBorrowDays)
            : base(deviceId, deviceName, yearOfUsage, purchasePrice, status, maxBorrowPeriods, maxBorrowDays)
        {
        }

        // ====================================================================
        // Polymorphic Methods
        // ====================================================================

        // Maintenance rate: 10% of initial acquisition cost per annum (due to frequent battery/capsule wear).
        public override decimal CalculateAnnualMaintenanceCost()
        {
            return PurchasePrice * 0.10m;
        }

        public override string ToString()
        {
            return $"[Microphone] {base.ToString()} | Max Days: {MaxBorrowDays}";
        }
    }
}