using System.ComponentModel.DataAnnotations;

namespace Lab2_DB.Models
{
    public partial class Department
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Назва відділу є обов’язковою.")]
        [StringLength(100, ErrorMessage = "Назва відділу не може перевищувати 100 символів.")]
        [Display(Name = "Назва відділу")]
        public string? TitleDepartment { get; set; }

        [Required(ErrorMessage = "ПІБ завідувача є обов’язковим.")]
        [StringLength(100, ErrorMessage = "ПІБ завідувача не може перевищувати 100 символів.")]
        [Display(Name = "Завідувач відділу")]
        public string? LeaderDepartment { get; set; }

        [Required(ErrorMessage = "Необхідно вказати бібліотеку.")]
        [Display(Name = "Ідентифікатор бібліотеки")]
        public long Library { get; set; }

        [Display(Name = "Фонди")]
        public virtual ICollection<Fund> Funds { get; set; } = new List<Fund>();

        [Display(Name = "Бібліотека")]
        public virtual Library LibraryNavigation { get; set; } = null!;
    }
}
