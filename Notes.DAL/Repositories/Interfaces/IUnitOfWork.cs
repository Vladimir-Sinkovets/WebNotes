using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        INoteRepository Notes { get; }
        ITagRepository Tags { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
