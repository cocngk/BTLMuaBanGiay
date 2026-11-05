<%@ Page Title="Sửa sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SuaSanPham.aspx.cs" Inherits="ShoeStore.SuaSanPham" %>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">

    <h1><asp:Literal ID="litTieuDe" runat="server" Text="Thêm sản phẩm" /></h1>

    <div class="form" style="max-width: 620px;">
        <div class="dong">
            <label for="<%= txtTen.ClientID %>">Tên sản phẩm</label>
            <asp:TextBox ID="txtTen" runat="server" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtHang.ClientID %>">Hãng</label>
            <asp:TextBox ID="txtHang" runat="server" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtGia.ClientID %>">Giá (đồng)</label>
            <asp:TextBox ID="txtGia" runat="server" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtTon.ClientID %>">Số lượng trong kho</label>
            <asp:TextBox ID="txtTon" runat="server" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtSize.ClientID %>">Các size, cách nhau bằng dấu phẩy</label>
            <asp:TextBox ID="txtSize" runat="server" CssClass="o-nhap" Text="38,39,40,41"></asp:TextBox>
        </div>

        <div class="dong">
            <label for="<%= txtAnh.ClientID %>">Đường dẫn ảnh</label>
            <asp:TextBox ID="txtAnh" runat="server" CssClass="o-nhap"></asp:TextBox>
            <p class="nhan">Ví dụ: Content/images/products/nike-1-sm.jpg</p>
        </div>

        <div class="dong">
            <label for="<%= txtMoTa.ClientID %>">Mô tả</label>
            <asp:TextBox ID="txtMoTa" runat="server" TextMode="MultiLine" Rows="4" CssClass="o-nhap"></asp:TextBox>
        </div>

        <div class="dong">
            <asp:CheckBox ID="chkNoiBat" runat="server" Text=" Hiện ở mục sản phẩm nổi bật" />
        </div>

        <asp:Button ID="btnLuu" runat="server" Text="Lưu" CssClass="nut" OnClick="btnLuu_Click" />
        <a class="nut phu" href="QuanTriSanPham.aspx">Quay lại</a>

        <asp:Literal ID="litThongBao" runat="server" />
    </div>

</asp:Content>
