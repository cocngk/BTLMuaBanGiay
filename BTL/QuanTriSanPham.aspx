<%@ Page Title="Quản trị sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuanTriSanPham.aspx.cs" Inherits="ShoeStore.QuanTriSanPham" %>
<%@ Register Src="~/Controls/TabQuanTri.ascx" TagPrefix="uc" TagName="TabQuanTri" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="Content/css/quantri.css" />
    <section class="admin-page">
    <h1>Quản trị sản phẩm</h1>
    <uc:TabQuanTri ID="ucTabs" runat="server" ActiveTab="products" />

    <asp:Literal ID="litThongBao" runat="server" />

    <p>
        <a class="nut" href="SuaSanPham.aspx">Thêm sản phẩm</a>
    </p>

    <div class="admin-table-wrap">
    <asp:GridView ID="gvSanPham" runat="server"
        AutoGenerateColumns="false"
        DataKeyNames="Id"
        CssClass="admin-table"
        GridLines="None"
        OnRowCommand="gvSanPham_RowCommand">
        <Columns>
            <asp:BoundField DataField="Id" HeaderText="Mã" />
            <asp:BoundField DataField="Name" HeaderText="Tên sản phẩm" />
            <asp:BoundField DataField="Brand" HeaderText="Hãng" />
            <asp:BoundField DataField="Price" HeaderText="Giá" DataFormatString="{0:N0} đ" />
            <asp:BoundField DataField="Stock" HeaderText="Tồn" />
            <asp:TemplateField HeaderText="Thao tác">
                <ItemTemplate>
                    <a href='SuaSanPham.aspx?id=<%# Eval("Id") %>'>Sửa</a>
                    &middot;
                    <asp:LinkButton ID="btnXoa" runat="server" Text="Xóa"
                        CommandName="Xoa"
                        CommandArgument='<%# Eval("Id") %>'
                        CssClass="nut-link"
                        OnClientClick="return confirm('Xóa sản phẩm này?');"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    </div>
    </section>

</asp:Content>
