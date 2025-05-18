using System.ComponentModel.DataAnnotations;

namespace Lab2_DB.Models
{
    public partial class Book
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "ISBN є обов’язковим.")]
        [Range(1000000000000, 9999999999999, ErrorMessage = "ISBN повинен бути числом із 13 цифр.")]
        [Display(Name = "ISBN")]
        public long Isbn { get; set; }

        [Required(ErrorMessage = "Назва книги є обов’язковою.")]
        [StringLength(200, ErrorMessage = "Назва книги не може перевищувати 200 символів.")]
        [Display(Name = "Назва книги")]
        public string? TitleBook { get; set; }

        [Required(ErrorMessage = "Автор є обов’язковим.")]
        [Display(Name = "Автор")]
        public long AuthorBook { get; set; }

        [Required(ErrorMessage = "Жанр є обов’язковим.")]
        [StringLength(100, ErrorMessage = "Жанр не може перевищувати 100 символів.")]
        [Display(Name = "Жанр")]
        public string? GenreBook { get; set; }

        [Required(ErrorMessage = "Статус доступності є обов’язковим.")]
        [StringLength(100)]
        [Display(Name = "Статус доступності")]
        public string? AvailabilityStatusBook { get; set; }

        [Required(ErrorMessage = "Кількість сторінок є обов’язковою.")]
        [Range(1, 10000, ErrorMessage = "Кількість сторінок має бути від 1 до 10000.")]
        [Display(Name = "Кількість сторінок")]
        public int NumberPages { get; set; }

        [Required(ErrorMessage = "Фонд є обов’язковим.")]
        [Display(Name = "Фонд")]
        public long FundBook { get; set; }

        [Required(ErrorMessage = "Видавництво є обов’язковим.")]
        [Display(Name = "Видавництво")]
        public int PublisherBook { get; set; }

        [Display(Name = "Автор")]
        public virtual Author AuthorBookNavigation { get; set; } = null!;

        [Display(Name = "Фонд")]
        public virtual Fund FundBookNavigation { get; set; } = null!;

        [Display(Name = "Видані книги")]
        public virtual ICollection<IssuedBook> IssuedBooks { get; set; } = new List<IssuedBook>();

        [Display(Name = "Видавництво")]
        public virtual PublishingHouse PublisherBookNavigation { get; set; } = null!;
    }
}
