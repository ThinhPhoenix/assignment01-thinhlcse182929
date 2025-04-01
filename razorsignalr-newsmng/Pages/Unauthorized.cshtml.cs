using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace razorsignalr_newsmng.Pages
{
    public class UnauthorizedModel : PageModel
    {
        public void OnGet()
        {
        }

        public void OnPostBackToLogin()
        {
            Response.Redirect("/login");
        }

        public void OnPostBackToHome()
        {
            Response.Redirect("/");
        }
    }
}
