/****************/
// Student ID: [Your Student ID]
// Full Name:  [Your Full Name]
/****************/

using System;
using System.Collections.Generic;
using System.Linq;
using DeviceManagement.Borrowing.Borrowers;
using DeviceManagement.Borrowing.Exceptions;
using DeviceManagement.Borrowing.Slips;
using DeviceManagement.Devices;
using DeviceManagement.Rooms;

namespace DeviceManagement.Test
{
    /// <summary>
    /// Verification harness executing operational lifecycle suites, polymorphic dispatch,
    /// and transactional boundary invariants across the asset management subsystem.
    /// </summary>
    public static class Test
    {
        public static void Main()
        {
            PrintBanner("ASSET MANAGEMENT & LENDING SUBSYSTEM: VERIFICATION HARNESS");

            // 1. Facility Asset Aggregation & Dynamic Maintenance Valuation
            VerifyFacilityInventoryLifecycle();

            // 2. Primary Lending Lifecycle, Fine Calculation & Ledger Reconciliation
            VerifyLendingAndSettlementPipeline();

            // 3. Transactional Invariants, Fault-Tolerance & Exception Handling
            VerifyTransactionalIntegrityAndInvariants();

            PrintBanner("ALL VERIFICATION SUITES COMPLETED SUCCESSFULLY");
        }

        // ====================================================================
        // Suite 1: Facility Inventory Aggregation & Polymorphic Dispatch
        // ====================================================================

        private static void VerifyFacilityInventoryLifecycle()
        {
            PrintSection("SUITE 1: FACILITY ASSET INVENTORY & MAINTENANCE VALUATION");

            var room = new ClassRoom("CR-402", "Advanced Computing Laboratory", 45);

            var acRemote = new AirConditionerRemote("AC-402", "Daikin Inverter Interface", 2021, 600000m, DeviceStatus.Available, 5, 7);
            var microphone = new Microphone("MIC-402", "Shure Wireless Transceiver", 2017, 2400000m, DeviceStatus.Available, 4, 3);
            var projectorRemote = new ProjectorRemote("PRJ-402", "Sony Laser Projector Interface", 2023, 800000m, DeviceStatus.Available, 6, 5);

            room.AddDevice(acRemote);
            room.AddDevice(microphone);
            room.AddDevice(projectorRemote);

            Console.WriteLine($"Classroom Initialised: '{room.RoomName}' [ID: {room.RoomId}, Capacity: {room.Capacity}]");
            Console.WriteLine($"Registered Hardware: {room.Devices.Count} unit(s) mounted.\n");

            // Verify polymorphic invocation of CalculateAnnualMaintenanceCost across heterogeneous subtypes
            // Tariffs: AC Remote (5%), Microphone (10%), Projector Remote (5%)
            decimal expectedCost = (600000m * 0.05m) + (2400000m * 0.10m) + (800000m * 0.05m);
            decimal actualCost = room.CalculateAnnualMaintenanceCost();

            Console.WriteLine($"Dynamic Maintenance Valuation: {actualCost:N0} (Expected: {expectedCost:N0})");
            Assert(actualCost == expectedCost, "Dynamic dispatch yielded an erroneous maintenance valuation.");

            // Verify lifecycle filtering: Operational tenure > 5 years OR UnderMaintenance
            List<Device> flaggedAssets = room.GetDevicesRequiringMaintenance();
            Console.WriteLine($"Servicing Audit: {flaggedAssets.Count} unit(s) flagged for scheduled maintenance:");
            foreach (Device asset in flaggedAssets)
            {
                int tenure = DateTime.Now.Year - asset.YearOfUsage;
                Console.WriteLine($"  - [{asset.DeviceId}] {asset.DeviceName} (Tenure: {tenure} yr(s), Status: {asset.Status})");
            }
            Assert(flaggedAssets.Any(d => d.DeviceId == "MIC-402"), "Asset deployed in 2017 (> 5 years) failed to be flagged.");

            Console.WriteLine();
        }

        // ====================================================================
        // Suite 2: Primary Lending Lifecycle & Ledger Reconciliation
        // ====================================================================

        private static void VerifyLendingAndSettlementPipeline()
        {
            PrintSection("SUITE 2: NOMINAL LENDING, SETTLEMENT & LEDGER RECONCILIATION");

            var ledger = new List<BorrowingSlip>();

            // Borrower initialisation
            var lecturer = new Lecturer("LEC-001", "Dr Alan Turing");
            Console.WriteLine($"Registered Borrower: {lecturer} [Quota Limit: {lecturer.GetMaximumDevices()}]");

            // Physical asset provisioning
            var projectorRemote = new ProjectorRemote("PRJ-01", "Sony Interface", 2023, 500000m, DeviceStatus.Available, 5, 5);
            var acRemote = new AirConditionerRemote("AC-01", "Daikin Interface", 2022, 400000m, DeviceStatus.Available, 5, 7);
            var wirelessMic = new Microphone("MIC-01", "Shure Transceiver", 2023, 2000000m, DeviceStatus.Available, 4, 3);
            Console.WriteLine("Provisioned Inventory: PRJ-01, AC-01, MIC-01 initialised in Available state.\n");

            // Issue primary slip
            var originDate = new DateTime(2026, 3, 10);
            var primarySlip = new BorrowingSlip("SLIP-01", lecturer, "Lecture Theatre 1", originDate);
            ledger.Add(primarySlip);
            Console.WriteLine($"Issued Slip '{primarySlip.SlipId}' for Facility '{primarySlip.TeachingRoom}' on {originDate:yyyy-MM-dd}.");

            // Allocate hardware assets
            primarySlip.AddDevice(projectorRemote, 3); // 3 days -> Due: 2026-03-13
            primarySlip.AddDevice(wirelessMic, 2);     // 2 days -> Due: 2026-03-12
            Console.WriteLine("Allocated Assets:");
            Console.WriteLine($"  - PRJ-01: Loaned for 3 day(s) | Due: {originDate.AddDays(3):yyyy-MM-dd} | State: {projectorRemote.Status}");
            Console.WriteLine($"  - MIC-01: Loaned for 2 day(s) | Due: {originDate.AddDays(2):yyyy-MM-dd} | State: {wirelessMic.Status}\n");

            // Execute punctual settlement
            DateTime punctualReturnDate = new DateTime(2026, 3, 13);
            decimal punctualFine = primarySlip.ReturnDevice("PRJ-01", punctualReturnDate);
            Console.WriteLine($"Settled Asset [PRJ-01] on {punctualReturnDate:yyyy-MM-dd} (Punctual). Fine: {punctualFine:N0} | State: {projectorRemote.Status}");
            Assert(punctualFine == 0m, "Punctual settlement must not incur a penalty fine.");
            Assert(projectorRemote.Status == DeviceStatus.Available, "Settled asset must revert to Available state.");

            // Execute overdue settlement (2 days beyond due date)
            DateTime overdueReturnDate = new DateTime(2026, 3, 14);
            decimal overdueFine = primarySlip.ReturnDevice("MIC-01", overdueReturnDate);
            Console.WriteLine($"Settled Asset [MIC-01] on {overdueReturnDate:yyyy-MM-dd} (2 Days Overdue). Fine: {overdueFine:N0} | State: {wirelessMic.Status}\n");
            Assert(overdueFine == 40000m, "Overdue penalty calculation mismatch (Expected 2 days @ 20,000/day = 40,000).");
            Assert(wirelessMic.Status == DeviceStatus.Available, "Settled asset must revert to Available state.");

            // Provision secondary slip with outstanding allocation to populate ledger categories
            var secondarySlip = new BorrowingSlip("SLIP-02", lecturer, "Seminar Room B", new DateTime(2026, 3, 11));
            secondarySlip.AddDevice(acRemote, 4);
            ledger.Add(secondarySlip);

            // Audit ledger segments: Overdue accounts vs Active outstanding loans
            Console.WriteLine("Ledger Audit Reconciliation:");

            var overdueSlips = ledger
                .Where(s => s.Items.Any(i => i.IsReturned && i.ReturnedDate!.Value.Date > i.DueDate.Date))
                .ToList();

            Console.WriteLine($"\n[CATEGORY A: SETTLED LOANS WITH OVERDUE PENALTIES] ({overdueSlips.Count} record(s))");
            foreach (var slip in overdueSlips)
            {
                Console.WriteLine($"  Slip ID: {slip.SlipId} | Borrower: {slip.Borrower.FullName} | Cumulative Penalty: {slip.CalculateTotalLateFee():N0}");
                foreach (var item in slip.Items.Where(i => i.IsReturned && i.ReturnedDate!.Value.Date > i.DueDate.Date))
                {
                    int daysLate = (item.ReturnedDate!.Value.Date - item.DueDate.Date).Days;
                    Console.WriteLine($"    -> [{item.Device.DeviceId}] {item.Device.DeviceName} (Due: {item.DueDate:yyyy-MM-dd}, Returned: {item.ReturnedDate:yyyy-MM-dd}, Latency: {daysLate} day(s))");
                }
            }

            var outstandingSlips = ledger
                .Where(s => s.Items.Any(i => !i.IsReturned))
                .ToList();

            Console.WriteLine($"\n[CATEGORY B: ACTIVE OUTSTANDING LOANS] ({outstandingSlips.Count} record(s))");
            foreach (var slip in outstandingSlips)
            {
                Console.WriteLine($"  Slip ID: {slip.SlipId} | Borrower: {slip.Borrower.FullName} | Facility: {slip.TeachingRoom}");
                foreach (var item in slip.Items.Where(i => !i.IsReturned))
                {
                    Console.WriteLine($"    -> [{item.Device.DeviceId}] {item.Device.DeviceName} (Checkout: {item.BorrowedDate:yyyy-MM-dd}, Due: {item.DueDate:yyyy-MM-dd})");
                }
            }

            Console.WriteLine();
        }

        // ====================================================================
        // Suite 3: Transactional Boundary Invariants & Exception Dispatch
        // ====================================================================

        private static void VerifyTransactionalIntegrityAndInvariants()
        {
            PrintSection("SUITE 3: TRANSACTIONAL INVARIANTS & EXCEPTION DISPATCH");

            var lecturer = new Lecturer("LEC-002", "Prof Charles Babbage");
            var assistant = new TeachingAssistant("TA-001", "Ada Lovelace");
            DateTime baseDate = new DateTime(2026, 4, 1);

            // Invariant 1: Allocation guard on decommissioned / servicing assets
            RunInvariantTest("Guard against allocation of assets in servicing state", () =>
            {
                var slip = new BorrowingSlip("SLIP-INV-1", lecturer, "Facility 101", baseDate);
                var defectiveAc = new AirConditionerRemote("AC-MNT", "Midea Interface", 2020, 400000m, DeviceStatus.Available, 5, 7);
                defectiveAc.SendToMaintenance();

                try
                {
                    slip.AddDevice(defectiveAc, 2);
                    Fail("Subsystem permitted allocation of an asset marked UnderMaintenance.");
                }
                catch (DeviceUnavailableException ex)
                {
                    Pass($"Caught {ex.GetType().Name}. Context: Asset='{ex.DeviceId}', Status='{ex.CurrentStatus}'");
                    Assert(slip.Items.Count == 0, "Slip item ledger mutated despite transaction abort.");
                }
                finally
                {
                    Console.WriteLine("    [CLEANUP] Maintenance boundary invariant intact.");
                }
            });

            // Invariant 2: Borrower statutory allocation limit enforcement
            RunInvariantTest("Enforce statutory borrowing limits (Teaching Assistant limit = 2)", () =>
            {
                var slip = new BorrowingSlip("SLIP-INV-2", assistant, "Facility 102", baseDate);
                var dev1 = new ProjectorRemote("PRJ-T1", "Optoma Remote", 2022, 300000m, DeviceStatus.Available, 5, 5);
                var dev2 = new Microphone("MIC-T2", "Rode Transceiver", 2023, 1000000m, DeviceStatus.Available, 5, 5);
                var dev3 = new AirConditionerRemote("AC-T3", "LG Remote", 2021, 350000m, DeviceStatus.Available, 5, 5);

                slip.AddDevice(dev1, 2);
                slip.AddDevice(dev2, 2);

                try
                {
                    slip.AddDevice(dev3, 2);
                    Fail("Subsystem permitted allocation beyond borrower statutory threshold.");
                }
                catch (BorrowLimitExceededException ex)
                {
                    Pass($"Caught {ex.GetType().Name}. Context: Borrower='{ex.BorrowerId}', Ceiling={ex.MaximumDevices}");
                    Assert(slip.Items.Count == 2, "Unauthorised item appended to slip collection.");
                    Assert(dev3.Status == DeviceStatus.Available, "Rejected asset state was corrupted.");
                }
                finally
                {
                    Console.WriteLine("    [CLEANUP] Statutory quota boundary invariant intact.");
                }
            });

            // Invariant 3: Asset duplication guard within identical transaction
            RunInvariantTest("Prevent duplicate asset entry on a single slip", () =>
            {
                var slip = new BorrowingSlip("SLIP-INV-3", lecturer, "Facility 103", baseDate);
                var mic = new Microphone("MIC-DUP", "Sony Handheld", 2024, 1500000m, DeviceStatus.Available, 4, 3);

                slip.AddDevice(mic, 1);

                try
                {
                    slip.AddDevice(mic, 1);
                    Fail("Subsystem allowed registration of duplicate asset identifier on active slip.");
                }
                catch (DuplicateDeviceException ex)
                {
                    Pass($"Caught {ex.GetType().Name}. Context: Duplicate ID='{ex.DeviceId}'");
                    Assert(slip.Items.Count == 1, "Duplicate record registered in slip collection.");
                }
                finally
                {
                    Console.WriteLine("    [CLEANUP] Asset uniqueness invariant intact.");
                }
            });

            // Invariant 4: Maximum lending duration constraint
            RunInvariantTest("Validate loan duration ceiling against asset parameters", () =>
            {
                var slip = new BorrowingSlip("SLIP-INV-4", lecturer, "Facility 104", baseDate);
                var constrainedMic = new Microphone("MIC-LIMIT", "Boundary Condenser", 2023, 2500000m, DeviceStatus.Available, 2, 1);

                try
                {
                    slip.AddDevice(constrainedMic, 3); // Requested: 3 days | Ceiling: 1 day
                    Fail("Subsystem allowed loan duration exceeding asset maximum allowable threshold.");
                }
                catch (InvalidBorrowPeriodException ex)
                {
                    Pass($"Caught {ex.GetType().Name}. Context: Asset='{ex.DeviceId}', Requested={ex.RequestedDays}d, Ceiling={ex.MaxAllowedDays}d");
                    Assert(constrainedMic.Status == DeviceStatus.Available, "Rejected asset status mutated.");
                    Assert(slip.Items.Count == 0, "Slip item count altered despite failure.");
                }
                finally
                {
                    Console.WriteLine("    [CLEANUP] Duration ceiling invariant intact.");
                }
            });

            // Invariant 5: Rejection of unassociated asset clearance
            RunInvariantTest("Reject return clearance for assets unassociated with target slip", () =>
            {
                var slip = new BorrowingSlip("SLIP-INV-5", lecturer, "Facility 105", baseDate);
                var boundDev = new AirConditionerRemote("AC-BOUND", "Panasonic Interface", 2021, 300000m, DeviceStatus.Available, 5, 5);
                slip.AddDevice(boundDev, 2);

                var foreignProjectorRemote = new ProjectorRemote("PRJ-FOREIGN", "Extraneous Interface", 2022, 500000m, DeviceStatus.Available, 5, 5);

                try
                {
                    slip.ReturnDevice(foreignProjectorRemote.DeviceId, baseDate.AddDays(1));
                    Fail("Subsystem cleared return for an asset unassociated with the slip.");
                }
                catch (DeviceNotInSlipException ex)
                {
                    Pass($"Caught {ex.GetType().Name}. Context: Unassociated Asset ID='{ex.DeviceId}'");
                }
                finally
                {
                    Console.WriteLine("    [CLEANUP] Slip association invariant intact.");
                }
            });

            // Invariant 6: Redundant settlement idempotency guard
            RunInvariantTest("Prevent redundant settlement clearance on previously returned items", () =>
            {
                var slip = new BorrowingSlip("SLIP-INV-6", lecturer, "Facility 106", baseDate);
                var dev = new ProjectorRemote("PRJ-CLR", "BenQ Interface", 2022, 400000m, DeviceStatus.Available, 5, 5);
                slip.AddDevice(dev, 2);

                // Initial return
                slip.ReturnDevice("PRJ-CLR", baseDate.AddDays(1));

                try
                {
                    // Redundant secondary return
                    slip.ReturnDevice("PRJ-CLR", baseDate.AddDays(2));
                    Fail("Subsystem processed secondary settlement on cleared line item.");
                }
                catch (DeviceAlreadyReturnedException ex)
                {
                    Pass($"Caught {ex.GetType().Name}. Context: Previously Cleared ID='{ex.DeviceId}'");
                }
                finally
                {
                    Console.WriteLine("    [CLEANUP] Settlement idempotency invariant intact.");
                }
            });

            // Invariant 7: Temporal monotonicity & Transactional rollback
            RunInvariantTest("Enforce temporal monotonicity: Return date must not precede loan checkout", () =>
            {
                DateTime loanDate = new DateTime(2026, 4, 10);
                var slip = new BorrowingSlip("SLIP-INV-7", lecturer, "Facility 107", loanDate);
                var dev = new AirConditionerRemote("AC-CHRONO", "Midea Interface", 2023, 250000m, DeviceStatus.Available, 5, 5);
                slip.AddDevice(dev, 3);

                DateTime chronologicalAnomalyDate = new DateTime(2026, 4, 8); // Precedes origin by 2 days

                try
                {
                    slip.ReturnDevice("AC-CHRONO", chronologicalAnomalyDate);
                    Fail("Subsystem accepted clearance date preceding transaction inception.");
                }
                catch (ArgumentException ex)
                {
                    Pass($"Caught {ex.GetType().Name}. Fault: {ex.Message}");

                    // Confirm transactional atomicity
                    BorrowItem item = slip.Items.First(i => i.Device.DeviceId == "AC-CHRONO");
                    Assert(!item.IsReturned, "Line item state mutated to cleared despite exception.");
                    Assert(dev.Status == DeviceStatus.Borrowed, "Asset state prematurely reverted to Available.");
                    Pass("Transactional atomicity confirmed: Asset state unchanged; Line item remains unsettled.");
                }
                finally
                {
                    Console.WriteLine("    [CLEANUP] Temporal monotonicity and atomicity preserved.");
                }
            });

            Console.WriteLine();
        }

        // ====================================================================
        // Diagnostic Harness & Formatting Helpers
        // ====================================================================

        private static void RunInvariantTest(string specification, Action action)
        {
            Console.WriteLine($"--- [SPECIFICATION] {specification} ---");
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                Fail($"Unexpected system fault during test execution: {ex.GetType().Name} -> {ex.Message}");
            }
            Console.WriteLine();
        }

        private static void Pass(string narrative)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("  [ASSERTION OK] ");
            Console.ResetColor();
            Console.WriteLine(narrative);
        }

        private static void Fail(string narrative)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("  [ASSERTION FAILED] ");
            Console.ResetColor();
            Console.WriteLine(narrative);
            throw new InvalidOperationException($"Invariant violation: {narrative}");
        }

        private static void Assert(bool condition, string failureNarrative)
        {
            if (!condition)
            {
                Fail(failureNarrative);
            }
        }

        private static void PrintBanner(string header)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', 80));
            Console.WriteLine($"  {header}");
            Console.WriteLine(new string('=', 80));
            Console.ResetColor();
        }

        private static void PrintSection(string title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('-', 80));
            Console.WriteLine($"  {title}");
            Console.WriteLine(new string('-', 80));
            Console.ResetColor();
        }
    }
}