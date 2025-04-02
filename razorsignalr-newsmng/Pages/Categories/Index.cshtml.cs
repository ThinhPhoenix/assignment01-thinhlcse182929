using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;
using newsmng_repository;

namespace razorsignalr_newsmng.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryRepository _context;

        public IndexModel(ICategoryRepository context)
        {
            _context = context;
        }

        public IList<Category> Category { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Category = _context.GetAll();
        }
    }
}
