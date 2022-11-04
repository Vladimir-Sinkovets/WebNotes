﻿using Microsoft.AspNetCore.Identity;
using Notes.BLL.Exceptions;
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
        private readonly INoteManager _notesManager;

        public AccountInfoManager(UserManager<UserEntry> userManager, INoteManager notesManager)
        {
            _userManager = userManager;
            _notesManager = notesManager;
        }

        public AccountInfo GetAccountInfo(string UserName)
        {
            UserEntry user = _userManager.Users.FirstOrDefault(u => u.UserName == UserName) 
                ?? throw new NotFoundException("User with this name does not exist");

            var accountInfo = new AccountInfo()
            {
                Email = user.Email,
                NotesCount = _notesManager.GetAllNotesForUser(UserName).Count(),
            };

            return accountInfo;
        }
    }
}
