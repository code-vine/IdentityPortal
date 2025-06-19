using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityPortal
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardModel : PageModel
    {
        public string GreetingMessage { get; private set; }

        public void OnGet()
        {
            GreetingMessage = $"Welcome back, {User.Identity?.Name}!";

        }
    }

}
