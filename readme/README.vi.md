# Công cụ AutoClick

[![English](https://img.shields.io/badge/Language-English-blue)](README.md)
[![Tiếng Việt](https://img.shields.io/badge/Language-Tiếng%20Việt-green)](readme/README.vi.md)

Công cụ tự động hóa chuột đa chức năng cho Windows với giao diện trực quan và quản lý tập lệnh dễ dàng.

## Tính năng chính

- **Đa dạng thao tác chuột**:
  - Click chuột trái/phải
  - Kéo thả chuột trái/phải
  - Cuộn chuột
- **Giao diện trực quan**:
  - Chọn vị trí thao tác đơn giản
  - Hiển thị trực quan các hành động
  - Bảng điều khiển thân thiện với người dùng
- **Tùy chỉnh thời gian**:
  - Cấu hình thời gian chờ giữa các hành động
  - Tùy chỉnh tốc độ kéo/cuộn
- **Quản lý tập lệnh**:
  - Lưu và tải tập lệnh (định dạng .acs)
  - Chỉnh sửa tập lệnh dễ dàng
- **Tối ưu hệ thống**:
  - Sử dụng tài nguyên thấp
  - Hoạt động ổn định trong nền

## Hướng dẫn sử dụng

### Tạo tập lệnh mới

1. Khởi động ứng dụng
2. Sử dụng các nút trên bảng điều khiển để thêm hành động:
   - **Left Click**: Thêm hành động click chuột trái
   - **Right Click**: Thêm hành động click chuột phải
   - **Left Drag**: Thêm hành động kéo thả chuột trái
   - **Right Drag**: Thêm hành động kéo thả chuột phải
   - **Mouse Scroll**: Thêm hành động cuộn chuột

3. Khi thêm hành động:
   - Với thao tác click: Nhấp vào vị trí mong muốn
   - Với thao tác kéo/cuộn: Nhấp vào điểm bắt đầu, sau đó kéo đến điểm kết thúc
   - Nhập thời gian chờ (tính bằng mili giây) sau khi hành động hoàn thành
   - Nhấp "Xác nhận" để thêm hành động vào tập lệnh

### Quản lý tập lệnh

- **Delete Action**: Xóa hành động đã chọn
- **Start/Stop** (F9): Bắt đầu hoặc dừng thực thi tập lệnh
- **Save Script**: Lưu tập lệnh hiện tại vào file (.acs)
- **Load Script**: Tải tập lệnh đã lưu trước đó
- **Settings**: Điều chỉnh cài đặt ứng dụng
- **About**: Thông tin về ứng dụng

### Cài đặt nâng cao

- **Drag/Scroll Time Ratio**: Điều chỉnh tỷ lệ thời gian giữa thực hiện hành động và chờ đợi
- **Hotkey Configuration**: Tùy chỉnh phím tắt (mặc định là F9)
- **Overlay Opacity**: Điều chỉnh độ trong suốt của lớp phủ hiển thị

## Yêu cầu hệ thống

- Hệ điều hành: Windows 10/11
- Framework: .NET 6.0 Runtime trở lên
- RAM: Tối thiểu 2GB (khuyến nghị 4GB)
- Dung lượng: Khoảng 50MB dung lượng đĩa

## Cài đặt

### Sử dụng file portable

1. Tải file AutoClick.exe từ phần [Releases](https://github.com/QuackPhuc/AutoClick/releases)
2. Chạy file executable - không cần cài đặt
3. Ứng dụng sẽ hoạt động trên bất kỳ máy tính Windows nào mà không cần cài đặt .NET

### Chạy từ mã nguồn

1. Đảm bảo bạn đã cài đặt .NET 6.0 SDK trở lên
2. Clone repository này
3. Chạy lệnh `dotnet build -c Release` trong thư mục src
4. Chạy file exe từ thư mục bin/Release/net6.0-windows

## Giấy phép

Dự án này được phân phối dưới giấy phép MIT. Xem file LICENSE để biết thêm thông tin chi tiết.

## Ngôn ngữ khác

README này cũng có sẵn bằng:
- [Tiếng Anh](README.md)