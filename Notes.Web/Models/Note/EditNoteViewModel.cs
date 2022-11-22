using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notes.Web.Models.Note
{
    public class EditNoteViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsImportant { get; set; }
        public IEnumerable<TagItemViewModel> AllTags { get; set; }
        public IEnumerable<TagItemViewModel> Tags { get; set; }
    }
}
