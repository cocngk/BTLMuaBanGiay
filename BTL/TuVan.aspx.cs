using System;
using System.Web.UI.WebControls;
using ShoeStore.Helpers;
using ShoeStore.Models;

namespace ShoeStore
{
    public partial class TuVan : ShopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ListItem item = ddlTopic.Items.FindByValue(Request.QueryString["topic"]);
                if (item != null) ddlTopic.SelectedValue = item.Value;
            }
        }

        protected void btnSubmitConsult_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            string phone = txtPhone.Text.Trim();
            if (!OrderValidator.IsValidPhone(phone))
            {
                Toast.Info(this, "Số điện thoại phải có từ 9 đến 11 chữ số!");
                return;
            }
            string topic = ddlTopic.SelectedItem == null ? "" : ddlTopic.SelectedItem.Text;
            string name = txtName.Text.Trim();
            txtName.Text = txtPhone.Text = txtContent.Text = "";
            Toast.Success(this, "Cảm ơn " + name + "! Yêu cầu '" + topic + "' đã được gửi đi. Đội ngũ SneakerX sẽ liên hệ lại trong thời gian sớm nhất.");
        }
    }
}
