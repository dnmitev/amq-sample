using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthChecks.DB
{
    public class PostContext : DbContext
    {
        public PostContext(DbContextOptions<PostContext> options)
            : base(options)
        {
        }

    public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PostConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
