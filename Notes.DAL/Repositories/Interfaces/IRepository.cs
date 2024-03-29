﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.DAL.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        IQueryable<T> GetAll();
        void Create(T entity);
        void Update(T entity);
        void DeleteById(int id);
    }
}
