using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ShoeStore
{
    // Mọi câu SQL trong project đi qua đây.
    // Mở kết nối, gắn tham số, đóng kết nối - không có tầng nào ở giữa.
    public static class KetNoi
    {
        // Mở kết nối tới SQL Server bằng chuỗi trong Web.config.
        public static SqlConnection Mo()
        {
            string chuoi = ConfigurationManager.ConnectionStrings["ShoeStoreDb"].ConnectionString;
            SqlConnection ketNoi = new SqlConnection(chuoi);
            ketNoi.Open();
            return ketNoi;
        }

        // Chạy câu SELECT, trả về DataTable để gán vào Repeater hoặc GridView.
        public static DataTable Bang(string sql, params SqlParameter[] thamSo)
        {
            using (SqlConnection ketNoi = Mo())
            using (SqlCommand lenh = new SqlCommand(sql, ketNoi))
            {
                lenh.Parameters.AddRange(thamSo);

                using (SqlDataAdapter boChuyen = new SqlDataAdapter(lenh))
                {
                    DataTable bang = new DataTable();
                    boChuyen.Fill(bang);
                    return bang;
                }
            }
        }

        // Chạy INSERT / UPDATE / DELETE, trả về số dòng bị tác động.
        public static int ThucThi(string sql, params SqlParameter[] thamSo)
        {
            using (SqlConnection ketNoi = Mo())
            using (SqlCommand lenh = new SqlCommand(sql, ketNoi))
            {
                lenh.Parameters.AddRange(thamSo);
                return lenh.ExecuteNonQuery();
            }
        }

        // Chạy câu trả về một giá trị duy nhất (COUNT, Id vừa thêm...).
        public static object Mot(string sql, params SqlParameter[] thamSo)
        {
            using (SqlConnection ketNoi = Mo())
            using (SqlCommand lenh = new SqlCommand(sql, ketNoi))
            {
                lenh.Parameters.AddRange(thamSo);
                return lenh.ExecuteScalar();
            }
        }

        // Tạo tham số. Giá trị null được đổi thành DBNull cho SQL hiểu.
        public static SqlParameter P(string ten, object giaTri)
        {
            return new SqlParameter(ten, giaTri == null ? DBNull.Value : giaTri);
        }
    }
}
