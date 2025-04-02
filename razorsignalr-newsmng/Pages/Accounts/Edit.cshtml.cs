using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;
using newsmng_repository;

namespace razorsignalr_newsmng.Pages.Accounts
{
    public class EditModel : PageModel
    {
        private readonly ISystemAccountRepository _systemAccountRepository;

        public EditModel(ISystemAccountRepository systemAccountRepository)
        {
            _systemAccountRepository = systemAccountRepository;
        }

        public SystemAccount CurrentUser { get; set;}
        public bool IsAdmin { get; set; }
        public short EditId { get; set; }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short id)
        {
            EditId = id;
            var user = HttpContext.Session.GetObject<SystemAccount>("user");
            
            if (user == null){
                Response.Redirect("/login");
            }
            else {
                // Set CurrentUser for ALL users (admin and non-admin)
                CurrentUser = user;
                
                // Set IsAdmin flag based on user role
                IsAdmin = user.AccountRole == 0;
                
                // Only non-admin users are restricted from editing other accounts
                if (!IsAdmin && user.AccountId != id){
                    Response.Redirect("/unauthorized");
                }
            }
            
            if (id == null)
            {
                return NotFound();
            }

            var systemaccount = _systemAccountRepository.GetOne(id);
            if (systemaccount == null)
            {
                return NotFound();
            }
            SystemAccount = systemaccount;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _systemAccountRepository.Update(SystemAccount);

            return RedirectToPage("./Index");
        }
    }
}
