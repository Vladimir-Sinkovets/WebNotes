using Microsoft.AspNetCore.Identity;
using Notes.BLL.Services.AccountInfoManagers.Models;
using Notes.BLL.Services.CurrentUserAccessor;
using Notes.BLL.Services.NoteManagers;
using Notes.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Services.AccountInfoManagers
{
    public class AccountInfoManager : IAccountInfoManager
    {
        private readonly UserManager<UserEntry> _userManager;
        private readonly INoteManager _notesManager;
        private readonly ICurrentUserAccessor _userAccessor;

        public AccountInfoManager(UserManager<UserEntry> userManager, INoteManager notesManager, ICurrentUserAccessor userService)
        {
            _userManager = userManager;
            _notesManager = notesManager;
            this._userAccessor = userService;
        }

        public AccountInfo GetAccountInfo()
        {
            UserEntry user = _userAccessor.Current;

            var accountInfo = new AccountInfo()
            {
                Email = user.Email,
                NotesCount = _notesManager.GetAllNotes().Count(),
            };

            return accountInfo;
        }
    }
}
