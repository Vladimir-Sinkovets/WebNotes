using System.Collections;
using System.Collections.Generic;

namespace Notes.Web.Models.Note
{
    public class NoteListViewModel
    {
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }
        public IEnumerable<ReadNoteViewModel> Notes { get; set; }
        public IEnumerable<string> AllTags { get; set; }

        public SearchFilterViewModel SearchFilter { get; set; }
    }
}
