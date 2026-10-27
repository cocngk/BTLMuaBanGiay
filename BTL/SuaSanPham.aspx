<%@ Page Title="Sửa sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SuaSanPham.aspx.cs" Inherits="ShoeStore.SuaSanPham" %>
<%@ Register Src="~/Controls/AdminTabs.ascx" TagPrefix="uc" TagName="AdminTabs" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/quantri.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlAdmin" runat="server" CssClass="admin-page" Visible="false">

        <div class="admin-head">
            <div>
                <h1 class="admin-title"><asp:Literal ID="litTitle" runat="server" Mode="Encode"></asp:Literal></h1>
                <p class="admin-sub"><asp:Literal ID="litSub" runat="server" Mode="Encode"></asp:Literal></p>
            </div>
            <a href="QuanTriSanPham.aspx" class="btn-admin btn-admin-ghost">
                <i class="fa-solid fa-arrow-left"></i> Danh sách
            </a>
        </div>

        <uc:AdminTabs ID="ucTabs" runat="server" />

        <section class="admin-card">

            <asp:Panel ID="pnlFormError" runat="server" Visible="false" CssClass="form-error-banner">
                <i class="fa-solid fa-circle-exclamation"></i>
                <asp:Literal ID="litFormError" runat="server" Mode="Encode"></asp:Literal>
            </asp:Panel>

            <asp:Panel ID="pnlForm" runat="server" CssClass="admin-form" DefaultButton="btnSave">

                <div class="admin-preview">
                    <asp:Image ID="imgPreview" runat="server" AlternateText="Ảnh xem trước của sản phẩm" />
                    <div class="admin-preview-meta">
                        <strong>Ảnh xem trước</strong><br />
                        Lấy từ đường dẫn ảnh bên dưới. Đổi ảnh sẽ bỏ bộ màu cũ của sản phẩm —
                        trang chi tiết chỉ còn hiện đúng một ảnh mới.
                    </div>
                </div>

                <div class="admin-grid-2">
                    <div class="admin-field">
                        <label>Tên sản phẩm *</label>
                        <asp:TextBox ID="txtName" runat="server" CssClass="admin-input" MaxLength="120"
                            Placeholder="Ví dụ: Nike Air Force 1 '07"></asp:TextBox>
                    </div>

                    <div class="admin-field">
                        <label>Thương hiệu *</label>
                        <asp:TextBox ID="txtBrand" runat="server" CssClass="admin-input" MaxLength="60"
                            Placeholder="Nike, Adidas, ..."></asp:TextBox>
                        <datalist id="dlBrands">
                            <asp:Repeater ID="rptBrands" runat="server">
                                <ItemTemplate><option value="<%# Container.DataItem %>"></option></ItemTemplate>
                            </asp:Repeater>
                        </datalist>
                        <div class="hint">Gõ tự do, hoặc chọn một hãng đã có để bộ lọc không bị tách đôi.</div>
                    </div>
                </div>

                <div class="admin-grid-2">
                    <div class="admin-field">
                        <label>Giá bán (đ) *</label>
                        <asp:TextBox ID="txtPrice" runat="server" CssClass="admin-input" Placeholder="2500000"></asp:TextBox>
                        <div class="hint">Chỉ nhập số, có thể dùng dấu chấm: 2.500.000</div>
                    </div>

                    <div class="admin-field">
                        <label>Giá gạch (đ)</label>
                        <asp:TextBox ID="txtOldPrice" runat="server" CssClass="admin-input" Placeholder="Để trống nếu không giảm giá"></asp:TextBox>
                        <div class="hint">Chỉ hiện trên trang khách khi cao hơn giá bán.</div>
                    </div>
                </div>

                <div class="admin-grid-2">
                    <div class="admin-field">
                        <label>Tồn kho *</label>
                        <asp:TextBox ID="txtStock" runat="server" CssClass="admin-input" Placeholder="10"></asp:TextBox>
                        <div class="hint">Số đôi còn bán. Đặt 0 là tạm hết hàng nhưng vẫn giữ sản phẩm.</div>
                    </div>

                    <div class="admin-field">
                        <label>Size có sẵn *</label>
                        <asp:TextBox ID="txtSizes" runat="server" CssClass="admin-input" Placeholder="38,39,40,41"></asp:TextBox>
                        <div class="hint">Cách nhau bằng dấu phẩy. Size phải trong khoảng 20–60.</div>
                    </div>
                </div>

                <div class="admin-field">
                    <label>Đường dẫn ảnh nhỏ *</label>
                    <asp:TextBox ID="txtImageUrl" runat="server" CssClass="admin-input" MaxLength="400"
                        Placeholder="~/Content/images/ten-anh.jpg"></asp:TextBox>
                    <div class="hint">
                        Bắt đầu bằng <code>~/</code> để luôn trỏ đúng gốc ứng dụng, hoặc dùng URL http(s).
                        Đây là đường dẫn chứ không phải tệp tải lên.
                    </div>
                </div>

                <div class="admin-field">
                    <label>Đường dẫn ảnh lớn</label>
                    <asp:TextBox ID="txtImageUrlLarge" runat="server" CssClass="admin-input" MaxLength="400"
                        Placeholder="Để trống thì dùng ảnh nhỏ"></asp:TextBox>
                </div>

                <div class="admin-field">
                    <label>Mô tả</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="admin-textarea" TextMode="MultiLine"
                        MaxLength="2000" Rows="5" Placeholder="Chất liệu, phong cách, ghi chú về form..."></asp:TextBox>
                    <div class="hint">Mô tả cũng được tìm kiếm ở trang Sản phẩm, tối đa 2000 ký tự.</div>
                </div>

                <div class="admin-note">
                    <i class="fa-solid fa-circle-info"></i>
                    <span>
                        Bản demo lưu danh mục trong bộ nhớ ứng dụng. Thay đổi có hiệu lực ngay cho
                        mọi phiên, nhưng mất khi IIS khởi động lại.
                    </span>
                </div>

                <div class="admin-actions">
                    <asp:Button ID="btnSave" runat="server" Text="Lưu sản phẩm" CssClass="btn-admin"
                        CausesValidation="false" OnClick="btnSave_Click" />
                    <a href="QuanTriSanPham.aspx" class="btn-admin btn-admin-ghost">Huỷ</a>
                    <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn-row btn-row-danger"
                        CausesValidation="false" Visible="false" OnClick="btnDelete_Click"
                        OnClientClick="return confirm('Xoá sản phẩm này? Thao tác không hoàn tác được.');">
                        <i class="fa-solid fa-trash"></i> Xoá sản phẩm
                    </asp:LinkButton>
                </div>

            </asp:Panel>
        </section>

    </asp:Panel>

</asp:Content>
