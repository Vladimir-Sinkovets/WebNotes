using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;

namespace Notes.Web.Models.Note
{
    public class ReadNoteViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsImportant { get; set; }
        public int Length { get; set; }
        public int LineCount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public IHtmlContent HtmlText { get; set; }
        public IEnumerable<TagItemViewModel> Tags { get; set; }
    }
}
