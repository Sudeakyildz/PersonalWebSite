using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalWebSite.Business.Interfaces;
using PersonalWebSite.Core.Models;

namespace PersonalWebSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;

        public AnswersController(IAnswerService answerService, IQuestionService questionService)
        {
            _answerService = answerService;
            _questionService = questionService;
        }

        // GET: api/Answers
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Answer>>> GetAnswers()
        {
            var answers = await _answerService.GetAllAnswersAsync();
            return Ok(answers);
        }

        // GET: api/Answers/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Answer>> GetAnswer(int id)
        {
            var answer = await _answerService.GetAnswerByIdAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            return Ok(answer);
        }

        // POST: api/Answers
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Answer>> CreateAnswer(Answer answer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var question = await _questionService.GetQuestionByIdAsync(answer.QuestionId);
            if (question == null)
            {
                return BadRequest("Soru bulunamadı.");
            }

            await _answerService.CreateAnswerAsync(answer);
            return CreatedAtAction(nameof(GetAnswer), new { id = answer.Id }, answer);
        }

        // PUT: api/Answers/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAnswer(int id, Answer answer)
        {
            if (id != answer.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAnswer = await _answerService.GetAnswerByIdAsync(id);
            if (existingAnswer == null)
            {
                return NotFound();
            }

            await _answerService.UpdateAnswerAsync(answer);
            return NoContent();
        }

        // DELETE: api/Answers/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnswer(int id)
        {
            var answer = await _answerService.GetAnswerByIdAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            await _answerService.DeleteAnswerAsync(id);
            return NoContent();
        }
    }
} 