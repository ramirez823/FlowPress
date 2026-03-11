using FlowPress.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace FlowPress.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Source> Sources { get; set; }
        public DbSet<SourceItem> SourceItems { get; set; }
        public DbSet<Secret> Secrets { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            // Configuración de la relación Secret → Source
            modelBuilder.Entity<Secret>()
                .HasOne(s => s.Source)          
                .WithMany()                      
                .HasForeignKey(s => s.SourceId)  
                .OnDelete(DeleteBehavior.SetNull); 
        }



    }
}

