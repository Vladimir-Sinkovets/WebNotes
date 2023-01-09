using Microsoft.AspNetCore.Html;
using Notes.BLL.Services.NoteManagers.Models;
using System;
using System.Collections.Generic;

namespace Notes.Web.Models.Note
{
    public class ReadNoteViewModel
    {
        public int Length 
        {
            get => Text != null ? Text.Length : 0; 
        }
        public int LineCount 
        {
            get => Text != null ? Text.Split('\n').Length : 0;
        }
        
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public bool IsImportant { get; set; }
        public DateTime CreatedDate { get; set; }
        public IHtmlContent? HtmlText { get; set; }
        public IEnumerable<TagItemViewModel>? Tags { get; set; }
    }
}
