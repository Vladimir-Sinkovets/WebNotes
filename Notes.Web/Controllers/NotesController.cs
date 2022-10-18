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

        private string CurrentUserName { get => User.Identity.Name; } 

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
            IEnumerable<Note> notes =_notesManager.GetAllNotesFor(CurrentUserName);

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

            await _notesManager.AddNoteAsync(note, CurrentUserName);

            return View();
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            var note = _notesManager.GetNoteById(id, CurrentUserName);

            var viewModel = _mapper.Map<UpdateNoteViewModel>(note);

            viewModel.AllTags = GetAllTagsForCurrentUser();
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UpdateNoteViewModel viewModel)
        {
            if (ModelState.IsValid == true)
            {
                var note = _mapper.Map<Note>(viewModel);

                await _notesManager.UpdateAsync(note);
            }

            var model = _notesManager.GetNoteById(viewModel.Id, CurrentUserName);

            viewModel = _mapper.Map<UpdateNoteViewModel>(model);
            
            viewModel.AllTags = GetAllTagsForCurrentUser();

            return View(viewModel);
        }

        private IEnumerable<TagViewModel> GetAllTagsForCurrentUser()
        {
            var allTags = _tagManager.GetAllTagsFor(CurrentUserName);

            var allViewModelTags = _mapper.Map<IEnumerable<Tag>, List<TagViewModel>>(allTags);

            return allViewModelTags;
        }

        [HttpGet]
        public IActionResult Read(int id)
        {
            var note = _notesManager.GetNoteById(id, CurrentUserName);

            var viewModel = _mapper.Map<NoteViewModel>(note);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddTagToNote(int noteId, int tagId)
        {
            _notesManager.AddTagToNote(noteId, tagId, CurrentUserName);

            return RedirectToActionPermanent("Update", new { id = noteId });
        }
        
        [HttpPost]
        public IActionResult RemoveTagFromNote(int noteId, int tagId)
        {
            _notesManager.RemoveTagFromNote(noteId, tagId, CurrentUserName);

            return RedirectToActionPermanent("Update", new { id = noteId });
        }


        [HttpGet]
        public IActionResult EditTags()
        {
            var tags = _tagManager.GetAllTagsFor(CurrentUserName);

            var tagViewModels = _mapper.Map<IEnumerable<Tag>, List<TagViewModel>>(tags); 
                
            var viewModel = new EditTagsViewModel() { AllTags = tagViewModels };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTag(string tagName)
        {
            var tag = new Tag() { Name = tagName };

            await _tagManager.AddTagAsync(tag, CurrentUserName);

            return RedirectToActionPermanent("EditTags");
        }

        [HttpPost]
        public IActionResult DeleteTag(int id)
        {
            _tagManager.DeleteTagById(id, CurrentUserName);

            return RedirectToActionPermanent("EditTags");
        }
    }
}