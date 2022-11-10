using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Notes.BLL.Services.NoteManagers;
using Notes.BLL.Services.NoteManagers.Models;
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
        public INoteManager _noteManager;

        public NotesController(INoteManager notesManager, IMapper mapper)
        {
            _noteManager = notesManager;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult NoteList()
        {
            IEnumerable<Note> notes = _noteManager.GetAllNotes();

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
            var note = _mapper.Map<NoteCreateData>(model);

            await _noteManager.CreateNewNoteAsync(note);

            return View();
        }

        [HttpGet]
        public ActionResult EditNote(int id)
        {
            var note = _noteManager.GetNoteById(id);

            var viewModel = _mapper.Map<EditNoteViewModel>(note);

            viewModel.AllTags = GetAllTagsForCurrentUser();
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditNote(EditNoteViewModel viewModel)
        {
            if (ModelState.IsValid == true)
            {
                var note = _mapper.Map<NoteUpdateData>(viewModel);

                await _noteManager.UpdateNoteAsync(note);
            }

            var model = _noteManager.GetNoteById(viewModel.Id);

            viewModel = _mapper.Map<EditNoteViewModel>(model);
            
            viewModel.AllTags = GetAllTagsForCurrentUser();

            return View(viewModel);
        }

        private IEnumerable<TagViewModel> GetAllTagsForCurrentUser()
        {
            var allTags = _noteManager.GetAllTags();

            var allViewModelTags = _mapper.Map<IEnumerable<Tag>, List<TagViewModel>>(allTags);

            return allViewModelTags;
        }

        [HttpGet]
        public IActionResult Read(int id)
        {
            var note = _noteManager.GetNoteById(id);

            var viewModel = _mapper.Map<NoteViewModel>(note);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddTagToNote(int noteId, int tagId)
        {
            _noteManager.AddTagToNote(noteId, tagId);

            return RedirectToActionPermanent("EditNote", new { id = noteId });
        }
        
        [HttpPost]
        public IActionResult RemoveTagFromNote(int noteId, int tagId)
        {
            _noteManager.RemoveTagFromNote(noteId, tagId);

            return RedirectToActionPermanent("EditNote", new { id = noteId });
        }


        [HttpGet]
        public IActionResult TagList()
        {
            var tags = _noteManager.GetAllTags();

            var tagViewModels = _mapper.Map<IEnumerable<Tag>, List<TagViewModel>>(tags); 
                
            var viewModel = new TagListViewModel() { AllTags = tagViewModels };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTag(string tagName)
        {
            var tag = new TagCreateData() { Name = tagName };

            await _noteManager.AddTagAsync(tag);

            return RedirectToActionPermanent("TagList");
        }

        [HttpPost]
        public IActionResult DeleteTag(int id)
        {
            _noteManager.DeleteTagById(id);

            return RedirectToActionPermanent("TagList");
        }
    }
}