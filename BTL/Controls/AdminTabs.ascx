<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminTabs.ascx.cs" Inherits="ShoeStore.AdminTabs" %>
<%-- Thanh tab của khu quản trị: một chỗ duy nhất biết có những khu vực nào,
     nên thêm khu mới không phải sửa bốn trang. --%>
<nav class="admin-tabs" aria-label="Khu quản trị">
    <a id="lnkProducts" runat="server" href="QuanTriSanPham.aspx" class="admin-tab">
        <i class="fa-solid fa-box-open"></i> Sản phẩm
    </a>
    <a id="lnkOrders" runat="server" href="QuanTriDonHang.aspx" class="admin-tab">
        <i class="fa-solid fa-receipt"></i> Đơn hàng
    </a>
    <a id="lnkUsers" runat="server" href="QuanTriNguoiDung.aspx" class="admin-tab">
        <i class="fa-solid fa-users"></i> Người dùng
    </a>
</nav>
