﻿using Notes.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.DAL.Repositories.Interfaces
{
    public interface ITagRepository : IRepository<TagEntry>
    {
        IQueryable<TagEntry> GetAllWithoutTracking();
    }
}
