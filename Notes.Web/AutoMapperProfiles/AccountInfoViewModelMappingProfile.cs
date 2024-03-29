﻿using AutoMapper;
using Notes.BLL.Services.AccountInfoManagers.Models;
using Notes.Web.Models.Account;

namespace Notes.Web.AutoMapperProfiles
{
    public class AccountInfoViewModelMappingProfile : Profile
    {
        public AccountInfoViewModelMappingProfile()
        {
            CreateMap<AccountInfo, AccountInfoViewModel>();
            CreateMap<AccountInfoViewModel, AccountInfo>();
        }
    }
}
