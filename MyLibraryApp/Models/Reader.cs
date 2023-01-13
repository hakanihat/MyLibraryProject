using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyLibraryApp.Models
{
    public class Reader
    {
        [Key]
        public int ReaderId { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? TelNum { get; set; }
        [Required]
        public string? Address { get; set; }


    }
}
