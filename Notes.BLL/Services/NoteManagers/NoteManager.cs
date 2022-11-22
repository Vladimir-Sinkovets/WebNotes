using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly ICurrentUserAccessor _userAccessor;

        public NoteManager(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userAccessor = userService;
        }


        public async Task CreateNewNoteAsync(NoteCreateData note)
        {
            if (note == null)
                throw new ArgumentNullException();

            var entry = _mapper.Map<NoteEntry>(note);

            entry.User = _userAccessor.Current;

            _unitOfWork.Notes.Create(entry);

            await _unitOfWork.SaveChangesAsync();
        }

        public IEnumerable<Note> GetAllNotes()
        {
            var userName = _userAccessor.Current.UserName;

            var noteEntries = _unitOfWork.Notes.GetAll()
                .Where(n => n.User.UserName == userName);

            var notes = _mapper.Map<IEnumerable<NoteEntry>, List<Note>>(noteEntries);

            return notes;
        }

        public async Task UpdateNoteAsync(NoteUpdateData updateData)
        {
            var notes = _unitOfWork.Notes.GetAllWithoutTracking();

            if (updateData == null)
                throw new ArgumentNullException(nameof(updateData));

            ThrowUserAccessExceptionForNotes(notes, updateData.Id);

            var entry = _mapper.Map<NoteEntry>(updateData);

            _unitOfWork.Notes.Update(entry);

            await _unitOfWork.SaveChangesAsync();
        }

        public Note GetNoteById(int noteId)
        {
            var entry = GetNoteEntryById(noteId);

            var note = _mapper.Map<Note>(entry);

            return note;
        }

        public void AddTagToNote(int noteId, int tagId)
        {
            var userName = _userAccessor.Current.UserName;

            var tags = _unitOfWork.Tags.GetAll();
            var notes = _unitOfWork.Notes.GetAll();

            ThrowNotFoundExceptionForTags(tags, tagId);
            ThrowNotFoundExceptionForNotes(notes, noteId);
            ThrowUserAccessExceptionForNotes(notes, noteId);
            ThrowUserAccessExceptionForTags(tags, tagId);

            var tagEntry = tags.FirstOrDefault(t => t.Id == tagId);
            var noteEntry = notes.FirstOrDefault(n => n.Id == noteId);

            noteEntry.Tags
                .Add(tagEntry);

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<Tag> GetNoteTagsById(int noteId)
        {
            var note = GetNoteById(noteId);

            return note.Tags;
        }

        public async Task SetNoteImportanceAsync(int noteId, bool isImportant)
        {
            var entry = GetNoteEntryById(noteId);

            entry.IsImportant = isImportant;
            entry.User = _userAccessor.Current;
            entry.Tags = null;

            _unitOfWork.Notes.Update(entry);

            await _unitOfWork.SaveChangesAsync();
        }

        private NoteEntry GetNoteEntryById(int noteId)
        {
            var userName = _userAccessor.Current.UserName;

            var notes = _unitOfWork.Notes.GetAllWithoutTracking();

            ThrowNotFoundExceptionForNotes(notes, noteId);
            ThrowUserAccessExceptionForNotes(notes, noteId);

            var entry = notes.FirstOrDefault(n => n.Id == noteId);

            return entry;
        }

        public async Task AddTagAsync(TagCreateData data)
        {
            if(data == null)
                throw new ArgumentNullException(nameof(data));

            var userName = _userAccessor.Current.UserName;

            if (_unitOfWork.Tags.GetAll()
                    .Any(t => t.Name == data.Name))
                throw new ExistedTagNameException("Cannot add tag with already existing name");

            var entry = _mapper.Map<TagEntry>(data);

            var user = _userAccessor.Current;

            entry.User = user;

            _unitOfWork.Tags.Create(entry);

            await _unitOfWork.SaveChangesAsync();
        }

        public void RemoveTagFromNote(int noteId, int tagId)
        {
            var userName = _userAccessor.Current.UserName;

            var tags = _unitOfWork.Tags.GetAllWithoutTracking();
            var notes = _unitOfWork.Notes.GetAllWithoutTracking();

            ThrowNotFoundExceptionForTags(tags, tagId);
            ThrowNotFoundExceptionForNotes(notes, noteId);
            ThrowUserAccessExceptionForNotes(notes, noteId);
            ThrowUserAccessExceptionForTags(tags, tagId);

            var tagEntity = _unitOfWork.Tags.GetAll().FirstOrDefault(t => t.Id == tagId);
            var noteEntity = _unitOfWork.Notes.GetAll().FirstOrDefault(n => n.Id == noteId);

            noteEntity.Tags.Remove(tagEntity);

            _unitOfWork.SaveChanges();
        }

        public void DeleteTagById(int tagId)
        {
            var tags = _unitOfWork.Tags.GetAllWithoutTracking();

            ThrowNotFoundExceptionForTags(tags, tagId);
            ThrowUserAccessExceptionForTags(tags, tagId);

            _unitOfWork.Tags.DeleteById(tagId);

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<Tag> GetAllTags()
        {
            var userName = _userAccessor.Current.UserName;

            var tagsEntry = _unitOfWork.Tags.GetAll()
                .Where(t => t.User.UserName == userName);

            var tags = _mapper.Map<IEnumerable<TagEntry>, List<Tag>>(tagsEntry);

            return tags;
        }

        public Tag GetTagById(int tagId)
        {
            var userName = _userAccessor.Current.UserName;

            IQueryable<TagEntry> tags = _unitOfWork.Tags.GetAll();

            ThrowNotFoundExceptionForTags(tags, tagId);
            ThrowUserAccessExceptionForTags(tags, tagId);

            var entry = tags.FirstOrDefault(t => t.Id == tagId && t.User.UserName == userName);

            var tag = _mapper.Map<Tag>(entry);

            return tag;
        }

        private static void ThrowNotFoundExceptionForNotes(IQueryable<NoteEntry> notes, int noteId)
        {
            var noteEntity = notes.FirstOrDefault(n => n.Id == noteId)
                ?? throw new NotFoundException("This note does not exist");
        }

        private void ThrowNotFoundExceptionForTags(IQueryable<TagEntry> tags, int tagId)
        {
            if (tags.FirstOrDefault(tag => tag.Id == tagId) == null)
                throw new NotFoundException("This tag does not exist");
        }

        private void ThrowUserAccessExceptionForNotes(IQueryable<NoteEntry> notes, int noteId)
        {
            if (notes.FirstOrDefault(n => n.Id == noteId).User.UserName != _userAccessor.Current.UserName)
                throw new UserAccessException();
        }

        private void ThrowUserAccessExceptionForTags(IQueryable<TagEntry> tags, int tagId)
        {
            if (tags.FirstOrDefault(t => t.Id == tagId).User.UserName != _userAccessor.Current.UserName)
                throw new UserAccessException($"User {_userAccessor.Current.UserName} have no access to this tag ( tagId = {tagId} )");
        }
    }
}