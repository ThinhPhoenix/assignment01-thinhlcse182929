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

        [BindProperty(SupportsGet = true)]
        public string SearchNewsTitle { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchNewsContent { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            CurrentUser = HttpContext.Session.GetObject<SystemAccount>("user") ?? new SystemAccount { AccountName = "Guest" };

            // Initialize CurrentUser to avoid null reference exceptions
            CurrentUser = new SystemAccount { AccountName = "Guest" };

            var pageQuery = HttpContext.Request.Query["page"];
            CurrentPage = 1;

            if (!string.IsNullOrEmpty(pageQuery) && int.TryParse(pageQuery, out int parsedPage))
            {
                CurrentPage = parsedPage;
            }

            var user = HttpContext.Session.GetObject<SystemAccount>("user");
            if (user != null)
            {
                CurrentUser = user;
                if(user.AccountRole == 1)
                {
                    dynamic pageable = _newsArticleRepository.Pageable(_newsArticleRepository.Search(SearchNewsTitle, SearchNewsContent), CurrentPage, 3);
                    TotalPages = pageable.TotalPages;
                    NewsArticle = pageable.Data;
                }
                else
                {
                    dynamic pageable = _newsArticleRepository.Pageable(_newsArticleRepository.SearchTrue(SearchNewsTitle, SearchNewsContent), CurrentPage, 3);
                    TotalPages = pageable.TotalPages;
                    NewsArticle = pageable.Data;
                }
            }
        }
    }
}