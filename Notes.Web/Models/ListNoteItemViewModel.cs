using Microsoft.AspNetCore.Html;
using System.Collections.Generic;

namespace Notes.Web.Models
{
    public class ListNoteItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public IHtmlContent HtmlText { get; set; }
        public IEnumerable<TagItemViewModel> Tags { get; set; }
    }
}
