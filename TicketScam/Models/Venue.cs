namespace TicketScam.Models
{
    public class Venue
    {
        public int VenueId { get; set; }//primary key

        public string VenueName { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int VenueCapacity { get; set; }

        List<Ticket>? Tickets { get; set; } //nav property
    }
}
