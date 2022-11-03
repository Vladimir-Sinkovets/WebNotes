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
using Notes.DAL.Repositories.Interfaces;

namespace Notes.BLL.Services
{
    public class TagManager : ITagManager
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

        public async Task AddTagAsync(Tag tag, string userName)
        {
            if (_unitOfWork.Tags.GetAll().Any(t => t.Name == tag.Name))
            {
                throw new ArgumentException("Tag with this name already exists");
            }

            var user = await _userManager.FindByNameAsync(userName);
          
            var entry = _mapper.Map<TagEntry>(tag);

            entry.User = user ?? throw new ArgumentException("Wrong user name");

            _unitOfWork.Tags.Create(entry);

            _unitOfWork.SaveChanges();
        }

        public void DeleteTagById(int tagId, string userName)
        {
            IQueryable<TagEntry> tagEntries = _unitOfWork.Tags.GetAll();

            if (tagEntries.Any(tag => tag.Id == tagId) == false)
            {
                throw new ArgumentException("This tag does not exist");
            }

            if (_userManager.FindByNameAsync(userName).Result == null)
            {
                throw new ArgumentException("User with this name doen't exist");
            }

            if (tagEntries.Any(tag => tag.Id == tagId && tag.User.UserName == userName) == false)
            {
                throw new ArgumentException("This user doesn't have access to this tag");
            }

            _unitOfWork.Tags.DeleteById(tagId);

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<Tag> GetAllTagsFor(string userName)
        {
            if (_userManager.FindByNameAsync(userName).Result == null) 
            {
                throw new ArgumentException("User with this name doen't exist");
            }

            var tagsEntry = _unitOfWork.Tags.GetAll()
                .Where(t => t.User.UserName == userName);

            var tags = _mapper.Map<IEnumerable<TagEntry>, List<Tag>>(tagsEntry);

            return tags;
        }

        public Tag GetTagById(int tagId, string userName)
        {
            IQueryable<TagEntry> tagEntries = _unitOfWork.Tags.GetAll();

            if (tagEntries.Any(tag => tag.Id == tagId) == false)
            {
                throw new ArgumentException("This tag does not exist");
            }

            if (_userManager.FindByNameAsync(userName).Result == null)
            {
                throw new ArgumentException("User with this name doen't exist");
            }

            var entry = _unitOfWork.Tags.GetAll()
                .FirstOrDefault(t => t.Id == tagId && t.User.UserName == userName);

            var tag = _mapper.Map<Tag>(entry);

            return tag;
        }
    }
}