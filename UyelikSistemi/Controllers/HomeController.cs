using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UyelikSistemi.Models;
using UyelikSistemi.ViewModels;
using UyelikSistemi.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace UyelikSistemi.Controllers
{
    
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public HomeController(ILogger<HomeController> logger,UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
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
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {


            if (!ModelState.IsValid)
            {
                return View();
            }
            IdentityResult identityResult= await userManager.CreateAsync(new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
               PhoneNumber=request.Phone
            },request.Password!);


            if (identityResult.Succeeded)
            {

                var exchangeClaim = new Claim("ExchangeExpireDate",DateTime.Now.AddDays(10).ToString());//key value tipli bir claim oluşturduk bu claime değer verdik 
                var user = await userManager.FindByNameAsync(request.UserName); //claim i hangi usera atayacağımızı bulmak için kullandık
                await userManager.AddClaimAsync(user,exchangeClaim);// yarattığımız yeni claimi user bilgisi ile usera ekledik.
                TempData["Message"] = "Üyelik işlemi başarıyla gerçekleşmiştir.";

                return RedirectToAction(nameof(HomeController.SignUp));
            }


            ModelState.AddModelErrorList(identityResult.Errors.Select(x=>x.Description).ToList());
         



            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request,string returnUrl=null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl =returnUrl ?? Url.Action("Index","Home");
            var hasUser=await userManager.FindByEmailAsync(request.Email!);//Cookie oluşturur başarılı ise.

            if (hasUser==null)
            {
                ModelState.AddModelError(string.Empty,"Email veya Şifre yanliştır");
                return View();
            }
            var result = await signInManager.PasswordSignInAsync(hasUser,request.Password!,request.RememberMe,true);

            if (result.Succeeded)
            {

                if (hasUser.BirthDay.HasValue)
                {
                    await signInManager.SignInWithClaimsAsync(hasUser, request.RememberMe, new[] { new Claim("birthdate",hasUser.BirthDay.HasValue.ToString()) });
                }
                
                return Redirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty,"3 dk boyunca giriş yapamazsınız.");
                return View();
            }
            ModelState.AddModelErrorList(new List<string>() { "Email veya şifreniz yanlış",$"Başarısız giriş sayısı:{ await userManager.GetAccessFailedCountAsync(hasUser)}"});

          

            return View();

     
        }



        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task< IActionResult> ResetPassword(ResetPasswordViewModel request)
        { if (!ModelState.IsValid) 
            { 
                return View();
            }
            var hasUser = await userManager.FindByEmailAsync(request.Email!);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty,"Kullanıcıya ait email bulunamamıştır");
                return View();
            }

            string passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(hasUser);
            //Burada bir token password resetlemek için usermanager üzerinden bir token oluşturuyoruz.Bu metotun kullanılabilmesi için içeriye bir user veriyoruz.
            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userıd = hasUser.Id, Token = passwordResetToken });
            // Burada kullanıcıya giden emailde tıklacayacağı linki üretiyoruz.


            //Email service


            TempData["Message"] = "Şifre yenileme linki, e posta adresine gönderilmiştir.";
            return RedirectToAction(nameof(ResetPassword));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}