using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PersonalWebSite.Business.Interfaces;
using PersonalWebSite.Core.Models;
using PersonalWebSite.Web.Models;

namespace PersonalWebSite.Web.Controllers
{
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

        // GET: Answer/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int questionId)
        {
            var question = await _questionService.GetQuestionByIdAsync(questionId);
            if (question == null)
            {
                return NotFound();
            }

            ViewBag.Question = question;
            ViewBag.IsAdmin = true;
            return View();
        }

        // POST: Answer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int questionId, [Bind("Content")] Answer answer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(answer.Content))
                {
                    ModelState.AddModelError("Content", "Cevap içeriği boş olamaz.");
                    var existingQuestion = await _questionService.GetQuestionByIdAsync(questionId);
                    ViewBag.Question = existingQuestion;
                    ViewBag.IsAdmin = true;
                    return View(answer);
                }

                var question = await _questionService.GetQuestionByIdAsync(questionId);
                if (question == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }

                answer.QuestionId = questionId;
                answer.UserId = currentUser.Id;
                answer.CreatedAt = DateTime.UtcNow;
                answer.IsActive = true;

                await _answerService.CreateAnswerAsync(answer);
                TempData["SuccessMessage"] = "Cevabınız başarıyla gönderildi.";
                return RedirectToAction("Details", "Question", new { id = questionId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Cevap gönderilirken bir hata oluştu: " + ex.Message);
                var existingQuestion = await _questionService.GetQuestionByIdAsync(questionId);
                ViewBag.Question = existingQuestion;
                ViewBag.IsAdmin = true;
                return View(answer);
            }
        }

        // GET: Answer/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var answer = await _answerService.GetAnswerByIdAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            var viewModel = new AnswerViewModel
            {
                Id = answer.Id,
                Content = answer.Content,
                QuestionId = answer.QuestionId,
                UserName = answer.User.UserName,
                CreatedAt = answer.CreatedAt,
                UpdatedAt = answer.UpdatedAt,
                IsActive = answer.IsActive
            };

            return View(viewModel);
        }

        // POST: Answer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content")] AnswerViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAnswer = await _answerService.GetAnswerByIdAsync(id);
                    if (existingAnswer == null)
                    {
                        return NotFound();
                    }

                    existingAnswer.Content = viewModel.Content;
                    existingAnswer.UpdatedAt = DateTime.UtcNow;

                    await _answerService.UpdateAnswerAsync(existingAnswer);
                    TempData["SuccessMessage"] = "Cevabınız başarıyla güncellendi.";
                    return RedirectToAction("Details", "Question", new { id = existingAnswer.QuestionId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Cevap güncellenirken bir hata oluştu: " + ex.Message);
                }
            }
            return View(viewModel);
        }

        // POST: Answer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] AnswerController.Delete tetiklendi. id: {id}");
                
                var answer = await _answerService.GetAnswerByIdAsync(id);
                if (answer == null)
                {
                    System.Diagnostics.Debug.WriteLine("[DEBUG] Silinecek cevap bulunamadı.");
                    return NotFound();
                }

                var questionId = answer.QuestionId;
                await _answerService.DeleteAnswerAsync(id);
                TempData["SuccessMessage"] = "Cevap başarıyla silindi.";
                return RedirectToAction("Details", "Question", new { id = questionId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] Silme hatası: {ex.Message}");
                TempData["ErrorMessage"] = "Cevap silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Details", "Question", new { id = id });
            }
        }

        // Test için GET action
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTest(int id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] AnswerController.DeleteTest tetiklendi. id: {id}");
                
                var answer = await _answerService.GetAnswerByIdAsync(id);
                if (answer == null)
                {
                    return NotFound();
                }

                var questionId = answer.QuestionId;
                await _answerService.DeleteAnswerAsync(id);
                TempData["SuccessMessage"] = "Cevap başarıyla silindi (GET ile test).";
                return RedirectToAction("Details", "Question", new { id = questionId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Cevap silinirken bir hata oluştu: " + ex.Message;
                return RedirectToAction("Details", "Question", new { id = id });
            }
        }
    }
} 