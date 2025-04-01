using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using newsmng_bussinessobject;
using newsmng_repository;
using razorsignalr_newsmng.Hubs;

namespace razorsignalr_newsmng.Pages.News
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly IHubContext<NewsHub> _hubContext;

        public CreateModel(ICategoryRepository categoryRepository, ISystemAccountRepository systemAccountRepository, INewsArticleRepository newsArticleRepository, IHubContext<NewsHub> hubContext)
        {
            _categoryRepository = categoryRepository;
            _systemAccountRepository = systemAccountRepository;
            _newsArticleRepository = newsArticleRepository;
            _hubContext = hubContext;
        }

        public SystemAccount CurrentUser { get; set; } = new SystemAccount { AccountName = "Guest", AccountRole = 0 };

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Get user from session
            CurrentUser = HttpContext.Session.GetObject<SystemAccount>("user");
            if (CurrentUser.AccountRole != 1)
            {
                return RedirectToPage("/unauthorized");
            }
            ViewData["CategoryId"] = new SelectList(_categoryRepository.GetSelectList(), "Value", "Text");
            ViewData["CreatedById"] = new SelectList(_systemAccountRepository.GetSelectList(), "Value", "Text");
            return Page();
        }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate select lists if validation fails
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetSelectList(), "Value", "Text");
                ViewData["CreatedById"] = new SelectList(_systemAccountRepository.GetSelectList(), "Value", "Text");
                return Page();
            }

            _newsArticleRepository.Add(NewsArticle);
            await _hubContext.Clients.All.SendAsync("Change");

            return RedirectToPage("./Index");
        }
    }
}
