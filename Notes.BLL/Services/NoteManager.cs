using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Notes.BLL.Interfaces;
using Notes.BLL.Models;
using Notes.DAL.Models;
using Notes.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.BLL
{
    public class NoteManager : INoteManager
    {
        private IUnitOfWork _unitOfWork;
        private readonly UserManager<UserEntry> _userManager;
        private readonly IMapper _mapper;

        public NoteManager(IUnitOfWork unitOfWork, UserManager<UserEntry> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }


        public async Task AddNoteForUserAsync(Note note, string userName)
        {
            var entry = _mapper.Map<Note, NoteEntry>(note);

            var user = await _userManager.FindByNameAsync(userName);

            entry.User = user ?? throw new NotFoundException("User with this name does not exist");

            _unitOfWork.Notes.Create(entry);

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<Note> GetAllNotesForUser(string userName)
        {
            var noteEntries = _unitOfWork.Notes.GetAll()
                .Where(n => n.User.UserName == userName);

            var notes = _mapper.Map<IEnumerable<NoteEntry>, List<Note>>(noteEntries);

            return notes;
        }

        public Note GetNoteByIdForUser(int id, string userName)
        {
            var entry = _unitOfWork.Notes.GetAll()
                .FirstOrDefault(n => n.Id == id && n.User.UserName == userName)
                ?? throw new NotFoundException("There is no such note");

            var note = _mapper.Map<Note>(entry);

            return note;
        }

        public async Task UpdateAsync(Note note)
        {
            if (note == null)
                throw new ArgumentNullException(nameof(note));

            var entry = _mapper.Map<NoteEntry>(note);

            _unitOfWork.Notes.Update(entry);

            await _unitOfWork.SaveChangesAsync();
        }

        public void AddTagToNoteForUser(int noteId, int tagId, string userName)
        {
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

        public IEnumerable<Tag> GetNoteTagsByIdForUser(int noteId, string userName)
        {
            var note = GetNoteByIdForUser(noteId, userName);

            return note.Tags;
        }

        public void RemoveTagFromNoteForUser(int noteId, int tagId, string userName)
        {
            var tagEntity = _unitOfWork.Tags.GetAll()
                .FirstOrDefault(t => t.Id == tagId && t.User.UserName == userName)
                ?? throw new NotFoundException("There is no such tag");

            var noteEntity = _unitOfWork.Notes.GetAll()
                .FirstOrDefault(n => n.Id == noteId && n.User.UserName == userName)
                ?? throw new NotFoundException("There is no such note");

            noteEntity.Tags.Remove(tagEntity);

            _unitOfWork.SaveChanges();
        }
    }
}
