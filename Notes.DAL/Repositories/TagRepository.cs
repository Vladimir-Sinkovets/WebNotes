using Microsoft.EntityFrameworkCore;
using Notes.DAL.Models;
using Notes.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.DAL.Repositories
{
    public class TagRepository : BaseRepository<TagEntry>, ITagRepository
    {
        public TagRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public override IQueryable<TagEntry> GetAll()
        {
            return _table.Include(t => t.Notes)
                .Include(t => t.User);
        }

        public IQueryable<TagEntry> GetAllWithoutTracking()
        {
            return _table.Include(t => t.Notes)
                .AsNoTrackingWithIdentityResolution()
                .Include(t => t.User)
                .AsNoTrackingWithIdentityResolution();
        }
    }
}
