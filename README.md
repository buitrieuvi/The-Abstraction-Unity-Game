https://drive.google.com/drive/folders/1GEylE2CiYRXm7m9bP85lcC3NmrnQsI0n?usp=sharing

Nội dung bao gồm:
- Công nghệ Unity, C# và Visual Studio.
- Zenject, R3, DOTween, Input System, TextMeshPro và ProBuilder.
- Chức năng nhân vật 3D, Inventory, Elevator và UI.
- Kiến trúc State Machine, Model/Controller/View và Dependency Injection.

![](./image.png)

Công nghệ và Package sử dụng
Dự án The Abstraction --- Unity Game sử dụng Unity và các package
được khai báo trong Packages/manifest.json.
Thư viện bên thứ ba
  Package                 Phiên bản / nguồn                          Chức năng
  R3                  Git URL: Cysharp/R3                      Reactive programming,
                          (src/R3.Unity/Assets/R3.Unity)           xử lý luồng sự kiện và
                                                                     dữ liệu thay đổi.
  UniTask             Git URL: Cysharp/UniTask                 Hỗ trợ lập trình bất
                          (src/UniTask/Assets/Plugins/UniTask)     đồng bộ trong Unity
                                                                     bằng async/await.
  Unity UI Rounded      Git URL:                                   Tạo góc bo tròn cho
  Corners               kirevdokimov/Unity-UI-Rounded-Corners    thành phần giao diện
                                                                     UI.
  Easy Text Effects for Git URL:                                   Tạo hiệu ứng cho văn
  Unity                 LeiQiaoZhi/Easy-Text-Effects-for-Unity   bản trong UI.
Unity packages
  Package                                  Phiên bản Chức năng
  Addressables                          2.11.2 Quản lý, tải và tham
                                                     chiếu asset theo địa
                                                     chỉ; hỗ trợ tổ chức
                                                     nội dung và tải tài
                                                     nguyên linh hoạt.
  AI Assistant                    2.20.0-pre.2 Tích hợp công cụ AI
                                                     Assistant trong Unity
                                                     Editor; phiên bản
                                                     preview.
  AI Inference                           2.6.1 Các API phục vụ suy
                                                     luận mô hình AI trong
                                                     Unity. Việc sử dụng
                                                     thực tế phụ thuộc
                                                     code và cấu hình dự
                                                     án.
  Cinemachine                            6.6.0 Công cụ quản lý
                                                     camera ảo, chuyển
                                                     động camera và hành
                                                     vi camera trong
                                                     gameplay.
  Collab Proxy                          2.13.6 Tích hợp các chức
                                                     năng cộng tác/Version
                                                     Control của Unity
                                                     theo package.
  Feature                                  1.0.1 Gói tính năng liên
  Worldbuilding                                    quan đến quy trình
                                                     worldbuilding; chức
                                                     năng cụ thể tùy các
                                                     package phụ thuộc
                                                     được cài kèm.
  IDE Rider                             3.0.40 Tích hợp Unity với
                                                     JetBrains Rider.
  IDE Visual Studio                     2.0.28 Tích hợp Unity với
                                                     Visual Studio để mở
                                                     và debug mã C#.
  Input System                          1.20.0 Quản lý đầu vào bằng
                                                     Input Actions, action
                                                     maps và bindings.
  Pipeline                         0.8.0-exp.1 Package pipeline ở
                                                     trạng thái
                                                     experimental; cần
                                                     kiểm tra tài liệu
                                                     package để xác định
                                                     vai trò chính xác
                                                     trong project.
  Universal Render                        17.6.0 Render đồ họa 3D/2D
  Pipeline (URP)                                   với pipeline có thể
                                                     cấu hình, hỗ trợ
                                                     shader, ánh sáng và
                                                     hiệu ứng hình ảnh.
  Terrain Tools                          5.3.3 Công cụ mở rộng để
                                                     tạo và chỉnh sửa địa
                                                     hình trong Unity
                                                     Editor.
  Unity UI (uGUI)                        2.6.0 Hệ thống Canvas,
                                                     Button, Image và các
                                                     thành phần giao diện
                                                 truyền thống.
Unity modules
Các module dưới đây được khai báo với phiên bản 1.0.0. Chúng cung cấp
API hoặc tính năng nền tảng của Unity; sự hiện diện trong manifest
không đồng nghĩa dự án đang sử dụng tất cả các module đó trong
gameplay.
  Module                                           Công dụng chính
  com.unity.modules.accessibility                API liên quan đến khả năng tiếp
                                                   cận.
  com.unity.modules.adaptiveperformance          API tích hợp Adaptive Performance.
  com.unity.modules.ai                           API AI/NavMesh của Unity.
  com.unity.modules.androidjni                   Tương tác với Java Native Interface
                                                   trên Android.
  com.unity.modules.animation                    Animation và Animator.
  com.unity.modules.assetbundle                  Đóng gói và tải AssetBundle.
  com.unity.modules.audio                        Phát và xử lý âm thanh.
  com.unity.modules.cloth                        Mô phỏng vải.
  com.unity.modules.director                     Playable/Timeline Director API.
  com.unity.modules.imageconversion              Chuyển đổi dữ liệu hình ảnh.
  com.unity.modules.imgui                        Immediate Mode GUI, thường dùng cho
                                                   công cụ/debug.
  com.unity.modules.jsonserialize                API serialization JSON của Unity.
  com.unity.modules.particlesystem               Particle System cho hiệu ứng như
                                                   bụi, mưa và khói.
  com.unity.modules.physics                      Vật lý 3D, Rigidbody và Collider.
  com.unity.modules.physics2d                    Vật lý 2D.
  com.unity.modules.physicscore2d                Thành phần lõi của hệ thống vật lý
                                                   2D.
  com.unity.modules.screencapture                Chụp ảnh màn hình.
  com.unity.modules.terrain                      Hỗ trợ Terrain.
  com.unity.modules.terrainphysics               Tích hợp vật lý cho Terrain.
  com.unity.modules.tetgen                       Hỗ trợ chức năng tetrahedralization
                                                   theo module Unity.
  com.unity.modules.tilemap                      Tilemap cho môi trường dạng ô.
  com.unity.modules.timelinefoundation           Thành phần nền tảng liên quan đến
                                                   Timeline.
  com.unity.modules.ui                           API UI của Unity.
  com.unity.modules.uielements                   UI Toolkit.
  com.unity.modules.umbra                        Hỗ trợ occlusion culling liên quan
                                                   đến Umbra.
  com.unity.modules.unityanalytics               Unity Analytics.
  com.unity.modules.unitywebrequest              Gửi HTTP/web request.
  com.unity.modules.unitywebrequestassetbundle   Tải AssetBundle qua
                                                   UnityWebRequest.
  com.unity.modules.unitywebrequestaudio         Tải dữ liệu âm thanh qua
                                                   UnityWebRequest.
  com.unity.modules.unitywebrequesttexture       Tải texture qua UnityWebRequest.
  com.unity.modules.unitywebrequestwww           API tương thích cũ liên quan đến
                                                   WWW request.
  com.unity.modules.vectorgraphics               Hỗ trợ đồ họa vector.
  com.unity.modules.vehicles                     Thành phần hỗ trợ xe/vehicle.
  com.unity.modules.video                        Phát video.
  com.unity.modules.wind                         API gió, thường liên quan đến môi
                                                   trường và cây cối.
  com.unity.modules.xr                           API nền tảng XR.
Lưu ý về phiên bản và cấu hình
- Danh sách trên được chuyển từ các mục dependencies do dự án cung
  cấp.
- Phiên bản Unity Editor cần được xác nhận trong
  ProjectSettings/ProjectVersion.txt.
- Package Git URL có thể thay đổi theo commit hoặc nhánh mặc định nếu
  không khóa revision cụ thể.
- Các package có hậu tố pre hoặc exp là bản preview/experimental;
  nên kiểm tra độ tương thích trước khi nâng cấp.
- Để tái tạo môi trường chính xác, nên commit cả
  Packages/manifest.json và Packages/packages-lock.json.
- manifest.json cho biết package đã khai báo, không chứng minh tất
  cả package đều được gọi trực tiếp trong code.
