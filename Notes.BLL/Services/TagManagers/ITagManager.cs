using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.BLL.Services.NoteManagers.Models;

namespace Notes.BLL.Services.TagManagers
{
    public interface ITagManager
    {
        Task AddTagAsync(Tag tag, string userName);
        void DeleteTagById(int id, string userName);
        IEnumerable<Tag> GetAllTagsFor(string user);
        Tag GetTagById(int id, string userName);
    }
}
