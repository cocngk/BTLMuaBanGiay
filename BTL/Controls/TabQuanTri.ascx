<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TabQuanTri.ascx.cs" Inherits="ShoeStore.TabQuanTri" %>
<%-- Thanh tab của khu quản trị: một chỗ duy nhất biết có những khu vực nào,
     nên thêm khu mới không phải sửa bốn trang.
     href để "#" trong markup và được đặt trong TabQuanTri.ascx.cs bằng
     ResolveUrl("~/..."): control nằm ở /Controls/ nên href tương đối sẽ bị
     ghép thành /Controls/QuanTriDonHang.aspx (404). --%>
<nav class="admin-tabs" aria-label="Khu quản trị">
    <a id="lnkProducts" runat="server" href="#" class="admin-tab">
        <i class="fa-solid fa-box-open"></i> Sản phẩm
    </a>
    <a id="lnkOrders" runat="server" href="#" class="admin-tab">
        <i class="fa-solid fa-receipt"></i> Đơn hàng
    </a>
    <a id="lnkUsers" runat="server" href="#" class="admin-tab">
        <i class="fa-solid fa-users"></i> Người dùng
    </a>
</nav>
