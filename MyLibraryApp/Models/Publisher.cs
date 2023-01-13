using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyLibraryApp.Models
{
    [Index(nameof(PubName), IsUnique = true)]
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }
        [Required(ErrorMessage = "Please enter correct name"),MinLength(2), MaxLength(30)]
        public string? PubName { get; set; }
        [Required(ErrorMessage = "Please enter correct address"), MinLength(2), MaxLength(100)]
        public string? PubAdress { get; set; }

    }
}
