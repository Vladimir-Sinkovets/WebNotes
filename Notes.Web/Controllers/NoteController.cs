using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Notes.BLL.Services.CurrentUserAccessor.Exceptions;
using Notes.BLL.Services.MarkdownRendererService;
using Notes.BLL.Services.NoteManagers;
using Notes.BLL.Services.NoteManagers.Exceptions;
using Notes.BLL.Services.NoteManagers.Models;
using Notes.Web.Models.Note;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Web.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMarkdownRenderer _markdownRenderer;
        private readonly INoteManager _noteManager;

        public NoteController(INoteManager notesManager, IMapper mapper, IMarkdownRenderer markdownRenderer)
        {
            _noteManager = notesManager;
            _mapper = mapper;
            _markdownRenderer = markdownRenderer;
        }

        [HttpGet]
        public ActionResult NoteList(int page = 1)
        {
            const int NotesInPage = 10;

            IEnumerable<Note> notes = _noteManager.GetAllNotes();

            int lastPage = (int)Math.Ceiling((float)notes.Count() / NotesInPage);

            int currentPage = page <= lastPage ? page : lastPage;

            var notesForPage = notes.Skip((currentPage - 1) * NotesInPage)
                .Take(NotesInPage);

            var viewModel = new NoteListViewModel()
            {
                Notes = _mapper.Map<List<ReadNoteViewModel>>(notesForPage),
                CurrentPage = currentPage,
                LastPage = lastPage,
            };

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

                    return RedirectToAction(nameof(Read), new { id = viewModel.Id });
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

        [HttpGet]
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

                var viewModel = _mapper.Map<ReadNoteViewModel>(note);

                var html = _markdownRenderer.RenderFromMarkdownToHTML(note.Text);

                viewModel.HtmlText = html;

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

        public async Task<IActionResult> StarNote(int id, bool isImportant, string returnUrl)
        {
            await _noteManager.SetNoteImportanceAsync(id, !isImportant);

            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult ImportantNoteList(int page = 1)
        {
            const int NotesInPage = 10;

            IEnumerable<Note> notes = _noteManager.GetAllImportantNotes();

            int lastPage = (int)Math.Ceiling((float)notes.Count() / NotesInPage);

            int currentPage = page <= lastPage ? page : lastPage;

            var notesForPage = notes.Skip((currentPage - 1) * NotesInPage)
                .Take(NotesInPage);

            var viewModel = new NoteListViewModel()
            {
                Notes = _mapper.Map<List<ReadNoteViewModel>>(notesForPage),
                CurrentPage = currentPage,
                LastPage = lastPage,
            };

            return View(viewModel);
        }
    }
}