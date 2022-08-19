using Notes.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Interfaces
{
    public interface INotesManager
    {
        void CreateNote(Note note);
        IEnumerable<Note> GetAllForCurrentUser();
    }
}
