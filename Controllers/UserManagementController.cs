using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Filters;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize]
    [CheckUserStatus]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["EmailSort"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["LastLoginSort"] = sortOrder == "LastLogin" ? "lastlogin_desc" : "LastLogin";
            ViewData["StatusSort"] = sortOrder == "Status" ? "status_desc" : "Status";

            var users = _userManager.Users.AsQueryable();

            users = sortOrder switch
            {
                "name_desc" => users.OrderByDescending(u => u.FullName),
                "Email" => users.OrderBy(u => u.Email),
                "email_desc" => users.OrderByDescending(u => u.Email),
                "LastLogin" => users.OrderBy(u => u.LastLoginTime),
                "lastlogin_desc" => users.OrderByDescending(u => u.LastLoginTime),
                "Status" => users.OrderBy(u => u.IsBlocked),
                "status_desc" => users.OrderByDescending(u => u.IsBlocked),
                _ => users.OrderBy(u => u.FullName)
            };

            return View(await users.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> BlockUsers(List<string> userIds)
        {
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.IsBlocked = true;
                    await _userManager.UpdateAsync(user);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUsers(List<string> userIds)
        {
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.IsBlocked = false;
                    await _userManager.UpdateAsync(user);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUsers(List<string> userIds)
        {
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
