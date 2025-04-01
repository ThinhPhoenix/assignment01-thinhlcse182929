using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using newsmng_bussinessobject;
using newsmng_repository;

namespace razorsignalr_newsmng.Pages.Accounts
{
    public class DeleteModel : PageModel
    {
        private readonly ISystemAccountRepository _systemAccountRepository;

        public DeleteModel(ISystemAccountRepository systemAccountRepository)
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
                Response.Redirect("/unauthorized");
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
            else
            {
                SystemAccount = systemaccount;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(short id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemaccount = _systemAccountRepository.GetOne(id);
            if (systemaccount != null)
            {
                SystemAccount = systemaccount;
                _systemAccountRepository.Delete(SystemAccount.AccountId);
            }

            return RedirectToPage("./Index");
        }
    }
}
