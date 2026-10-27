<%@ Page Title="Quản trị đơn hàng" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuanTriDonHang.aspx.cs" Inherits="ShoeStore.QuanTriDonHang" %>
<%@ Register Src="~/Controls/AdminTabs.ascx" TagPrefix="uc" TagName="AdminTabs" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/quantri.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlAdmin" runat="server" CssClass="admin-page" Visible="false">

        <div class="admin-head">
            <div>
                <h1 class="admin-title">Quản trị đơn hàng</h1>
                <p class="admin-sub">
                    Đơn của mọi khách, mới nhất trước. Đổi trạng thái để theo dõi giao hàng.
                    Huỷ đơn sẽ trả hàng về kho; mở lại đơn đã huỷ sẽ lấy lại hàng và bị chặn
                    nếu kho không còn đủ.
                </p>
            </div>
        </div>

        <uc:AdminTabs ID="ucTabs" runat="server" />

        <section class="admin-card">
            <div class="admin-toolbar">
                <asp:Panel ID="pnlFilter" runat="server" CssClass="admin-search" DefaultButton="btnFilter">
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="admin-select"></asp:DropDownList>
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="admin-input"
                        Placeholder="Mã đơn, tên khách hoặc số điện thoại..."></asp:TextBox>
                    <asp:Button ID="btnFilter" runat="server" Text="Lọc" CausesValidation="false"
                        CssClass="btn-admin btn-admin-ghost" OnClick="btnFilter_Click" />
                </asp:Panel>
                <span class="admin-count"><asp:Literal ID="litCount" runat="server" Mode="Encode"></asp:Literal></span>
            </div>

            <asp:Panel ID="pnlTable" runat="server">
                <div class="admin-table-wrap">
                    <table class="admin-table">
                        <thead>
                            <tr>
                                <th>Mã đơn</th>
                                <th>Đặt lúc</th>
                                <th>Khách</th>
                                <th>Số món</th>
                                <th>Tổng</th>
                                <th>Trạng thái</th>
                                <th>Đổi trạng thái</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptOrders" runat="server"
                                OnItemCommand="rptOrders_ItemCommand" OnItemDataBound="rptOrders_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <span class="cell-name cell-nowrap"><%#: Eval("Code") %></span>
                                            <details class="order-details">
                                                <summary><%#: Eval("ItemSummary") %></summary>
                                                <ul class="order-lines-mini">
                                                    <asp:Repeater ID="rptLines" runat="server">
                                                        <ItemTemplate>
                                                            <li>
                                                                <span><%#: Eval("Name") %> <span class="qty">· size <%#: Eval("Size") %></span></span>
                                                                <span class="qty">x<%# Eval("Quantity") %></span>
                                                            </li>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ul>
                                                <div class="cell-sub"><%#: Eval("Address") %></div>
                                            </details>
                                        </td>
                                        <td><%#: Eval("CreatedAtText") %></td>
                                        <td>
                                            <span class="cell-name"><%#: Eval("CustomerName") %></span>
                                            <div class="cell-sub"><%#: Eval("Phone") %></div>
                                        </td>
                                        <td class="cell-num"><%# Eval("Quantity") %></td>
                                        <td class="cell-num"><%#: Eval("TotalText") %></td>
                                        <td><span class='<%# Eval("StatusClass") %>'><%#: Eval("Status") %></span></td>
                                        <td>
                                            <div class="admin-row-status">
                                                <asp:DropDownList ID="ddlRowStatus" runat="server" CssClass="admin-select"></asp:DropDownList>
                                                <asp:LinkButton ID="btnUpdateStatus" runat="server" CssClass="btn-row"
                                                    CommandName="ChangeStatus" CommandArgument='<%# Eval("Code") %>'
                                                    CausesValidation="false"><i class="fa-solid fa-check"></i> Lưu</asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmpty" runat="server" CssClass="admin-empty" Visible="false">
                <i class="fa-solid fa-receipt"></i>
                <asp:Literal ID="litEmpty" runat="server" Mode="Encode"></asp:Literal>
            </asp:Panel>

            <asp:Panel ID="pnlPager" runat="server" CssClass="admin-pager" Visible="false">
                <asp:LinkButton ID="btnPrev" runat="server" CssClass="btn-row" CausesValidation="false"
                    OnClick="btnPrev_Click"><i class="fa-solid fa-chevron-left"></i> Trước</asp:LinkButton>
                <span class="pager-info">
                    Trang <asp:Literal ID="litPage" runat="server"></asp:Literal> /
                    <asp:Literal ID="litTotalPages" runat="server"></asp:Literal>
                </span>
                <asp:LinkButton ID="btnNext" runat="server" CssClass="btn-row" CausesValidation="false"
                    OnClick="btnNext_Click">Sau <i class="fa-solid fa-chevron-right"></i></asp:LinkButton>
            </asp:Panel>
        </section>

    </asp:Panel>

</asp:Content>
