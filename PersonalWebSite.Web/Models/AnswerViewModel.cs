using System.ComponentModel.DataAnnotations;

namespace PersonalWebSite.Web.Models
{
    public class AnswerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cevap içeriği zorunludur.")]
        [Display(Name = "Cevabınız")]
        public string Content { get; set; } = string.Empty;

        public int QuestionId { get; set; }

        /// <summary>Herkese gösterilen ad (e-posta adresi değil).</summary>
        public string AuthorName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateAnswerViewModel
    {
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Cevap içeriği zorunludur.")]
        [Display(Name = "Cevabınız")]
        public string Content { get; set; } = string.Empty;

        // Sadece görüntüleme için; sunucu tarafında doldurulur.
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionContent { get; set; } = string.Empty;
    }
}
