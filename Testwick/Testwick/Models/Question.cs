namespace Testwick.Models
{
    public class Question
    {
        // Question owned content
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int CorrectChoice { get; set; } // Constraint FK_CorrectChoice

        // queried data
        public List<QuestionChoice> Choices { get; set; } = [];
        public List<Topic> Topics { get; set; } = [];
    }

}
