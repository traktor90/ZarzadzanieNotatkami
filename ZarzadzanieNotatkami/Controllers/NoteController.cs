﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZarzadzanieNotatkami.Models;
using ZarzadzanieNotatkami.ViewModels;

namespace ZarzadzanieNotatkami.Controllers
{
    public class NoteController : Controller
    {
        NotesDBContext context;
        const int allNotesSelected = -1;

        public NoteController(NotesDBContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            List<Note> notes = context.Notes.ToList();
            List<User> users = context.Users.Include(user => user.Notes).ToList();
            NotesViewModel model = PrepareModelWithUserId();

            //sample objects needed to render titles for user and note
            var headerForNote = context.Notes?.FirstOrDefault();
            var headerForUser = context.Users.FirstOrDefault();

            model.HeaderForNote = headerForNote;
            model.HeaderForUser = headerForUser;
            return View(model);
        }

        private NotesViewModel PrepareModelWithUserId()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            int userIdInt;
            bool userIdOk = int.TryParse(userId, out userIdInt);
            NotesViewModel model;
            if (userIdOk)
            {
                model = CreateNotesViewModelToReturn(userIdInt);
                model.UserId = userIdInt;
            }
            else
            {
                model = CreateNotesViewModelToReturn(allNotesSelected);
                model.UserId = userIdInt;

            }

            return model;
        }

        [HttpGet]
        public IActionResult Create()
        {
            NoteUsersViewModel model = PrepareModelForCreate();
            return View(model);
        }

        private NoteUsersViewModel PrepareModelForCreate()
        {
            return new NoteUsersViewModel()
            {
                Users = context.Users.ToList(),
                userItems = context.Users.ToList().ConvertAll(user =>
                {
                    return new SelectListItem
                    {
                        Text = user.Name,
                        Value = user.Id.ToString()
                    };
                })
            };
        }

        [HttpPost]
        public IActionResult Create(NoteUsersViewModel noteModel)
        {
            if (noteModel == null)
                throw new ArgumentNullException("ViewModel cannot be null");

            if (ModelState.IsValid)
            {
                noteModel.Note.User = context.Users.FirstOrDefault(u => u.Id == noteModel.SelectedUser);

                context.Notes.Add(noteModel.Note);
                context.SaveChanges();

                NotesViewModel model = CreateNotesViewModelToReturn(allNotesSelected);
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
            if (id < 0)
                throw new ArgumentOutOfRangeException("Note id cannot be less than zero");
            Note note = context.Notes.FirstOrDefault(n => n.Id == id);

            return View(note);
        }

        //sort notes ascending
        public IActionResult SortAsc()
        {
            List<Note> notes = new List<Note>();

            try
            {
                notes = GetUserNotes();
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }

            notes.Sort((x, y) => x.Title.CompareTo(y.Title));
            List<User> users = context.Users.ToList();

            NotesViewModel model = CreateNotesViewModelToReturn(allNotesSelected);
            return View("Index", model);
        }

        //sort notes descending
        public IActionResult SortDesc()
        {
            List<Note> notes = new List<Note>();

            try
            {
                notes = GetUserNotes();
            }catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
            //sort notes by title
            notes.Sort((x, y) => y.Title.CompareTo(x.Title));
            List<User> users = context.Users.ToList();

            NotesViewModel model = CreateNotesViewModelToReturn(allNotesSelected);

            return View("Index", model);
        }

        private List<Note> GetUserNotes()
        {
            List<Note> notes;

            int UserId;
            bool cookieExist = int.TryParse(HttpContext.Request.Cookies["UserId"], out UserId);

            if (!cookieExist)
                throw new Exception("User cookie does not exist");

            //if user is selected
            if (UserId != allNotesSelected)
                notes = context.Notes.Where(n => n.User.Id == UserId).ToList();
            //if all user option is selected
            else
                notes = context.Notes.ToList();
            return notes;
        }

        public IActionResult Importants(NotesViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException("View model cannot be null");

            //do this for every note
            for (int noteIndex = 0; noteIndex < model.Notes.Count; noteIndex++)
            {
                Note note = PrepareNote(model, noteIndex);
                context.Notes.Update(note);
            }

            context.SaveChanges();
            
            var listNotes = context.Notes.ToList();
            NotesViewModel modelToReturn = CreateNotesViewModelToReturn(allNotesSelected);
            return View("Index",modelToReturn);
        }

        private static Note PrepareNote(NotesViewModel model, int noteIndex)
        {
            if (model == null)
                throw new ArgumentNullException("ViewModel cannot be null");
            if (noteIndex < 0)
                throw new ArgumentOutOfRangeException("Wrong id of Note");

            Note note = new Note
            {
                Id = model.Notes[noteIndex].Id,
                //specifically only this field is updating
                Important = model.Notes[noteIndex].Important,
                Text = model.Notes[noteIndex].Text,
                Title = model.Notes[noteIndex].Title,
                User = model.Notes[noteIndex].User,
                UserId = model.Notes[noteIndex].UserId
            };
            return note;
        }

        public IActionResult CreateUser()
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException("Wrong note id");
            //get note with this id
            Note note = context.Notes.FirstOrDefault(n => n.Id == id);
            //remove this note with above id
            context.Notes.Remove(note);
            context.SaveChanges();
            NotesViewModel model = CreateNotesViewModelToReturn(allNotesSelected);
            return View("Index", model);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException("Wrong noteId");

            NoteUsersViewModel model = PrepareModelForEdit(id);

            return View(model);
        }

        private NoteUsersViewModel PrepareModelForEdit(int noteId)
        {
            if (noteId < 0)
                throw new ArgumentOutOfRangeException("Note id is wrong");

            Note note = context.Notes.FirstOrDefault(n => n.Id == noteId);
            List<User> users = context.Users.ToList();

            NoteUsersViewModel model = new NoteUsersViewModel()
            {
                Note = note,
                Users = users,
                userItems = context.Users.ToList().ConvertAll(user =>
                {
                    return new SelectListItem
                    {
                        Text = user.Name,
                        Value = user.Id.ToString()
                    };
                })
            };
            return model;
        }

        [HttpPost]
        public IActionResult Edit(NoteUsersViewModel noteIn)
        {
            if (noteIn == null)
                throw new ArgumentNullException("View model cannot be null");

            NotesViewModel model = CreateNotesViewModelToReturn(allNotesSelected);
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
            if (id < -1)
                throw new ArgumentOutOfRangeException("UserId is wrong");

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
                model = CreateNotesViewModelToReturn(id);
                model.UserId = id;
            }
            //if all user option is selected
            else
            {

                model = CreateNotesViewModelToReturn(allNotesSelected);
            }

            return View("Index",model);
        }

        //preparing ViewModel so it can be returned
        private NotesViewModel CreateNotesViewModelToReturn(int userId)
        {
            if (userId < -1)
                throw new ArgumentOutOfRangeException("UserId is wrong");

            List<SelectListItem> userListItems = context.Users.ToList().ConvertAll(user =>
            {
                return new SelectListItem
                {
                    Text = user.Name,
                    Value = user.Id.ToString()
                };
            });
            
            if(userId!=allNotesSelected)
                return new NotesViewModel
                {
                    Importants = null,
                    Notes = context.Notes.Where(n=>n.UserId==userId).ToList(),
                    Users = context.Users.ToList(),
                    UserListItems=userListItems
                };
            else
                return new NotesViewModel
                {
                    Importants = null,
                    Notes = context.Notes.ToList(),
                    Users = context.Users.ToList(),
                    UserListItems = userListItems
                };
        }
    }
}