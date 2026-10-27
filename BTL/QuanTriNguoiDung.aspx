<%@ Page Title="Quản trị người dùng" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuanTriNguoiDung.aspx.cs" Inherits="ShoeStore.QuanTriNguoiDung" %>
<%@ Register Src="~/Controls/AdminTabs.ascx" TagPrefix="uc" TagName="AdminTabs" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/quantri.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlAdmin" runat="server" CssClass="admin-page" Visible="false">

        <div class="admin-head">
            <div>
                <h1 class="admin-title">Quản trị người dùng</h1>
                <p class="admin-sub">
                    Danh sách tài khoản đã đăng ký và quyền quản trị. Không xoá tài khoản:
                    đơn hàng, giỏ và danh sách yêu thích đều gắn với tài khoản, xoá sẽ để lại
                    dữ liệu mồ côi. Không tự thu quyền của chính mình, và luôn phải còn ít
                    nhất một quản trị viên.
                </p>
            </div>
        </div>

        <uc:AdminTabs ID="ucTabs" runat="server" />

        <section class="admin-card">
            <div class="admin-toolbar">
                <asp:Panel ID="pnlSearch" runat="server" CssClass="admin-search" DefaultButton="btnSearch">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="admin-input"
                        Placeholder="Tìm theo họ tên, tên đăng nhập, email hoặc số điện thoại..."></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="Tìm" CausesValidation="false"
                        CssClass="btn-admin btn-admin-ghost" OnClick="btnSearch_Click" />
                </asp:Panel>
                <span class="admin-count"><asp:Literal ID="litCount" runat="server" Mode="Encode"></asp:Literal></span>
            </div>

            <asp:Panel ID="pnlTable" runat="server">
                <div class="admin-table-wrap">
                    <table class="admin-table">
                        <thead>
                            <tr>
                                <th>Họ tên</th>
                                <th>Tài khoản</th>
                                <th>Email</th>
                                <th>Điện thoại</th>
                                <th>Quyền</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptUsers" runat="server" OnItemCommand="rptUsers_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <span class="cell-name"><%#: Eval("FullName") %></span>
                                        </td>
                                        <td class="cell-nowrap"><%#: Eval("Username") %></td>
                                        <td><%#: Eval("Email") %></td>
                                        <td><%#: Eval("Phone") %></td>
                                        <td>
                                            <span class='<%# (bool)Eval("IsAdmin") ? "role-badge is-admin" : "role-badge" %>'><%#: Eval("RoleLabel") %></span>
                                        </td>
                                        <td class="cell-actions">
                                            <asp:LinkButton ID="btnToggle" runat="server" Visible='<%# (bool)Eval("CanToggle") %>'
                                                CssClass="btn-row" CommandName="Toggle" Text='<%# Eval("ToggleLabel") %>'
                                                CommandArgument='<%# Eval("Username") %>' CausesValidation="false"></asp:LinkButton>
                                            <asp:Label ID="lblLocked" runat="server" CssClass="cell-muted"
                                                Visible='<%# !(bool)Eval("CanToggle") %>' Text='<%# Eval("LockedReason") %>'></asp:Label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmpty" runat="server" CssClass="admin-empty" Visible="false">
                <i class="fa-solid fa-user"></i>
                <asp:Literal ID="litEmpty" runat="server" Mode="Encode"></asp:Literal>
            </asp:Panel>
        </section>

    </asp:Panel>

</asp:Content>
