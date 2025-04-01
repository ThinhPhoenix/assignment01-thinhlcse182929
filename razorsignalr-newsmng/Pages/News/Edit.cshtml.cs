using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;
using newsmng_repository;
using razorsignalr_newsmng.Hubs;

namespace razorsignalr_newsmng.Pages.News
{
    public class EditModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHubContext<NewsHub> _hubContext;

        public EditModel(INewsArticleRepository newsArticleRepository, ISystemAccountRepository systemAccountRepository, ICategoryRepository categoryRepository, IHubContext<NewsHub> hubContext)
        {
            _newsArticleRepository = newsArticleRepository;
            _systemAccountRepository = systemAccountRepository;
            _categoryRepository = categoryRepository;
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

            // Check if the current user is the author of this news article
            if (newsarticle.CreatedById != CurrentUser.AccountId)
            {
                return RedirectToPage("/unauthorized");
            }
            NewsArticle = newsarticle;
            ViewData["CategoryId"] = new SelectList(_categoryRepository.GetSelectList(), "Value", "Text");
            ViewData["CreatedById"] = new SelectList(_systemAccountRepository.GetSelectList(), "Value", "Text");
            return Page();
        }
        [BindProperty]
        public SystemAccount CurrentUser { get; set; }
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            CurrentUser = HttpContext.Session.GetObject<SystemAccount>("user");
    
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_categoryRepository.GetSelectList(), "Value", "Text");
                ViewData["CreatedById"] = new SelectList(_systemAccountRepository.GetSelectList(), "Value", "Text");
                return Page();
            }
    
            // Update modification information
            NewsArticle.ModifiedDate = DateTime.Now;
            NewsArticle.UpdatedById = CurrentUser.AccountId;
    
            _newsArticleRepository.Update(NewsArticle);
            await _hubContext.Clients.All.SendAsync("Change");

            return RedirectToPage("./Index");
        }
    }
}
