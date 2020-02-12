using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZarzadzanieNotatkami.Models
{
    public class NotesViewModel
    {
        public List<Note> Notes { get; set; }
        public List<bool> Importants { get; set; }
        public List<User> Users { get; set; }
    }
}
