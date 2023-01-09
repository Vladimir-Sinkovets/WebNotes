using System.Collections.Generic;

namespace Notes.Web.Models.Note
{
    public class TagListViewModel
    {
        public IEnumerable<TagItemViewModel>? AllTags { get; set; }
    }
}
