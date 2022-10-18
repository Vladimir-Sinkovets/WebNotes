using System.Collections.Generic;

namespace Notes.Web.Models
{
    public class EditNoteViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public IEnumerable<TagViewModel> AllTags { get; set; }
        public IEnumerable<TagViewModel> Tags { get; set; }
    }
}
