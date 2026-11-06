<%@ Page Title="Chi tiết sản phẩm" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChiTietSanPham.aspx.cs" Inherits="ShoeStore.ChiTietSanPham" %>

<asp:Content ID="TaiNguyenTrang" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Content/css/chi-tiet-san-pham-don-gian.css" />
    <script src="Content/js/chi-tiet-san-pham.js" defer></script>
</asp:Content>

<asp:Content ID="NoiDung" ContentPlaceHolderID="MainContent" runat="server">
    <nav class="duong-dan" aria-label="Điều hướng">
        <a href="TrangChu.aspx">Trang chủ</a><span>/</span>
        <a href="SanPham.aspx">Sản phẩm</a><span>/</span>
        <span><asp:Literal ID="litDuongDan" runat="server" /></span>
    </nav>

    <asp:Literal ID="litThongBao" runat="server" />

    <asp:Panel ID="pnlSanPham" runat="server" CssClass="chi-tiet-trang">
        <section class="chi-tiet-chinh" aria-label="Thông tin sản phẩm">
            <div class="khu-vuc-anh">
                <div class="anh-lon-wrap">
                    <asp:Image ID="imgAnh" runat="server" CssClass="anh-san-pham-lon" AlternateText="Ảnh sản phẩm" />
                    <span class="nhan-anh">SneakerX</span>
                </div>
                <div class="anh-nho-list" aria-label="Các ảnh khác của sản phẩm">
                    <asp:Repeater ID="rptAnh" runat="server">
                        <ItemTemplate>
                            <button type="button" class='anh-nho-nut <%# Container.ItemIndex == 0 ? "dang-chon" : "" %>' data-large='<%#: Eval("ImageUrl") %>' aria-label='Xem ảnh <%# Container.ItemIndex + 1 %>' aria-pressed='<%# Container.ItemIndex == 0 ? "true" : "false" %>'>
                                <img class="anh-san-pham-nho" src='<%#: Eval("ThumbnailUrl") %>' alt='<%#: Eval("AltText") %>' />
                            </button>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <div class="thong-tin-san-pham">
                <span class="hang-tag"><asp:Literal ID="litHang" runat="server" /></span>
                <h1 class="ten-san-pham"><asp:Literal ID="litTen" runat="server" /></h1>
                <p class="ma-san-pham">Mã sản phẩm: <asp:Literal ID="litMa" runat="server" /></p>

                <div class="gia-va-ton">
                    <span class="gia-chi-tiet"><asp:Literal ID="litGia" runat="server" /></span>
                    <asp:Literal ID="litTonKho" runat="server" />
                </div>

                <p class="mo-ta-ngan"><asp:Literal ID="litMoTa" runat="server" /></p>

                <div class="chon-size">
                    <div class="nhan-dong">
                        <span class="nhan-muc">Chọn size</span>
                        <a href="#huong-dan-size">Hướng dẫn chọn size</a>
                    </div>
                    <asp:HiddenField ID="hfSize" runat="server" />
                    <div class="cac-size">
                        <asp:Repeater ID="rptSize" runat="server">
                            <ItemTemplate>
                                <button type="button" class='nut-size <%# Container.ItemIndex == 0 ? "dang-chon" : "" %>' data-size='<%#: Container.DataItem %>' aria-pressed='<%# Container.ItemIndex == 0 ? "true" : "false" %>'><%#: Container.DataItem %></button>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="chon-so-luong">
                    <label class="nhan-muc" for="<%= txtSoLuong.ClientID %>">Số lượng</label>
                    <div class="bo-dem-so-luong">
                        <button type="button" class="nut-tang-giam" data-qty-step="-1" aria-label="Giảm số lượng">−</button>
                        <asp:TextBox ID="txtSoLuong" runat="server" Text="1" TextMode="Number" min="1" CssClass="o-so-luong" />
                        <button type="button" class="nut-tang-giam" data-qty-step="1" aria-label="Tăng số lượng">+</button>
                    </div>
                </div>

                <div class="nut-hanh-dong">
                    <asp:Button ID="btnThem" runat="server" Text="Thêm vào giỏ hàng" CssClass="nut-them-gio" OnClick="btnThem_Click" />
                    <asp:Button ID="btnMuaNgay" runat="server" Text="Mua ngay" CssClass="nut-mua-ngay" OnClick="btnMuaNgay_Click" />
                </div>
                <div class="hanh-dong-phu">
                    <asp:Button ID="btnYeuThich" runat="server" Text="♡ Yêu thích" CssClass="nut-yeu-thich" OnClick="btnYeuThich_Click" CausesValidation="false" />
                </div>
                <asp:Literal ID="litKetQua" runat="server" />

                <div class="cam-ket">
                    <div><span class="bieu-tuong-cam-ket">↗</span><span><b>Giao hàng</b><small>Chi tiết giao hàng tại bước thanh toán</small></span></div>
                    <div><span class="bieu-tuong-cam-ket">✓</span><span><b>Hỗ trợ chọn size</b><small>Tham khảo bảng size trước khi đặt</small></span></div>
                </div>
            </div>
        </section>

        <section class="mo-ta-day-du" aria-labelledby="tieu-de-mo-ta">
            <p class="nhan-phan">CHI TIẾT</p>
            <h2 id="tieu-de-mo-ta">Mô tả sản phẩm</h2>
            <p><asp:Literal ID="litMoTaDayDu" runat="server" /></p>
            <div class="thong-tin-bo-sung">
                <h3>Thông tin sản phẩm</h3>
                <dl>
                    <div><dt>Thương hiệu</dt><dd><asp:Literal ID="litHangThongTin" runat="server" /></dd></div>
                    <div><dt>Mã sản phẩm</dt><dd><asp:Literal ID="litMaThongTin" runat="server" /></dd></div>
                    <div><dt>Size hiện có</dt><dd><asp:Literal ID="litSizeThongTin" runat="server" /></dd></div>
                    <div><dt>Tồn kho</dt><dd><asp:Literal ID="litTonKhoThongTin" runat="server" /></dd></div>
                </dl>
            </div>
            <details id="huong-dan-size" class="bang-huong-dan-size">
                <summary>Hướng dẫn chọn size giày</summary>
                <p>Đo chiều dài bàn chân từ gót đến đầu ngón dài nhất, sau đó đối chiếu bảng tham khảo dưới đây.</p>
                <div class="bang-size-scroll">
                    <table>
                        <thead><tr><th>Size EU</th><th>Chiều dài chân tham khảo</th></tr></thead>
                        <tbody>
                            <tr><td>38</td><td>24,0 cm</td></tr>
                            <tr><td>39</td><td>24,5 cm</td></tr>
                            <tr><td>40</td><td>25,0 cm</td></tr>
                            <tr><td>41</td><td>26,0 cm</td></tr>
                            <tr><td>42</td><td>26,5 cm</td></tr>
                            <tr><td>43</td><td>27,5 cm</td></tr>
                        </tbody>
                    </table>
                </div>
            </details>
        </section>

        <section class="danh-gia" aria-labelledby="tieu-de-danh-gia">
            <h2 id="tieu-de-danh-gia">Đánh giá từ khách hàng</h2>
            <div class="tom-tat-danh-gia">
                <strong><asp:Literal ID="litDiemTrungBinh" runat="server" /> <span>/ 5</span></strong>
                <span class="sao-danh-gia"><asp:Literal ID="litSaoTrungBinh" runat="server" /></span>
                <span><asp:Literal ID="litSoDanhGia" runat="server" /></span>
            </div>

            <asp:Panel ID="pnlCanMuaDeDanhGia" runat="server" CssClass="nhac-danh-gia" Visible="false">
                Đăng nhập và mua sản phẩm này để gửi đánh giá sau khi trải nghiệm.
            </asp:Panel>

            <asp:Panel ID="pnlFormDanhGia" runat="server" CssClass="form-danh-gia" Visible="false">
                <h3>Viết đánh giá của bạn</h3>
                <label for="<%= ddlSao.ClientID %>">Mức đánh giá</label>
                <asp:DropDownList ID="ddlSao" runat="server" CssClass="chon-danh-gia">
                    <asp:ListItem Value="5">5 sao - Rất tốt</asp:ListItem>
                    <asp:ListItem Value="4">4 sao - Tốt</asp:ListItem>
                    <asp:ListItem Value="3">3 sao - Bình thường</asp:ListItem>
                    <asp:ListItem Value="2">2 sao - Chưa tốt</asp:ListItem>
                    <asp:ListItem Value="1">1 sao - Không hài lòng</asp:ListItem>
                </asp:DropDownList>
                <label for="<%= txtDanhGia.ClientID %>">Nhận xét</label>
                <asp:TextBox ID="txtDanhGia" runat="server" TextMode="MultiLine" Rows="4" MaxLength="1000" CssClass="o-danh-gia" placeholder="Chia sẻ trải nghiệm của bạn..." />
                <asp:Button ID="btnGuiDanhGia" runat="server" Text="Gửi đánh giá" CssClass="nut-gui-danh-gia" OnClick="btnGuiDanhGia_Click" CausesValidation="false" />
                <asp:Literal ID="litThongBaoDanhGia" runat="server" />
            </asp:Panel>

            <asp:Panel ID="pnlChuaCoDanhGia" runat="server" CssClass="danh-gia-rong" Visible="false">
                Sản phẩm chưa có đánh giá nào.
            </asp:Panel>
            <asp:Repeater ID="rptDanhGia" runat="server">
                <ItemTemplate>
                    <article class="mot-danh-gia">
                        <div class="danh-gia-nguoi-dung"><strong><%#: Eval("CustomerName") %></strong><time><%#: Eval("CreatedAt", "{0:dd/MM/yyyy HH:mm}") %></time></div>
                        <div class="sao-danh-gia"><%#: Eval("Stars") %></div>
                        <p><%#: Eval("Comment") %></p>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </section>

        <asp:Panel ID="pnlLienQuan" runat="server" CssClass="san-pham-lien-quan">
            <div class="dau-muc-lien-quan">
                <div><p class="nhan-phan">CÓ THỂ BẠN CŨNG THÍCH</p><h2>Sản phẩm liên quan</h2></div>
                <a href="SanPham.aspx">Xem tất cả sản phẩm →</a>
            </div>
            <div class="luoi-lien-quan">
                <asp:Repeater ID="rptLienQuan" runat="server">
                    <ItemTemplate>
                        <a class="the-lien-quan" href='ChiTietSanPham.aspx?id=<%#: Eval("Id") %>'>
                            <img src='<%#: ResolveUrl(Convert.ToString(Eval("ImageUrl"))) %>' alt='<%#: Eval("Name") %>' />
                            <span class="thuong-hieu-lien-quan"><%#: Eval("Brand") %></span>
                            <strong><%#: Eval("Name") %></strong>
                            <span class="gia-lien-quan"><%#: string.Format("{0:N0} đ", Eval("Price")) %></span>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </asp:Panel>

        <p class="quay-lai"><a href="SanPham.aspx">← Tiếp tục xem sản phẩm</a></p>
    </asp:Panel>
</asp:Content>
