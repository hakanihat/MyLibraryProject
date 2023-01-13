using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLibraryApp.Models
{
    public class Borrow
    {   [Key]
        public int IdBorrow { get; set; }
        public int Isbn { get; set; }
        public virtual Book? Book { get; set; }
        public int ReaderId { get; set; }
        public virtual Reader? Reader { get; set; }
        [Display(Name = "Borrowed Date")]
        public DateTime BorrowedDate { get; set; } = DateTime.Now;
        [Display(Name = "Returned Date")]
        public DateTime ReturnedDate { get; set; } 




    }
}
