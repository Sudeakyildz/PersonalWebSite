using System.ComponentModel.DataAnnotations;

namespace PersonalWebSite.Web.Models
{
    public class AnswerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cevap içeriği zorunludur.")]
        public string Content { get; set; } = string.Empty;

        public int QuestionId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
} 