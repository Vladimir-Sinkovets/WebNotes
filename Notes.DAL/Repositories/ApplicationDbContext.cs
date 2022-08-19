using Microsoft.EntityFrameworkCore;
using Notes.DAL.Models;
using System;
using System.Collections.Generic;

namespace Notes.DAL.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public List<NoteEntry> Notes { get; } = new List<NoteEntry>() 
        { 
            new NoteEntry() { Title = "ASP.NET Core 5", }, 
            new NoteEntry() { Title = "ASP.NET Core 6", }, 
            new NoteEntry() { Title = "Паттерны проектирования", }, 
            new NoteEntry() { Title = "Асинхронное программирование", }, 
            new NoteEntry() { Title = "JavaScript", }, 
        };
    }
}
