using System.Collections.Generic;

namespace Notes.Web.Models.Note
{
    public class SearchFilterViewModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public bool IsImportant { get; set; }
    }
}