using Notes.BLL.Models;
using Notes.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Interfaces
{
    public interface INotesManager
    {
        Task AddNoteAsync(Note note, string userName);
        IEnumerable<Note> GetAllNotesFor(string userName);
        Note GetNoteById(int id, string userName);
        IEnumerable<Tag> GetNoteTags(int noteId, string userName);
        Task UpdateAsync(Note note);
        void AddTagToNote(int noteId, int tagId, string userName);
        void RemoveTagFromNote(int noteId, int tagId, string userName);
    }
}