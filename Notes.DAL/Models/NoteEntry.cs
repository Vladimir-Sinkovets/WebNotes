using Notes.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.DAL.Models
{
    public class NoteEntry : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public UserEntry User { get; set; }
        public ICollection<TagEntry> Tags { get; set; }
        public bool IsImportant { get; set; }
    }
}
