using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2_DB.Models
{
    public partial class Fund
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Назва фонду є обов’язковою.")]
        [StringLength(100, ErrorMessage = "Назва фонду не може перевищувати 100 символів.")]
        [Display(Name = "Назва фонду")]
        public string? TitleFund { get; set; }

        [Required(ErrorMessage = "Тип фонду є обов’язковим.")]
        [StringLength(50, ErrorMessage = "Тип фонду не може перевищувати 50 символів.")]
        [Display(Name = "Тип фонду")]
        public string? TypeFund { get; set; }

        [Required(ErrorMessage = "Загальна кількість примірників є обов’язковою.")]
        [Range(0, int.MaxValue, ErrorMessage = "Кількість примірників має бути невід'ємним числом.")]
        [Display(Name = "Загальна кількість примірників")]
        public int TotalNumberCopies { get; set; }

        [Required(ErrorMessage = "Необхідно вказати відділ.")]
        [Display(Name = "Ідентифікатор відділу")]
        public long DepartmentFund { get; set; }

        [Display(Name = "Книги")]
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

        [Display(Name = "Відділ")]
        public virtual Department DepartmentFundNavigation { get; set; } = null!;
    }
}
