# Nguồn ảnh — Credits

Toàn bộ ảnh trong `Content/images/` được tải về từ [Unsplash](https://unsplash.com) để site chạy được khi offline. Theo điều khoản API của Unsplash, mỗi ảnh đã dùng đều cần ghi rõ tác giả.

## Ảnh đại diện sản phẩm (`products/`)

12 ảnh chính, tải ở hai cỡ `?w=400` (`-sm`) và `?w=1200` (`-lg`). Chúng nằm trong cùng bộ ảnh với thư viện bên dưới.

## Thư viện ảnh (`gallery/`)

22 ảnh, mỗi ảnh một cỡ 800px dùng cho khung ảnh lớn ở trang chi tiết.

| File | Tác giả | Trang ảnh |
|---|---|---|
| `u16` | Brian Hall | [OShz0kboddc](https://unsplash.com/photos/white-nike-low-top-sneakers-OShz0kboddc) |
| `b4` | Vlad Ciutacu | [C-U1RQFr6NY](https://unsplash.com/photos/white-sneakers-with-black-lettering-C-U1RQFr6NY) |
| `u10` | Aymen Chabani | [IpSk8rGM64Q](https://unsplash.com/photos/pair-of-white-low-top-sneakers-IpSk8rGM64Q) |
| `u12` | Nathaniel Pena | [hrXEwNz6Yyo](https://unsplash.com/photos/a-pair-of-white-shoes-with-a-white-shoelace-hrXEwNz6Yyo) |
| `u17` | Piyush Haswani | [gAVIw1zs1fU](https://unsplash.com/photos/a-pair-of-blue-and-black-sneakers-on-a-black-background-gAVIw1zs1fU) |
| `u06` | Behnam Norouzi | [F4rWoM3cYjI](https://unsplash.com/photos/a-pair-of-black-and-orange-sneakers-on-a-white-background-F4rWoM3cYjI) |
| `u14` | Hitomi Bremmer | [pnf_gpH_YtA](https://unsplash.com/photos/a-black-and-white-shoe-pnf_gpH_YtA) |
| `b11` | Rachael Ren | [Ueu_6ZrVb80](https://unsplash.com/photos/black-converse-all-star-high-top-shoes-Ueu_6ZrVb80) |
| `u11` | Ricardo vivona | [87W1itKmMdw](https://unsplash.com/photos/blue-and-white-high-top-sneakers-87W1itKmMdw) |
| `u05` | Hipkicks | [HcqA34-uWo4](https://unsplash.com/photos/black-and-white-low-top-sneakers-HcqA34-uWo4) |
| `u01` | Artem Bondarchuk | [XPBYi4K8vFI](https://unsplash.com/photos/white-and-red-nike-athletic-shoe-XPBYi4K8vFI) |
| `u04` | Artem Bondarchuk | [vbdIgnfzrdg](https://unsplash.com/photos/white-black-and-red-nike-athletic-shoes-vbdIgnfzrdg) |
| `u21` | Pedro Chosco | [7W2MeUBGh80](https://unsplash.com/photos/a-pair-of-yellow-and-black-tennis-shoes-7W2MeUBGh80) |
| `u07` | Josh Marshall | [PcI3V1VbbrU](https://unsplash.com/photos/unpaired-white-nike-low-top-shoe-PcI3V1VbbrU) |
| `u15` | Oskar Hagberg | [1mMIpisWzn8](https://unsplash.com/photos/a-red-shoe-on-a-black-background-1mMIpisWzn8) |
| `n04` | yogesh kumar | [-iNq63c2vt8](https://unsplash.com/photos/white-nike-air-force-1-shoe--iNq63c2vt8) |
| `n00` | LoboStudio Hamburg | [4lf8mVuZESQ](https://unsplash.com/photos/pair-of-brown-leather-lace-up-shoes-on-white-surface-4lf8mVuZESQ) |
| `n05` | Alexandra Gorn | [CJ6SJO_yR5w](https://unsplash.com/photos/pair-of-shoes-and-white-box-near-plant-CJ6SJO_yR5w) |
| `v01` | Max Petrunin | [A4fETzh_wlo](https://unsplash.com/photos/a-pair-of-purple-shoes-A4fETzh_wlo) |
| `v02` | Yasmin Dangor | [tuDtWCfsEy8](https://unsplash.com/photos/person-wearing-white-and-green-low-top-sneakers-tuDtWCfsEy8) |
| `v03` | Liming Wang | [qhyHWZTN9O4](https://unsplash.com/photos/a-pair-of-white-shoes-sitting-on-top-of-a-wooden-floor-qhyHWZTN9O4) |
| `v05` | Sayan Majhi | [ujTqgoBjfOU](https://unsplash.com/photos/a-black-and-white-photo-of-a-laptop-keyboard-headphones-and-a-ujTqgoBjfOU) |

## Ảnh bài viết (`blogs/`)

Ảnh minh hoạ trong `Data/BlogStore.cs`.

---

## Lưu ý

* **Ảnh đã đúng với tên sản phẩm.** Danh mục cũ gắn ảnh theo hãng rồi đặt tên theo hãng, nên nhiều trang hiện ảnh của hãng khác — ví dụ "Puma Suede Classic" lại dùng ảnh Nike, "Adidas Forum Low White" dùng ảnh New Balance, và hai ảnh đại diện thực chất là **dây nhảy dụng cụ** chứ không phải giày. Danh mục hiện tại chỉ dùng những hãng có ảnh thật tương ứng (Nike, Converse, Vans, New Balance) cộng dòng sản phẩm của chính shop (SneakerX) cho những ảnh không có logo. Trước khi chọn ảnh phải xem tải về: trường `description` của API thường ghi sai (có ảnh được mô tả là "product photography" nhưng thực tế chụp cả tay người).
* **Mỗi sản phẩm có 2 góc ảnh, không phải 3.** 12 sản phẩm × 3 góc cần 36 ảnh sạch, hiện chỉ tìm được 22. Quan trọng hơn con số: ảnh Unsplash là ảnh chụp tự nhiên, **mỗi đôi chỉ có một góc duy nhất**, không có bộ 3 góc như ảnh sản phẩm chụp studio — nên tìm thêm cũng không giải quyết được, ba ảnh của cùng một đôi AF1 trắng vẫn giống nhau. Vì vậy trang chi tiết đã bỏ dải thumbnail và hàm `swapMainImage` (trước đó hàm ý "chọn được góc khác"), thay bằng ảnh phụ nằm ngay dưới ảnh chính. Xem `BindSecondaryImage` trong `ChiTietSanPham.aspx.cs`.
* **Loại bỏ trong lúc dọn:** 22 file ảnh nhỏ 240px (`*-t.jpg`) vốn chỉ phục vụ dải thumbnail, cùng trường `ProductImage.Small` — không còn chỗ nào đọc tới.
* Site cần chạy khi không có mạng (demo trên máy cục bộ), nên ảnh **được tải về lưu trong repo** thay vì hotlink. Điều khoản API của Unsplash khuyến nghị dùng URL trực tiếp và không lưu lại ảnh — nếu muốn tuân thủ tuyệt đối thì đổi các đường dẫn trong `ProductStore`/`BlogStore` về URL gốc và bỏ thư mục `Content/images`, đổi lại site sẽ phụ thuộc mạng.
* Tất cả ảnh đã dùng đều đã báo lượt tải qua `track_download` của Unsplash API.