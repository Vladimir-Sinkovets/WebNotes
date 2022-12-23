using System.Collections.Generic;

namespace Notes.BLL.Services.NoteManagers.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<Note>? Notes { get; set; }
    }
}
