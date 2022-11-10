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
        Task UpdateNoteAsync(NoteUpdateData note);
        Note GetNoteById(int noteId);
        Task CreateNewNoteAsync(NoteCreateData note);
        IEnumerable<Note> GetAllNotes();
        void AddTagToNote(int noteId, int tagId);
        IEnumerable<Tag> GetNoteTagsById(int noteId);
        void RemoveTagFromNote(int noteId, int tagId);

        Task AddTagAsync(TagCreateData data);
        Tag GetTagById(int tagId);
        void DeleteTagById(int tagId);
        IEnumerable<Tag> GetAllTags();
    }
}