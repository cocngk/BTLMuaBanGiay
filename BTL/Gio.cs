using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace ShoeStore
{
    // Một dòng trong giỏ hàng.
    public class MatHang
    {
        public int MaSanPham { get; set; }
        public string Ten { get; set; }
        public decimal Gia { get; set; }
        public string Size { get; set; }
        public int SoLuong { get; set; }
        public string Anh { get; set; }

        // Thành tiền của dòng này.
        public decimal ThanhTien
        {
            get { return Gia * SoLuong; }
        }
    }

    // Giỏ hàng đơn giản: một danh sách trong Session.
    public static class Gio
    {
        private const string Khoa = "gio";

        private static HttpSessionState Phien
        {
            get { return HttpContext.Current.Session; }
        }

        // Đọc giỏ hàng hiện tại.
        public static List<MatHang> Doc()
        {
            if (Phien == null) return new List<MatHang>();

            List<MatHang> danhSach = Phien[Khoa] as List<MatHang>;
            if (danhSach == null)
            {
                danhSach = new List<MatHang>();
                Phien[Khoa] = danhSach;
            }
            return danhSach;
        }

        // Ghi giỏ hàng trở lại Session.
        public static void Luu(List<MatHang> danhSach)
        {
            if (Phien != null) Phien[Khoa] = danhSach;
        }

        // Thêm một món. Cùng sản phẩm và cùng size thì cộng số lượng.
        public static void Them(MatHang hang)
        {
            List<MatHang> danhSach = Doc();

            foreach (MatHang dong in danhSach)
            {
                if (dong.MaSanPham == hang.MaSanPham && dong.Size == hang.Size)
                {
                    dong.SoLuong = dong.SoLuong + hang.SoLuong;
                    Luu(danhSach);
                    return;
                }
            }

            danhSach.Add(hang);
            Luu(danhSach);
        }

        // Đổi số lượng của một dòng.
        public static void DoiSoLuong(int maSanPham, string size, int soLuong)
        {
            List<MatHang> danhSach = Doc();

            foreach (MatHang dong in danhSach)
            {
                if (dong.MaSanPham == maSanPham && dong.Size == size)
                {
                    dong.SoLuong = soLuong;
                    break;
                }
            }

            Luu(danhSach);
        }

        // Xóa một dòng khỏi giỏ.
        public static void Xoa(int maSanPham, string size)
        {
            List<MatHang> danhSach = Doc();

            for (int i = danhSach.Count - 1; i >= 0; i = i - 1)
            {
                if (danhSach[i].MaSanPham == maSanPham && danhSach[i].Size == size)
                    danhSach.RemoveAt(i);
            }

            Luu(danhSach);
        }

        // Xóa sạch giỏ, dùng sau khi đặt hàng xong.
        public static void XoaHet()
        {
            if (Phien != null) Phien.Remove(Khoa);
        }

        // Tổng số món, hiện trên header.
        public static int SoMon()
        {
            int tong = 0;
            foreach (MatHang dong in Doc()) tong = tong + dong.SoLuong;
            return tong;
        }

        // Tổng tiền của cả giỏ.
        public static decimal TongTien()
        {
            decimal tong = 0;
            foreach (MatHang dong in Doc()) tong = tong + dong.ThanhTien;
            return tong;
        }
    }
}
