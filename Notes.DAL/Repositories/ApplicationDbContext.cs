using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notes.DAL.Models;
using Notes.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.DAL.Repositories
{
    public class ApplicationDbContext : IdentityDbContext<UserEntry>, IUnitOfWork
    {
        public DbSet<NoteEntry>? Notes { get; set; }

        public DbSet<TagEntry>? Tags { get; set; }

        public INoteRepository NotesRepository { get; }
        public ITagRepository TagsRepository { get; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            TagsRepository = new TagRepository(this);
            NotesRepository = new NoteRepository(this);
        }

        void IUnitOfWork.SaveChanges()
        {
            base.SaveChanges();
        }

        void IDisposable.Dispose()
        {
            base.Dispose();
        }

        async Task IUnitOfWork.SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }
    }
}