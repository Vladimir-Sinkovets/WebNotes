using System.Collections.Generic;

namespace Notes.Web.Models.Note
{
    public class FilterSettings
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public bool IsImportant { get; set; }
    }
}