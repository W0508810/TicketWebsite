namespace TicketScam.Models
{
    public class User
    {

        public int UserId { get; set; } //primary key
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PaymentInfo { get; set; } = string.Empty;

        public int TicketId { get; set; } //foreign key 
        List<Order>? Orders { get; set; } //nav property
    }
}
