# Điều khiển thời tiết và thời gian

1. Tạo GameObject `Weather Manager`, gắn `WeatherController`.
2. Kéo Directional Light vào **Sun**, Particle System mưa hiện có vào **Rain Particles**.
3. **Moon** và **Snow Particles** là tùy chọn; cần gán đèn/hiệu ứng riêng để có hình ảnh tương ứng. Không gán cùng một Particle System cho cả mưa và tuyết.
4. Chỉnh ngày bắt đầu, **Starting Hour**, **Minutes Per Day** (mặc định 20 phút thật = một ngày game).
5. **Automatic Weather** đổi thời tiết mỗi 6 giờ game. Tắt để giữ thời tiết do bạn chọn.

Mùa theo Bắc bán cầu: xuân tháng 3–5, hè 6–8, thu 9–11, đông 12–2. Lịch tự xử lý số ngày trong tháng và năm nhuận. Ngày từ 06:00 đến 18:00, có thể chỉnh trong Inspector.

```csharp
weatherController.SetTime(18, 30);
weatherController.SetDateTime(2026, 12, 24, 8, 0);
weatherController.SetAutomaticWeather(false);
weatherController.SetWeather(WeatherType.Rain);
weatherController.ClockPaused = true;
string clock = weatherController.ClockText; // HH:mm
```

Các event `TimeChanged`, `SeasonChanged`, `WeatherChanged` hỗ trợ UI và gameplay. `TimeChanged` báo khi phút thay đổi; nếu nhảy thời gian, chỉ báo thời điểm cuối. `AdvanceHours` thay đổi lịch; lịch tự đổi thời tiết được đếm trong Update. `ClockPaused` dừng đồng hồ và bộ đếm thời tiết, còn chuyển tiếp hình ảnh vẫn chạy.

Cloudy làm giảm ánh sáng; Storm tăng mưa và làm tối trời, chưa có sấm chớp hoặc âm thanh. Script không tạo mesh mây, hiệu ứng tuyết hay đổi cây cối theo mùa. Ambient Intensity hiệu quả phụ thuộc thiết lập Environment Lighting; skybox và exposure hiện có có thể cần chỉnh để đêm tối hơn. Chỉ dùng một WeatherController trong scene. Chưa lưu thời gian qua lần thoát game.
