using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Notes.BLL.Services.CurrentUserAccessor.Exceptions;
using Notes.BLL.Services.NoteManagers;
using Notes.BLL.Services.NoteManagers.Exceptions;
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

            IEnumerable<ListNoteItemViewModel> viewModel = _mapper.Map<IEnumerable<Note>, List<ListNoteItemViewModel>>(notes);

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

            try
            {
                await _noteManager.CreateNewNoteAsync(note);

                return View();
            }
            catch (NotAuthorizedException)
            {
                return RedirectToAction("Register", "Account");
            }
            catch (UserAccessException)
            {
                return StatusCode(403);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public ActionResult EditNote(int id)
        {
            try
            {
                var note = _noteManager.GetNoteById(id);

                var viewModel = _mapper.Map<EditNoteViewModel>(note);

                viewModel.AllTags = GetAllTagsForCurrentUser();
            
                return View(viewModel);
            }
            catch (NotAuthorizedException)
            {
                return RedirectToAction("Register", "Account");
            }
            catch (NotFoundException)
            {
                return StatusCode(404);
            }
            catch (UserAccessException)
            {
                return StatusCode(403);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditNote(EditNoteViewModel viewModel)
        {
            try
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
            catch (NotAuthorizedException)
            {
                return RedirectToAction("Register", "Account");
            }
            catch (NotFoundException)
            {
                return StatusCode(404);
            }
            catch (UserAccessException)
            {
                return StatusCode(403);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IEnumerable<TagItemViewModel> GetAllTagsForCurrentUser()
        {
            var allTags = _noteManager.GetAllTags();

            var allViewModelTags = _mapper.Map<IEnumerable<Tag>, List<TagItemViewModel>>(allTags);

            return allViewModelTags;
        }

        [HttpGet]
        public IActionResult Read(int id)
        {
            try
            {
                var note = _noteManager.GetNoteById(id);

                var viewModel = _mapper.Map<ListNoteItemViewModel>(note);

                return View(viewModel);
            }
            catch (NotAuthorizedException)
            {
                return RedirectToAction("Register", "Account");
            }
            catch (NotFoundException)
            {
                return StatusCode(404);
            }
            catch (UserAccessException)
            {
                return StatusCode(403);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult AddTagToNote(int noteId, int tagId)
        {
            try
            {
                _noteManager.AddTagToNote(noteId, tagId);

                return RedirectToActionPermanent("EditNote", new { id = noteId });
            }
            catch (NotAuthorizedException)
            {
                return RedirectToAction("Register", "Account");
            }
            catch (NotFoundException)
            {
                return StatusCode(404);
            }
            catch (UserAccessException)
            {
                return StatusCode(403);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        [HttpPost]
        public IActionResult RemoveTagFromNote(int noteId, int tagId)
        {
            try
            {
                _noteManager.RemoveTagFromNote(noteId, tagId);

                return RedirectToActionPermanent("EditNote", new { id = noteId });
            }
            catch (NotAuthorizedException)
            {
                return RedirectToAction("Register", "Account");
            }
            catch (NotFoundException)
            {
                return StatusCode(404);
            }
            catch (UserAccessException)
            {
                return StatusCode(403);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        public IActionResult TagList()
        {
            try
            {
                var tags = _noteManager.GetAllTags();

                var tagViewModels = _mapper.Map<IEnumerable<Tag>, List<TagItemViewModel>>(tags); 
                
                var viewModel = new TagListViewModel() { AllTags = tagViewModels };

                return View(viewModel);
            }
            catch (NotAuthorizedException)
            {
                return RedirectToAction("Register", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTag(string tagName)
        {
            try
            {
                var tag = new TagCreateData() { Name = tagName };

                await _noteManager.AddTagAsync(tag);

                return RedirectToActionPermanent("TagList");
            }
            catch (NotAuthorizedException)
            {
                return RedirectToAction("Register", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult DeleteTag(int id)
        {
            try
            {
                _noteManager.DeleteTagById(id);

                return RedirectToActionPermanent("TagList");
            }
            catch (NotAuthorizedException)
            {
                return RedirectToAction("Register", "Account");
            }
            catch (NotFoundException)
            {
                return StatusCode(404);
            }
            catch (UserAccessException)
            {
                return StatusCode(403);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}