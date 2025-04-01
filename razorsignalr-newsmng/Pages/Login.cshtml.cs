using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using newsmng_bussinessobject;
using newsmng_repository;

namespace razorsignalr_newsmng.Pages
{
    public class LoginModel : PageModel
    {
        ISystemAccountRepository _systemAccountRepository;

        public LoginModel(ISystemAccountRepository systemAccountRepository)
        {
            _systemAccountRepository = systemAccountRepository;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            var email = Request.Form["email"];
            var password = Request.Form["password"];
            var user = _systemAccountRepository.GetAccount(email, password);
            int[] newsAllows = { 1, 2 };
            int[] adminAllows = { 0 };
            if (user != null && user.AccountRole.HasValue && newsAllows.Contains(user.AccountRole.Value))
            {
                HttpContext.Session.SetObject<SystemAccount>("user", user);
                Response.Redirect("/news");
            }
            else if (user != null && user.AccountRole.HasValue && adminAllows.Contains(user.AccountRole.Value))
            {
                HttpContext.Session.SetObject<SystemAccount>("user", user);
                Response.Redirect("/accounts");
            }
        }
    }
}