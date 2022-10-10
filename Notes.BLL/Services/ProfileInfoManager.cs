using Microsoft.AspNetCore.Identity;
using Notes.BLL.Interfaces;
using Notes.BLL.Models;
using Notes.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Services
{
    public class AccountInfoManager : IAccountInfoManager
    {
        private readonly UserManager<UserEntry> _userManager;
        private readonly INotesManager _notesManager;

        public AccountInfoManager(UserManager<UserEntry> userManager, INotesManager notesManager)
        {
            _userManager = userManager;
            _notesManager = notesManager;
        }

        public AccountInfo GetAccountInfo(string UserName)
        {
            var accountInfo = new AccountInfo()
            {
                Email = _userManager.Users.FirstOrDefault(u => u.UserName == UserName).Email,
                NotesCount = _notesManager.GetAllFor(UserName).Count(),
            };

            return accountInfo;
        }
    }
}
