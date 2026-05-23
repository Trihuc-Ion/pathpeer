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
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<CourseVote> CourseVotes { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }
    public DbSet<UserLicense> UserLicenses { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<P2PTransfer> P2PTransfers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CourseVote: User ↔ Course (Many-to-Many with payload)
        modelBuilder.Entity<CourseVote>()
            .HasIndex(v => new { v.UserId, v.CourseId })
            .IsUnique();
        
        modelBuilder.Entity<CourseVote>()
            .HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CourseVote>()
            .HasOne(v => v.Course)
            .WithMany(c => c.Votes)
            .HasForeignKey(v => v.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Enrollment: User ↔ Course (Many-to-Many)
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique();
            
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

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

        // UserPreferences: 1-to-1 cu User
        modelBuilder.Entity<UserPreferences>()
            .HasOne(p => p.User)
            .WithOne(u => u.Preferences)
            .HasForeignKey<UserPreferences>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserLicense: User ↔ Course (Many-to-Many cu payload)
        modelBuilder.Entity<UserLicense>()
            .HasIndex(l => new { l.UserId, l.CourseId })
            .IsUnique();

        modelBuilder.Entity<UserLicense>()
            .HasOne(l => l.User)
            .WithMany(u => u.Licenses)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLicense>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Licenses)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<P2PTransfer>()
            .HasOne(t => t.Sender)
            .WithMany()
            .HasForeignKey(t => t.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<P2PTransfer>()
            .HasOne(t => t.Receiver)
            .WithMany()
            .HasForeignKey(t => t.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<P2PTransfer>()
            .HasOne(t => t.Course)
            .WithMany()
            .HasForeignKey(t => t.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}