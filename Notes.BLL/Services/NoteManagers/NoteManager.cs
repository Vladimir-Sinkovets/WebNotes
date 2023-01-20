using AutoMapper;
using Microsoft.Extensions.Logging;
using Notes.BLL.Services.CurrentUserAccessor;
using Notes.BLL.Services.NoteManagers.Enums;
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
        private readonly ILogger<NoteManager> _logger;

        public NoteManager(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor userService, ILogger<NoteManager> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userAccessor = userService;
            _logger = logger;
        }


        public async Task CreateNewNoteAsync(NoteCreateData note)
        {
            var entry = _mapper.Map<NoteEntry>(note);

            entry.User = _userAccessor.Current;

            entry.CreatedDate = DateTime.Now;

            _unitOfWork.NotesRepository.Create(entry);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogDebug($"New note created with title {entry.Title}");
        }

        public IEnumerable<Note> GetAllNotes()
        {
            var userName = _userAccessor.Current.UserName;

            var noteEntries = _unitOfWork.NotesRepository.GetAllWithoutTracking()
                .Where(n => n.User!.UserName == userName);

            var notes = _mapper.Map<IEnumerable<NoteEntry>, List<Note>>(noteEntries);

            return notes;
        }

        public async Task UpdateNoteAsync(NoteUpdateData updateData)
        {
            var notes = _unitOfWork.NotesRepository.GetAllWithoutTracking();

            ThrowUserAccessExceptionForNotes(notes, updateData.Id);

            var entry = _mapper.Map<NoteEntry>(updateData);

            _unitOfWork.NotesRepository.Update(entry);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogDebug($"Note updated, note title: {entry.Title}");
        }

        public Note GetNoteById(int noteId)
        {
            var entry = GetNoteEntryById(noteId);

            var note = _mapper.Map<Note>(entry);

            return note;
        }

        public void AddTagToNote(int noteId, int tagId)
        {
            var tagEntry = GetTagEntryById(tagId);
            var noteEntry = GetNoteEntryById(noteId);

            noteEntry.Tags?.Add(tagEntry);

            _unitOfWork.SaveChanges();

            _logger.LogDebug($"Tag added to note, tag id: {tagEntry.Id}, note id: {noteEntry.Id}");
        }

        public IEnumerable<Tag> GetNoteTagsById(int noteId)
        {
            var note = GetNoteById(noteId);

            return note.Tags!;
        }

        public async Task SetNoteImportanceAsync(int noteId, bool isImportant)
        {
            var entry = GetNoteEntryById(noteId);

            entry.IsImportant = isImportant;

            _unitOfWork.NotesRepository.Update(entry);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogDebug($"Note updated with {entry.Title}");
        }


        public async Task AddTagAsync(TagCreateData tag)
        {
            if (_unitOfWork.TagsRepository.GetAll()
                    .Any(t => t.Name == tag.Name))
                throw new ExistedTagNameException("Cannot add tag with already existing name");

            var entry = _mapper.Map<TagEntry>(tag);

            entry.User = _userAccessor.Current;

            _unitOfWork.TagsRepository.Create(entry);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogDebug($"Tag \"{entry.Name}\" added");
        }

        public void RemoveTagFromNote(int noteId, int tagId)
        {
            var tagEntity = GetTagEntryById(tagId);

            var noteEntity = GetNoteEntryById(noteId);

            noteEntity.Tags?.Remove(tagEntity);

            _unitOfWork.SaveChanges();

            _logger.LogDebug($"Remove tag from note, note id: {noteId}");
        }

        public void DeleteTagById(int tagId)
        {
            var tags = _unitOfWork.TagsRepository.GetAllWithoutTracking();

            ThrowUserAccessExceptionForTags(tags, tagId);

            _unitOfWork.TagsRepository.DeleteById(tagId);

            _unitOfWork.SaveChanges();

            _logger.LogDebug($"Tag deleted");
        }

        public IEnumerable<Tag> GetAllTags()
        {
            var userName = _userAccessor.Current.UserName;

            var tagsEntry = _unitOfWork.TagsRepository.GetAll()
                .Where(t => t.User!.UserName == userName);

            var tags = _mapper.Map<IEnumerable<TagEntry>, List<Tag>>(tagsEntry);

            return tags;
        }

        public Tag GetTagById(int tagId)
        {
            var entry = GetTagEntryById(tagId);

            var tag = _mapper.Map<Tag>(entry);

            return tag;
        }

        public IEnumerable<Note> GetAllByFilter(SearchFilter filter)
        {
            var userId = _userAccessor.Current.Id;

            var noteEntries = _unitOfWork.NotesRepository.GetAllWithoutTracking()
                .Where(n => n.User!.Id == userId)
                .AsEnumerable();

            if (string.IsNullOrEmpty(filter.Title) == false)
                noteEntries = noteEntries.Where(
                    n => n.Title != null &&
                    n.Title.Contains(filter.Title, StringComparison.CurrentCultureIgnoreCase));

            if (string.IsNullOrEmpty(filter.Text) == false)
                noteEntries = noteEntries.Where(
                    n => n.Text != null &&
                    n.Text.Contains(filter.Text, StringComparison.CurrentCultureIgnoreCase));

            if (filter.Tags?.Any() == true)
                noteEntries = noteEntries.Where(n =>
                    filter.Tags?.Except(n.Tags!.Select(t => t.Name))
                        .Any() == false);

            if (filter.UseMinLength == true)
                noteEntries = noteEntries.Where(
                    n => n.Text?.Length >= filter.MinLength);

            if (filter.UseMaxLength == true)
                noteEntries = noteEntries.Where(
                    n => n.Text?.Length <= filter.MaxLength);

            if (filter.Importance != ImportanceFilterUsing.None)
            {
                bool isImportant = filter.Importance switch
                {
                    ImportanceFilterUsing.Important => true,
                    ImportanceFilterUsing.Unimportant => false,
                    ImportanceFilterUsing.None => throw new NotImplementedException(),
                    _ => throw new NotImplementedException(),
                };

                noteEntries = noteEntries.Where(n => n.IsImportant == isImportant);
            }

            var notes = _mapper.Map<IEnumerable<NoteEntry>, List<Note>>(noteEntries);

            return notes;
        }



        private TagEntry GetTagEntryById(int tagId)
        {
            var userName = _userAccessor.Current.UserName;

            var tags = _unitOfWork.TagsRepository.GetAll();

            var entry = tags.FirstOrDefault(t => t.Id == tagId && t.User!.UserName == userName);

            if (entry == null)
                throw new NotFoundException("This tag does not exist");

            return entry;
        }

        private NoteEntry GetNoteEntryById(int noteId)
        {
            var userName = _userAccessor.Current.UserName;

            var notes = _unitOfWork.NotesRepository.GetAll();

            var entry = notes.FirstOrDefault(n => n.Id == noteId && n.User!.UserName == userName);

            if (entry == null)
                throw new NotFoundException("This note does not exist");

            return entry;
        }


        private void ThrowUserAccessExceptionForNotes(IQueryable<NoteEntry> notes, int noteId)
        {
            var currentUserName = _userAccessor.Current.UserName;

            if (notes.FirstOrDefault(n => n.Id == noteId && n.User!.UserName == currentUserName) == null)
            {
                _logger.LogDebug($"User {_userAccessor.Current.UserName} have no access to this note ( noteId = {noteId} )");

                throw new UserAccessException($"User {_userAccessor.Current.UserName} have no access to this note ( noteId = {noteId} )");
            }
        }

        private void ThrowUserAccessExceptionForTags(IQueryable<TagEntry> tags, int tagId)
        {
            var currentUserName = _userAccessor.Current.UserName;

            if (tags.FirstOrDefault(t => t.Id == tagId && t.User!.UserName == currentUserName) == null)
            {
                _logger.LogDebug($"");

                throw new NotFoundException("This tag does not exist" + $"Tag (id = {tagId}) does not exist, user: {currentUserName}");
            }
        }
    }
}