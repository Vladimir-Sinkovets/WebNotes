using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Web.Models
{
    public class NoteViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public IEnumerable<TagViewModel> Tags { get; set; }
    }
}
