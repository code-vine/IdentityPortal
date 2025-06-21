using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace IdentityPortal.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<UserWithRoles> UsersWithRoles { get; set; } = new();

        public class UserWithRoles
        {
            public IdentityUser User { get; set; } = default!;
            public List<string> Roles { get; set; } = new();
        }

        public List<string> AllRoles { get; set; } = new();

        public async Task OnGet()
        {
            UsersWithRoles = new List<UserWithRoles>();

            foreach (var user in _userManager.Users.ToList())
            {
                var roles = await _userManager.GetRolesAsync(user);
                UsersWithRoles.Add(new UserWithRoles
                {
                    User = user,
                    Roles = roles.ToList()
                });
            }
            AllRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
        }

        public async Task<IActionResult> OnPostAssignRoleAsync(string userId, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !await _userManager.IsInRoleAsync(user, selectedRole))
            {
                var result = await _userManager.AddToRoleAsync(user, selectedRole);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, $"Failed to assign role {selectedRole}!");
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveRoleAsync(string userId, string roleToRemove)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _userManager.IsInRoleAsync(user, roleToRemove))
            {
                // prevent accidental admin lockout of demo admin account
                if (roleToRemove == "Admin" && user.Email == "admin@demo.com")
                {
                    return RedirectToPage();
                }
                var result = await _userManager.RemoveFromRoleAsync(user, roleToRemove);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, $"Failed to remove role {roleToRemove}!");
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Failed to delete user!");
                }
            }
            return RedirectToPage();
        }
    }
}
