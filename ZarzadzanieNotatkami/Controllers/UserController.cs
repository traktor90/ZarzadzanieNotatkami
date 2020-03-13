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

        //creating user by post method
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
                //if all is valid return main view
                return View("~/Views/Note/Index.cshtml",model);
            }
            else
            {
                ModelState.AddModelError("", "Something wrong with the data");
                return View();
            }
        }

        //return to manage View with all users
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

        public IActionResult Details(int id)
        {
            Models.User user = context.Users.FirstOrDefault(u => u.Id == id);
            return View(user);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == id);
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User userFromInput)
        {
            Models.User userFromDB = context.Users.FirstOrDefault(u => u.Id == userFromInput.Id);
            //update user name
            userFromDB.Name = userFromInput.Name;
            context.SaveChanges();
            return View("Manage", context.Users.ToList());
        }
    }
}