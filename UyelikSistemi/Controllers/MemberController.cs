using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Security.Claims;
using UyelikSistemi.Extensions;
using UyelikSistemi.Models;
using UyelikSistemi.ViewModels;

namespace UyelikSistemi.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly IFileProvider fileProvider;

        public MemberController(SignInManager<AppUser> signInManager,UserManager<AppUser> userManager,IFileProvider fileProvider)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.fileProvider = fileProvider;
        }
        public IActionResult LogOut()
        {
            signInManager.SignOutAsync();


            return RedirectToAction("SignIn","Home");
        }
        public async Task<IActionResult> Index()
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var currentUser = await userManager.FindByNameAsync(User.Identity!.Name);// Giriş İşlemi Yapıldıktan sonra Autherize Olan kullanıcının Şuanki bilgileri üzerinden çekiyoruz.
            MemberViewModel memberViewModel = new MemberViewModel
            {
                Email = currentUser.Email,
                Name = currentUser.UserName,
                Phone =currentUser.PhoneNumber,
                PictureUrl=currentUser.Picture

            };
           
            return View(memberViewModel);
        }

        public IActionResult PasswordChange()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        { 
            if(!ModelState.IsValid) return View();

        //Authorize ile damgalandığı için sadece üye olanlar girebilecek onun içinde bu değerin null olma ihtimali yok.
            AppUser? currentUser = await userManager.FindByNameAsync(User.Identity!.Name!);


            bool checkOldPassword = await userManager.CheckPasswordAsync(currentUser!, request.PasswordOld);
            // Eski şifreyi doğrulasın diye bir kontrol işlemi yapıyoruz burada Bir kullanıcı ve bir şifreyi veriyoruz.

            if(!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty,"Eski şifreniz yanlıştır.");
                return View();
            }
            //Burada Eski şifre ve yeni şifreyi vererek şifrelerin değişmesi işlemini yapıyoruz .
            var result= await userManager.ChangePasswordAsync(currentUser,request.PasswordOld,request.PasswordNew);
            //eğer işlem başarılı değilse Hataları alıp dönüyoruz.
         if(!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());
                return View();
            }

         // Update işleminden sonra security stamp değerinide güncelliyorum hassas bilgiler olduğu için başka bir platformda açıksa çıkış yapsın diye.
         await userManager.UpdateSecurityStampAsync(currentUser);

            // Şifre değiştirdikten sonra kullanıcıyı çıkış yaptırıyorum 
           await signInManager.SignOutAsync();

            // Kullanıcıyı çıkış yaptırdıktan sonra tekrar giriş yaptırıyorum yeni şifreyle
            await signInManager.PasswordSignInAsync(currentUser,request.PasswordNew,true,false);


            TempData["Message"] = "Şifreniz Başarıyla Değiştirilmiştir..";



            return View();
        }




        public async Task<IActionResult> UserEdit()
        {    // Gender Bilgisinin sayfaya gönderiyoruz.
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

            // Aktif olan kullanıcının bilgilerinin view modelle sayfaya tasıyoruz. Önce aktif olan kullanıcıyı buluyoruz sonra taşıyoruz.
            var currentUser = await userManager.FindByNameAsync(User.Identity!.Name!);

            UserEditViewModel userEditViewModel = new UserEditViewModel
            { UserName= currentUser.UserName,
               Email= currentUser.Email,
               Phone= currentUser.PhoneNumber,
               City= currentUser.City,
               BirthDay=currentUser.BirthDay,
                Gender=currentUser.Gender,
             
            };

            return View(userEditViewModel);
        }
       [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {


            if (!ModelState.IsValid)  return View();
           // o an giriş yapmış olan kullanıcıyı çağırıyoruz ve bilgilerini tek tek güncelliyoruz.
            var currentUser = await userManager.FindByNameAsync(User.Identity!.Name!);
            currentUser!.UserName= request.UserName;
            currentUser.Email= request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.City= request.City; 
            currentUser.BirthDay= request.BirthDay;
            currentUser.Gender= request.Gender;
             // kullanıcının bilgilerini update ediyoruz.
         

            if(request.Picture!=null && request.Picture.Length > 0)
            { //bir folder tanımladık wwwroot olan bunu file provider ile aldık.
                var wwwrootFolder = fileProvider.GetDirectoryContents("wwwroot");

                //Burada Dosyanın ismini belirliyoruz.bu şeklde bir random belirleyip üstüste binmelerinin önüne geçiyoruz dosyaların.
                var randomFileName = $" {Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";//.jpg.png
                // aldığım isimle artık dosyamı buluşturmam gerekiyor.wwwrootFolderın userpicturesın fiziksel pathini alıyorum fiziksel adresini alıyorum
                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);

                // artık yeni resmin pathi var ve kaydetme işine geçiyorum.

                using var stream = new FileStream(newPicturePath,FileMode.Create);
                //requestten gelen dosyayı artık streame kopyalıyorum.
                await request.Picture.CopyToAsync(stream);
                //dosyanın yolunu veritabanına yazdırıyoruz dosyanın ismiyle değil ama çünkü dosyanın ismini değiştirisek veritabanında da değiştirmek gerekebilir.
                currentUser.Picture = randomFileName;
            // son olarak dosyanın ismini artık picture ismini veriyoruz ve veri tabanında dosyanın ismini kaydediyoruz.
            }
            // Güncelleme işlemini yapıyoruz.
          var updateToUserResult =await userManager.UpdateAsync(currentUser);
            // gelen Identityresultı succeed değilse hataları gönderiyoruz.
            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors.Select(x=>x.Description).ToList());
            }
            // Olan güncellemeden sonra artık security stampi güncelliyoruz.
            await userManager.UpdateSecurityStampAsync(currentUser);
            // Çıkış yaptırıyoruz
            await signInManager.SignOutAsync();
            if (!request.BirthDay.HasValue)
            {
                await signInManager.SignInAsync(currentUser, true);
               
            }
            else
            await signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("birthdate", currentUser.BirthDay.HasValue.ToString()) });

            TempData["Message"] = "Hesabınız başarıyla güncellenmiştir...";
            // daha sonra update işlemi yapılmış currentuserı usereditviewmodele dönüştürüp gönderiyoruz.
            UserEditViewModel userEditViewModel = new UserEditViewModel
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                City = currentUser.City,
                BirthDay = currentUser.BirthDay,
                Gender = currentUser.Gender,
                
            };
            //ardından oluşan viewmodeli viewa tekrar göndereceğiz.
            return View(userEditViewModel);
        }

        public async Task<IActionResult> AccessDenied(string returnUrl)
        {
            return View();
        }
        [Authorize(Policy ="AnkaraPolicy")]
        public IActionResult AnkaraPolicy()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        public IActionResult ExchangePolicy()
        {
            return View();
        }

    }
}
