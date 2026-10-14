/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

namespace DeviceManagement.Devices
{
    /// <summary>
    /// Concrete class representing an air conditioner remote control unit.
    /// </summary>
    public class AirConditionerRemote : TeachingDevice
    {
        // ====================================================================
        // Constructors
        // ====================================================================

        public AirConditionerRemote(
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
        // Methods
        // ====================================================================

        // Maintenance rate: 5% of initial acquisition cost per annum.
        public override decimal CalculateAnnualMaintenanceCost()
        {
            return PurchasePrice * 0.05m;
        }

        public override string ToString()
        {
            return $"[AC Remote] {base.ToString()} | Max Days: {MaxBorrowDays}";
        }
    }
}