using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Notes.BLL.Interfaces;
using Notes.BLL.Models;
using Notes.DAL.Models;
using Notes.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Services
{
    public class TagManager : ITagManager
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<UserEntry> _userManager;

        public TagManager(UnitOfWork unitOfWork, IMapper mapper, UserManager<UserEntry> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task AddTagAsync(Tag tag, string userName)
        {
            var entry = _mapper.Map<TagEntry>(tag);

            entry.User = await _userManager.FindByNameAsync(userName);

            _unitOfWork.Tags.Create(entry);
        }

        public IEnumerable<Tag> GetAllTagsFor(string userName)
        {
            var tagsEntry = _unitOfWork.Tags.GetAll()
                .Where(t => t.Name == userName);

            var tags = _mapper.Map<IEnumerable<TagEntry>, List<Tag>>(tagsEntry);

            return tags;
        }
    }
}
