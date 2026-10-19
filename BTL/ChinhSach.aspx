<%@ Page Title="Chính sách &amp; Điều khoản" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChinhSach.aspx.cs" Inherits="ShoeStore.ChinhSach" %>
<%@ Import Namespace="ShoeStore.Models" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="description" content="Chính sách đổi trả, bảo hành, vận chuyển, thanh toán và điều khoản sử dụng khi mua giày tại SneakerX." />
    <link rel="stylesheet" href="Content/chinhsach.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="policy-container">

        <div class="policy-header">
            <h1 class="policy-title"><i class="fa-solid fa-file-shield"></i> Chính sách &amp; Điều khoản</h1>
            <p class="policy-subtitle">
                Áp dụng cho mọi đơn hàng đặt tại SneakerX. Nếu còn điều chưa rõ,
                bạn có thể <a href="TuVan.aspx">gửi câu hỏi cho shop</a> — nhưng phần
                dưới đây là bản đầy đủ để bạn đọc trước khi đặt hàng.
            </p>
        </div>

        <nav class="policy-nav" aria-label="Mục lục chính sách">
            <a href="#doi-tra">Đổi trả</a>
            <a href="#bao-hanh">Bảo hành</a>
            <a href="#van-chuyen">Vận chuyển</a>
            <a href="#thanh-toan">Thanh toán &amp; bảo mật</a>
            <a href="#dieu-khoan">Điều khoản sử dụng</a>
        </nav>

        <!-- ================= ĐỔI TRẢ ================= -->
        <section id="doi-tra" class="policy-card">
            <h2 class="policy-heading"><i class="fa-solid fa-rotate-left"></i> Chính sách đổi trả</h2>
            <ol class="policy-list">
                <li><strong>Thời hạn:</strong> đổi trong vòng 30 ngày kể từ ngày nhận hàng.</li>
                <li><strong>Điều kiện:</strong> giày chưa qua sử dụng, còn nguyên hộp, phụ kiện và hoá đơn (hoặc mã đơn hàng).</li>
                <li><strong>Phí đổi:</strong> lỗi từ shop (sai size, sai mẫu, hàng lỗi) — shop chịu phí hai chiều. Đổi vì thay đổi ý định — khách chịu phí vận chuyển hai chiều.</li>
                <li><strong>Hoàn tiền:</strong> trong 3–5 ngày làm việc, qua đúng phương thức bạn đã thanh toán.</li>
            </ol>
            <p class="policy-note">
                <i class="fa-solid fa-circle-info"></i>
                Không áp dụng đổi trả cho giày đặt riêng theo yêu cầu, giày đã qua sửa chữa bên ngoài
                và sản phẩm xả kho có ghi rõ "không đổi trả" trên trang sản phẩm.
            </p>
        </section>

        <!-- ================= BẢO HÀNH ================= -->
        <section id="bao-hanh" class="policy-card">
            <h2 class="policy-heading"><i class="fa-solid fa-screwdriver-wrench"></i> Bảo hành sản phẩm</h2>
            <ol class="policy-list">
                <li><strong>Thời hạn:</strong> 6 tháng cho các lỗi keo, đế, chỉ may từ nhà sản xuất.</li>
                <li><strong>Hình thức:</strong> dán keo, dán đế, khâu lại chỉ. Shop không đổi sản phẩm mới cho trường hợp bảo hành.</li>
                <li><strong>Thời gian xử lý:</strong> 7–10 ngày làm việc kể từ khi nhận được sản phẩm.</li>
                <li><strong>Chi phí:</strong> miễn phí với lỗi nhà sản xuất; báo giá trước nếu hư hỏng do sử dụng.</li>
            </ol>
            <p class="policy-note">
                <i class="fa-solid fa-circle-info"></i>
                Không bảo hành khi giày bị ngập nước, mòn tự nhiên, hư do va đập hoặc đã tự sửa chữa
                ở nơi khác.
            </p>
        </section>

        <!-- ================= VẬN CHUYỂN ================= -->
        <section id="van-chuyen" class="policy-card">
            <h2 class="policy-heading"><i class="fa-solid fa-truck-fast"></i> Vận chuyển &amp; giao hàng</h2>
            <ol class="policy-list">
                <li><strong>Phí vận chuyển:</strong> miễn phí toàn quốc cho mọi đơn hàng.</li>
                <li><strong>Thời gian:</strong> nội thành Hà Nội và TP.HCM 1–2 ngày; các tỉnh khác 2–4 ngày làm việc.</li>
                <li><strong>Đối tác:</strong> ViettelPost và Giao Hàng Nhanh. Mã vận đơn được gửi qua số điện thoại bạn đặt hàng.</li>
                <li><strong>Kiểm tra hàng:</strong> được mở hộp kiểm tra trước khi thanh toán với đơn COD.</li>
            </ol>
            <p class="policy-note">
                <i class="fa-solid fa-circle-info"></i>
                Đơn giao thất bại 2 lần do không liên hệ được sẽ được huỷ và hoàn về kho.
                Vui lòng kiểm tra kỹ số điện thoại khi đặt hàng.
            </p>
        </section>

        <!-- ================= THANH TOÁN ================= -->
        <section id="thanh-toan" class="policy-card">
            <h2 class="policy-heading"><i class="fa-solid fa-shield-halved"></i> Thanh toán &amp; bảo mật</h2>
            <ol class="policy-list">
                <li><strong>Hình thức:</strong> thanh toán khi nhận hàng (COD) hoặc chuyển khoản / quét mã VietQR.</li>
                <li><strong>Thông tin lưu trữ:</strong> chỉ gồm họ tên, số điện thoại, địa chỉ giao hàng và nội dung đơn. Shop không lưu số thẻ hay mật khẩu ngân hàng.</li>
                <li><strong>Mật khẩu tài khoản:</strong> được băm một chiều trước khi lưu, không đọc được dưới dạng văn bản.</li>
                <li><strong>Cảnh giác:</strong> nhân viên SneakerX không bao giờ hỏi mật khẩu, mã OTP hay yêu cầu chuyển khoản vào tài khoản cá nhân.</li>
            </ol>
            <p class="policy-note">
                <i class="fa-solid fa-circle-info"></i>
                Nếu nhận được tin nhắn nghi vấn mạo danh shop, hãy gọi hotline
                <a href="tel:<%= ShopInfo.Hotline %>"><%= ShopInfo.Hotline %></a> để xác minh.
            </p>
        </section>

        <!-- ================= ĐIỀU KHOẢN ================= -->
        <section id="dieu-khoan" class="policy-card">
            <h2 class="policy-heading"><i class="fa-solid fa-file-contract"></i> Điều khoản sử dụng</h2>
            <ol class="policy-list">
                <li><strong>Tài khoản:</strong> bạn chịu trách nhiệm về thông tin đã đăng ký và bảo mật mật khẩu của mình.</li>
                <li><strong>Giá và tồn kho:</strong> có thể thay đổi mà không báo trước. Đơn hàng chỉ được xác nhận sau khi shop liên hệ qua số điện thoại đã đặt.</li>
                <li><strong>Hình ảnh:</strong> mang tính minh hoạ; màu sắc thực tế có thể chênh lệch nhẹ do ánh sáng và màn hình.</li>
                <li><strong>Nội dung website:</strong> thuộc quyền của SneakerX, không sao chép cho mục đích thương mại khi chưa được đồng ý.</li>
                <li><strong>Luật áp dụng:</strong> pháp luật Việt Nam. Tranh chấp được ưu tiên giải quyết qua thương lượng.</li>
            </ol>
        </section>

        <div class="policy-cta">
            <i class="fa-solid fa-headset"></i>
            Cần hỏi thêm về một chính sách?
            <a href="TuVan.aspx">Gửi câu hỏi tư vấn</a> hoặc gọi
            <a href="tel:<%= ShopInfo.Hotline %>"><%= ShopInfo.Hotline %></a>.
        </div>

    </div>

</asp:Content>
