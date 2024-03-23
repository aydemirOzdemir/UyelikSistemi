using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace UyelikSistemi.Extensions
{
    public static class ModelStateExtension
    {
        public static void AddModelErrorList(this ModelStateDictionary modelState,List<string> errors)
        {
            foreach (string item in errors)
            {
                modelState.AddModelError(string.Empty,item);
            }
           
        }

    }
}
