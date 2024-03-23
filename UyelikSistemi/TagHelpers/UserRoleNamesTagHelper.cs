using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using UyelikSistemi.Models;

namespace UyelikSistemi.TagHelpers
{
    public class UserRoleNamesTagHelper : TagHelper
    {
        private readonly UserManager<AppUser> userManager;

        public string Id { get; set; }
        public UserRoleNamesTagHelper(UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
        }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var user = await userManager.FindByIdAsync(Id);
            var userRoles= await userManager.GetRolesAsync(user);
            var stringBuilder = new StringBuilder();
            foreach (var item in userRoles.ToList())
            {
                stringBuilder.Append(@$"
                   <span class='badge bg-secondary mx-1'>{item.ToLower()}</span>
                        ");
            }
            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}
