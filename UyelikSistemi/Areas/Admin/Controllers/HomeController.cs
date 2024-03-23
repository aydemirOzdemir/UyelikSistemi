using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UyelikSistemi.Areas.Admin.Models;
using UyelikSistemi.Models;

namespace UyelikSistemi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult >Userlist()
        {
            List<AppUser> userList= await userManager.Users.ToListAsync();
       
           var userViewModels = userList.Select(x=>new UserViewModel
            {
                UserId = x.Id,
                UserEmail=x.Email,
                UserName=x.UserName
            });
            return View(userViewModels);
        }
    }
}
