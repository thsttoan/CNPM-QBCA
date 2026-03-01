
# 📚 CNPM-QBCA: Question Bank Consistency Analyzer

  **Hệ Thống Hỗ Trợ Phân Tích & Quản Lý Tính Đồng Nhất Ngân Hàng Câu Hỏi**

</div>

---

## 📖 Giới thiệu (Introduction)

**CNPM-QBCA** là một hệ thống web ASP.NET Core MVC toàn diện được phát triển nhằm mục đích số hóa và tối ưu hóa quy trình quản lý ngân hàng câu hỏi thi tại các cơ sở giáo dục. Hệ thống không chỉ đơn thuần là nơi lưu trữ, mà còn tích hợp các luồng công việc (workflows) kiểm duyệt nhiều cấp, kết hợp công nghệ AI để tự động phát hiện và ngăn chặn việc trùng lặp câu hỏi, đảm bảo chất lượng và tính duy nhất của đề thi.

---

## 🛠️ Công nghệ sử dụng (Tech Stack)

Hệ thống được xây dựng trên kiến trúc Multi-Tier Architecture, đảm bảo tính mở rộng và dễ bảo trì:

- **Backend:** C# / .NET 8, ASP.NET Core MVC
- **Database:** Microsoft SQL Server, Entity Framework Core (Code-First)
- **Frontend:** HTML5, CSS3 (Neumorphic Design), Bootstrap, Vanilla JavaScript
- **AI Integration:** Jina AI (Duplicate checking logic)

---

## ✨ Tính năng chính (Key Features)

Hệ thống cung cấp một luồng làm việc khép kín thông qua mô hình phân quyền (Role-Based Access Control) chặt chẽ với 5 vai trò chính:

### 🔬 1. R&D Staff (Nhân viên Nghiên cứu & Phát triển)

* **Quản lý Cấu trúc học thuật:** Tạo và quản lý Môn học (Subjects), Chuẩn đầu ra (CLOs) và Mức độ khó (Difficulty Levels).
* **Hoạch định Kế hoạch (Exam Plans):** Phân bổ Kế hoạch bài thi và tỷ lệ phân phối câu hỏi cho từng môn học.
* **AI Checking:** Sử dụng công cụ kiểm tra trùng lặp câu hỏi độc lập.
* **System Monitor:** Theo dõi bảng tổng hợp tiến độ chung.

### 🏫 2. Head of Department (Trưởng Khoa / Trưởng bộ môn cấp cao)

* **Tiếp nhận Kế hoạch:** Nhận danh sách Assigned Plans từ R&D.
* **Điều phối (Task Assignment):** Giao việc cụ thể trực tiếp cho các Giảng viên (Lecturers) hoặc Trưởng môn (Subject Leaders).
* **Phê duyệt cuối cùng:** Rà soát và phê duyệt các bảng tổng hợp đề thi (Submissions).

### 👨‍🏫 3. Subject Leader (Trưởng Bộ môn)

* **Quản lý Team:** Xem và nắm bắt các Task của cả nhóm (Team Tasks).
* **Ủy quyền:** Tiếp tục phân bổ (Assign) các công việc rà soát/tạo câu hỏi xuống cho Giảng viên.
* **Kiểm duyệt chuyên môn:** Review và Approve danh sách câu hỏi cuối cùng trước khi đưa vào ngân hàng.
* **Tổng hợp:** Bấm Submit hệ thống các bộ đề hoàn chỉnh lên cấp cao hơn.

### 🎓 4. Lecturer (Giảng viên)

* **Thực thi Task:** Xem danh sách công việc (My Tasks) được giao.
* **Đóng góp Ngân hàng:** Viết câu hỏi, upload lên hệ thống (Question Bank).
* **Quản lý chất lượng:** Tự kiểm tra trùng lặp những câu hỏi mới của mình với kho dữ liệu chung.
* **Mô phỏng (Mock Exam):** Làm bài thi thử để test độ khó của đề thi.

### 🏛️ 5. Exam Head (Trưởng phòng Khảo thí)

* **Đồng bộ hóa:** Nhận thông báo các Kế hoạch và Cập nhật đề thi.
* **Phê bình Đề thi:** Đưa ra nhận xét (Review Exams) đối với bộ đề.
* **Phê duyệt cuối cùng:** Cấp giấy phép/Approve để chính thức phát hành đề thi (Update Exam Approve Task).

---

## ⚙️ Luồng hoạt động cơ bản (Workflows)

1. **Khởi tạo:** R&D thiết lập Môn học, CLO và Kế hoạch (Exam Plan).
2. **Phân bổ:** Kế hoạch được đẩy xuống Trưởng Khoa (Head) -> Phân task cho Trưởng bộ môn/Giảng viên.
3. **Thực thi:** Giảng viên soạn thảo câu hỏi -> AI tự động cảnh báo nếu trùng lặp.
4. **Kiểm duyệt:** Trưởng bộ môn (Subject Leader) rà soát các câu hỏi và Submit.
5. **Ban hành:** Trưởng khoa (Head) và Trưởng Khảo thí (Exam Head) phê duyệt cuối cùng. Tất cả quá trình đều được tự động gửi thông báo (Notifications).

---

## 🚀 Hướng dẫn Cài đặt (Installation & Setup)

Để chạy dự án này trên môi trường local, bạn cần làm theo các bước sau:

**1. Yêu cầu hệ thống (Prerequisites)**

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (hoặc SQL Server Express)
- Visual Studio 2022 (khuyến nghị) hoặc VS Code

**2. Cài đặt CSDL (Database Setup)**

- Mở file `appsettings.json` trong thư mục `CNPM-QBCA.Web`.
- Thay đổi chuỗi cấu hình `DefaultConnection` sao cho trỏ đúng tới Local SQL Server của bạn.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=CNPM-QBCA;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False"
}
```

**3. Áp dụng Migrations**
Mở sơ đồ Package Manager Console trong Visual Studio hoặc dùng Terminal và gõ:

```bash
cd CNPM-QBCA.Web
dotnet ef database update
```

*(Nếu database trống, bạn cần Add seed data cho bảng Roles (ID từ 1->5) và Users)*

**4. Chạy ứng dụng**

```bash
dotnet run --project CNPM-QBCA.Web
```

Ứng dụng sẽ khả dụng tại `http://localhost:XXXX`

---
