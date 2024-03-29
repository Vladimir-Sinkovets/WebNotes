﻿using Notes.BLL.Services.NoteManagers.Enums;
using System.Collections.Generic;

namespace Notes.BLL.Services.NoteManagers.Models
{
    public class SearchFilter
    {
        public SearchFilter()
        {
            Tags = new List<string>();

            UseMaxLength = false;

            UseMinLength = false;
        }

        public string? Title { get; set; }
        public string? Text { get; set; }
        public IEnumerable<string>? Tags { get; set; } 

        public bool UseMinLength { get; set; }
        public bool UseMaxLength { get; set; }

        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public ImportanceFilterUsing Importance { get; set; }
    }
}
