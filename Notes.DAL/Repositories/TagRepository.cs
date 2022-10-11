using Notes.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.DAL.Repositories
{
    public class TagRepository : IRepository<TagEntry>
    {
        private readonly ApplicationDbContext _dbContext;

        public TagRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Create(TagEntry entity)
        {
            _dbContext.Add(entity);
        }

        public void Delete(int id)
        {
            var entry = _dbContext.Tags.FirstOrDefault(t => t.Id == id);

            if(entry != null)
            {
                _dbContext.Remove(entry);
            }
        }

        public IQueryable<TagEntry> GetAll()
        {
            return _dbContext.Tags;
        }

        public void Update(TagEntry entity)
        {
            _dbContext.Update(entity);
        }
    }
}
