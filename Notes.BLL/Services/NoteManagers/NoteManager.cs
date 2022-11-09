using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Notes.BLL.Services.CurrentUserAccessor;
using Notes.BLL.Services.NoteManagers.Exceptions;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.DAL.Models;
using Notes.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.BLL.Services.NoteManagers
{
    public class NoteManager : INoteManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<UserEntry> _userManager;
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _userService;

        public NoteManager(IUnitOfWork unitOfWork, UserManager<UserEntry> userManager, IMapper mapper, ICurrentUserAccessor userService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            this._userService = userService;
        }


        public async Task CreateNewNoteAsync(NoteCreateData note)
        {
            var entry = _mapper.Map<NoteEntry>(note);

            var userName = _userService.Current.UserName;

            var user = await _userManager.FindByNameAsync(userName);

            entry.User = user ?? throw new NotFoundException("User with this name does not exist");

            _unitOfWork.Notes.Create(entry);

            _unitOfWork.SaveChanges();
        }

        public async Task UpdateNoteAsync(NoteUpdateData note)
        {
            //if (_unitOfWork.Notes.GetAll().FirstOrDefault(note.)) // проверить на доступ пользователя

            if (note == null)
                throw new ArgumentNullException(nameof(note));

            var entry = _mapper.Map<NoteEntry>(note);

            _unitOfWork.Notes.Update(entry);

            await _unitOfWork.SaveChangesAsync();
        }

        public Note GetNoteById(int id)
        {
            var userName = _userService.Current.UserName;

            var entry = _unitOfWork.Notes.GetAll()
                .FirstOrDefault(n => n.Id == id && n.User.UserName == userName)
                ?? throw new NotFoundException("There is no such note");

            var note = _mapper.Map<Note>(entry);

            return note;
        }

        public IEnumerable<Note> GetAllNotes()
        {
            var userName = _userService.Current.UserName;

            var noteEntries = _unitOfWork.Notes.GetAll()
                .Where(n => n.User.UserName == userName);

            var notes = _mapper.Map<IEnumerable<NoteEntry>, List<Note>>(noteEntries);

            return notes;
        }

        public void AddTagToNote(int noteId, int tagId)
        {
            var userName = _userService.Current.UserName;

            var tagEntry = _unitOfWork.Tags.GetAll()
                .FirstOrDefault(t => t.Id == tagId && t.User.UserName == userName)
                ?? throw new NotFoundException("There is no such tag");

            var noteEntry = _unitOfWork.Notes.GetAll()
                .FirstOrDefault(n => n.Id == noteId && n.User.UserName == userName)
                ?? throw new NotFoundException("There is no such note");

            noteEntry.Tags
                .Add(tagEntry);

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<Tag> GetNoteTagsById(int noteId)
        {
            var note = GetNoteById(noteId);

            return note.Tags;
        }


        public void RemoveTagFromNote(int noteId, int tagId)
        {
            var userName = _userService.Current.UserName;

            var tagEntity = _unitOfWork.Tags.GetAll()
                .FirstOrDefault(t => t.Id == tagId && t.User.UserName == userName)
                ?? throw new NotFoundException("There is no such tag");

            var noteEntity = _unitOfWork.Notes.GetAll()
                .FirstOrDefault(n => n.Id == noteId && n.User.UserName == userName)
                ?? throw new NotFoundException("There is no such note");

            noteEntity.Tags.Remove(tagEntity);

            _unitOfWork.SaveChanges();
        }

        public void DeleteTagById(int tagId)
        {
            _unitOfWork.Tags.DeleteById(tagId);

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<Tag> GetAllTagsFor()
        {
            var userName = _userService.Current.UserName;

            if (_userManager.FindByNameAsync(userName).Result == null)
            {
                throw new NotFoundException("User with this name does not exist");
            }

            var tagsEntry = _unitOfWork.Tags.GetAll()
                .Where(t => t.User.UserName == userName);

            var tags = _mapper.Map<IEnumerable<TagEntry>, List<Tag>>(tagsEntry);

            return tags;
        }

        public Tag GetTagById(int tagId)
        {
            var userName = _userService.Current.UserName;

            IQueryable<TagEntry> tagEntries = _unitOfWork.Tags.GetAll();

            if (tagEntries.Any(tag => tag.Id == tagId) == false)
            {
                throw new NotFoundException("This tag does not exist");
            }

            if (_userManager.FindByNameAsync(userName).Result == null)
            {
                throw new NotFoundException("User with this name does not exist");
            }

            var entry = _unitOfWork.Tags.GetAll()
                .FirstOrDefault(t => t.Id == tagId && t.User.UserName == userName);

            var tag = _mapper.Map<Tag>(entry);

            return tag;
        }

        public async Task AddTagAsync(TagCreateData data)
        {
            var userName = _userService.Current.UserName;

            if (_unitOfWork.Tags.GetAll()
                    .Any(t => t.Name == data.Name))
                throw new ExistedTagNameException("Cannot add tag with already existing name");

            var entry = _mapper.Map<TagEntry>(data);

            var user = await _userManager.FindByNameAsync(userName);

            entry.User = user ?? throw new NotFoundException("User with this name does not exist");

            _unitOfWork.Tags.Create(entry);

            _unitOfWork.SaveChanges();
        }
    }
}