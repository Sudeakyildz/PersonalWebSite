using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalWebSite.Business.Interfaces;
using PersonalWebSite.Core.Models;

namespace PersonalWebSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;

        public QuestionsController(IQuestionService questionService, IAnswerService answerService)
        {
            _questionService = questionService;
            _answerService = answerService;
        }

        // GET: api/Questions
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            var questions = await _questionService.GetAllQuestionsAsync();
            return Ok(questions);
        }

        // GET: api/Questions/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }

        // GET: api/Questions/5/Answers
        [HttpGet("{id}/Answers")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Answer>>> GetQuestionAnswers(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            var answers = await _answerService.GetAnswersByQuestionIdAsync(id);
            return Ok(answers);
        }

        // POST: api/Questions
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Question>> CreateQuestion(Question question)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _questionService.CreateQuestionAsync(question);
            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }

        // PUT: api/Questions/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateQuestion(int id, Question question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingQuestion = await _questionService.GetQuestionByIdAsync(id);
            if (existingQuestion == null)
            {
                return NotFound();
            }

            await _questionService.UpdateQuestionAsync(question);
            return NoContent();
        }

        // DELETE: api/Questions/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            await _questionService.DeleteQuestionAsync(id);
            return NoContent();
        }
    }
} 