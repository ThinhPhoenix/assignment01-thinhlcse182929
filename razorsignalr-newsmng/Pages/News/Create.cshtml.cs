using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ICategoryRepository categoryRepository, ISystemAccountRepository systemAccountRepository, INewsArticleRepository newsArticleRepository, IHubContext<NewsHub> hubContext, ILogger<CreateModel> logger)
        {
            _categoryRepository = categoryRepository;
            _systemAccountRepository = systemAccountRepository;
            _newsArticleRepository = newsArticleRepository;
            _hubContext = hubContext;
            _logger = logger;
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

            // Initialize NewsArticle if it's null
            NewsArticle = NewsArticle ?? new NewsArticle();

            // Set CreatedById to CurrentUser.AccountId
            NewsArticle.CreatedById = CurrentUser.AccountId;
            NewsArticle.CreatedDate = DateTime.Now;

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

            // Log the CurrentUser.AccountId to verify its value
            _logger.LogInformation($"CurrentUser.AccountId: {CurrentUser.AccountId}");

            try
            {
                _newsArticleRepository.Add(NewsArticle);
                await _hubContext.Clients.All.SendAsync("Change");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the news article.");
                // Optionally, you can add a user-friendly error message to the ModelState
                ModelState.AddModelError(string.Empty, "An error occurred while saving the news article. Please try again.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}