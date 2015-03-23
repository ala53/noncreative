using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NonCreative.Error_Handlers
{
    public partial class ErrorHandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            ErrorMessage.InnerText = ex.Message;
            Details.InnerText = ex.StackTrace + "\n" + ex.ToString();
        }
    }
}