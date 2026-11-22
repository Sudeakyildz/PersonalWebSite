using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalWebSite.Business.Interfaces;
using PersonalWebSite.Core.Models;
using PersonalWebSite.Web.Models;

namespace PersonalWebSite.Web.Controllers
{
    // Sadece Admin cevap yazabilir, düzenleyebilir ve silebilir.
    [Authorize(Roles = "Admin")]
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;
        private readonly UserManager<User> _userManager;

        public AnswerController(
            IAnswerService answerService,
            IQuestionService questionService,
            UserManager<User> userManager)
        {
            _answerService = answerService;
            _questionService = questionService;
            _userManager = userManager;
        }

        // GET: Answer/Create?questionId=5
        [HttpGet]
        public async Task<IActionResult> Create(int questionId)
        {
            var question = await _questionService.GetQuestionByIdAsync(questionId);
            if (question == null)
            {
                return NotFound();
            }

            return View(new CreateAnswerViewModel
            {
                QuestionId = questionId,
                QuestionTitle = question.Title,
                QuestionContent = question.Content
            });
        }

        // POST: Answer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnswerViewModel model)
        {
            var question = await _questionService.GetQuestionByIdAsync(model.QuestionId);
            if (question == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Content))
            {
                if (string.IsNullOrWhiteSpace(model.Content))
                {
                    ModelState.AddModelError(nameof(model.Content), "Cevap içeriği boş olamaz.");
                }
                model.QuestionTitle = question.Title;
                model.QuestionContent = question.Content;
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            await _answerService.CreateAnswerAsync(new Answer
            {
                QuestionId = model.QuestionId,
                UserId = currentUser.Id,
                Content = model.Content.Trim()
            });

            TempData["SuccessMessage"] = "Cevabınız başarıyla gönderildi.";
            return RedirectToAction("Details", "Question", new { id = model.QuestionId });
        }

        // GET: Answer/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var answer = await _answerService.GetAnswerByIdAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            return View(answer.ToViewModel());
        }

        // POST: Answer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content")] AnswerViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            var existingAnswer = await _answerService.GetAnswerByIdAsync(id);
            if (existingAnswer == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(viewModel.Content))
            {
                viewModel.QuestionId = existingAnswer.QuestionId;
                viewModel.AuthorName = ViewModelMapper.DisplayName(existingAnswer.User);
                return View(viewModel);
            }

            existingAnswer.Content = viewModel.Content.Trim();
            await _answerService.UpdateAnswerAsync(existingAnswer);

            TempData["SuccessMessage"] = "Cevap güncellendi.";
            return RedirectToAction("Details", "Question", new { id = existingAnswer.QuestionId });
        }

        // POST: Answer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var answer = await _answerService.GetAnswerByIdAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            var questionId = answer.QuestionId;
            await _answerService.DeleteAnswerAsync(id);

            TempData["SuccessMessage"] = "Cevap silindi.";
            return RedirectToAction("Details", "Question", new { id = questionId });
        }
    }
}
