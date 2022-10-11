using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notes.DAL.Models;
using System;
using System.Collections.Generic;

namespace Notes.DAL.Repositories
{
    public class ApplicationDbContext : IdentityDbContext<UserEntry>
    {
        public DbSet<NoteEntry> Notes { get; set; }

        public DbSet<TagEntry> NoteTags { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    }
}