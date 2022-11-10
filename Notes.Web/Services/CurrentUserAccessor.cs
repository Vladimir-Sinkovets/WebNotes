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
        private UserEntry _current;
        public UserEntry Current { get => _current ??= GetCurrentUser(); }

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<UserEntry> _userManager;

        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, UserManager<UserEntry> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        private UserEntry GetCurrentUser()
        {
            string userName = _httpContextAccessor.HttpContext.User.Identity.Name;

            var currentUser = _userManager.Users.FirstOrDefault(u => u.UserName == userName)
                ?? throw new NotAuthorizedException("User is not authorized");

            return currentUser;
        }
    }
}
