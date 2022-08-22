using Microsoft.EntityFrameworkCore;
using Notes.DAL.Models;
using System;
using System.Collections.Generic;

namespace Notes.DAL.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<NoteEntry> Notes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}
