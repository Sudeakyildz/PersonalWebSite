using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalWebSite.Business.Interfaces;
using PersonalWebSite.Core.Models;
using PersonalWebSite.Web.Models;

namespace PersonalWebSite.Web.Controllers
{
    [Route("Question")]
    public class QuestionController : Controller
    {
        private const int PageSize = 6;

        private readonly IQuestionService _questionService;
        private readonly UserManager<User> _userManager;

        public QuestionController(IQuestionService questionService, UserManager<User> userManager)
        {
            _questionService = questionService;
            _userManager = userManager;
        }

        // ---------- JSON API (herkese açık, hassas alan içermez) ----------

        [HttpGet("api")]
        [Produces("application/json")]
        public async Task<IActionResult> GetQuestionsApi()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Json(questions.Select(q => new
            {
                q.Id,
                q.Title,
                q.Content,
                Author = ViewModelMapper.DisplayName(q.User),
                q.CreatedAt,
                q.UpdatedAt,
                AnswerCount = q.Answers.Count
            }));
        }

        [HttpGet("api/{id:int}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetQuestionApi(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
                return NotFound();

            return Json(new
            {
                question.Id,
                question.Title,
                question.Content,
                Author = ViewModelMapper.DisplayName(question.User),
                question.CreatedAt,
                question.UpdatedAt,
                Answers = question.Answers.OrderBy(a => a.CreatedAt).Select(a => new
                {
                    a.Id,
                    a.Content,
                    Author = ViewModelMapper.DisplayName(a.User),
                    a.CreatedAt,
                    a.UpdatedAt
                })
            });
        }

        [HttpGet("api/{id:int}/answers")]
        [Produces("application/json")]
        public async Task<IActionResult> GetQuestionAnswersApi(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
                return NotFound();

            var answers = await _questionService.GetQuestionAnswersAsync(id);
            return Json(answers.Select(a => new
            {
                a.Id,
                a.Content,
                Author = ViewModelMapper.DisplayName(a.User),
                a.CreatedAt,
                a.UpdatedAt
            }));
        }

        // ---------- MVC ----------

        // GET: Question?search=...&page=1
        [HttpGet("")]
        [HttpGet("Index")]
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            var result = await _questionService.GetQuestionsPagedAsync(search, page, PageSize);

            var model = new QuestionListViewModel
            {
                Questions = result.Items.Select(q => q.ToViewModel()).ToList(),
                Search = search,
                Page = result.Page,
                TotalPages = result.TotalPages,
                TotalCount = result.TotalCount
            };
            return View(model);
        }

        // GET: Question/Details/5
        [HttpGet("Details/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            return View(question.ToViewModel(userId, User.IsInRole("Admin")));
        }

        // GET: Question/Create
        [HttpGet("Create")]
        [Authorize]
        public IActionResult Create()
        {
            return View(new QuestionViewModel());
        }

        // POST: Question/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Title,Content")] QuestionViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var question = await _questionService.CreateQuestionAsync(new Question
            {
                Title = viewModel.Title.Trim(),
                Content = viewModel.Content.Trim(),
                UserId = currentUser.Id
            });

            TempData["SuccessMessage"] = "Sorunuz başarıyla yayınlandı.";
            return RedirectToAction(nameof(Details), new { id = question.Id });
        }

        // GET: Question/Edit/5
        [HttpGet("Edit/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            if (!await CanManageAsync(question))
            {
                return Forbid();
            }

            return View(question.ToViewModel());
        }

        // POST: Question/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content")] QuestionViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            var existingQuestion = await _questionService.GetQuestionByIdAsync(id);
            if (existingQuestion == null)
            {
                return NotFound();
            }

            if (!await CanManageAsync(existingQuestion))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            existingQuestion.Title = viewModel.Title.Trim();
            existingQuestion.Content = viewModel.Content.Trim();

            await _questionService.UpdateQuestionAsync(existingQuestion);
            TempData["SuccessMessage"] = "Soru güncellendi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Question/Delete/5
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            if (!await CanManageAsync(question))
            {
                return Forbid();
            }

            await _questionService.DeleteQuestionAsync(id);
            TempData["SuccessMessage"] = "Soru başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> CanManageAsync(Question question)
        {
            var userId = _userManager.GetUserId(User);
            var allowed = User.IsInRole("Admin") || (userId != null && question.UserId == userId);
            return Task.FromResult(allowed);
        }
    }
}
