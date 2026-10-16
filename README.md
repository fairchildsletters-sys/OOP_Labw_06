# Hệ thống Quản lý Thiết bị Giảng dạy và Mượn trả (Teaching Equipment & Borrowing Management System)

Chương trình quản lý danh mục thiết bị giảng dạy, phòng học và quy trình mượn - trả thiết bị bằng ngôn ngữ C# (.NET), áp dụng chặt chẽ các nguyên lý lập trình hướng đối tượng (OOP): Đóng gói, Kế thừa, Lớp trừu tượng, Giao diện (Interface), Đa hình, Kết tập/Hợp thành và Hệ thống kiểm soát ngoại lệ nghiệp vụ.

**Thông tin sinh viên:**
* **Mã sinh viên:** 202418838
* **Họ tên:**       Bùi Tuấn Anh

---

## 1. Kiến trúc và Các nguyên lý OOP

1. **Hệ phân cấp Kế thừa (Inheritance Hierarchy):**
   * Lớp trừu tượng `Device` đóng vai trò lớp cơ sở xác định các thuộc tính phần cứng cốt lõi và hành vi chung.
   * Lớp trừu tượng `TeachingDevice` kế thừa từ `Device`, bổ sung đặc tính đặc thù cho công tác giảng dạy như giới hạn thời lượng mượn tối đa (`MaxBorrowPeriod`/`MaxBorrowDays`).
   * Các lớp cụ thể `AirConditionerRemote`, `Microphone`, `ProjectorRemote` kế thừa trực tiếp từ `TeachingDevice`, cài đặt thuật toán tính chi phí bảo trì dự kiến riêng biệt cho từng chủng loại.

2. **Lớp trừu tượng và Giao diện (Abstract Class & Interface):**
   * **Lớp trừu tượng `Device`:** Ngăn chặn việc khởi tạo thực thể thiết bị chung chung; bắt buộc các lớp con thực thi phương thức trừu tượng `CalculateAnnualMaintenanceCost()` và ghi đè phương thức mô tả `ToString()`.
   * **Giao diện `IBorrower`:** Chuẩn hóa vai trò người mượn độc lập với hệ thống quản lý nhân sự, gồm `BorrowerId`, `FullName` và phương thức hợp đồng `GetMaximumDevices()`.
   * Các lớp `Lecturer` (tối đa 3 thiết bị) và `TeachingAssistant` (tối đa 2 thiết bị) thực thi `IBorrower`, cho phép mở rộng thêm các đối tượng người mượn mới mà không làm thay đổi logic xử lý phiếu mượn.

3. **Tính Đa hình và Liên kết Động (Polymorphism & Dynamic Dispatch):**
   * **Đa hình kiểu con (Subtype Polymorphism):** Phương thức `CalculateAnnualMaintenanceCost()` được ghi đè đa hình tại từng dòng thiết bị cụ thể. Lớp quản lý `ClassRoom` tính tổng chi phí bảo trì toàn phòng mà không dùng câu lệnh rẽ nhánh điều kiện (`if/else` hoặc `switch-case`) để kiểm tra kiểu đối tượng.
   * **Đa hình qua Interface:** Lớp `BorrowingSlip` tương tác với đối tượng mượn hoàn toàn thông qua hợp đồng `IBorrower`, tự động áp dụng đúng hạn mức mượn tương ứng tại thời điểm thực thi.

4. **Quan hệ Kết tập và Hợp thành (Aggregation & Composition):**
   * **Kết tập (Aggregation) trong `ClassRoom`:** Quản lý tập hợp `List<Device>`. Thiết bị tồn tại độc lập với vòng đời của phòng học.
   * **Hợp thành (Composition) trong `BorrowingSlip`:** Một phiếu mượn sở hữu danh sách `BorrowItem`. Các dòng mượn `BorrowItem` phụ thuộc hoàn toàn vào vòng đời của `BorrowingSlip`.
   * **Tham chiếu không sở hữu:** `BorrowItem` chỉ tham chiếu đến `TeachingDevice` và `BorrowingSlip` chỉ tham chiếu đến `IBorrower`. Việc xóa phiếu mượn không làm biến mất thiết bị hoặc hồ sơ người mượn khỏi hệ thống.

5. **Đóng gói, Bất biến và Tính toàn vẹn trạng thái (Encapsulation & State Consistency):**
   * **Tính bất biến (Immutability):** Các mã định danh (`DeviceId`, `SlipId`, `BorrowerId`) chỉ có khối đọc (`{ get; }`) và chỉ được thiết lập thông qua constructor.
   * **Ràng buộc miền dữ liệu:** Kiểm tra nghiêm ngặt `PurchasePrice > 0`, `YearOfUsage <= DateTime.Now.Year`, thời gian mượn dương, ngày trả không được trước ngày mượn.
   * **Bảo toàn trạng thái nguyên tử (Transactional Consistency):** Trong `AddDevice()`, mọi kiểm tra hợp lệ (thiết bị tồn tại, giới hạn mượn, trạng thái `Available`) phải hoàn tất trước khi tạo `BorrowItem` và chuyển trạng thái thiết bị sang `Borrowed`. Nếu xảy ra lỗi, trạng thái hệ thống được giữ nguyên vẹn.

6. **Hệ phân cấp Ngoại lệ Nghiệp vụ (Domain Exception Hierarchy):**
   * Xây dựng lớp cơ sở `BorrowingException` kế thừa từ `System.Exception`.
   * Các ngoại lệ dẫn xuất cụ thể chứa dữ liệu ngữ cảnh hỗ trợ truy vết lỗi:
     * `DeviceUnavailableException`: Mang theo `DeviceId`, `CurrentStatus`.
     * `BorrowLimitExceededException`: Mang theo `BorrowerId`, `MaximumDevices`.
     * `InvalidBorrowPeriodException`: Mang theo thông tin số tiết/ngày yêu cầu so với giới hạn của thiết bị.
     * `DuplicateDeviceException`: Phát sinh khi thiết bị đã tồn tại trong cùng một phiếu mượn.
     * `DeviceNotInSlipException`: Phát sinh khi yêu cầu trả thiết bị không nằm trong phiếu.
     * `DeviceAlreadyReturnedException`: Phát sinh khi yêu cầu trả một thiết bị đã hoàn tất trả trước đó.

---

## 2. Cấu trúc thư mục

```text
.
├── src/
│   ├── Devices/
│   │   ├── DeviceStatus.cs                  # Enum trạng thái thiết bị (Available, Borrowed, UnderMaintenance)
│   │   ├── Device.cs                        # Lớp trừu tượng cơ sở định nghĩa thiết bị chung
│   │   ├── TeachingDevice.cs                # Lớp trừu tượng thiết bị giảng dạy (giới hạn thời lượng mượn)
│   │   ├── AirConditionerRemote.cs          # Thiết bị điều khiển điều hòa
│   │   ├── Microphone.cs                    # Thiết bị micro giảng dạy
│   │   └── ProjectorRemote.cs               # Thiết bị điều khiển máy chiếu
│   │
│   ├── Rooms/
│   │   └── ClassRoom.cs                     # Lớp quản lý phòng học và kết tập thiết bị
│   │
│   ├── Borrowing/
│   │   ├── Borrowers/
│   │   │   ├── IBorrower.cs                 # Giao diện chuẩn hóa vai trò người mượn
│   │   │   ├── Lecturer.cs                  # Lớp giảng viên (hạn mức: 3)
│   │   │   └── TeachingAssistant.cs         # Lớp trợ giảng (hạn mức: 2)
│   │   │
│   │   ├── Slips/
│   │   │   ├── BorrowItem.cs                # Thực thể đại diện dòng thiết bị mượn
│   │   │   └── BorrowingSlip.cs             # Thực thể phiếu mượn, xử lý nghiệp vụ mượn/trả và trễ hạn
│   │   │
│   │   └── Exceptions/
│   │       ├── BorrowingException.cs        # Ngoại lệ cơ sở cho phân hệ mượn trả
│   │       ├── DeviceUnavailableException.cs
│   │       ├── BorrowLimitExceededException.cs
│   │       ├── InvalidBorrowPeriodException.cs
│   │       ├── DuplicateDeviceException.cs
│   │       ├── DeviceNotInSlipException.cs
│   │       └── DeviceAlreadyReturnedException.cs
│   │
│   └── Test/
│       └── TestRunner.cs                    # Điểm khởi chạy (Main) thực thi kịch bản kiểm thử
│
├── OOP_Labw_06.csproj                       # File cấu hình dự án .NET
├── AI_Collaboration.md                      # Log AI
└── README.md                                # Tài liệu đặc tả kỹ thuật và hướng dẫn

```

---

## 3. Hướng dẫn biên dịch và thực thi

### Yêu cầu môi trường

* .NET SDK 8.0 trở lên (hoặc môi trường quản lý gói Pixi đã tích hợp `dotnet-sdk`).

### Lệnh thực thi

Sử dụng .NET CLI tiêu chuẩn:

```bash
dotnet run

```

Hoặc thực thi qua Pixi:

```bash
pixi run dotnet run

```

---

## 4. Kịch bản kiểm thử (Verification Suite)

Chương trình kiểm thử tập trung tại `src/Test/TestRunner.cs` bao phủ toàn diện cả hai luồng nghiệp vụ theo đặc tả yêu cầu:

### 4.1. Luồng thành công (Happy Path)

1. **Khởi tạo dữ liệu mẫu:** Khởi tạo thông tin Giảng viên (`Lecturer`), 1 điều khiển máy chiếu, 1 điều khiển điều hòa và 1 micro không dây.
2. **Khởi tạo phiếu mượn:** Lập phiếu mượn cho phòng giảng dạy tương ứng.
3. **Thêm thiết bị:** Đăng ký thành công điều khiển máy chiếu và micro vào phiếu mượn; trạng thái các thiết bị tự động chuyển từ `Available` sang `Borrowed`.
4. **Trả thiết bị đúng hạn:** Trả điều khiển máy chiếu đúng hạn; trạng thái thiết bị được hoàn trả về `Available`, phí phạt trả trễ ghi nhận 0 VNĐ.
5. **Trả thiết bị trễ hạn:** Trả micro trễ 2 ngày; hệ thống ghi nhận thời gian trả, kích hoạt tính phí trễ hạn đa hình và chuyển thiết bị về `Available`.
6. **Báo cáo trạng thái:** Kết xuất danh mục các phiếu mượn có thiết bị trả muộn và các thiết bị chưa hoàn trả.

### 4.2. Luồng phát sinh ngoại lệ (Exception Handling Path)

Kiểm chứng cơ chế an toàn và tính nhất quán trạng thái thông qua các khối `try-catch`:

1. **Thiết bị không khả dụng:** Thử mượn thiết bị đang có trạng thái `UnderMaintenance`; bắt ngoại lệ `DeviceUnavailableException`.
2. **Vượt hạn mức mượn:** Trợ giảng cố gắng mượn thiết bị thứ 3 (vượt giới hạn 2); bắt ngoại lệ `BorrowLimitExceededException`.
3. **Trùng thiết bị trong phiếu:** Thêm cùng một micro hai lần vào cùng một phiếu; bắt ngoại lệ `DuplicateDeviceException`.
4. **Vượt thời lượng cho phép:** Đăng ký mượn micro 3 ngày trong khi giới hạn tối đa của thiết bị là 1 ngày; bắt ngoại lệ `InvalidBorrowPeriodException`.
5. **Trả thiết bị ngoại lai:** Thực hiện thao tác trả một thiết bị không tồn tại trong danh sách của phiếu mượn; bắt ngoại lệ `DeviceNotInSlipException`.
6. **Trả thiết bị hai lần:** Thực hiện thao tác trả lại một thiết bị đã được ghi nhận trả trước đó; bắt ngoại lệ `DeviceAlreadyReturnedException`.
7. **Thời gian trả không hợp lệ:** Cung cấp ngày trả trước ngày lập phiếu mượn; hệ thống từ chối cập nhật và phát sinh ngoại lệ hợp lệ.