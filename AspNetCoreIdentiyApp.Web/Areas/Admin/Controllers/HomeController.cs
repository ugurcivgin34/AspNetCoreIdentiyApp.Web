using AspNetCoreIdentiyApp.Web.Areas.Admin.Models;
using AspNetCoreIdentiyApp.Web.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentiyApp.Web.Areas.Admin.Controllers
{
    [Authorize(Roles="admin")]
    //Diğer controllerdeki yani ana controllerdeki Home sınıfı ile çakıştığı için bunun başına bu attribute belirttik.Area olduğunu söylemiş olduk
    [Area("Admin")]
    public class HomeController : Controller
    {

        private readonly UserManager<AppUser> _userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var userList = await _userManager.Users.ToListAsync();
            var userViewModelList = userList.Select(x => new UserViewModel()
            {
                Id = x.Id,
                Email = x.Email,
                Name = x.UserName
            }).ToList();
            return View(userViewModelList);
        }
    }
}
