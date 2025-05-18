using System.ComponentModel.DataAnnotations;

namespace Lab2_DB.Models
{
    public partial class Request
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Оберіть бібліотекаря.")]
        [Display(Name = "Бібліотекар")]
        public long PassNumberLibrarian { get; set; }

        [Required(ErrorMessage = "Оберіть читача.")]
        [Display(Name = "Читач")]
        public long CardNumberReader { get; set; }

        [Required(ErrorMessage = "Дата створення запиту є обов’язковою.")]
        [Display(Name = "Дата створення запиту")]
        public DateOnly CreationDateRequest { get; set; }

        [Required(ErrorMessage = "Тип запиту є обов’язковим.")]
        [StringLength(100, ErrorMessage = "Тип запиту не може перевищувати 100 символів.")]
        [Display(Name = "Тип запиту")]
        public string? RequestType { get; set; }

        [Required(ErrorMessage = "Статус запиту є обов’язковим.")]
        [StringLength(50, ErrorMessage = "Статус не може перевищувати 50 символів.")]
        [Display(Name = "Статус запиту")]
        public string? RequestStatus { get; set; } 

        [Required(ErrorMessage = "ISBN є обов’язковим.")]
        [Display(Name = "ISBN книги")]
        public long Isbn { get; set; }

        [Display(Name = "Читач")]
        public virtual Reader CardNumberReaderNavigation { get; set; } = null!;

        [Display(Name = "Видані книги")]
        public virtual ICollection<IssuedBook> IssuedBooks { get; set; } = new List<IssuedBook>();

        [Display(Name = "Бібліотекар")]
        public virtual Librarian PassNumberLibrarianNavigation { get; set; } = null!;
    }
}
