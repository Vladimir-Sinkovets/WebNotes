using System.Collections.Generic;
s
namespace Notes.Web.Models
{
    public class ListNoteItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public IEnumerable<TagItemViewModel> Tags { get; set; }
    }
}
