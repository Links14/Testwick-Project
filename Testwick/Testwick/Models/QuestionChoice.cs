namespace Testwick.Models
{
    public class QuestionChoice
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;

        // Navigation
        public ICollection<QuestionChoiceOrder> QuestionChoiceOrders { get; set; } = [];
    }
}
