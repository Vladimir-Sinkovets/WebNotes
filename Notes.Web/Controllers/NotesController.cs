using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Notes.BLL.Services.NoteManagers;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.BLL.Services.TagManagers;
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

        public INoteManager _notesManager { get; set; }

        public NotesController(INoteManager notesManager, IMapper mapper, ITagManager tagManager)
        {
            _notesManager = notesManager;
            _mapper = mapper;
            _tagManager = tagManager;
        }

        [HttpGet]
        public ActionResult NoteList()
        {
            IEnumerable<Note> notes =_notesManager.GetAllNotesForUser(CurrentUserName);

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

            await _notesManager.AddNoteForUserAsync(note, CurrentUserName);

            return View();
        }

        [HttpGet]
        public ActionResult EditNote(int id)
        {
            var note = _notesManager.GetNoteByIdForUser(id, CurrentUserName);

            var viewModel = _mapper.Map<EditNoteViewModel>(note);

            viewModel.AllTags = GetAllTagsForCurrentUser();
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditNote(EditNoteViewModel viewModel)
        {
            if (ModelState.IsValid == true)
            {
                var note = _mapper.Map<Note>(viewModel);

                await _notesManager.UpdateAsync(note);
            }

            var model = _notesManager.GetNoteByIdForUser(viewModel.Id, CurrentUserName);

            viewModel = _mapper.Map<EditNoteViewModel>(model);
            
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
            var note = _notesManager.GetNoteByIdForUser(id, CurrentUserName);

            var viewModel = _mapper.Map<NoteViewModel>(note);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddTagToNote(int noteId, int tagId)
        {
            _notesManager.AddTagToNoteForUser(noteId, tagId, CurrentUserName);

            return RedirectToActionPermanent("EditNote", new { id = noteId });
        }
        
        [HttpPost]
        public IActionResult RemoveTagFromNote(int noteId, int tagId)
        {
            _notesManager.RemoveTagFromNoteForUser(noteId, tagId, CurrentUserName);

            return RedirectToActionPermanent("EditNote", new { id = noteId });
        }


        [HttpGet]
        public IActionResult TagList()
        {
            var tags = _tagManager.GetAllTagsFor(CurrentUserName);

            var tagViewModels = _mapper.Map<IEnumerable<Tag>, List<TagViewModel>>(tags); 
                
            var viewModel = new TagListViewModel() { AllTags = tagViewModels };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTag(string tagName)
        {
            var tag = new Tag() { Name = tagName };

            await _tagManager.AddTagAsync(tag, CurrentUserName);

            return RedirectToActionPermanent("TagList");
        }

        [HttpPost]
        public IActionResult DeleteTag(int id)
        {
            _tagManager.DeleteTagById(id, CurrentUserName);

            return RedirectToActionPermanent("TagList");
        }
    }
}