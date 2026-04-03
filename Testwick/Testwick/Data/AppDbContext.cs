using Microsoft.EntityFrameworkCore;
using Testwick.Models;

namespace Testwick.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuestionChoice> QuestionChoices { get; set; }
        public DbSet<QuestionTopic> QuestionTopics { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
        public DbSet<QuizResultAnswer> QuizResultAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Question
            modelBuilder.Entity<Question>()
                .HasOne(q => q.CorrectChoice)
                .WithMany()
                .HasForeignKey(q => q.CorrectChoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Question Choice
            modelBuilder.Entity<QuestionChoice>()
                .HasOne(c => c.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(q => q.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuestionChoice>()
                .HasIndex(c => new { c.QuestionId, c.Position })
                .IsUnique();

            // Question Topic
            modelBuilder.Entity<QuestionTopic>()
                .HasIndex(qt => new { qt.QuestionId, qt.TopicId })
                .IsUnique();

            // Quiz Question
            modelBuilder.Entity<QuizQuestion>()
                .HasIndex(qq => new { qq.QuizId, qq.QuestionId })
                .IsUnique();

            modelBuilder.Entity<QuizQuestion>()
                .HasIndex(qq => new { qq.QuizId, qq.Position })
                .IsUnique();

            // Quiz
            modelBuilder.Entity<Quiz>()
                .HasIndex(q => q.AdminToken).IsUnique();
            modelBuilder.Entity<Quiz>()
                .HasIndex(q => q.ContributorToken).IsUnique();

            // Quiz Result Answer
            modelBuilder.Entity<QuizResultAnswer>()
                .HasOne(a => a.ChosenChoice)
                .WithMany()
                .HasForeignKey(a => a.ChosenChoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuizResultAnswer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int retries = 5;

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    return await base.SaveChangesAsync(cancellationToken);
                }
                catch (Microsoft.Data.Sqlite.SqliteException ex)
                    when (ex.SqliteErrorCode == 5)
                {
                    // maxes out at a 2.5 second delay
                    await Task.Delay(500 * (i + 1), cancellationToken);
                }
            }

            throw new Exception($"Database save failed after {retries} retries.");
        }
    }
}