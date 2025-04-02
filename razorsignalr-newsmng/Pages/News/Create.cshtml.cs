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
using newsmng_dao;
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

        public CreateModel(ICategoryRepository categoryRepository,
                          ISystemAccountRepository systemAccountRepository,
                          INewsArticleRepository newsArticleRepository,
                          IHubContext<NewsHub> hubContext,
                          ILogger<CreateModel> logger)
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

        [BindProperty]
        public List<int> SelectedTagIds { get; set; } = new List<int>();

        public List<Tag> AvailableTags { get; set; } = new List<Tag>();

        public IActionResult OnGet()
        {
            // Get user from session
            CurrentUser = HttpContext.Session.GetObject<SystemAccount>("user");
            if (CurrentUser.AccountRole != 1)
            {
                return RedirectToPage("/unauthorized");
            }

            // Load categories for dropdown
            ViewData["CategoryId"] = new SelectList(_categoryRepository.GetSelectList(), "Value", "Text");
            ViewData["CreatedById"] = new SelectList(_systemAccountRepository.GetSelectList(), "Value", "Text");

            // Load all available tags
            AvailableTags = TagDAO.Instance.GetAll();

            // Initialize NewsArticle if it's null
            NewsArticle = NewsArticle ?? new NewsArticle();

            // Set CreatedById to CurrentUser.AccountId
            NewsArticle.CreatedById = CurrentUser.AccountId;
            NewsArticle.CreatedDate = DateTime.Now;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate select lists if validation fails
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetSelectList(), "Value", "Text");
                ViewData["CreatedById"] = new SelectList(_systemAccountRepository.GetSelectList(), "Value", "Text");
                AvailableTags = TagDAO.Instance.GetAll();
                return Page();
            }

            try
            {
                // Process selected tags
                if (SelectedTagIds != null && SelectedTagIds.Count > 0)
                {
                    NewsArticle.Tags = new List<Tag>();
                    foreach (var tagId in SelectedTagIds)
                    {
                        var tag = TagDAO.Instance.GetOne(tagId);
                        if (tag != null)
                        {
                            NewsArticle.Tags.Add(tag);
                        }
                    }
                }

                // Add the news article with its tags
                _newsArticleRepository.Add(NewsArticle);
                await _hubContext.Clients.All.SendAsync("Change");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the news article.");
                ModelState.AddModelError(string.Empty, "An error occurred while saving the news article. Please try again.");

                // Repopulate data for the form
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetSelectList(), "Value", "Text");
                ViewData["CreatedById"] = new SelectList(_systemAccountRepository.GetSelectList(), "Value", "Text");
                AvailableTags = TagDAO.Instance.GetAll();
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}