/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

using System;
using System.Collections.Generic;
using System.Linq;
using DeviceManagement.Devices;

namespace DeviceManagement.Rooms
{
    /// <summary>
    /// Represents a classroom managing a collection of devices.
    /// </summary>
    public class ClassRoom
    {
        // ====================================================================
        // Fields
        // ====================================================================

        private string _roomName;
        private int _capacity;
        private readonly List<Device> _devices;

        // ====================================================================
        // Properties
        // ====================================================================

        // ID: immutable post-initialisation.
        public string RoomId { get; }

        // Room name: non-null, non-empty
        public string RoomName
        {
            get => _roomName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Room name cannot be null or empty.", nameof(value));
                }
                _roomName = value.Trim();
            }
        }

        // Capacity: must be strictly greater than zero
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Capacity must be strictly greater than zero.");
                }
                _capacity = value;
            }
        }

        // Exposes an unmodifiable read-only wrapper of the internal device list
        public IReadOnlyList<Device> Devices => _devices.AsReadOnly();

        // ====================================================================
        // Constructors
        // ====================================================================

        public ClassRoom(string roomId, string roomName, int capacity)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                throw new ArgumentException("Room ID cannot be null or empty.", nameof(roomId));
            }
            RoomId = roomId.Trim();

            RoomName = roomName;        // setter
            Capacity = capacity;        // setter
            _devices = new List<Device>();
        }

        // ====================================================================
        // Device Management Methods (Aggregation semantics)
        // ====================================================================

        // Adds a device to the room ensuring no nulls and no duplicate IDs
        public void AddDevice(Device device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device), "Device cannot be null.");
            }

            if (_devices.Any(d => d.DeviceId.Equals(device.DeviceId, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Device with ID '{device.DeviceId}' already exists in room '{RoomId}'.");
            }

            _devices.Add(device);
        }

        // Removes a device by ID; returns true if removed, false if not found
        public bool RemoveDevice(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException("Device ID to remove cannot be null or empty.", nameof(deviceId));
            }

            Device? deviceToRemove = FindDevice(deviceId);
            if (deviceToRemove == null)
            {
                return false;
            }

            return _devices.Remove(deviceToRemove);
        }

        // Finds and returns a device matching the ID, or null if absent
        public Device? FindDevice(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new ArgumentException("Device ID to find cannot be null or empty.", nameof(deviceId));
            }

            return _devices.FirstOrDefault(d => d.DeviceId.Equals(deviceId.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        // Computes total annual maintenance cost (dynamic polymorphism)
        public decimal CalculateAnnualMaintenanceCost()
        {
            decimal totalCost = 0m;
            foreach (Device device in _devices)
            {
                // Polymorphic invocation: runtime dispatches to the derived class implementation
                totalCost += device.CalculateAnnualMaintenanceCost();
            }
            return totalCost;
        }

        // Filters and returns assets currently under maintenance or deployed for over 5 years
        public List<Device> GetDevicesRequiringMaintenance()
        {
            int currentYear = DateTime.Now.Year;
            List<Device> results = new List<Device>();

            foreach (Device device in _devices)
            {
                bool isUnderMaintenance = device.Status == DeviceStatus.UnderMaintenance;
                bool isOverFiveYearsOld = (currentYear - device.YearOfUsage) > 5;

                if (isUnderMaintenance || isOverFiveYearsOld)
                {
                    results.Add(device);
                }
            }

            return results;
        }
    }
}