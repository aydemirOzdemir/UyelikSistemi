using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UyelikSistemi.Models;

namespace UyelikSistemi.Seeds
{
    public  class PermissionSeed
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            bool hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole { Name="BasicRole"});
                AppRole basicRole = await roleManager.FindByNameAsync("BasicRole");
            await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permissions.Permission.Stock.Read) );
                await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permissions.Permission.Order.Read));
                await roleManager.AddClaimAsync(basicRole, new Claim("Permission", Permissions.Permission.Catalog.Read));
            }



            bool hasAdvanceRole = await roleManager.RoleExistsAsync("AdvanceRole");

            if (!hasAdvanceRole)
            {
                await roleManager.CreateAsync(new AppRole { Name = "AdvanceRole" });
                AppRole advanceRole = await roleManager.FindByNameAsync("AdvanceRole");
                await roleManager.AddClaimAsync(advanceRole, new Claim("Permission", Permissions.Permission.Stock.Read));
                await roleManager.AddClaimAsync(advanceRole, new Claim("Permission", Permissions.Permission.Order.Read));
                await roleManager.AddClaimAsync(advanceRole, new Claim("Permission", Permissions.Permission.Catalog.Read));



                await roleManager.AddClaimAsync(advanceRole, new Claim("Permission", Permissions.Permission.Stock.Update));
                await roleManager.AddClaimAsync(advanceRole, new Claim("Permission", Permissions.Permission.Order.Update));
                await roleManager.AddClaimAsync(advanceRole, new Claim("Permission", Permissions.Permission.Catalog.Update));



                await roleManager.AddClaimAsync(advanceRole, new Claim("Permission", Permissions.Permission.Stock.Create));
                await roleManager.AddClaimAsync(advanceRole, new Claim("Permission", Permissions.Permission.Order.Create));
                await roleManager.AddClaimAsync(advanceRole, new Claim("Permission", Permissions.Permission.Catalog.Create));

            }











            bool hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");

            if (!hasAdminRole)
            {
                await roleManager.CreateAsync(new AppRole { Name = "AdminRole" });
                AppRole adminRole = await roleManager.FindByNameAsync("AdminRole");
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Stock.Read));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Order.Read));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Catalog.Read));



                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Stock.Update));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Order.Update));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Catalog.Update));

                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Stock.Create));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Order.Create));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Catalog.Create));


                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Stock.Delete));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Order.Delete));
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", Permissions.Permission.Catalog.Delete));




            }












        }






    }
    }

