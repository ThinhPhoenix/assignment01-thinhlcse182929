using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;
using newsmng_repository;
using Newtonsoft.Json;

namespace razorsignalr_newsmng.Pages.News
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ISystemAccountRepository _systemAccountRepository;

        public IndexModel(INewsArticleRepository newsArticleRepository, ISystemAccountRepository systemAccountRepository)
        {
            _newsArticleRepository = newsArticleRepository;
            _systemAccountRepository = systemAccountRepository;
        }

        public IList<NewsArticle> NewsArticle { get; set; } = default!;

        [BindProperty]
        public SystemAccount CurrentUser { get; set; }

        public async Task OnGetAsync()
        {
            CurrentUser = HttpContext.Session.GetObject<SystemAccount>("user") ?? new SystemAccount { AccountName = "Guest" };

            // Initialize CurrentUser to avoid null reference exceptions
            CurrentUser = new SystemAccount { AccountName = "Guest" };

            var user = HttpContext.Session.GetObject<SystemAccount>("user");
            if (user != null)
            {
                CurrentUser = user;
                if(user.AccountRole == 1)
                {
                    NewsArticle = _newsArticleRepository.GetAll();
                }
                else
                {
                    NewsArticle = _newsArticleRepository.GetAllTrue();
                }
            }
        }
    }
}