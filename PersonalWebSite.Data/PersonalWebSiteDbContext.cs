using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalWebSite.Core.Models;

namespace PersonalWebSite.Data
{
    public class PersonalWebSiteDbContext : IdentityDbContext<User>
    {
        public PersonalWebSiteDbContext(DbContextOptions<PersonalWebSiteDbContext> options)
            : base(options)
        {
        }

        // Migration işlemleri için parametresiz constructor
        public PersonalWebSiteDbContext() : base() { }

        // Migration işlemleri için gerekli OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PersonalWebSiteDb;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Question configurations
            builder.Entity<Question>(entity =>
            {
                entity.HasKey(q => q.Id);
                entity.Property(q => q.Title).IsRequired().HasMaxLength(200);
                entity.Property(q => q.Content).IsRequired();
                entity.HasOne(q => q.User)
                    .WithMany(u => u.Questions)
                    .HasForeignKey(q => q.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Answer configurations
            builder.Entity<Answer>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Content).IsRequired();
                entity.HasOne(a => a.Question)
                    .WithMany(q => q.Answers)
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(a => a.User)
                    .WithMany(u => u.Answers)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
} 