using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notes.BLL.Interfaces;
using Notes.BLL.Models;
using Notes.DAL.Models;
using Notes.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Web.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly UserManager<UserEntry> _userManager;
        private readonly IMapper _mapper;

        public INotesManager _notesManager { get; set; }

        public NotesController(INotesManager notesManager, UserManager<UserEntry> userManager, IMapper mapper)
        {
            _notesManager = notesManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult ShowAll()
        {
            IEnumerable<Note> notes =_notesManager.GetAllForCurrentUser();

            IEnumerable<NoteViewModel> viewModel = _mapper.Map<IEnumerable<Note>, List<NoteViewModel>>(notes);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(NoteCreateViewModel model)
        {
            Note note = _mapper.Map<NoteCreateViewModel, Note>(model);

            await _notesManager.AddNoteAsync(note, User.Identity.Name);

            return View();
        }
    }
}