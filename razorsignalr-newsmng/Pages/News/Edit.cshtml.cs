using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using newsmng_bussinessobject;
using newsmng_dao;
using newsmng_repository;
using razorsignalr_newsmng.Hubs;

namespace razorsignalr_newsmng.Pages.News
{
    public class EditModel : PageModel
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly IHubContext<NewsHub> _hubContext;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ICategoryRepository categoryRepository,
                        ISystemAccountRepository systemAccountRepository,
                        INewsArticleRepository newsArticleRepository,
                        IHubContext<NewsHub> hubContext,
                        ILogger<EditModel> logger)
        {
            _categoryRepository = categoryRepository;
            _systemAccountRepository = systemAccountRepository;
            _newsArticleRepository = newsArticleRepository;
            _hubContext = hubContext;
            _logger = logger;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;
        
        [BindProperty]
        public List<int> SelectedTagIds { get; set; } = new List<int>();
        
        public List<Tag> AvailableTags { get; set; } = new List<Tag>();
        
        public SystemAccount CurrentUser { get; set; } = new SystemAccount { AccountName = "Guest", AccountRole = 0 };

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            // Get user from session
            CurrentUser = HttpContext.Session.GetObject<SystemAccount>("user");
            if (CurrentUser.AccountRole != 1)
            {
                return RedirectToPage("/unauthorized");
            }

            // Load the news article with its tags
            NewsArticle = _newsArticleRepository.GetOne(id);
            
            if (NewsArticle == null)
            {
                return NotFound();
            }
            
            // Load available tags
            AvailableTags = TagDAO.Instance.GetAll();
            
            // Set selected tag IDs from the article's current tags
            SelectedTagIds = NewsArticle.Tags.Select(t => t.TagId).ToList();
            
            // Load dropdown data
            ViewData["CategoryId"] = new SelectList(_categoryRepository.GetSelectList(), "Value", "Text");
            ViewData["CreatedById"] = new SelectList(_systemAccountRepository.GetSelectList(), "Value", "Text");
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate form data
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetSelectList(), "Value", "Text");
                ViewData["CreatedById"] = new SelectList(_systemAccountRepository.GetSelectList(), "Value", "Text");
                AvailableTags = TagDAO.Instance.GetAll();
                return Page();
            }

            try
            {
                // Set modified information
                NewsArticle.ModifiedDate = DateTime.Now;
                NewsArticle.UpdatedById = CurrentUser.AccountId;
                
                // Handle selected tags
                NewsArticle.Tags = new List<Tag>();
                if (SelectedTagIds != null && SelectedTagIds.Any())
                {
                    foreach (var tagId in SelectedTagIds)
                    {
                        NewsArticle.Tags.Add(new Tag { TagId = tagId });
                    }
                }
                
                // Update the article
                _newsArticleRepository.Update(NewsArticle);
                await _hubContext.Clients.All.SendAsync("Change");
                
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news article");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the news article. Please try again.");
                
                // Repopulate form data
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetSelectList(), "Value", "Text");
                ViewData["CreatedById"] = new SelectList(_systemAccountRepository.GetSelectList(), "Value", "Text");
                AvailableTags = TagDAO.Instance.GetAll();
                return Page();
            }
        }
    }
}