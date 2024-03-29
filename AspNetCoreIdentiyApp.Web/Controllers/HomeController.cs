﻿using AspNetCoreIdentiyApp.Web.Extensions;
using AspNetCoreIdentiyApp.Web.Models;
using AspNetCoreIdentiyApp.Web.Models.Entity;
using AspNetCoreIdentiyApp.Web.Services;
using AspNetCoreIdentiyApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace AspNetCoreIdentiyApp.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;


		//Kullanııcı ile ilgili işlemleri yapmak istediğimizde kullanacağımız sınıftır.
		private readonly UserManager<AppUser> _userManager;

		//Kullanıcının giriş yapması,çıkış yapmsı, third part yazılımların kullanılması giri işlevleri bu sınıf sağlar .Kullanıcının cookie oluşturması işlemlerini bu sınıf gerçekleştirir
		private readonly SignInManager<AppUser> _signInManager;

		private readonly IEmailService _emailService;
		public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			_emailService = emailService;
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

			if (!identityResult.Succeeded)
			{
				//foreach (IdentityError item in identityResult.Errors)
				//{
				//    ModelState.AddModelError(string.Empty, item.Description);
				//}

				ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
				return View();

			}

			//Bu kodların genel olarak yaptığı işlem, bir kullanıcıya "ExcahangeExpireDate" adlı bir hak talebi (claim) eklemektir. Bu hak talebi, kullanıcının değişim yapabileceği son tarihi belirtir ve şu anda eklenen hak talebi, mevcut tarihten 10 gün sonraya ayarlanmıştır.
			var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());

			var user = await _userManager.FindByNameAsync(request.UserName);

			var claimResult = await _userManager.AddClaimAsync(user, exchangeExpireClaim);

			if (!claimResult.Succeeded)
			{
				ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
				return View();
			}

			//TempData yapmamızın sebebi SignUp metodunun get metoduna gittiği zaman uyarı mesajını kaybetmemiz lazım.ViewBagyapınca bu mesajı kaybediyorduk.Bu yüzden kullandı.Aynı zamanda TempData kullnarak bir kere cookie set etmiş oluyoruz.
			TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarılıyla gerçeklemiştir.";
			return RedirectToAction(nameof(HomeController.SignIn)); //veya ("SignUp") şeklinde de yazılabilir.Bu şekilde tip güveniliği sağlamış olduk
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

			if (!ModelState.IsValid)
			{
				return View();
			}

			// returnUrl, kullanıcının başarılı giriş yaptıktan sonra yönlendirileceği sayfayı belirtir.
			// Eğer returnUrl değeri null ise varsayılan olarak ana sayfaya yönlendirilir.
			returnUrl = returnUrl ?? Url.Action("Index", "Home");

			// Verilen email adresine sahip kullanıcı var mı diye kontrol edilir.
			// _userManager bir kullanıcı yönetimi nesnesidir ve FindByEmailAsync metodu ile kullanıcıları email adresleri ile arar.
			var hasUser = await _userManager.FindByEmailAsync(model.Email);

			// Eğer böyle bir kullanıcı yoksa, ModelState üzerine bir hata eklenir ve aynı sayfa tekrar gösterilir.
			if (hasUser == null)
			{
				ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
				return View();
			}


			// Kullanıcı varsa, verilen şifreyi kullanarak oturum açılır.En sondaki true ise kitleme içindir.
			// _signInManager, oturum açma işlemlerini yönetir.
			//Remember true olunca kullanıcının tarayıcısında uzun ömürlü bir kimlik bilgisi (cookie) oluşturarak kullanıcının oturum açtığını hatırlayacaktır. Bu sayede, kullanıcı bir sonraki ziyaretinde tekrar oturum açmak zorunda kalmayacak ve oturum bilgileri tarayıcı tarafından hatırlanacaktır.
			var signUnResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, true);

			if (signUnResult.IsLockedOut)
			{
				ModelState.AddModelErrorList(new List<string>() { "3 dakika boyunca giriş yapamazsınız." });
				return View(model);

			}

			if (!signUnResult.Succeeded)
			{
				// Eğer oturum açma başarısız olduysa, ModelState üzerine bir hata eklenir ve aynı sayfa tekrar gösterilir.
				ModelState.AddModelErrorList(new List<string>() { "Email veya şifre yanlış", $"Başarısız giriş sayısı={await _userManager.GetAccessFailedCountAsync(hasUser)}" });
				return View();
			}

			//Bu yöntem daha best pracitce , diğer yöntem ile claimprovider ile yapmıştık, fakat kullanıcı her giriş yaptığında ve her sayfaya gititğinde orası sürekli çalışacak,ama bu yöntemle cookie bir kere yazdığı için sürekli onu kullanmış olacak.
			if (hasUser.BirthDate.HasValue)
			{
				//Kullanıcnın doğum tarihi varsa login olduktan sonra claime de ekleyecek doğum tarihi bilgisini

				await _signInManager.SignInWithClaimsAsync(hasUser, model.RememberMe, new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });
			}
			return Redirect(returnUrl!);



		}




		public IActionResult ForgetPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
		{
			var hasUser = await _userManager.FindByEmailAsync(request.Email);

			if (hasUser == null)
			{
				ModelState.AddModelError(String.Empty, "Bu email adresine sahip kullanıcı bulunamamıştır.");
				return View();
			}

			string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

			//ASP.NET Core uygulamalarında URL'lerin şemasını (HTTP veya HTTPS) almak için kullanılan bir özelliktir.
			var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);
			//örnek link https://localhost:7006?userId=12213&token=aajsdfjdsalkfjkdsfj

			await _emailService.SendResetPasswordEmail(passwordResetLink, hasUser.Email);

			TempData["SuccessMessage"] = "Şifre yenileme linki, eposta adresenize gönderilmiştir";

			return RedirectToAction(nameof(ForgetPassword));
		}


		//Bu kod, "ResetPassword" adlı bir eylem yöntemi tanımlar. Bu eylem, iki dize parametresi alır: "userId" ve "token". Bu iki parametre, HTTP sorgu dizesinden gelen ve .NET'in otomatik olarak eşleştirdiği isimlerdir.

		//TempData, verileri bir eylemden diğerine taşımak için kullanılan bir bellek alanıdır.Bu kod, "userId" ve "token" değerlerini TempData nesnesine kaydeder. Bu değerler, sonraki bir HTTP isteği tarafından erişilebilir hale gelir. Bu değerlerin post işleminde kullanılmak üzere kaydedildiği anlamına gelir.
		public IActionResult ResetPassword(string userId, string token)
		{
			//Aşağıdaki Tempdataları kullanmamızın sebebi bu sayafaya geldği zaman bunları alıp post metodda kullanmak istiyoruz.Bu yüzden Tempdata ları kullandık
			TempData["userId"] = userId;
			TempData["token"] = token;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
		{
			var userId = TempData["userId"];
			var token = TempData["token"];

			if (userId == null || token == null)
			{
				throw new Exception("Bir hata meydana geldi");
			}

			var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);

			if (hasUser == null)
			{
				ModelState.AddModelError(String.Empty, "Kullanıcı bulunamamıştır.");
				return View();
			}

			IdentityResult result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, request.Password);

			if (result.Succeeded)
			{
				TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir";
			}
			else
			{
				ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
			}

			return View();
		}




		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}