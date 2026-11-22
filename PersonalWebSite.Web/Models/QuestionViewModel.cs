using System.ComponentModel.DataAnnotations;
using PersonalWebSite.Core.Models;

namespace PersonalWebSite.Web.Models
{
    public class QuestionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik alanı zorunludur.")]
        [Display(Name = "İçerik")]
        public string Content { get; set; } = string.Empty;

        /// <summary>Herkese gösterilen ad (e-posta adresi değil).</summary>
        public string AuthorName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        /// <summary>Mevcut kullanıcı bu soruyu düzenleyip silebilir mi?</summary>
        public bool CanManage { get; set; }

        public List<AnswerViewModel> Answers { get; set; } = new();
    }

    public class QuestionListViewModel
    {
        public IReadOnlyList<QuestionViewModel> Questions { get; init; } = Array.Empty<QuestionViewModel>();
        public string? Search { get; init; }
        public int Page { get; init; }
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }

    public static class ViewModelMapper
    {
        private const string DeletedUser = "Silinmiş Kullanıcı";

        public static string DisplayName(User? user)
        {
            if (user == null) return DeletedUser;
            var name = $"{user.FirstName} {user.LastName}".Trim();
            return string.IsNullOrWhiteSpace(name) ? "Kullanıcı" : name;
        }

        public static AnswerViewModel ToViewModel(this Answer a) => new()
        {
            Id = a.Id,
            Content = a.Content,
            QuestionId = a.QuestionId,
            AuthorName = DisplayName(a.User),
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt,
            IsActive = a.IsActive
        };

        public static QuestionViewModel ToViewModel(this Question q, string? currentUserId = null, bool isAdmin = false) => new()
        {
            Id = q.Id,
            Title = q.Title,
            Content = q.Content,
            AuthorName = DisplayName(q.User),
            CreatedAt = q.CreatedAt,
            UpdatedAt = q.UpdatedAt,
            IsActive = q.IsActive,
            CanManage = isAdmin || (currentUserId != null && q.UserId == currentUserId),
            Answers = q.Answers?
                .OrderBy(a => a.CreatedAt)
                .Select(a => a.ToViewModel())
                .ToList() ?? new List<AnswerViewModel>()
        };
    }
}
