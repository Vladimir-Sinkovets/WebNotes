using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        INoteRepository NotesRepository { get; }
        ITagRepository TagsRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
