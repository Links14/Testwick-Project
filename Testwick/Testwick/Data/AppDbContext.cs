using Microsoft.EntityFrameworkCore;
using Testwick.Models;

namespace Testwick.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuestionChoice> QuestionChoices { get; set; }

        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionChoiceOrder> QuestionChoiceOrders { get; set; }
        public DbSet<QuestionTopic> QuestionTopics { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }

        public DbSet<QuizResult> QuizResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Topics
            modelBuilder.Entity<Topic>(entity => 
            {
                entity.Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired();
            });

            // Questions
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasOne(q => q.CorrectChoice)   // "A Question has one CorrectChoice..."
                  .WithMany()                      // "...and a CorrectChoice can belong to many Questions"
                  .HasForeignKey(q => q.CorrectChoiceId)  // "...linked by this column"
                  .OnDelete(DeleteBehavior.Restrict);     // "...and block deletion if referenced"
            });
        }
    }
}