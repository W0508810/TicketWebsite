namespace TicketScam.Models
{
    public class Order
    {
        public int OrderId { get; set; } //primary key
        public DateTime OrderDate { get; set; }
        public int UserId { get; set; } //foreign key
        public int TicketId { get; set; } //foreign key

        public int VenueId { get; set; } //foreign key
        public User? User { get; set; } //nav property
        public Ticket? Ticket { get; set; } //nav property
    }
}
