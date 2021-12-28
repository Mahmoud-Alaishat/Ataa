using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ataa.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Ataa.Areas.Identity.Pages.Account.Manage
{
    public class ShowRecoveryCodesModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        public ShowRecoveryCodesModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }
    
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (RecoveryCodes == null || RecoveryCodes.Length == 0)
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }
            ViewData["fullName"] = user.FirstName + " " + user.LastName;

            return Page();
        }
    }
}
