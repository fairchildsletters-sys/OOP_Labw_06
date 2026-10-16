# Teaching Equipment & Borrowing Management System

A C# (.NET) management system governing educational hardware inventory, classroom equipment allocation, and transactional borrowing workflows. The architecture strictly adheres to core Object-Oriented Programming (OOP) paradigms: Encapsulation, Inheritance, Abstract Classes, Interfaces, Subtype Polymorphism, Aggregation/Composition, and robust Domain Exception Handling.

---

## 1. Architecture & OOP Principles

1. **Inheritance Hierarchy:**
   * The abstract class `Device` serves as the foundational base, defining intrinsic hardware attributes and shared lifecycle contracts.
   * The intermediate abstract class `TeachingDevice` inherits from `Device`, introducing domain-specific properties for pedagogical contexts, notably borrowing constraints (`MaxBorrowPeriod`/`MaxBorrowDays`).
   * Concrete specialisations `AirConditionerRemote`, `Microphone`, and `ProjectorRemote` inherit directly from `TeachingDevice`, each encapsulating domain-specific algorithms for projected annual maintenance costs.

2. **Abstract Classes & Interfaces:**
   * **Abstract Class `Device`:** Enforces structural contracts across all inventory items whilst preventing direct instantiation of non-specific devices. Requires concrete subclasses to supply implementations for `CalculateAnnualMaintenanceCost()` and override `ToString()`.
   * **Interface `IBorrower`:** Establishes a decoupling abstraction for borrower roles independently of university personnel structures. Dictates the contract for `BorrowerId`, `FullName`, and `GetMaximumDevices()`.
   * Concrete implementations `Lecturer` (limit: 3 devices) and `TeachingAssistant` (limit: 2 devices) realize `IBorrower`, facilitating open-ended extension to new borrower roles without modifying slip orchestration logic.

3. **Polymorphism & Dynamic Dispatch:**
   * **Subtype Polymorphism:** Annual maintenance expenditure is dynamically resolved across specific device classes via the overridden `CalculateAnnualMaintenanceCost()` method. The managing `ClassRoom` aggregate computes compound maintenance budgets without relying on conditional branch inspection (`if/else` or `switch-case`).
   * **Interface-Driven Polymorphism:** The `BorrowingSlip` aggregate coordinates borrower interactions strictly through the `IBorrower` interface, dynamically enforcing distinct quota limits at runtime.

4. **Aggregation vs. Composition:**
   * **Aggregation (`ClassRoom`):** Classrooms aggregate a collection of `Device` instances (`List<Device>`). The lifecycle of a physical device is independent of any classroom allocation.
   * **Composition (`BorrowingSlip`):** A borrowing slip wholly owns its constituent `BorrowItem` entities. Disposing of or clearing a slip cascades across its dependent items.
   * **Non-owning References:** A `BorrowItem` merely references its associated `TeachingDevice`, and `BorrowingSlip` maintains an unowned reference to `IBorrower`. Deleting a transaction slip never deletes the underlying device or personnel records from the system.

5. **Encapsulation, Immutability & State Consistency:**
   * **Immutability:** Primary keys and entity identifiers (`DeviceId`, `SlipId`, `BorrowerId`) expose immutable getter-only accessors (`{ get; }`) initialised strictly via constructor injection.
   * **Domain Invariants:** Enforced via property validation logic (`PurchasePrice > 0`, `YearOfUsage <= DateTime.Now.Year`, non-null string invariants, and chronologically sound borrowing intervals).
   * **Transactional Integrity:** Methods altering system state (such as `AddDevice`) validate all pre-conditions (valid non-null reference, quota availability, absence of duplicate items, and `DeviceStatus.Available` availability) before mutating device status to `Borrowed`. Should validation fail, operations abort cleanly without partial mutations.

6. **Domain-Specific Exception Hierarchy:**
   * A root `BorrowingException` derived from `System.Exception` establishes consistent domain error boundaries.
   * Expressive specialized sub-exceptions preserve diagnostic runtime context:
     * `DeviceUnavailableException`: Exposes `DeviceId` and `CurrentStatus`.
     * `BorrowLimitExceededException`: Exposes `BorrowerId` and `MaximumDevices`.
     * `InvalidBorrowPeriodException`: Encapsulates requested duration against permissible thresholds.
     * `DuplicateDeviceException`: Raised upon redundant device insertion into the same slip.
     * `DeviceNotInSlipException`: Raised when return workflows encounter an untracked device.
     * `DeviceAlreadyReturnedException`: Guard against redundant return operations.

---

## 2. Directory Layout

```text
.
├── src/
│   ├── Devices/
│   │   ├── DeviceStatus.cs                  # Device operational status enumeration
│   │   ├── Device.cs                        # Foundational abstract hardware class
│   │   ├── TeachingDevice.cs                # Abstract educational equipment class with quota constraints
│   │   ├── AirConditionerRemote.cs          # Specialised air conditioner remote control
│   │   ├── Microphone.cs                    # Specialised classroom microphone
│   │   └── ProjectorRemote.cs               # Specialised projector remote control
│   │
│   ├── Rooms/
│   │   └── ClassRoom.cs                     # Classroom aggregate managing device inventory
│   │
│   ├── Borrowing/
│   │   ├── Borrowers/
│   │   │   ├── IBorrower.cs                 # Decoupled borrower interface contract
│   │   │   ├── Lecturer.cs                  # Lecturer borrower implementation (quota: 3)
│   │   │   └── TeachingAssistant.cs         # Teaching assistant implementation (quota: 2)
│   │   │
│   │   ├── Slips/
│   │   │   ├── BorrowItem.cs                # Itemized transaction record entity
│   │   │   └── BorrowingSlip.cs             # Aggregate slip entity coordinating borrow/return workflows
│   │   │
│   │   └── Exceptions/
│   │       ├── BorrowingException.cs        # Base domain exception
│   │       ├── DeviceUnavailableException.cs
│   │       ├── BorrowLimitExceededException.cs
│   │       ├── InvalidBorrowPeriodException.cs
│   │       ├── DuplicateDeviceException.cs
│   │       ├── DeviceNotInSlipException.cs
│   │       └── DeviceAlreadyReturnedException.cs
│   │
│   └── Test/
│       └── TestRunner.cs                    # Centralized harness containing the Main entry point
│
├── Lab06.csproj                             # .NET project configuration file
├── Design.pdf
└── README.md                                # System technical specifications and documentation

```

---

## 3. Build & Execution Guide

### Prerequisites

* .NET SDK 8.0 or later (or an active Pixi workspace providing `dotnet-sdk`).

### Running the Application

Using the standard .NET CLI:

```bash
dotnet run

```

Alternatively, invoking via Pixi:

```bash
pixi run dotnet run

```

---

## 4. Verification Suite

The test harness located in `src/Test/TestRunner.cs` exercises full verification coverage across normal and abnormal domain scenarios:

### 4.1. Success Scenarios (Happy Path)

1. **Fixture Initialisation:** Instantiates a `Lecturer`, a projector remote, an air conditioner remote, and a wireless microphone.
2. **Slip Provisioning:** Creates an operational `BorrowingSlip` bound to a specific lecture hall.
3. **Device Enrolment:** Successfully appends the projector remote and microphone; verifies atomic transition from `Available` to `Borrowed`.
4. **Timely Return:** Processes the return of the projector remote on schedule; confirms the transition back to `Available` with a late fee of 0.
5. **Overdue Return & Penalty Computation:** Processes the return of the microphone two days overdue; verifies fine calculations and state restitution to `Available`.
6. **Reporting:** Displays active registers for overdue slips and unreturned assets.

### 4.2. Exception Scenarios (Fault Trapping & Invariance)

Validates boundary integrity and atomic rollback using controlled `try-catch` assertions:

1. **Unavailable Asset Request:** Attempting to borrow a remote marked `UnderMaintenance`; traps `DeviceUnavailableException`.
2. **Quota Transgression:** A `TeachingAssistant` attempting to checkout a third item; traps `BorrowLimitExceededException`.
3. **Duplicate Enrolment:** Inserting an identical microphone twice within the same slip; traps `DuplicateDeviceException`.
4. **Period Threshold Violation:** Requesting a 3-day loan on a microphone constrained to a 1-day ceiling; traps `InvalidBorrowPeriodException`.
5. **Unregistered Asset Return:** Submitting a return for a device not associated with the slip; traps `DeviceNotInSlipException`.
6. **Redundant Return Attempt:** Submitting a return for an item already marked returned; traps `DeviceAlreadyReturnedException`.
7. **Invalid Return Date:** Supplying a return timestamp preceding the borrowing date; rejected with validation failure.