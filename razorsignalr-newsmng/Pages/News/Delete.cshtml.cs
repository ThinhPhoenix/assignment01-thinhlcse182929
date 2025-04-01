using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using newsmng_bussinessobject;
using newsmng_repository;
using razorsignalr_newsmng.Hubs;

namespace razorsignalr_newsmng.Pages.News
{
    public class DeleteModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly IHubContext<NewsHub> _hubContext;

        public DeleteModel(INewsArticleRepository newsArticleRepository, IHubContext<NewsHub> hubContext)
        {
            _newsArticleRepository = newsArticleRepository;
            _hubContext = hubContext;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            CurrentUser = HttpContext.Session.GetObject<SystemAccount>("user");
            if (CurrentUser.AccountRole != 1)
            {
                return RedirectToPage("/unauthorized");
            }

            if (id == null)
            {
                return NotFound();
            }

            var newsarticle = _newsArticleRepository.GetOne(id);

            if (newsarticle == null)
            {
                return NotFound();
            }
            else
            {
                NewsArticle = newsarticle;
            }
            // Check if the current user is the author of this news article
            if (newsarticle.CreatedById != CurrentUser.AccountId)
            {
                return RedirectToPage("/unauthorized");
            }
            return Page();
        }
        [BindProperty]
        public SystemAccount CurrentUser { get; set; }
        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _newsArticleRepository.Delete(id);
            await _hubContext.Clients.All.SendAsync("Change");

            return RedirectToPage("./Index");
        }
    }
}
