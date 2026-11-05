<%@ Page Title="Quản trị sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="QuanTriSanPham.aspx.cs" Inherits="ShoeStore.QuanTriSanPham" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Quản trị sản phẩm</h1>

    <asp:Literal ID="litThongBao" runat="server" />

    <p>
        <a class="nut" href="SuaSanPham.aspx">Thêm sản phẩm</a>
    </p>

    <asp:GridView ID="gvSanPham" runat="server"
        AutoGenerateColumns="false"
        DataKeyNames="Id"
        CssClass="bang"
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

</asp:Content>
