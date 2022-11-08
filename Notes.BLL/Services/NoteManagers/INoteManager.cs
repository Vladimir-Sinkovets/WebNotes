using Notes.BLL.Services.NoteManagers.Models;
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
        Task AddNoteForUserAsync(Note note, string userName);
        IEnumerable<Note> GetAllNotesForUser(string userName);
        Note GetNoteByIdForUser(int id, string userName);
        IEnumerable<Tag> GetNoteTagsByIdForUser(int noteId, string userName);
        Task UpdateAsync(Note note);
        void AddTagToNoteForUser(int noteId, int tagId, string userName);
        void RemoveTagFromNoteForUser(int noteId, int tagId, string userName);
    }
}