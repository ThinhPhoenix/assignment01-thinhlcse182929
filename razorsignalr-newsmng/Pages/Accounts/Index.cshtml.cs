using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;
using newsmng_repository;

namespace razorsignalr_newsmng.Pages.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly INewsArticleRepository _newsArticleRepository;

        public IndexModel(ISystemAccountRepository systemAccountRepository, INewsArticleRepository newsArticleRepository)
        {
            _systemAccountRepository = systemAccountRepository;
            _newsArticleRepository = newsArticleRepository;
        }

        public IList<SystemAccount> SystemAccount { get; set; } = default!;
        
        // Properties for the dashboard
        public Dictionary<int, int> MonthlyData { get; set; } = new Dictionary<int, int>();
        public int SelectedYear { get; set; }

        public async Task OnGetAsync(int? year)
        {
            var user = HttpContext.Session.GetObject<SystemAccount>("user");
            if (user == null){
                Response.Redirect("/login");
            }
            else if(user.AccountRole != 0){
                Response.Redirect("/unauthorized");
            }

            SystemAccount = _systemAccountRepository.GetAll();
            
            // Set the selected year (default to current year if not specified)
            SelectedYear = year ?? DateTime.Now.Year;
            
            // Get monthly data for the selected year
            MonthlyData = _newsArticleRepository.GetDashboard(SelectedYear);
        }
    }
}