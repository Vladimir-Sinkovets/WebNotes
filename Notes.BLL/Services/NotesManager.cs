﻿using AutoMapper;
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
        private UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotesManager(UnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void CreateNote(Note note, UserEntry user)
        {
            NoteEntry entry = _mapper.Map<Note, NoteEntry>(note);

            entry.User = user;

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
