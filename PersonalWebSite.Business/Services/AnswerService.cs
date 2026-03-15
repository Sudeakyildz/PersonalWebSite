using Microsoft.EntityFrameworkCore;
using PersonalWebSite.Business.Interfaces;
using PersonalWebSite.Core.Models;
using PersonalWebSite.Data;

namespace PersonalWebSite.Business.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly PersonalWebSiteDbContext _context;

        public AnswerService(PersonalWebSiteDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Answer>> GetAllAnswersAsync()
        {
            return await _context.Answers
                .Include(a => a.User)
                .Include(a => a.Question)
                .ToListAsync();
        }

        public async Task<Answer> GetAnswerByIdAsync(int id)
        {
            var answer = await _context.Answers
                .Include(a => a.User)
                .Include(a => a.Question)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (answer == null)
            {
                throw new KeyNotFoundException($"ID'si {id} olan cevap bulunamadı.");
            }

            return answer;
        }

        public async Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            return await _context.Answers
                .Include(a => a.User)
                .Where(a => a.QuestionId == questionId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Answer> CreateAnswerAsync(Answer answer)
        {
            answer.CreatedAt = DateTime.UtcNow;
            answer.IsActive = true;

            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            return answer;
        }

        public async Task<Answer> UpdateAnswerAsync(Answer answer)
        {
            var existingAnswer = await _context.Answers.FindAsync(answer.Id);
            if (existingAnswer == null)
            {
                throw new KeyNotFoundException($"ID'si {answer.Id} olan cevap bulunamadı.");
            }

            existingAnswer.Content = answer.Content;
            existingAnswer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingAnswer;
        }

        public async Task DeleteAnswerAsync(int id)
        {
            var answer = await _context.Answers.FindAsync(id);
            if (answer == null)
            {
                throw new KeyNotFoundException($"ID'si {id} olan cevap bulunamadı.");
            }

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
        }
    }
} 