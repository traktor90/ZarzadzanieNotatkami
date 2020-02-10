using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZarzadzanieNotatkami.Models
{
    public class Note
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }

        //max length 1 bilion characters
        [Display(Name = "Note")]
        public string Text { get; set; }

        public bool Important { get; set; }
    }
}
