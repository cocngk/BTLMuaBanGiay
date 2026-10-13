using System;
using System.Web.UI;

namespace ShoeStore
{
    public partial class TuVan : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSubmitConsult_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string content = txtContent.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(content))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Vui lòng điền đầy đủ thông tin trước khi gửi!');", true);
                return;
            }

            // Xóa trắng form sau khi gửi
            txtName.Text = "";
            txtPhone.Text = "";
            txtContent.Text = "";

            // Thông báo thành công
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Cảm ơn bạn! Yêu cầu tư vấn đã được gửi đi. Đội ngũ SneakerX sẽ liên hệ lại trong thời gian sớm nhất.');", true);
        }
    }
}