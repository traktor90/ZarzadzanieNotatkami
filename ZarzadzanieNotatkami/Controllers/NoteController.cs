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
            List<Note> notes=context.Notes.ToList();
            List<User> users = context.Users.ToList();

            NotesViewModel model = CreateNotesViewModelToReturn();
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

                NotesViewModel model = CreateNotesViewModelToReturn();
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
            List<User> users = context.Users.ToList();

            NotesViewModel model = CreateNotesViewModelToReturn();
            return View("Index", model);
        }

        public IActionResult SortDesc()
        {
            List<Note> notes = context.Notes.ToList();
            notes.Sort((x, y) => y.Title.CompareTo(x.Title));
            List<User> users = context.Users.ToList();

            NotesViewModel model = CreateNotesViewModelToReturn();
            return View("Index", model);
        }

        public IActionResult Importants(NotesViewModel model)
        {

            for (int i = 0; i < model.Notes.Count; i++)
            {
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
            NotesViewModel modelToReturn = CreateNotesViewModelToReturn();
            return View("Index",modelToReturn);
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            Note note = context.Notes.FirstOrDefault(n => n.Id == id);
            context.Notes.Remove(note);
            context.SaveChanges();
            NotesViewModel model = CreateNotesViewModelToReturn();
            return View("Index", model);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            Note note = context.Notes.FirstOrDefault(n => n.Id == id);
            return View(note);
        }

        [HttpPost]
        public IActionResult Edit(Note noteIn)
        {
            NotesViewModel model = CreateNotesViewModelToReturn();
            if (ModelState.IsValid)
            {
                Note noteFromBase = context.Notes.FirstOrDefault(n => n.Id == noteIn.Id);
                noteFromBase.Important = noteIn.Important;
                noteFromBase.Text = noteIn.Text;
                noteFromBase.Title = noteIn.Title;
                context.SaveChanges();
                return View("Index", model);
            }
            else
            {
                ModelState.AddModelError("", "Data not valid");
                return View();
            }

        }
        private NotesViewModel CreateNotesViewModelToReturn()
        {
            return new NotesViewModel
            {
                Importants = null,
                Notes = context.Notes.ToList(),
                Users = context.Users.ToList()
            };
        }
    }
}