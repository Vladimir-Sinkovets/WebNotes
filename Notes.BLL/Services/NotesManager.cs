using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Notes.BLL.Interfaces;
using Notes.BLL.Models;
using Notes.DAL.Models;
using Notes.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.BLL
{
    public class NotesManager : INotesManager
    {
        private UnitOfWork _unitOfWork;
        private readonly UserManager<UserEntry> _userManager;
        private readonly IMapper _mapper;

        public NotesManager(UnitOfWork unitOfWork, UserManager<UserEntry> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task AddNoteAsync(Note note, string userName)
        {
            NoteEntry entry = _mapper.Map<Note, NoteEntry>(note);

            entry.User = await _userManager.FindByNameAsync(userName);

            _unitOfWork.Notes.Create(entry);

            _unitOfWork.SaveChanges();
        }

        public IEnumerable<Note> GetAllFor(string userName)
        {
            var noteEntries = _unitOfWork.Notes.GetAll()
                .Where(n => n.User.UserName == userName);

            var notes = _mapper.Map<IEnumerable<NoteEntry>, List<Note>>(noteEntries);

            return notes;
        }

        public Note GetByTitle(string title, string userName)
        {
            NoteEntry entry = _unitOfWork.Notes.GetAll()
                .FirstOrDefault(n => n.User.UserName == userName && n.Title == title);

            Note note = _mapper.Map<Note>(entry);

            return note;
        }
    }
}
