using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Notes.BLL.Interfaces;
using Notes.BLL.Models;
using Notes.DAL.Models;
using Notes.DAL.Repositories;
using System.Collections.Generic;
using System;
using System.Linq;
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
            if (_unitOfWork.Tags.GetAll()
                    .Any(t => t.Name == tag.Name))
                return;

            var entry = _mapper.Map<TagEntry>(tag);

            entry.User = await _userManager.FindByNameAsync(userName);

            _unitOfWork.Tags.Create(entry);

            _unitOfWork.SaveChanges();
        }

        public void DeleteTagById(int tagId, string userName)
        {
            if(_unitOfWork.Tags
                .GetAll()
                .Any(tag => tag.Id == tagId && tag.User.UserName == userName) == false)
            {
                throw new ArgumentException("Tag does not exist");
            }

            _unitOfWork.Tags.Delete(tagId);

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<Tag> GetAllTagsFor(string userName)
        {
            var tagsEntry = _unitOfWork.Tags.GetAll()
                .Where(t => t.User.UserName == userName);

            var tags = _mapper.Map<IEnumerable<TagEntry>, List<Tag>>(tagsEntry);

            return tags;
        }

        public Tag GetTagById(int id, string userName)
        {
            var entry = _unitOfWork.Tags.GetAll()
                .FirstOrDefault(t => t.Id == id && t.User.UserName == userName);

            var tag = _mapper.Map<Tag>(entry);

            return tag;
        }
    }
}