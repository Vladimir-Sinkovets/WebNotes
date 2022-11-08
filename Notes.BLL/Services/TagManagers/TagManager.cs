using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Notes.DAL.Models;
using Notes.DAL.Repositories;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Notes.DAL.Repositories.Interfaces;
using Notes.BLL.Exceptions;

namespace Notes.BLL.Services.TagManagers
{
    public class TagManager 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<UserEntry> _userManager;

        public TagManager(IUnitOfWork unitOfWork, IMapper mapper, UserManager<UserEntry> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }


    }
}