using Microsoft.EntityFrameworkCore;
using Notes.DAL.Repositories.Interfaces;
using System.Linq;

namespace Notes.DAL.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly DbSet<T> _table;
        private readonly DbContext _context;

        public BaseRepository(DbContext context)
        {
            _table = context.Set<T>();
            _context = context;
        }

        public virtual IQueryable<T> GetAll()
        {
            return _table;
        }

        public virtual void Create(T entity)
        {
            _table.Add(entity);
        }

        public virtual void Update(T entity)
        {
            _table.Update(entity);
        }

        public virtual void DeleteById(int id)
        {
            var entry = _table.FirstOrDefault(n => n.Id == id);

            if (entry != null)
            {
                _table.Remove(entry);
            }
        }

    }
}
