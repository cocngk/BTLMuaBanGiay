# Nguồn ảnh — Credits

Toàn bộ ảnh trong `Content/images/` được tải về từ [Unsplash](https://unsplash.com) để site chạy được khi offline. Theo điều khoản API của Unsplash, mỗi ảnh đã dùng đều cần ghi rõ tác giả.

## Ảnh đại diện sản phẩm (`products/`)

12 ảnh chính, tải ở hai cỡ `?w=400` (`-sm`) và `?w=1200` (`-lg`). Chúng nằm trong cùng bộ ảnh với thư viện bên dưới.

## Thư viện ảnh (`gallery/`)

24 ảnh, mỗi ảnh hai cỡ: 800px cho khung ảnh lớn, 240px cho ô màu trong dải chọn.

| File | Tác giả | Trang ảnh |
|---|---|---|
| `b11` | Rachael Ren | [Ueu_6ZrVb80](https://unsplash.com/photos/black-converse-all-star-high-top-shoes) |
| `b4` | Vlad Ciutacu | [C-U1RQFr6NY](https://unsplash.com/photos/white-sneakers-with-black-lettering) |
| `n04` | yogesh kumar | [-iNq63c2vt8](https://unsplash.com/photos/white-nike-air-force-1-shoe-) |
| `u01` | Artem Bondarchuk | [XPBYi4K8vFI](https://unsplash.com/photos/white-and-red-nike-athletic-shoe) |
| `u04` | Artem Bondarchuk | [vbdIgnfzrdg](https://unsplash.com/photos/white-black-and-red-nike-athletic-shoes) |
| `u05` | Hipkicks | [HcqA34-uWo4](https://unsplash.com/photos/black-and-white-low-top-sneakers) |
| `u06` | Behnam Norouzi | [F4rWoM3cYjI](https://unsplash.com/photos/a-pair-of-black-and-orange-sneakers-on-a-white-background) |
| `u07` | Josh Marshall | [PcI3V1VbbrU](https://unsplash.com/photos/unpaired-white-nike-low-top-shoe) |
| `u10` | Aymen Chabani | [IpSk8rGM64Q](https://unsplash.com/photos/pair-of-white-low-top-sneakers) |
| `u11` | Ricardo vivona | [87W1itKmMdw](https://unsplash.com/photos/blue-and-white-high-top-sneakers) |
| `u12` | Nathaniel Pena | [hrXEwNz6Yyo](https://unsplash.com/photos/a-pair-of-white-shoes-with-a-white-shoelace) |
| `u14` | Hitomi Bremmer | [pnf_gpH_YtA](https://unsplash.com/photos/a-black-and-white-shoe) |
| `u15` | Oskar Hagberg | [1mMIpisWzn8](https://unsplash.com/photos/a-red-shoe-on-a-black-background) |
| `u16` | Brian Hall | [OShz0kboddc](https://unsplash.com/photos/white-nike-low-top-sneakers) |
| `u17` | Piyush Haswani | [gAVIw1zs1fU](https://unsplash.com/photos/a-pair-of-blue-and-black-sneakers-on-a-black-background) |
| `v01` | Max Petrunin | [A4fETzh_wlo](https://unsplash.com/photos/a-pair-of-purple-shoes) |
| `v03` | Liming Wang | [qhyHWZTN9O4](https://unsplash.com/photos/a-pair-of-white-shoes-sitting-on-top-of-a-wooden-floor) |
| `w01` | Nicolas Nieves-Quiroz | [j4uSBFasVrA](https://unsplash.com/photos/white-nike-air-force-1-low) |
| `w02` | Shubham Verma | [w0H7w9ObiBw](https://unsplash.com/photos/pair-of-air-jordan-1-sneakers-and-their-box) |
| `w03` | Luis Felipe Lins | [8T-LD_wxlSo](https://unsplash.com/photos/white-red-and-black-nike-air-max) |
| `w04` | kawah kaos dakwah | [plTCLtG2p8I](https://unsplash.com/photos/a-pair-of-black-and-white-shoes-sitting-on-top-of-a-chair) |
| `w05` | Imani Bahati | [LxVxPA1LOVM](https://unsplash.com/photos/unpaired-gray-nike-running-shoe) |
| `w06` | Oskar Hagberg | [5TZuyH_TRTY](https://unsplash.com/photos/a-blue-and-white-shoe) |
| `w07` | van asten maarten | [5Z9ZClW3MYI](https://unsplash.com/photos/sneakers-sit-in-the-sand-at-the-beach) |

## Ảnh bài viết (`blogs/`)

Ảnh minh hoạ trong `Data/BlogStore.cs`.

---

## Lưu ý

* **Ảnh đã đúng với tên sản phẩm.** Danh mục cũ gắn ảnh theo hãng rồi đặt tên theo hãng, nên nhiều trang hiện ảnh của hãng khác — ví dụ "Puma Suede Classic" lại dùng ảnh Nike, "Adidas Forum Low White" dùng ảnh New Balance, và hai ảnh đại diện thực chất là **dây nhảy dụng cụ** chứ không phải giày. Danh mục hiện tại chỉ dùng những hãng có ảnh thật tương ứng (Nike, Converse, Vans, New Balance) cộng dòng sản phẩm của chính shop (SneakerX) cho những ảnh không có logo.
* **Cách chọn ảnh: phải xem bản gốc, không tin tên và không tin thumbnail.** Trường `description` của API sai thường xuyên (có ảnh ghi "product photography" nhưng thực tế chụp cả tay người). Tệ hơn: ghép ảnh vào lưới nhỏ khoảng 150px rồi ngắm là sai — chữ "N" của New Balance và dấu ba sọc Adidas đều biến mất ở kích thước đó, từ đó đã gán nhầm một đôi New Balance thành "Nike Air Force 1 Tím", một đôi Adidas thành "Vans", một đôi Reebok thành màu của Nike Air Max. **Mỗi ảnh được dùng đều phải xem bản đầy đủ và nhìn thấy logo**, hoặc không gán hãng nào cho nó — bỏ dùng nếu không xác minh được.
* **Mỗi sản phẩm có 2–3 màu, không phải nhiều góc nhìn.** Ảnh Unsplash là ảnh chụp tự nhiên, **mỗi đôi chỉ có một góc duy nhất** — không có bộ 3 góc như ảnh sản phẩm chụp studio, nên tìm thêm ảnh cũng không tạo được góc thứ hai. Thay vì giả vờ là "góc nhìn", ảnh phụ được đặt nhãn tên màu (`ProductColourway.Name`) và hiện thành dải chọn màu: hai ảnh gần giống nhau trông như người xem đang chọn màu, thay vì như thiếu ảnh. Xem `BindColourways` trong `ChiTietSanPham.aspx.cs`.
* Site cần chạy khi không có mạng (demo trên máy cục bộ), nên ảnh **được tải về lưu trong repo** thay vì hotlink. Điều khoản API của Unsplash khuyến nghị dùng URL trực tiếp và không lưu lại ảnh — nếu muốn tuân thủ tuyệt đối thì đổi các đường dẫn trong `ProductStore`/`BlogStore` về URL gốc và bỏ thư mục `Content/images`, đổi lại site sẽ phụ thuộc mạng.
* Tất cả 24 ảnh đã dùng đều đã báo lượt tải qua `track_download` của Unsplash API.