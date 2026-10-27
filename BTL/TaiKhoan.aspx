<%@ Page Title="Tài khoản của tôi" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TaiKhoan.aspx.cs" Inherits="ShoeStore.TaiKhoan" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <meta name="description" content="Xem và cập nhật hồ sơ, đổi mật khẩu đăng nhập tại SneakerX." />
    <link rel="stylesheet" href="Content/taikhoan.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlPage" runat="server" CssClass="account-page" Visible="false">

        <div class="account-head">
            <h1 class="account-title">Tài khoản của tôi</h1>
            <p class="account-sub">Xem và cập nhật hồ sơ, đổi mật khẩu đăng nhập.</p>
        </div>

        <div class="account-grid">
            <aside class="account-summary">
                <div class="account-avatar"><i class="fa-solid fa-user"></i></div>
                <h2 class="account-name"><asp:Literal ID="litName" runat="server" Mode="Encode"></asp:Literal></h2>
                <p class="account-username">@<asp:Literal ID="litUsername" runat="server" Mode="Encode"></asp:Literal></p>
                <span id="spnRole" runat="server" class="account-role"><asp:Literal ID="litRole" runat="server" Mode="Encode"></asp:Literal></span>

                <ul class="account-facts">
                    <li><i class="fa-solid fa-envelope"></i><span><asp:Literal ID="litEmail" runat="server" Mode="Encode"></asp:Literal></span></li>
                    <li><i class="fa-solid fa-phone"></i><span><asp:Literal ID="litPhone" runat="server" Mode="Encode"></asp:Literal></span></li>
                </ul>

                <div class="account-links">
                    <a href="DonHang.aspx"><i class="fa-solid fa-receipt"></i> Đơn hàng của tôi</a>
                    <asp:Panel ID="pnlAdminEntry" runat="server" Visible="false">
                        <a href="QuanTriSanPham.aspx"><i class="fa-solid fa-gauge-high"></i> Khu quản trị</a>
                    </asp:Panel>
                </div>
            </aside>

            <div class="account-forms">

                <section class="account-card">
                    <h2 class="account-card-title"><i class="fa-solid fa-id-card"></i> Hồ sơ cá nhân</h2>
                    <p class="account-card-sub">
                        Tên đăng nhập dùng để đăng nhập nên không đổi được. Email phải là duy nhất
                        trong hệ thống — đổi sang email người khác đang dùng sẽ bị từ chối.
                    </p>

                    <asp:Panel ID="pnlProfileForm" runat="server" DefaultButton="btnSaveProfile">
                        <div class="acct-row">
                            <div class="acct-field">
                                <label>Họ và tên *</label>
                                <asp:TextBox ID="txtFullName" runat="server" CssClass="acct-input" MaxLength="80" Placeholder="Nhập họ và tên"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="reqFullName" runat="server" ControlToValidate="txtFullName"
                                    ValidationGroup="vgProfile" ErrorMessage="Vui lòng nhập họ và tên."
                                    CssClass="field-error" Display="Dynamic" />
                            </div>

                            <div class="acct-field">
                                <label>Tên đăng nhập</label>
                                <asp:TextBox ID="txtUsername" runat="server" CssClass="acct-input" ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>

                        <div class="acct-row">
                            <div class="acct-field">
                                <label>Địa chỉ Email *</label>
                                <%-- Không dùng TextMode="Email": Mono (ASP.NET trên Linux) bỏ qua
                                     mode này và render ô nhập KHÔNG kèm value, nên hồ sơ hiện ra
                                     trống. Kiểu email đã được kiểm bằng RegularExpressionValidator
                                     bên dưới nên không mất gì về mặt kiểm tra. --%>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="acct-input" MaxLength="120" Placeholder="example@domain.com"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="reqEmail" runat="server" ControlToValidate="txtEmail"
                                    ValidationGroup="vgProfile" ErrorMessage="Vui lòng nhập địa chỉ email."
                                    CssClass="field-error" Display="Dynamic" />
                                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                                    ValidationGroup="vgProfile" ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                    ErrorMessage="Địa chỉ email không hợp lệ." CssClass="field-error" Display="Dynamic" />
                            </div>

                            <div class="acct-field">
                                <label>Số điện thoại *</label>
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="acct-input" MaxLength="11" Placeholder="0912345678"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="reqPhone" runat="server" ControlToValidate="txtPhone"
                                    ValidationGroup="vgProfile" ErrorMessage="Vui lòng nhập số điện thoại."
                                    CssClass="field-error" Display="Dynamic" />
                                <asp:RegularExpressionValidator ID="revPhone" runat="server" ControlToValidate="txtPhone"
                                    ValidationGroup="vgProfile" ValidationExpression="^[0-9]{9,11}$"
                                    ErrorMessage="Số điện thoại phải có từ 9 đến 11 chữ số." CssClass="field-error" Display="Dynamic" />
                            </div>
                        </div>

                        <asp:Panel ID="pnlProfileError" runat="server" Visible="false" CssClass="form-error-banner">
                            <i class="fa-solid fa-circle-exclamation"></i>
                            <asp:Literal ID="litProfileError" runat="server" Mode="Encode"></asp:Literal>
                        </asp:Panel>

                        <asp:Button ID="btnSaveProfile" runat="server" Text="Lưu hồ sơ" CssClass="btn-acct"
                            ValidationGroup="vgProfile" OnClick="btnSaveProfile_Click" />
                    </asp:Panel>
                </section>

                <section class="account-card">
                    <h2 class="account-card-title"><i class="fa-solid fa-lock"></i> Đổi mật khẩu</h2>
                    <p class="account-card-sub">
                        Cần nhập mật khẩu hiện tại để xác nhận. Mật khẩu mới phải có ít nhất 6 ký tự.
                    </p>

                    <asp:Panel ID="pnlPasswordForm" runat="server" DefaultButton="btnChangePassword">
                        <div class="acct-field">
                            <label>Mật khẩu hiện tại *</label>
                            <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="acct-input" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqCurrentPassword" runat="server" ControlToValidate="txtCurrentPassword"
                                ValidationGroup="vgPassword" ErrorMessage="Vui lòng nhập mật khẩu hiện tại."
                                CssClass="field-error" Display="Dynamic" />
                        </div>

                        <div class="acct-row">
                            <div class="acct-field">
                                <label>Mật khẩu mới *</label>
                                <asp:TextBox ID="txtNewPassword" runat="server" CssClass="acct-input" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="reqNewPassword" runat="server" ControlToValidate="txtNewPassword"
                                    ValidationGroup="vgPassword" ErrorMessage="Vui lòng nhập mật khẩu mới."
                                    CssClass="field-error" Display="Dynamic" />
                                <asp:RegularExpressionValidator ID="revNewPassword" runat="server" ControlToValidate="txtNewPassword"
                                    ValidationGroup="vgPassword" ValidationExpression="^.{6,}$"
                                    ErrorMessage="Mật khẩu phải có ít nhất 6 ký tự." CssClass="field-error" Display="Dynamic" />
                            </div>

                            <div class="acct-field">
                                <label>Xác nhận mật khẩu mới *</label>
                                <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="acct-input" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="reqConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword"
                                    ValidationGroup="vgPassword" ErrorMessage="Vui lòng xác nhận mật khẩu mới."
                                    CssClass="field-error" Display="Dynamic" />
                                <asp:CompareValidator ID="cmpConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword"
                                    ControlToCompare="txtNewPassword" ValidationGroup="vgPassword"
                                    ErrorMessage="Mật khẩu xác nhận không trùng khớp." CssClass="field-error" Display="Dynamic" />
                            </div>
                        </div>

                        <asp:Panel ID="pnlPasswordError" runat="server" Visible="false" CssClass="form-error-banner">
                            <i class="fa-solid fa-circle-exclamation"></i>
                            <asp:Literal ID="litPasswordError" runat="server" Mode="Encode"></asp:Literal>
                        </asp:Panel>

                        <asp:Button ID="btnChangePassword" runat="server" Text="Đổi mật khẩu" CssClass="btn-acct"
                            ValidationGroup="vgPassword" OnClick="btnChangePassword_Click" />
                    </asp:Panel>
                </section>

            </div>
        </div>
    </asp:Panel>

</asp:Content>
