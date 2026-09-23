using ELOR.Razzle.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ELOR.Razzle.Data
{
    public sealed class RazzleDbContext : DbContext
    {
        public RazzleDbContext(DbContextOptions<RazzleDbContext> options) : base(options)
        {
        }

        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<CashFlow> CashFlows => Set<CashFlow>();
        public DbSet<TagNote> TagNotes => Set<TagNote>();
        public DbSet<TagTask> TagTasks => Set<TagTask>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>(e =>
            {
                e.ToTable("tags");
                e.HasKey(t => t.Id);
                e.Property(t => t.Id).ValueGeneratedOnAdd();
                e.Property(t => t.Type).IsRequired();
                e.Property(t => t.Name).IsRequired().HasMaxLength(64);
            });

            modelBuilder.Entity<TaskEntity>(e =>
            {
                e.ToTable("tasks");
                e.HasKey(t => t.Id);
                e.Property(t => t.Id).ValueGeneratedOnAdd();
                e.Property(t => t.CreatedAt).IsRequired();
                e.Property(t => t.Flags).IsRequired();
                e.Property(t => t.Name).IsRequired().HasMaxLength(128);

                // SetNull for CompletionNoteId
                e.HasOne(t => t.CompletionNote)
                    .WithMany()
                    .HasForeignKey(t => t.CompletionNoteId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Note>(e =>
            {
                e.ToTable("notes");
                e.HasKey(n => n.Id);
                e.Property(n => n.Id).ValueGeneratedOnAdd();
                e.Property(n => n.CreatedAt).IsRequired();
                e.Property(n => n.Flags).IsRequired();
                e.Property(n => n.Text).IsRequired();

                e.HasOne(n => n.Task)
                    .WithMany(t => t.Notes)
                    .HasForeignKey(n => n.TaskId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);
            });

            modelBuilder.Entity<CashFlow>(e =>
            {
                e.ToTable("cash_flows");
                e.HasKey(c => c.Id);
                e.Property(c => c.Id).ValueGeneratedOnAdd();
                e.Property(c => c.NoteId).IsRequired();

                e.HasOne(c => c.Note)
                    .WithOne(n => n.CashFlow)
                    .HasForeignKey<CashFlow>(c => c.NoteId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                e.HasIndex(c => c.NoteId).IsUnique();
            });

            modelBuilder.Entity<TagNote>(e =>
            {
                e.ToTable("tags_notes");
                e.HasKey(tn => new { tn.TagId, tn.NoteId });

                e.HasOne(tn => tn.Tag)
                    .WithMany(t => t.TagNotes)
                    .HasForeignKey(tn => tn.TagId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(tn => tn.Note)
                    .WithMany(n => n.TagNotes)
                    .HasForeignKey(tn => tn.NoteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TagTask>(e =>
            {
                e.ToTable("tags_tasks");
                e.HasKey(tt => new { tt.TagId, tt.TaskId });

                e.HasOne(tt => tt.Tag)
                    .WithMany(t => t.TagTasks)
                    .HasForeignKey(tt => tt.TagId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(tt => tt.Task)
                    .WithMany(t => t.TagTasks)
                    .HasForeignKey(tt => tt.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}