using AspNetCoreIdentiyApp.Web.Areas.Admin.Models;
using AspNetCoreIdentiyApp.Web.Extensions;
using AspNetCoreIdentiyApp.Web.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AspNetCoreIdentiyApp.Web.Areas.Admin.Controllers
{
    [Authorize(Roles ="admin")] //Admin yetkisine sahip kişiler sadece bu sayfaya girebilecek , extra rolaction yetkisi var ise aşağıdaki metodlara yapabilecek
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();
            return View(roles);
        }

        [Authorize(Roles ="role-action")]
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }
            TempData["SuccessMessage"] = "Rol oluşturulmuştur."; //Hata mesajları view kısmında ModelONly tarafında gözükecektir
            return RedirectToAction(nameof(RolesController.Index));
        }

        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır.");
            }


            return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id, Name = roleToUpdate!.Name! });
        }

        [Authorize(Roles = "role-action")]
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {

            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır.");
            }

            roleToUpdate.Name = request.Name;

            await _roleManager.UpdateAsync(roleToUpdate);


            ViewData["SuccessMessage"] = "Rol bilgisi güncellenmiştir";

            return View();
        }
         
        [Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);

            if (roleToDelete == null)
            {
                throw new Exception("Silinecek rol bulunamamıştır.");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }

            TempData["SuccessMessage"] = "Rol silinmiştir";
            return RedirectToAction(nameof(RolesController.Index));




            //}


            //public async Task<IActionResult> AssignRoleToUser(string id)
            //{
            //    var currentUser = (await _userManager.FindByIdAsync(id))!;
            //    ViewBag.userId = id;
            //    var roles = await _roleManager.Roles.ToListAsync();
            //    var userRoles = await _userManager.GetRolesAsync(currentUser);
            //    var roleViewModelList = new List<AssignRoleToUserViewModel>();

            //    foreach (var role in roles)
            //    {

            //        var assignRoleToUserViewModel = new AssignRoleToUserViewModel() { Id = role.Id, Name = role.Name! };


            //        if (userRoles.Contains(role.Name!))
            //        {
            //            assignRoleToUserViewModel.Exist = true;
            //        }

            //        roleViewModelList.Add(assignRoleToUserViewModel);


            //    }

            //    return View(roleViewModelList);
            //}

            //[HttpPost]
            //public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
            //{

            //    var userToAssignRoles = (await _userManager.FindByIdAsync(userId))!;

            //    foreach (var role in requestList)
            //    {

            //        if (role.Exist)
            //        {
            //            await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);

            //        }
            //        else
            //        {
            //            await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
            //        }

            //    }

            //    return RedirectToAction(nameof(HomeController.UserList), "Home");
            //}


        }
        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            //AssignRoleToUser metodu, birinci parametre olan "id" parametresiyle atama işlemi yapılacak kullanıcının ID'sini belirtir. Metod, UserManager.FindByIdAsync() yöntemiyle bu ID'ye sahip kullanıcıyı bulur.
            var currentUser = (await _userManager.FindByIdAsync(id))!;
            ViewBag.userId = id; /*Sonrasında, ViewBag.userId özelliği atanır.Bu özellik, Post yöntemi tarafından kullanılacak olan kullanıcının ID'sini taşır.*/


            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var roleViewModelList = new List<AssignRoleToUserViewModel>();
            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel() { Id = role.Id, Name = role.Name! };
                if (userRoles.Contains(role.Name!))
                {
                    assignRoleToUserViewModel.Exist = true;
                }
                roleViewModelList.Add(assignRoleToUserViewModel);
            }
            return View(roleViewModelList);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
        {
            var userToAssignRoles = (await _userManager.FindByIdAsync(userId))!;
            foreach (var role in requestList)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }
            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }
    }
}
