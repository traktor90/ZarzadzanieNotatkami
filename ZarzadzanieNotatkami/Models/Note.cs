using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ZarzadzanieNotatkami.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        //max length 1 bilion characters
        [Display(Name = "Note")]
        public string Text { get; set; }

        public bool Important { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
