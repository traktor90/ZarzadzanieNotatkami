using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZarzadzanieNotatkami.Models;

namespace ZarzadzanieNotatkami.Controllers
{
    public class NoteController : Controller
    {
        NotesDBContext context;
        public NoteController(NotesDBContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Note> notes;

            notes = context.Notes.ToList();
            return View(notes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Note model)
        {
            if (ModelState.IsValid)
            {
                context.Notes.Add(model);
                context.SaveChanges();
                return View("Index",context.Notes);
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong");
                return View();
            }
        }
    }
}