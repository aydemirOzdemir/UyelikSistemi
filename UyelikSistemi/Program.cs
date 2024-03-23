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



// bu yap�yla beraber bir s�n�f�n constructor�nda �fileprovider tan�mlay�p istedi�im klas�re bu interface ile ula�abilirim. Hangi klas�re gidece�imi belirlemek i�inde bir referans noktas� vermem gerek bunuda �uan �al��t���m klas�r� veriyorum.
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
        opt.AddRequirements(new ExchangeExpirementRequirement());// burada belirlemi� oldu�umuz ri�uirment i�lemindeki seyleri g�nderiyorum

    });
       options.AddPolicy("ViolencePolicy", opt =>
       {
           opt.AddRequirements(new ViolenceRequirement() { ThresholdAge=18});// burada belirlemi� oldu�umuz ri�uirment i�lemindeki seyleri g�nderiyorum

       });
    options.AddPolicy("OrderReadCreatePolicy", opt =>
     {
                opt.RequireClaim("Permission",Permission.Stock.Read);
         opt.RequireClaim("Permission",  Permission.Order.Create);
         opt.RequireClaim("Permission", Permission.Order.Read);

     });// her bir i�lemde ihtiyac�n olacak  claime g�re policy olu�turabilirsin.Her bir istedi�ini Ayr� Ayr� Claime ekle yoksa tek birisine sahipse kurtat�r durumu.








});//Policy tan�mlamak i�in �nce addAuthorization yap�yoruz.Bunun i�ine opt olarak addpolicy y�kl�yoruz. policynin i�ine policynin i�ine policynin ismini veriyoruz (AnkaraPolicy gibi) daha sonra bu policyi claim olarak eklemek i�in yine bir option olarak a��p requireClaim olarak �nce claimin ismini sonra izinli kullan�c� giri�lerini tan�ml�yoruz.Bir policynin i�ine birden fazla kural yaz�labilir.
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
   // opt.User.AllowedUserNameCharacters = "asdfghjkl�i�po�uytrewqzxcvbnm��";

    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 6;
    opt.Password.RequireDigit = false;

    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    // Burada Lokout oldukdan sonra kullan�c� ne kadar s�re boyuunca ayn� email veya �ifreyle siteye giri� yapamaca��n� bellirliyoruz.
    opt.Lockout.MaxFailedAccessAttempts = 3;
    // Kullan�c� �ifre veya emailini 3 den fazla yanl�� girerse ayn� emaille siteye giri� yapmas�na izin vermeyecek lokout s�resi boyunca


}).AddPasswordValidator<PasswordValidation>().AddUserValidator<UserValidation>().AddErrorDescriber<LocalizationIdentityErrorsDescriber>().AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();


// default olarak token �retmesini istedim bana kullan�c�n�n �ifresini unuttu�unda emailine link g�ndermek i�in
// custom password,user,ve hatalar�n dil se�ene�ini yapt���m�z yerleri program cs.e s�yl�yoruz.

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
{
    opt.TokenLifespan = TimeSpan.FromHours(2);
});
// Burada olu�an token�n �mr�n� belirliyoruz 2 saatten sonra linke ula��ld���nda i�lem yap�lamas�n.

builder.Services.Configure<SecurityStampValidatorOptions>(opt =>
{
    opt.ValidationInterval = TimeSpan.FromHours(1);
});
//Burada Security Stamp de�erlerini kar��la�t�r�r ve bu de�erlerde bir de�i�iklik varsa kullan�c�y� login ekran�na tekrar atar.Bu kontrol s�resini burada 1 saat olarak ayarlad�k her bir saatte bir kontrol edecek. security stamp de�erinin �nemi �rne�in bir web ve mobil uygulamanda kullan�c�ya ait kritik bilgileri g�ncelledin ve bu sayede yeni bir security stamp de�eri olu�turdun. Ama di�er uygulamada hala eski veriler var onun i�in uygulama bu sefer di�er ekranda da zorla tekrar login i�lemi yapmay� sa�lar 1 saatin sonunda. Kullan�c� g�ncelleme kodalar� yaz�ld���nda bu security stampi de g�ncelle.


// Olu�an Cookielerin Optimize Edilmesi i�lemi
builder.Services.ConfigureApplicationCookie(opt=>
{  var cookieBuilder=new CookieBuilder();
    cookieBuilder.Name = "UdemyAppCookie";//Cookie Builder olu�turduk ve ismini verdik.
    opt.Cookie = cookieBuilder;//Cookieye atad�k bunu
    opt.LoginPath = new PathString("/Home/Sigin");
    //Kullan�c� Login yapmadan bir sayfaya ula�mak istedi�inde bunu login sayfas�na atacak direk.
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);
    //Bir kullan�c� giri� yapt���nda 60 g�n boyunca bir cookieyi taray�c�da tutacak ve giri� sayfas�na atmadan i�lemleri yapma hakk� verecek e�er 60 g�n boyunca hi� giri� yapmazsa siteye cookie yok olacak ve tekrar giri� yaparak yeni cookie olu�turacak.
    opt.SlidingExpiration = true;
    // Bu i�lemde de bir cookie olu�tuktan sonra 60 g�n boyunca kullan�c� bir kere bile giri� yapsa yeni bir 60 g�nl�k seri ba�latarak cookieyi ayakta tutacak b�ylece siteye login ekran�na girmesine gerek kalmayacak

    //accessDenied sayfas�n�n nerede oldu�unu bildiriyorum art�k

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
app.UseAuthentication();// Authentication i�lemlerini yapmak i�in ekledik.

app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
