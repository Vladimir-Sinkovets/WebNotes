using Notes.BLL.Services.NoteManagers.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Services.NoteManagers.Models
{
    public class SearchFilter
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public bool UseLength { get; set; }

        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public ImportanceFilterUsing IsImportant { get; set; }
    }
}
