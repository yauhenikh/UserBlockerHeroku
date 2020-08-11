using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CustomIdentityApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CustomIdentityApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            List<User> users = await _userManager.Users.ToListAsync();
            
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Block(List<User> usersInView)
        {
            return await DoBlockingJob(usersInView, true);
        }

        [HttpPost]
        public async Task<IActionResult> Unblock(List<User> usersInView)
        {
            return await DoBlockingJob(usersInView, false);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(List<User> usersInView)
        {
            IEnumerable<User> checkedUsers = usersInView.Where(u => u.Checked == true);

            foreach (User checkedUser in checkedUsers)
            {
                User userInDb = await _userManager.FindByIdAsync(checkedUser.Id);
                await _userManager.DeleteAsync(userInDb);
            }

            User currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task<IActionResult> DoBlockingJob(List<User> usersInView, bool isNeedToBlock)
        {
            IEnumerable<User> checkedUsers = usersInView.Where(u => u.Checked == true);

            foreach (User checkedUser in checkedUsers)
            {
                User userInDb = await _userManager.FindByIdAsync(checkedUser.Id);
                userInDb.IsBlocked = isNeedToBlock;
                await _userManager.UpdateAsync(userInDb);
            }

            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.IsBlocked)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
