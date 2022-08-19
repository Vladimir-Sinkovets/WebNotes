using AutoMapper;
using Notes.BLL.Interfaces;
using Notes.BLL.Models;
using Notes.DAL.Models;
using Notes.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Notes.BLL
{
    public class NotesManager : INotesManager
    {
        private ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public NotesManager(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            this._mapper = mapper;
        }

        public void CreateNote(Note note)
        {
            NoteEntry entry = _mapper.Map<Note, NoteEntry>(note);

            _dbContext.Notes.Add(entry);
        }

        public IEnumerable<Note> GetAllForCurrentUser()
        {
            IEnumerable<NoteEntry> noteEntries = _dbContext.Notes;

            IEnumerable<Note> notes = _mapper.Map<IEnumerable<NoteEntry>, List<Note>>(noteEntries);

            return notes;
        }
    }
}
