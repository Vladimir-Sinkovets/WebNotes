using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.DAL.Models
{
    public class TagEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public UserEntry User { get; set; }
        public ICollection<UserEntry> Users { get; set; }
    }
}
