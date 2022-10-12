using Notes.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.BLL.Interfaces
{
    public interface ITagManager
    {
        Task AddTagAsync(Tag tag, string userName);
        void DeleteTagById(int id, string userName);
        IEnumerable<Tag> GetAllTagsFor(string user);
    }
}
