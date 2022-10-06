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
        Note GetNoteById(int id, string userName);
        IEnumerable<Note> GetAllForCurrentUser();
    }
}