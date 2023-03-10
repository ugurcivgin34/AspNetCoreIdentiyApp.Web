using AspNetCoreIdentiyApp.Web.Extensions;
using AspNetCoreIdentiyApp.Web.Models;
using AspNetCoreIdentiyApp.Web.Models.Entity;
using AspNetCoreIdentiyApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AspNetCoreIdentiyApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //Kullanııcı ile ilgili işlemleri yapmak istediğimizde kullanacağımız sınıftır.
        private readonly UserManager<AppUser> _userManager;

        //Kullanıcının giriş yapması,çıkış yapmsı, third part yazılımların kullanılması giri işlevleri bu sınıf sağlar .Kullanıcının cookie oluşturması işlemlerini bu sınıf gerçekleştirir
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }

        //Asenkron programlama var olan threadlerin o metodu çalıştıran threadın blokklanması üzerine kurulu olan bir yaklaşamdır.Multithread ile alakası yoktur
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {


            if (!ModelState.IsValid)
            {
                return View();
            }
            //CreateAsync metodu user ile password ister,passwordu verdğimizde veritabanına hashlemiş şekilde kaydeder.
            var identityResult = await _userManager.CreateAsync(new() { UserName = request.UserName, PhoneNumber = request.Phone, Email = request.Email }, request.PasswordConfirm);

            if (identityResult.Succeeded)
            {
                //TempData yapmamızın sebebi SignUp metodunun get metoduna gittiği zaman uyarı mesajını kaybetmemiz lazım.ViewBagyapınca bu mesajı kaybediyorduk.Bu yüzden kullandı.Aynı zamanda TempData kullnarak bir kere cookie set etmiş oluyoruz.
                TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarılıyla gerçeklemiştir.";
                return RedirectToAction(nameof(HomeController.SignUp)); //veya ("SignUp") şeklinde de yazılabilir.Bu şekilde tip güveniliği sağlamış olduk
            }

            //foreach (IdentityError item in identityResult.Errors)
            //{
            //    ModelState.AddModelError(string.Empty, item.Description);
            //}

            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());

            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        /// <summary>
        /// Metod:Kullanıcı giriş ekranı
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl">Kullanıcı sadece kullanıcılara özel bir sayfaya giriş yapmadan gitmeye kalktığında direk login sayfasına yönlendirecek,giriş yaptıkdan sonra girmeye çalıştığı sayfaya gidecektir.Bunun url bilgisini tutan parametre</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = null)
        {

            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(model.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
                return View();
            }

            var signUnResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, false);

            if (signUnResult.Succeeded) //BAşarılı ise cookie oluşturur
            {
                return Redirect(returnUrl);
            }

            ModelState.AddModelErrorList(new List<string>() { "Email veya şifre yanlış" });

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}