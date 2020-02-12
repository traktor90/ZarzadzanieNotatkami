using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZarzadzanieNotatkami.Models;

namespace ZarzadzanieNotatkami.Controllers
{
    public class UserController : Controller
    {
        private readonly NotesDBContext context;

        public UserController(NotesDBContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            
            if (ModelState.IsValid)
            {
                context.Users.Add(user);
                context.SaveChanges();
                NotesViewModel model = new NotesViewModel
                {
                    Importants = null,
                    Notes = context.Notes.ToList(),
                    Users = context.Users.ToList()
                };
                return View("~/Views/Note/Index.cshtml",model);
            }
            else
            {
                ModelState.AddModelError("", "Something wrong with the data");
                return View();
            }
        }

        public IActionResult Manage()
        {
            return View(context.Users.ToList());
        }

        public IActionResult Delete(int id)
        {
            Models.User user = context.Users.FirstOrDefault(u=>u.Id==id);
            context.Users.Remove(user);
            context.SaveChanges();
            return View("Manage", context.Users.ToList());
        }
    }
}