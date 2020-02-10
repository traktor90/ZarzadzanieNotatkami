using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarzadzanieNotatkami.Models
{
    public class NotesDBContext : DbContext
    {
        public NotesDBContext(DbContextOptions<NotesDBContext> options): base(options) { }
        public DbSet<Note> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
    }
}
