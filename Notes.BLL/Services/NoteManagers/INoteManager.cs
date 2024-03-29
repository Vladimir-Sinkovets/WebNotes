﻿using Notes.BLL.Services.NoteManagers.Models;
using Notes.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Services.NoteManagers
{
    public interface INoteManager
    {
        Note GetNoteById(int noteId);
        IEnumerable<Note> GetAllNotes();
        void AddTagToNote(int noteId, int tagId);
        Task UpdateNoteAsync(NoteUpdateData note);
        Task CreateNewNoteAsync(NoteCreateData note);
        IEnumerable<Tag> GetNoteTagsById(int noteId);
        void RemoveTagFromNote(int noteId, int tagId);
        Task SetNoteImportanceAsync(int noteId, bool important);
        IEnumerable<Note> GetAllByFilter(SearchFilter filter);
        Tag GetTagById(int tagId);
        IEnumerable<Tag> GetAllTags();
        void DeleteTagById(int tagId);
        Task AddTagAsync(TagCreateData data);
    }
}