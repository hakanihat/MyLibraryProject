using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLibraryApp.Models
{
    [Index(nameof(Isbn), IsUnique = true)]
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("ISBN")]
        public int Isbn { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? Genre { get; set; }
        [DisplayName("Publisher")]
        [Required]
        public int PublisherId { get; set; }
        public virtual Publisher? Publisher { get; set; }
        [DisplayName("Available")]
        public bool IsAvaiable { get; set; } = true;
        public string? Annotation { get; set; }

        

    }
}
