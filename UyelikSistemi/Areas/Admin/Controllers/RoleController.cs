using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UyelikSistemi.Areas.Admin.Models;
using UyelikSistemi.Extensions;
using UyelikSistemi.Models;

namespace UyelikSistemi.Areas.Admin.Controllers
{         // Bir Role controller oluştur onu area olarak damgala daha sonra authorize olarak damgala
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<AppUser>     userManager;//kullanıcı ile ilgili işlemler
        private readonly SignInManager<AppUser> signInManager;//kullanıcı giriş ve çıkış işlemleri aynı zamanda cookie oluşturma 
        private readonly RoleManager<AppRole> roleManager;//rollerle ilgili işlemler

        public RoleController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<AppRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }
        // roller için bir index sayfası oluştur
        [Authorize(Roles = "admin,roleaction")]
        public async Task<IActionResult> Index()
        {
          List<RoleViewModel> roles= await roleManager.Roles.Select(x=>new RoleViewModel
            {
                Id=x.Id,
                Name=x.Name
            }).ToListAsync();

            return View(roles);
        }
        // rolleri ekleme sayfası oluştur.
        [Authorize(Roles ="admin,roleaction")]
        public IActionResult RoleAdd()
        {
            return View();
        }
        [Authorize(Roles = "admin,roleaction")]
        [HttpPost]
        public async Task<IActionResult> RoleAdd(RoleAddViewModel request)
        {
            var result = await roleManager.CreateAsync(new AppRole
            {
                Name= request.Name,
            });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors.Select(x=>x.Description).ToList());
                return View();
            }
            TempData["Message"] = "Ekleme işlemi başarıyla Tamamlanmıştır.";
            return RedirectToAction(nameof(RoleController.Index));
        }
        [Authorize(Roles = "admin,roleaction")]
        public  async Task< IActionResult> RoleEdit(string id) 
        {
            //Dışardan seçili olan bir id alıyoruz ve güncellenecek olan rolü seçiyoruz.
            // id ile ekrana gelecek olan update edilcek rolu geçiyoruz.
            var roleUpdate = await roleManager.FindByIdAsync(id);
            //ardından gelen approlü view modele çevirip model ekranına gönderiyoruz.
            if (roleUpdate == null)
                throw new Exception("Güncellenecek role bulunamamıştır.");
            UpdateRoleViewModel updateRole = new()
            {   Id=roleUpdate.Id,
                Name=roleUpdate.Name!
            };

            return View(updateRole);

        }
        [Authorize(Roles = "admin,roleaction")]
        [HttpPost]
        public async Task<IActionResult> RoleEdit(UpdateRoleViewModel request)
        {
            var roleToUpdate= await roleManager.FindByIdAsync(request.Id);
            if (roleToUpdate == null)
                throw new Exception("Güncellenecek role bulunamamıştır.");
            roleToUpdate.Name = request.Name;
            var result=await roleManager.UpdateAsync(roleToUpdate);
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors.Select(x=>x.Description).ToList());
            }

            TempData["Message"] = "Güncelleme işlemi başarıyla Tamamlanmıştır.";


            return View();
        }
        [Authorize(Roles = "admin,roleaction")]
        public async Task<IActionResult> RoleRomove(string id)
        { var roleToRemove= await roleManager.FindByIdAsync(id);
            if (roleToRemove == null)
                throw new Exception($"Role {id} does not exist");
            var deleteRole= await roleManager.DeleteAsync(roleToRemove);
            if (!deleteRole.Succeeded)
            {
                ModelState.AddModelErrorList(deleteRole.Errors.Select(x=>x.Description).ToList());
                return View();
            }
            TempData["Message"] = "Silme işlemi başarıyla Tamamlanmıştır.";
            return RedirectToAction(nameof(RoleController.Index));
        }
        


        public async Task<IActionResult> RoleAssignToUser(string id)
        { 
            //Dışardan gelen id ile  userı buluyorum
            var currentUser= await userManager.FindByIdAsync(id);
            //var olan rolleri listeliyorum
            ViewBag.Id = id;
            var roles=await roleManager.Roles.ToListAsync();
            //Rolleri  tutacak bir liste oluşturuyorum.
            var roleViewModelList=new List<AssignRoleToUserViewModel>();
            // Idsini bildiğimiz kullanıcya ait rolleri listeliyorum
            IList<string> userRoles=await userManager.GetRolesAsync(currentUser!);

            foreach (var role in roles)
            {
                var assignRoleToUser=new AssignRoleToUserViewModel() { Id=role.Id,Name=role.Name!};
                //Kullanıcının rolleri içerisinde iterasyonun içerisindeki rol varsa exist propertisinin trueya çekiyorum var diye
                if (userRoles.Contains(role.Name!))
                {
                    assignRoleToUser.Exist = true;
                }
                //artık rol listesine rolleri yükleyip geri dönüyorum.
                roleViewModelList.Add(assignRoleToUser);

            }

            return View(roleViewModelList);
        }
        [HttpPost]
        public async Task<IActionResult> RoleAssignToUser(List<AssignRoleToUserViewModel> request,string id)
        {
            var currentUser=await userManager.FindByIdAsync(id);
            //Gelen roller arasında checkbox işaretlenmiş olan yani exist true olan rolü usera atıyorum işaretlenmemiş olanı userdan kaldırıyorum.
            foreach (var role in request)
            {
                if (role.Exist)
                {
                   await userManager.AddToRoleAsync(currentUser,role.Name);
                }
                else
                {
                    await userManager.RemoveFromRoleAsync(currentUser,role.Name);
                }

            }

            return RedirectToAction("Userlist","Home");
        }


    }
}
