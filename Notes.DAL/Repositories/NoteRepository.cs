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
    public class NoteRepository : BaseRepository<NoteEntry>, INoteRepository
    {
        public NoteRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public override IQueryable<NoteEntry> GetAll()
        {
            return _table.Include(n => n.Tags);
        }
    }
}
