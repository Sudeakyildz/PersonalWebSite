using Microsoft.EntityFrameworkCore;
using PersonalWebSite.Business.Interfaces;
using PersonalWebSite.Core.Models;
using PersonalWebSite.Data;

namespace PersonalWebSite.Business.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly PersonalWebSiteDbContext _context;

        public QuestionService(PersonalWebSiteDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
        {
            return await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<PagedResult<Question>> GetQuestionsPagedAsync(string? search, int page, int pageSize)
        {
            pageSize = Math.Clamp(pageSize, 1, 50);

            var query = _context.Questions.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(q => q.Title.Contains(term) || q.Content.Contains(term));
            }

            var total = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            page = Math.Clamp(page, 1, totalPages);

            var items = await query
                .Include(q => q.User)
                .Include(q => q.Answers)
                .OrderByDescending(q => q.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Question>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<Question?> GetQuestionByIdAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Question> CreateQuestionAsync(Question question)
        {
            question.CreatedAt = DateTime.UtcNow;
            question.IsActive = true;

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return question;
        }

        public async Task<Question> UpdateQuestionAsync(Question question)
        {
            var existingQuestion = await _context.Questions.FindAsync(question.Id);
            if (existingQuestion == null)
            {
                throw new KeyNotFoundException($"ID'si {question.Id} olan soru bulunamadı.");
            }

            existingQuestion.Title = question.Title;
            existingQuestion.Content = question.Content;
            existingQuestion.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingQuestion;
        }

        public async Task DeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                throw new KeyNotFoundException($"ID'si {id} olan soru bulunamadı.");
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Question>> GetQuestionsByUserIdAsync(string userId)
        {
            return await _context.Questions
                .Where(q => q.UserId == userId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Answer>> GetQuestionAnswersAsync(int questionId)
        {
            return await _context.Answers
                .Include(a => a.User)
                .Where(a => a.QuestionId == questionId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
} 