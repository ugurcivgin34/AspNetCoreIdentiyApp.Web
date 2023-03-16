using AspNetCoreIdentiyApp.Web.Extensions;
using AspNetCoreIdentiyApp.Web.Models.Entity;
using AspNetCoreIdentiyApp.Web.Models.Enum;
using AspNetCoreIdentiyApp.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace AspNetCoreIdentiyApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {

        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var userViewModel = new UserViewModel
            {
                Email = currentUser.Email,
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl=currentUser.Picture
            };

            return View(userViewModel);
        }
        public IActionResult PasswordChange()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        {
            // Form modeli geçersizse (yani, kullanıcı modeli doğru bir şekilde doldurmadıysa), tekrar formu görüntüler.
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Kullanıcının kimliği, authorize özniteliği sınıfına sahip olduğundan dolu olarak gelecektir.
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            // Kullanıcının eski şifresi doğru değilse, bir hata mesajı gösterir ve formu tekrar görüntüler.
            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, request.PasswordOld);
            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski şifreniz yanlış");
                return View();
            }

            // Kullanıcının şifresi değiştirilir ve işlem başarısız olursa hata mesajları gösterilir ve form tekrar görüntülenir.
            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.PasswordOld, request.PasswordNew);
            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors);
                return View();
            }

            // Kullanıcının güvenlik damgası güncellenir ve oturum kapatılır.
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();

            // Kullanıcı oturumu, yeni şifresiyle açılır.
            //Bu kısım yapılmazsa şifre değiştirme işlemi tamamlanmış olsa bile, kullanıcının oturumunu yeni şifresiyle açmak için bu işlem yapılmalıdır. Aksi takdirde, kullanıcı şifresini değiştirmiş olmasına rağmen, oturumunu açamaz ve siteye erişemez.
            await _signInManager.PasswordSignInAsync(currentUser, request.PasswordNew, true, false);

            // Başarılı bir şekilde şifre değiştirildiğine dair bir mesaj gösterilir ve form görüntülenir.
            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir";
            return View();
        }
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };

            return View(userEditViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            currentUser.UserName = request.UserName;
            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.BirthDate = request.BirthDate;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;

            if (request.Picture != null && request.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";

                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);

            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }

            //username ve emil gibi kritik kısımları güncellendiği için SecurityStamp ı da güncellemek gerekiyor
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, true);

            //if (request.BirthDate.HasValue)
            //{
            //    await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("birthdate", currentUser.BirthDate!.Value.ToString()) });
            //}

            //else
            //{
            //    await _signInManager.SignInAsync(currentUser, true);
            //}


            TempData["SuccessMessage"] = "Üye bilgileri başarıyla değiştirilmiştir";

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };

            return View(userEditViewModel);
        }



    }
}
