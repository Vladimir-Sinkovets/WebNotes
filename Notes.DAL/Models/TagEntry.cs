using Notes.DAL.Repositories.Interfaces;
using System.Collections.Generic;

namespace Notes.DAL.Models
{
    public class TagEntry : IEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public UserEntry? User { get; set; }
        public ICollection<NoteEntry>? Notes { get; set; }
    }
}
