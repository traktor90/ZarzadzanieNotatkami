using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
            List<Note> notes;

            notes = context.Notes.ToList();

            NotesViewModel model = new NotesViewModel
            {
                Notes = notes,
                Importants = null
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Note note)
        {
            if (ModelState.IsValid)
            {
                context.Notes.Add(note);
                context.SaveChanges();

                NotesViewModel model = new NotesViewModel
                {
                    Notes = context.Notes.ToList(),
                    Importants = null
                };
                return View("Index",model);
            }
            else
            {
                //add error to model to show validation error
                ModelState.AddModelError("", "Something went wrong");
                return View();
            }
        }

        public IActionResult Details(int id)
        {
            Note note = context.Notes.FirstOrDefault(n => n.Id == id);
            return View(note);
        }

        public IActionResult SortAsc()
        {
            List<Note> notes = context.Notes.ToList();
            notes.Sort((x, y) => x.Title.CompareTo(y.Title));
            NotesViewModel model = new NotesViewModel
            {
                Notes = notes,
                Importants = null
            };
            return View("Index", model);
        }

        public IActionResult SortDesc()
        {
            List<Note> notes = context.Notes.ToList();
            notes.Sort((x, y) => y.Title.CompareTo(x.Title));
            NotesViewModel model = new NotesViewModel
            {
                Notes = notes,
                Importants = null
            };
            return View("Index", model);
        }

        public IActionResult Importants(NotesViewModel model)
        {

            for (int i = 0; i < model.Notes.Count; i++)
            {
                //listNotes[i] = model.Notes[i];
                Note note = new Note
                {
                    Id = model.Notes[i].Id,
                    Important = model.Notes[i].Important,
                    Text = model.Notes[i].Text,
                    Title = model.Notes[i].Title
                };
                context.Notes.Update(note);
            }
            
            context.SaveChanges();
            
            var listNotes = context.Notes.ToList();
            NotesViewModel modelToReturn = new NotesViewModel
            {
                Notes = listNotes,
                Importants = null
            };
            return View("Index",modelToReturn);
        }
    }
}