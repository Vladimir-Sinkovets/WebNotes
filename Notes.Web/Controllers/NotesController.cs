using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
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
        private readonly IMapper _mapper;
        private readonly ITagManager _tagManager;

        public INotesManager _notesManager { get; set; }

        public NotesController(INotesManager notesManager, IMapper mapper, ITagManager tagManager)
        {
            _notesManager = notesManager;
            _mapper = mapper;
            _tagManager = tagManager;
        }

        [HttpGet]
        public ActionResult ShowAll()
        {
            IEnumerable<Note> notes =_notesManager.GetAllNotesFor(User.Identity.Name);

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

        [HttpGet]
        public ActionResult Update(int id)
        {
            var note = _notesManager.GetNoteById(id, User.Identity.Name);

            var viewModel = _mapper.Map<UpdateNoteViewModel>(note);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdateNoteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var note = _mapper.Map<Note>(model);

            await _notesManager.UpdateAsync(note);

            return View(model);
        }

        [HttpGet]
        public IActionResult Read(int id)
        {
            var note = _notesManager.GetNoteById(id, User.Identity.Name);

            var viewModel = _mapper.Map<NoteViewModel>(note);

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult EditTags()
        {
            var tags = _tagManager.GetAllTagsFor(User.Identity.Name);

            var tagViewModels = _mapper.Map<IEnumerable<Tag>, List<TagViewModel>>(tags); // might it be changed to automapper using?

            var viewModel = new EditTagsViewModel() { Tags = tagViewModels };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTag(string tagName)
        {
            var tag = new Tag() { Name = tagName };

            await _tagManager.AddTagAsync(tag, User.Identity.Name);

            return RedirectToActionPermanent("EditTags");
        }

        public IActionResult DeleteTag(int id)
        {
            _tagManager.DeleteTagById(id, User.Identity.Name);

            return RedirectToActionPermanent("EditTags");
        }
    }
}