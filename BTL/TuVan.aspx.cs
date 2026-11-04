using System;
using System.Web.UI.WebControls;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class TuVan : ShopPage
    {
        // Xử lý khi trang được tải.
        protected void Page_Load(object nguonSuKien, EventArgs e)
        {
            if (!IsPostBack)
            {
                string chuDe = Request.QueryString["topic"];
                ListItem phanTu = ddlTopic.Items.FindByValue(chuDe);
                if (phanTu != null)
                    ddlTopic.SelectedValue = phanTu.Value;
            }
        }

        // Xử lý khi người dùng bấm nút.
        protected void btnSubmitConsult_Click(object nguonSuKien, EventArgs e)
        {
            if (!Page.IsValid) return;
            string soDienThoai = txtPhone.Text.Trim();
            if (!OrderValidator.IsValidPhone(soDienThoai))
            {
                Toast.Info(this, "Số điện thoại phải có từ 9 đến 11 chữ số!");
                return;
            }

            string topic = ddlTopic.SelectedItem == null ? "" : ddlTopic.SelectedItem.Text;
            string ten = txtName.Text.Trim();
            txtName.Text = txtPhone.Text = txtContent.Text = "";
            Toast.Success(this, "Cảm ơn " + ten + "! Yêu cầu '" + topic + "' đã được gửi đi. Đội ngũ SneakerX sẽ liên hệ lại trong thời gian sớm nhất.");
        }
    }
}
