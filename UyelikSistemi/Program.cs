using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using UyelikSistemi.ClaimProvider;
using UyelikSistemi.CustomValidation;
using UyelikSistemi.Localization;
using UyelikSistemi.Models;
using UyelikSistemi.Permissions;
using UyelikSistemi.Requirements;
using UyelikSistemi.Seeds;

var builder = WebApplication.CreateBuilder(args);



// bu yapýyla beraber bir sýnýfýn constructorýnda ýfileprovider tanýmlayýp istediðim klasöre bu interface ile ulaþabilirim. Hangi klasöre gideceðimi belirlemek içinde bir referans noktasý vermem gerek bunuda þuan çalýþtýðým klasörü veriyorum.
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));


builder.Services.AddScoped<IClaimsTransformation,UserClaimProvider>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AnkaraPolicy", opt =>
    {
        opt.RequireClaim("city", "Ankara", "Manisa");
        opt.RequireRole("admin");
    });
    options.AddPolicy("ExchangePolicy", opt =>
    {
        opt.AddRequirements(new ExchangeExpirementRequirement());// burada belirlemiþ olduðumuz riðuirment iþlemindeki seyleri gönderiyorum

    });
       options.AddPolicy("ViolencePolicy", opt =>
       {
           opt.AddRequirements(new ViolenceRequirement() { ThresholdAge=18});// burada belirlemiþ olduðumuz riðuirment iþlemindeki seyleri gönderiyorum

       });
    options.AddPolicy("OrderReadCreatePolicy", opt =>
     {
                opt.RequireClaim("Permission",Permission.Stock.Read);
         opt.RequireClaim("Permission",  Permission.Order.Create);
         opt.RequireClaim("Permission", Permission.Order.Read);

     });// her bir iþlemde ihtiyacýn olacak  claime göre policy oluþturabilirsin.Her bir istediðini Ayrý Ayrý Claime ekle yoksa tek birisine sahipse kurtatýr durumu.








});//Policy tanýmlamak için önce addAuthorization yapýyoruz.Bunun içine opt olarak addpolicy yüklüyoruz. policynin içine policynin içine policynin ismini veriyoruz (AnkaraPolicy gibi) daha sonra bu policyi claim olarak eklemek için yine bir option olarak açýp requireClaim olarak önce claimin ismini sonra izinli kullanýcý giriþlerini tanýmlýyoruz.Bir policynin içine birden fazla kural yazýlabilir.
//

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt=>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
});
builder.Services.AddIdentity<AppUser, AppRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;
   // opt.User.AllowedUserNameCharacters = "asdfghjklþiðpoýuytrewqzxcvbnmöç";

    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 6;
    opt.Password.RequireDigit = false;

    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    // Burada Lokout oldukdan sonra kullanýcý ne kadar süre boyuunca ayný email veya þifreyle siteye giriþ yapamacaðýný bellirliyoruz.
    opt.Lockout.MaxFailedAccessAttempts = 3;
    // Kullanýcý þifre veya emailini 3 den fazla yanlýþ girerse ayný emaille siteye giriþ yapmasýna izin vermeyecek lokout süresi boyunca


}).AddPasswordValidator<PasswordValidation>().AddUserValidator<UserValidation>().AddErrorDescriber<LocalizationIdentityErrorsDescriber>().AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();


// default olarak token üretmesini istedim bana kullanýcýnýn þifresini unuttuðunda emailine link göndermek için
// custom password,user,ve hatalarýn dil seçeneðini yaptýðýmýz yerleri program cs.e söylüyoruz.

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromHours(2);
});
// Burada oluþan tokenýn ömrünü belirliyoruz 2 saatten sonra linke ulaþýldýðýnda iþlem yapýlamasýn.

builder.Services.Configure<SecurityStampValidatorOptions>(opt =>
{
    opt.ValidationInterval = TimeSpan.FromHours(1);
});
//Burada Security Stamp deðerlerini karþýlaþtýrýr ve bu deðerlerde bir deðiþiklik varsa kullanýcýyý login ekranýna tekrar atar.Bu kontrol süresini burada 1 saat olarak ayarladýk her bir saatte bir kontrol edecek. security stamp deðerinin önemi örneðin bir web ve mobil uygulamanda kullanýcýya ait kritik bilgileri güncelledin ve bu sayede yeni bir security stamp deðeri oluþturdun. Ama diðer uygulamada hala eski veriler var onun için uygulama bu sefer diðer ekranda da zorla tekrar login iþlemi yapmayý saðlar 1 saatin sonunda. Kullanýcý güncelleme kodalarý yazýldýðýnda bu security stampi de güncelle.


// Oluþan Cookielerin Optimize Edilmesi iþlemi
builder.Services.ConfigureApplicationCookie(opt=>
{  var cookieBuilder=new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";//Cookie Builder oluþturduk ve ismini verdik.
    opt.Cookie = cookieBuilder;//Cookieye atadýk bunu
    opt.LoginPath = new PathString("/Home/Sigin");
    //Kullanýcý Login yapmadan bir sayfaya ulaþmak istediðinde bunu login sayfasýna atacak direk.
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);
    //Bir kullanýcý giriþ yaptýðýnda 60 gün boyunca bir cookieyi tarayýcýda tutacak ve giriþ sayfasýna atmadan iþlemleri yapma hakký verecek eðer 60 gün boyunca hiç giriþ yapmazsa siteye cookie yok olacak ve tekrar giriþ yaparak yeni cookie oluþturacak.
    opt.SlidingExpiration = true;
    // Bu iþlemde de bir cookie oluþtuktan sonra 60 gün boyunca kullanýcý bir kere bile giriþ yapsa yeni bir 60 günlük seri baþlatarak cookieyi ayakta tutacak böylece siteye login ekranýna girmesine gerek kalmayacak

    //accessDenied sayfasýnýn nerede olduðunu bildiriyorum artýk

    opt.AccessDeniedPath = new PathString("/Member/AccessDenied");
});

var app = builder.Build();



using (var scope=app.Services.CreateScope())
{
    var roleManager=scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    await PermissionSeed.Seed(roleManager);
}

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();// Authentication iþlemlerini yapmak için ekledik.

app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
