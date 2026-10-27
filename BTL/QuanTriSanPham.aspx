<%@ Page Title="Quản trị sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuanTriSanPham.aspx.cs" Inherits="ShoeStore.QuanTriSanPham" %>
<%@ Import Namespace="ShoeStore.Helpers" %>
<%@ Register Src="~/Controls/AdminTabs.ascx" TagPrefix="uc" TagName="AdminTabs" %>

<asp:Content ID="HeadCss" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/quantri.css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlAdmin" runat="server" CssClass="admin-page" Visible="false">

        <div class="admin-head">
            <div>
                <h1 class="admin-title">Quản trị sản phẩm</h1>
                <p class="admin-sub">
                    Toàn bộ danh mục đang bán: thêm, sửa, xoá. Thay đổi chỉ nằm trong bộ nhớ
                    của ứng dụng (bản demo), khởi động lại IIS là quay về dữ liệu mẫu.
                </p>
            </div>
            <a href="SuaSanPham.aspx" class="btn-admin"><i class="fa-solid fa-plus"></i> Thêm sản phẩm</a>
        </div>

        <uc:AdminTabs ID="ucTabs" runat="server" />

        <section class="admin-card">
            <div class="admin-toolbar">
                <asp:Panel ID="pnlSearch" runat="server" CssClass="admin-search" DefaultButton="btnSearch">
                    <asp:TextBox ID="txtSearch" runat="server" CssClass="admin-input"
                        Placeholder="Tìm theo tên, hãng hoặc mô tả..."></asp:TextBox>
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
                                <th>Ảnh</th>
                                <th>Sản phẩm</th>
                                <th>Hãng</th>
                                <th>Giá</th>
                                <th>Tồn</th>
                                <th>Size</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptProducts" runat="server" OnItemCommand="rptProducts_ItemCommand">
                                <ItemTemplate>
                                    <tr>
                                        <td class="cell-img">
                                            <img src='<%# ResolveUrl(Convert.ToString(Eval("ImageUrl"))) %>'
                                                alt='<%# Eval("Name") %>' loading="lazy" />
                                        </td>
                                        <td>
                                            <a class="cell-name" href='<%# "SuaSanPham.aspx?id=" + Eval("Id") %>'><%#: Eval("Name") %></a>
                                            <div class="cell-sub"><%#: Eval("Description") %></div>
                                        </td>
                                        <td><%#: Eval("Brand") %></td>
                                        <td class="cell-num"><%# Money.Format(Convert.ToDecimal(Eval("Price"))) %></td>
                                        <td class="cell-num">
                                            <span class='<%# StockClass(Eval("Stock")) %>'><%# Eval("Stock") %></span>
                                        </td>
                                        <td><%#: Eval("Sizes") %></td>
                                        <td class="cell-actions">
                                            <a class="btn-row" href='<%# "SuaSanPham.aspx?id=" + Eval("Id") %>'>
                                                <i class="fa-solid fa-pen"></i> Sửa
                                            </a>
                                            <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn-row btn-row-danger"
                                                CommandName="Delete" CommandArgument='<%# Eval("Id") %>' CausesValidation="false"
                                                OnClientClick="return confirm('Xoá sản phẩm này? Thao tác không hoàn tác được.');">
                                                <i class="fa-solid fa-trash"></i> Xoá
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmpty" runat="server" CssClass="admin-empty" Visible="false">
                <i class="fa-solid fa-folder-open"></i>
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
