<%@ Page Title="Tư Vấn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TuVan.aspx.cs" Inherits="ShoeStore.TuVan" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/tuvan.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="consult-container">
        <div class="consult-header">
            <h1 class="consult-title"><i class="fa-solid fa-headset"></i> Trung Tâm Tư Vấn SneakerX</h1>
            <p class="consult-subtitle">Bạn cần hỗ trợ chọn Size, tư vấn mẫu giày phù hợp hoặc phản hồi dịch vụ? Đội ngũ hỗ trợ luôn sẵn sàng 24/7!</p>
        </div>

        <div class="consult-layout">
            <!-- BÊN TRÁI: FORM GỬI CÂU HỎI TƯ VẤN -->
            <div class="card-box">
                <h3 class="card-title"><i class="fa-solid fa-paper-plane"></i> Gửi Yêu Cầu Tư Vấn</h3>

                <div class="form-group-item">
                    <label class="form-label-custom">Họ và tên của bạn</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-input-pill" Placeholder="Ví dụ: Nguyễn Văn A"></asp:TextBox>
                </div>

                <div class="form-group-item">
                    <label class="form-label-custom">Số điện thoại / Zalo</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-input-pill" Placeholder="Nhập số điện thoại liên hệ"></asp:TextBox>
                </div>

                <div class="form-group-item">
                    <label class="form-label-custom">Chủ đề tư vấn</label>
                    <asp:DropDownList ID="ddlTopic" runat="server" CssClass="form-input-pill">
                        <asp:ListItem Value="Size">Tư vấn chọn Size giày chuẩn</asp:ListItem>
                        <asp:ListItem Value="Model">Tư vấn kiểu dáng / Thương hiệu</asp:ListItem>
                        <asp:ListItem Value="Order">Thắc mắc về đơn hàng & Vận chuyển</asp:ListItem>
                        <asp:ListItem Value="Other">Vấn đề khác</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-group-item">
                    <label class="form-label-custom">Nội dung câu hỏi</label>
                    <asp:TextBox ID="txtContent" runat="server" CssClass="form-input-pill" TextMode="MultiLine" Rows="4" Placeholder="Mô tả chi tiết thắc mắc của bạn (ví dụ: Chiều dài chân 25cm nên chọn size bao nhiêu?)..."></asp:TextBox>
                </div>

                <asp:Button ID="btnSubmitConsult" runat="server" Text="GỬI CÂU HỎI TƯ VẤN" CssClass="btn-pink-submit" OnClick="btnSubmitConsult_Click" />
            </div>

            <!-- BÊN PHẢI: CÂU HỎI THƯỜNG GẶP (FAQ) & LIÊN HỆ TRỰC TIẾP -->
            <div>
                <div class="card-box">
                    <h3 class="card-title"><i class="fa-solid fa-circle-question"></i> Câu Hỏi Thường GẶP</h3>

                    <div class="faq-item">
                        <div class="faq-question"><i class="fa-solid fa-circle-dot"></i> Làm sao để chọn đúng Size giày khi mua Online?</div>
                        <div class="faq-answer">Bạn chỉ cần đo chiều dài bàn chân từ gót đến ngón dài nhất. Hãy gửi số đo cho chúng tôi để nhân viên tư vấn bảng Size chuẩn từng thương hiệu (Nike, Adidas, Jordan có form khác nhau).</div>
                    </div>

                    <div class="faq-item">
                        <div class="faq-question"><i class="fa-solid fa-circle-dot"></i> Sản phẩm mua về không vừa có được đổi Size không?</div>
                        <div class="faq-answer">SneakerX hỗ trợ đổi Size miễn phí trong vòng 7 ngày kể từ khi nhận hàng (sản phẩm còn nguyên tem mạc và chưa qua sử dụng).</div>
                    </div>

                    <div class="faq-item">
                        <div class="faq-question"><i class="fa-solid fa-circle-dot"></i> Thời gian giao hàng mất bao lâu?</div>
                        <div class="faq-answer">Nội thành giao nhanh trong 1-2 ngày. Các tỉnh thành khác thời gian vận chuyển từ 2-4 ngày làm việc.</div>
                    </div>
                </div>

                <!-- HOTLINE HỖ TRỢ NHANH -->
                <div class="quick-contact-box">
                    <div class="quick-contact-icon"><i class="fa-solid fa-phone"></i></div>
                    <div>
                        <div style="font-size:12px; color:#666;">Tư vấn trực tiếp qua Hotline:</div>
                        <div style="font-size:18px; font-weight:800; color:#e63946;">1900 6789 - 0988 123 456</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>