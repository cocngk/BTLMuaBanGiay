<%@ Page Title="Tư Vấn" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TuVan.aspx.cs" Inherits="ShoeStore.TuVan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .consult-container {
            max-width: 1100px;
            margin: 40px auto;
            padding: 0 20px;
        }

        .consult-header {
            text-align: center;
            margin-bottom: 40px;
        }

        .consult-title {
            font-size: 28px;
            font-weight: 800;
            color: #e63946;
            margin-bottom: 8px;
        }

        .consult-subtitle {
            font-size: 14px;
            color: #666;
        }

        /* GRID CỘT: FORM TƯ VẤN & FAQ */
        .consult-layout {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 35px;
        }

        @media (max-width: 850px) {
            .consult-layout { grid-template-columns: 1fr; }
        }

        .card-box {
            background: #ffffff;
            border-radius: 25px;
            padding: 30px;
            border: 1px solid #fce8ec;
            box-shadow: 0 5px 20px rgba(0,0,0,0.03);
        }

        .card-title {
            font-size: 18px;
            font-weight: 800;
            color: #111;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .card-title i { color: #e63946; }

        .form-group-item {
            margin-bottom: 16px;
        }

        .form-label-custom {
            display: block;
            font-size: 13px;
            font-weight: 700;
            color: #444;
            margin-bottom: 6px;
        }

        .form-input-pill {
            width: 100%;
            padding: 12px 18px;
            border: 1px solid #f8d7da;
            border-radius: 15px;
            font-size: 13px;
            outline: none;
            background: #fafafa;
        }

        .form-input-pill:focus {
            border-color: #e63946;
            background: #fff;
        }

        .btn-pink-submit {
            width: 100%;
            background: #e63946;
            color: #ffffff;
            border: none;
            padding: 13px;
            border-radius: 15px;
            font-size: 15px;
            font-weight: 700;
            cursor: pointer;
            box-shadow: 0 4px 12px rgba(230, 57, 70, 0.25);
            transition: background 0.3s;
        }

        .btn-pink-submit:hover { background: #c1121f; }

        /* DANH SÁCH FAQ */
        .faq-item {
            margin-bottom: 15px;
            border-bottom: 1px dashed #f8d7da;
            padding-bottom: 15px;
        }

        .faq-item:last-child {
            border-bottom: none;
            padding-bottom: 0;
            margin-bottom: 0;
        }

        .faq-question {
            font-size: 14px;
            font-weight: 700;
            color: #222;
            margin-bottom: 6px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .faq-question i { color: #e63946; font-size: 12px; }

        .faq-answer {
            font-size: 13px;
            color: #666;
            line-height: 1.5;
            padding-left: 20px;
        }

        /* CONTACT QUICK CHAT */
        .quick-contact-box {
            display: flex;
            gap: 15px;
            margin-top: 25px;
            background: #fdf2f4;
            padding: 15px 20px;
            border-radius: 20px;
            align-items: center;
        }

        .quick-contact-icon {
            background: #e63946;
            color: white;
            width: 45px;
            height: 45px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
        }
    </style>

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