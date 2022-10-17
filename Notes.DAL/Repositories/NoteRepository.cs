using Microsoft.EntityFrameworkCore;
using Notes.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.DAL.Repositories
{
    public class NoteRepository : IRepository<NoteEntry>
    {
        private ApplicationDbContext _dbContext;

        public NoteRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(NoteEntry entity)
        {
            _dbContext.Add(entity);
        }

        public void Delete(int id)
        {
            var entry = _dbContext.Notes.FirstOrDefault(n => n.Id == id);

            if(entry != null)
            {
                _dbContext.Remove(entry);
            }
        }

        public IQueryable<NoteEntry> GetAll()
        {
            return _dbContext.Notes.Include(n => n.Tags);
        }

        public void Update(NoteEntry entity)
        {
            _dbContext.Update(entity);
        }
    }
}
