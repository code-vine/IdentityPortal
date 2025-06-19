using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityPortal.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager  )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<UserWithRoles> UsersWithRoles { get; set; } = new ();  

        public class UserWithRoles
        {
            public IdentityUser User { get; set; } = default!;
            public List<string> Roles { get; set; } = new List<string>();
        }

        public async Task OnGet()
        {
            UsersWithRoles = new List<UserWithRoles>();

            foreach (var user in _userManager.Users.ToList())
            {
                var roles = await _userManager.GetRolesAsync(user);
                UsersWithRoles.Add(new UserWithRoles { User = user,
                    Roles = roles.ToList()
                });
            }
        }
    }
}
