using Microsoft.AspNetCore.Identity;
using Notes.DAL.Models;
using System.Threading.Tasks;

namespace Notes.Web
{
    public static class RoleInitializer
    {
        public static async Task InitializeAcync(RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.FindByNameAsync("defaultUser") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("defaultUser"));
            }
        }
    }
}
