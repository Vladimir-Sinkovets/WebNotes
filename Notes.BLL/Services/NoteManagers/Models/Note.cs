using System;
using System.Collections.Generic;

namespace Notes.BLL.Services.NoteManagers.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public bool IsImportant { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
