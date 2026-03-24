namespace Testwick.DTOs
{
    public class ChoiceDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class CreateChoiceDto
    {
        public string Text { get; set; } = string.Empty;
    }
}
