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

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short id)
        {
            var user = HttpContext.Session.GetObject<SystemAccount>("user");
            if (user == null){
                Response.Redirect("/login");
            }
            else if(user.AccountRole != 0){
                if (user.AccountId != id){
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
