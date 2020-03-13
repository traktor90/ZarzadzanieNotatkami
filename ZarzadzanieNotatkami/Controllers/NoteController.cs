using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZarzadzanieNotatkami.Models;
using ZarzadzanieNotatkami.ViewModels;

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
            List<User> users = context.Users.Include(user=>user.Notes).ToList();

            NotesViewModel model = CreateNotesViewModelToReturn();
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            NoteUsersViewModel model = new NoteUsersViewModel()
            {
                Users = context.Users.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(NoteUsersViewModel noteModel)
        {
            if (ModelState.IsValid)
            {
                noteModel.Note.User = context.Users.FirstOrDefault(u => u.Id == noteModel.SelectedUser);

                context.Notes.Add(noteModel.Note);
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

        //sort notes ascending
        public IActionResult SortAsc()
        {
            //get userId from cookie
            int UserId = int.Parse(HttpContext.Request.Cookies["UserId"]);
            List<Note> notes = new List<Note>();
            
            //if user is selected
            if (UserId != -1)
                notes = context.Notes.Where(n => n.User.Id == UserId).ToList();
            //if all user is selected
            else
                notes = context.Notes.ToList();
            notes.Sort((x, y) => x.Title.CompareTo(y.Title));
            List<User> users = context.Users.ToList();

            NotesViewModel model = new NotesViewModel
            {
                Notes = notes,
                Importants = null,
                Users = users
            };
            return View("Index", model);
        }

        //sort notes descending
        public IActionResult SortDesc()
        {
            //get userId from cookie
            int UserId = int.Parse(HttpContext.Request.Cookies["UserId"]);
            List<Note> notes = new List<Note>();
            //if user is selected
            if (UserId != -1)
                notes = context.Notes.Where(n => n.User.Id == UserId).ToList();
            //if all user option is selected
            else
                notes = context.Notes.ToList();
            //sort notes by title
            notes.Sort((x, y) => y.Title.CompareTo(x.Title));
            List<User> users = context.Users.ToList();

            NotesViewModel model = new NotesViewModel
            {
                Notes = notes,
                Importants = null,
                Users = users
            };
            return View("Index", model);
        }

        public IActionResult Importants(NotesViewModel model)
        {
            //do this for every note
            for (int i = 0; i < model.Notes.Count; i++)
            {
                Note note = new Note
                {
                    Id = model.Notes[i].Id,
                    //specifically only this field is updating
                    Important = model.Notes[i].Important,
                    Text = model.Notes[i].Text,
                    Title = model.Notes[i].Title,
                    User = model.Notes[i].User,
                    UserId = model.Notes[i].UserId
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
            //get note with this id
            Note note = context.Notes.FirstOrDefault(n => n.Id == id);
            //remove this note with above id
            context.Notes.Remove(note);
            context.SaveChanges();
            NotesViewModel model = CreateNotesViewModelToReturn();
            return View("Index", model);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            Note note = context.Notes.FirstOrDefault(n => n.Id == id);
            List<User> users = context.Users.ToList();

            NoteUsersViewModel model = new NoteUsersViewModel()
            {
                Note = note,
                Users=users
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(NoteUsersViewModel noteIn)
        {
            NotesViewModel model = CreateNotesViewModelToReturn();
            if (ModelState.IsValid)
            {
                //get note with id from ViewModel
                Note noteFromBase = context.Notes.FirstOrDefault(n => n.Id == noteIn.Note.Id);
                noteFromBase.Important = noteIn.Note.Important;
                noteFromBase.Text = noteIn.Note.Text;
                noteFromBase.Title = noteIn.Note.Title;
                //set user for note while getting info from ViewModel SelectedUser
                noteFromBase.User = context.Users.FirstOrDefault(u => u.Id == noteIn.SelectedUser);
                context.SaveChanges();
                return View("Index", model);
            }
            else
            {
                //prepare ViewModel to return in post method because there was an error, returning back to same page
                Note note = context.Notes.FirstOrDefault(n => n.Id == noteIn.Note.Id);
                List<User> users = context.Users.ToList();

                NoteUsersViewModel modelUsers = new NoteUsersViewModel()
                {
                    Note = note,
                    Users = users
                };
                ModelState.AddModelError("", "Data not valid");
                return View(modelUsers);
            }

        }

        public IActionResult ChangeUser(int id)
        {
            //add cookie with selected user
            HttpContext.Response.Cookies.Append("UserId", id.ToString(),new Microsoft.AspNetCore.Http.CookieOptions()
            {
                //save cookie even you user didnt agree
                IsEssential=true
            });
            Models.User user;
            NotesViewModel model;
            //if user is selected
            if (id != -1)
            {
                //include notes db into users db and select user with given id
                user = context.Users.Include(u => u.Notes).FirstOrDefault(u => u.Id == id);
                model = new NotesViewModel
                {
                    Importants = null,
                    Notes = user.Notes?.ToList(),
                    Users = context.Users.ToList()
                };
            }
            //if all user option is selected
            else
            {

                model = new NotesViewModel
                {
                    Importants = null,
                    Notes = context.Notes.ToList(),
                    Users = context.Users.ToList()
                };
            }

            return View("Index",model);
        }

        //preparing ViewModel so it can be returned
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