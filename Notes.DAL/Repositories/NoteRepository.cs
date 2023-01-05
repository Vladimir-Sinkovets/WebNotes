using Microsoft.EntityFrameworkCore;
using Notes.DAL.Models;
using Notes.DAL.Repositories.Interfaces;
using System.Linq;

namespace Notes.DAL.Repositories
{
    public class NoteRepository : BaseRepository<NoteEntry>, INoteRepository
    {
        public NoteRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public override IQueryable<NoteEntry> GetAll()
        {
            return _table.Include(n => n.Tags)
                .Include(n => n.User);
        }

        public IQueryable<NoteEntry> GetAllWithoutTracking()
        {
            return _table.AsNoTrackingWithIdentityResolution()
                .Include(n => n.Tags)
                .Include(n => n.User);
        }
    }
}
