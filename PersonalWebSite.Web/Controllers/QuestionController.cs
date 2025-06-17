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
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly UserManager<User> _userManager;

        public QuestionController(
            IQuestionService questionService,
            IAnswerService answerService,
            UserManager<User> userManager)
        {
            _questionService = questionService;
            _answerService = answerService;
            _userManager = userManager;
        }

        // API Endpoints
        [HttpGet("api")]
        [Produces("application/json")]
        public async Task<IActionResult> GetQuestionsApi()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Json(questions);
        }

        [HttpGet("api/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetQuestionApi(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
                return NotFound();
            return Json(question);
        }

        [HttpGet("api/{id}/answers")]
        [Produces("application/json")]
        public async Task<IActionResult> GetQuestionAnswersApi(int id)
        {
            var answers = await _questionService.GetQuestionAnswersAsync(id);
            return Json(answers);
        }

        // GET: Question
        [HttpGet]
        [Route("")]
        [Route("Index")]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            var viewModels = questions.Select(q => new QuestionViewModel
            {
                Id = q.Id,
                Title = q.Title,
                Content = q.Content,
                UserName = q.User?.UserName ?? "Silinmiş Kullanıcı",
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
                IsActive = q.IsActive,
                Answers = q.Answers?.Select(a => new AnswerViewModel
                {
                    Id = a.Id,
                    Content = a.Content,
                    QuestionId = a.QuestionId,
                    UserName = a.User?.UserName ?? "Silinmiş Kullanıcı",
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    IsActive = a.IsActive
                }).ToList() ?? new List<AnswerViewModel>()
            });
            return View(viewModels);
        }

        // GET: Question/Details/5
        [HttpGet]
        [Route("Details/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            var viewModel = new QuestionViewModel
            {
                Id = question.Id,
                Title = question.Title,
                Content = question.Content,
                UserName = question.User?.UserName ?? "Silinmiş Kullanıcı",
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt,
                IsActive = question.IsActive,
                Answers = question.Answers?.Select(a => new AnswerViewModel
                {
                    Id = a.Id,
                    Content = a.Content,
                    QuestionId = a.QuestionId,
                    UserName = a.User?.UserName ?? "Silinmiş Kullanıcı",
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    IsActive = a.IsActive
                }).ToList() ?? new List<AnswerViewModel>()
            };

            return View(viewModel);
        }

        // GET: Question/Create
        [HttpGet]
        [Route("Create")]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Question/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Title,Content")] QuestionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                var question = new Question
                {
                    Title = viewModel.Title,
                    Content = viewModel.Content,
                    UserId = currentUser.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _questionService.CreateQuestionAsync(question);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Question/Edit/5
        [HttpGet]
        [Route("Edit/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || (question.UserId != currentUser.Id && !User.IsInRole("Admin")))
            {
                return Unauthorized();
            }

            var viewModel = new QuestionViewModel
            {
                Id = question.Id,
                Title = question.Title,
                Content = question.Content,
                UserName = question.User?.UserName ?? "Silinmiş Kullanıcı",
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt,
                IsActive = question.IsActive
            };

            return View(viewModel);
        }

        // POST: Question/Edit/5
        [HttpPost]
        [Route("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content")] QuestionViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingQuestion = await _questionService.GetQuestionByIdAsync(id);
                if (existingQuestion == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || (existingQuestion.UserId != currentUser.Id && !User.IsInRole("Admin")))
                {
                    return Unauthorized();
                }

                existingQuestion.Title = viewModel.Title;
                existingQuestion.Content = viewModel.Content;
                existingQuestion.UpdatedAt = DateTime.UtcNow;

                await _questionService.UpdateQuestionAsync(existingQuestion);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost]
        [Route("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || (question.UserId != currentUser.Id && !User.IsInRole("Admin")))
            {
                return Unauthorized();
            }

            await _questionService.DeleteQuestionAsync(id);
            TempData["SuccessMessage"] = "Soru başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("DeleteTest/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] QuestionController.DeleteTest tetiklendi. id: {id}");
                
                var question = await _questionService.GetQuestionByIdAsync(id);
                if (question == null)
                {
                    return NotFound();
                }

                await _questionService.DeleteQuestionAsync(id);
                TempData["SuccessMessage"] = "Soru başarıyla silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Soru silme hatası: {ex.Message}");
                TempData["ErrorMessage"] = "Soru silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 