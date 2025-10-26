namespace TicketScam.Models
{
    public class Ticket
    {
        public int TicketId { get; set; } //primary key
        public int VenueId { get; set; } //foreign key

        public int? OrderId { get; set; } //foreign key

        public string SeatNumber { get; set; } = string.Empty;

        public Show? Show { get; set; } //nav property
        public Venue? Venue { get; set; } //nav property
        List<Order>? Orders { get; set; } //nav property
    }
}
