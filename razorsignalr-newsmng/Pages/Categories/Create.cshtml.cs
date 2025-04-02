using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using newsmng_bussinessobject;
using newsmng_repository;

namespace razorsignalr_newsmng.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryRepository _context;

        public CreateModel(ICategoryRepository context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ParentCategoryId"] = new SelectList(_context.GetSelectList(), "Value", "Text");
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["ParentCategoryId"] = new SelectList(_context.GetSelectList(), "Value", "Text");
                return Page();
            }

            _context.Add(Category);

            return RedirectToPage("./Index");
        }
    }
}
