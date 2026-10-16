using System;
using System.Web.UI;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class TuVan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSubmitConsult_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string topic = ddlTopic.SelectedItem != null ? ddlTopic.SelectedItem.Text : "";
            string content = txtContent.Text.Trim();

            if (!OrderValidator.IsValidPhone(phone))
            {
                // Phòng client bị bypass: báo lại bằng toast rồi dừng
                Toast.Info(this, "Toast_" + Guid.NewGuid().ToString("N"),
                    "Số điện thoại phải có từ 9 đến 11 chữ số!");
                return;
            }

            // Xóa trắng form sau khi gửi
            txtName.Text = "";
            txtPhone.Text = "";
            txtContent.Text = "";

            // Thông báo thành công, nêu rõ chủ đề đã chọn
            Toast.Success(this, "Toast_" + Guid.NewGuid().ToString("N"),
                "Cảm ơn " + name + "! Yêu cầu '" + topic + "' đã được gửi đi. Đội ngũ SneakerX sẽ liên hệ lại trong thời gian sớm nhất.");
        }
    }
}
