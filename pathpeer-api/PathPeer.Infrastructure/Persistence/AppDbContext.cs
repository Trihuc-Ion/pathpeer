using Microsoft.EntityFrameworkCore;
using PathPeer.Domain.Entities;

namespace PathPeer.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonBlock> LessonBlocks => Set<LessonBlock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Course → Sections
        modelBuilder.Entity<Section>()
            .HasOne(s => s.Course)
            .WithMany(c => c.Sections)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Section → Lessons
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Section)
            .WithMany(s => s.Lessons)
            .HasForeignKey(l => l.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Lesson → Blocks
        modelBuilder.Entity<LessonBlock>()
            .HasOne(b => b.Lesson)
            .WithMany(l => l.Blocks)
            .HasForeignKey(b => b.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}