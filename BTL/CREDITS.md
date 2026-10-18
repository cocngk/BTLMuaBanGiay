# Nguồn ảnh — Credits

Toàn bộ ảnh trong `Content/images/` được tải về từ [Unsplash](https://unsplash.com) để site chạy được khi offline. Theo điều khoản API của Unsplash, mỗi ảnh đã dùng đều cần ghi rõ tác giả.

## Ảnh đại diện sản phẩm (`products/`, `hero/`)

Ảnh ghép trong `ProductStore` và `trangchu.css`, tải theo mẫu `?w=400` và `?w=1200`.

## Thư viện ảnh (`gallery/`)

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
| `u13` | Luis Felipe Lins | [8mK-YvyOcSE](https://unsplash.com/photos/white-nike-air-force-1-high-8mK-YvyOcSE) |
| `u01` | Artem Bondarchuk | [XPBYi4K8vFI](https://unsplash.com/photos/white-and-red-nike-athletic-shoe-XPBYi4K8vFI) |
| `u04` | Artem Bondarchuk | [vbdIgnfzrdg](https://unsplash.com/photos/white-black-and-red-nike-athletic-shoes-vbdIgnfzrdg) |
| `u21` | Pedro Chosco | [7W2MeUBGh80](https://unsplash.com/photos/a-pair-of-yellow-and-black-tennis-shoes-7W2MeUBGh80) |
| `u07` | Josh Marshall | [PcI3V1VbbrU](https://unsplash.com/photos/unpaired-white-nike-low-top-shoe-PcI3V1VbbrU) |
| `u15` | Oskar Hagberg | [1mMIpisWzn8](https://unsplash.com/photos/a-red-shoe-on-a-black-background-1mMIpisWzn8) |
| `n06` | kushal bajracharya | [sTlD1IYOvKc](https://unsplash.com/photos/person-holding-blue-and-white-nike-athletic-shoe-sTlD1IYOvKc) |
| `n04` | yogesh kumar | [-iNq63c2vt8](https://unsplash.com/photos/white-nike-air-force-1-shoe--iNq63c2vt8) |
| `n00` | LoboStudio Hamburg | [4lf8mVuZESQ](https://unsplash.com/photos/pair-of-brown-leather-lace-up-shoes-on-white-surface-4lf8mVuZESQ) |
| `n05` | Alexandra Gorn | [CJ6SJO_yR5w](https://unsplash.com/photos/pair-of-shoes-and-white-box-near-plant-CJ6SJO_yR5w) |

## Ảnh bài viết (`blogs/`)

Ảnh minh hoạ trong `Data/BlogStore.cs`.

---

## Lưu ý

* Site cần chạy khi không có mạng (demo trên máy cục bộ), nên ảnh **được tải về lưu trong repo** thay vì hotlink. Điều khoản API của Unsplash khuyến nghị dùng URL trực tiếp và không lưu lại ảnh — nếu muốn tuân thủ tuyệt đối thì đổi các đường dẫn trong `ProductStore`/`BlogStore` về URL gốc và bỏ thư mục `Content/images`, đổi lại site sẽ phụ thuộc mạng.
* Ảnh trong thư viện là ảnh chụp giày thật của tác giả, **không phải đúng mẫu ghi trong tên sản phẩm** — danh mục dùng ảnh minh hoạ theo thương hiệu, tương tự phần lớn site bán hàng dùng ảnh stock.
* Cả 20 ảnh trong thư viện đã báo lượt tải qua `track_download` của Unsplash API.