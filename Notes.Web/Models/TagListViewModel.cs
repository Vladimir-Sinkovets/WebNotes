using System.Collections.Generic;

namespace Notes.Web.Models
{
    public class TagListViewModel
    {
        public IEnumerable<TagItemViewModel> AllTags { get; set; }
    }
}
