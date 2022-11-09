using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Notes.BLL.Services.CurrentUserAccessor;
using Notes.BLL.Services.CurrentUserAccessor.Exceptions;
using Notes.DAL.Models;
using System.Linq;

namespace Notes.Web.Services
{
    public class CurrentUserAccessor : ICurrentUserAccessor
    {
        public UserEntry Current { get; set; }

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, UserManager<UserEntry> userManager)
        {
            Current = GetCurrentUser(httpContextAccessor, userManager);
        }

        private static UserEntry GetCurrentUser(IHttpContextAccessor httpContextAccessor, UserManager<UserEntry> userManager)
        {
            string userName = httpContextAccessor.HttpContext.User.Identity.Name;

            var currentUser = userManager.Users.FirstOrDefault(u => u.UserName == userName)
                ?? throw new NotAuthorizedException("User is not authorized");

            return currentUser;
        }
    }
}
