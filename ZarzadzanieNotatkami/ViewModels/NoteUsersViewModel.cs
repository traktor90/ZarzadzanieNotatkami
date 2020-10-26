using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZarzadzanieNotatkami.Models;

namespace ZarzadzanieNotatkami.ViewModels
{
    public class NoteUsersViewModel
    {
        public Note Note { get; set; }
        public List<User> Users { get; set; }
        public int SelectedUser { get; set; }
        public List<SelectListItem> userItems{ get; set; }
    }
}
