using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Add this for IFormFile

namespace TicketScam.Models
{
    public class Show
    {
        public int ShowId { get; set; }

        [Required]
        public string ShowName { get; set; } = string.Empty;

        [Required]
        public DateTime ShowDate { get; set; }

        public string ShowDescription { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000)]
        public decimal TicketPrice { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int VenueId { get; set; }

     
        public string? ImageFileName { get; set; }

        [NotMapped] 
        public IFormFile? ImageFile { get; set; }
        public Venue? Venue { get; set; }
        public Category? Category { get; set; }
    }
}