using AutoMapper;
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

        public NoteManager(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserAccessor userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userAccessor = userService;
        }


        public async Task CreateNewNoteAsync(NoteCreateData note)
        {
            var entry = _mapper.Map<NoteEntry>(note);

            entry.User = _userAccessor.Current;

            entry.CreatedDate = DateTime.Now;

            _unitOfWork.Notes.Create(entry);

            await _unitOfWork.SaveChangesAsync();
        }

        public IEnumerable<Note> GetAllNotes()
        {
            var userName = _userAccessor.Current.UserName;

            var noteEntries = _unitOfWork.Notes.GetAllWithoutTracking()
                .Where(n => n.User!.UserName == userName);

            var notes = _mapper.Map<IEnumerable<NoteEntry>, List<Note>>(noteEntries);

            return notes;
        }

        public async Task UpdateNoteAsync(NoteUpdateData updateData)
        {
            var notes = _unitOfWork.Notes.GetAllWithoutTracking();

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
            var tagEntry = GetTagEntryById(tagId);
            var noteEntry = GetNoteEntryById(noteId);

            noteEntry.Tags?.Add(tagEntry);

            _unitOfWork.SaveChanges();
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

            _unitOfWork.Notes.Update(entry);

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task AddTagAsync(TagCreateData tag)
        {
            if (_unitOfWork.Tags.GetAll()
                    .Any(t => t.Name == tag.Name))
                throw new ExistedTagNameException("Cannot add tag with already existing name");

            var entry = _mapper.Map<TagEntry>(tag);

            entry.User = _userAccessor.Current;

            _unitOfWork.Tags.Create(entry);

            await _unitOfWork.SaveChangesAsync();
        }

        public void RemoveTagFromNote(int noteId, int tagId)
        {
            var tagEntity = GetTagEntryById(tagId);

            var noteEntity = GetNoteEntryById(noteId);

            noteEntity.Tags?.Remove(tagEntity);

            _unitOfWork.SaveChanges();
        }

        public void DeleteTagById(int tagId)
        {
            var tags = _unitOfWork.Tags.GetAllWithoutTracking();

            ThrowUserAccessExceptionForTags(tags, tagId);

            _unitOfWork.Tags.DeleteById(tagId);

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<Tag> GetAllTags()
        {
            var userName = _userAccessor.Current.UserName;

            var tagsEntry = _unitOfWork.Tags.GetAll()
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

            var noteEntries = _unitOfWork.Notes.GetAllWithoutTracking()
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

            var tags = _unitOfWork.Tags.GetAll();

            var entry = tags.FirstOrDefault(t => t.Id == tagId && t.User!.UserName == userName);

            if (entry == null)
                throw new NotFoundException("This tag does not exist");

            return entry;
        }

        private NoteEntry GetNoteEntryById(int noteId)
        {
            var userName = _userAccessor.Current.UserName;

            var notes = _unitOfWork.Notes.GetAll();

            var entry = notes.FirstOrDefault(n => n.Id == noteId && n.User!.UserName == userName);

            if (entry == null)
                throw new NotFoundException("This note does not exist");

            return entry;
        }


        private void ThrowUserAccessExceptionForNotes(IQueryable<NoteEntry> notes, int noteId)
        {
            var currentUserName = _userAccessor.Current.UserName;

            if (notes.FirstOrDefault(n => n.Id == noteId && n.User!.UserName == currentUserName) == null)
                throw new UserAccessException($"User {_userAccessor.Current.UserName} have no access to this note ( noteId = {noteId} )");
        }

        private void ThrowUserAccessExceptionForTags(IQueryable<TagEntry> tags, int tagId)
        {
            var currentUserName = _userAccessor.Current.UserName;

            if (tags.FirstOrDefault(t => t.Id == tagId && t.User!.UserName == currentUserName) == null)
                throw new NotFoundException("This tag does not exist");
        }
    }
}