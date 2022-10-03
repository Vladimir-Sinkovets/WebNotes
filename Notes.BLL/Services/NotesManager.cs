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

        public IEnumerable<Note> GetAllForCurrentUser()
        {
            IEnumerable<NoteEntry> noteEntries = _unitOfWork.Notes.GetAll();

            IEnumerable<Note> notes = _mapper.Map<IEnumerable<NoteEntry>, List<Note>>(noteEntries);

            return notes;
        }
    }
}
