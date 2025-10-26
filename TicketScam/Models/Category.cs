namespace TicketScam.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Show>? Shows { get; set; }
    }
}
