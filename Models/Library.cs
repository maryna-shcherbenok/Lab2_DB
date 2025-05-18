using System.ComponentModel.DataAnnotations;

namespace Lab2_DB.Models
{
    public partial class Library
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Назва бібліотеки є обов’язковою.")]
        [StringLength(100, ErrorMessage = "Назва не може перевищувати 100 символів.")]
        [Display(Name = "Назва бібліотеки")]
        public string? TitleLibrary { get; set; }

        [Required(ErrorMessage = "Номер телефону є обов’язковим.")]
        [Phone(ErrorMessage = "Некоректний формат номера телефону.")]
        [Display(Name = "Номер телефону")]
        public string? PhoneNumberLibrary { get; set; }

        [Required(ErrorMessage = "Email є обов’язковим.")]
        [EmailAddress(ErrorMessage = "Некоректний формат email адреси.")]
        [Display(Name = "Email")]
        public string? EmailLibrary { get; set; }

        [Required(ErrorMessage = "Адреса бібліотеки є обов’язковою.")]
        [StringLength(200, ErrorMessage = "Адреса не може перевищувати 200 символів.")]
        [Display(Name = "Адреса")]
        public string? LibraryAddress { get; set; }

        [Required(ErrorMessage = "ПІБ керівника є обов’язковим.")]
        [StringLength(100, ErrorMessage = "ПІБ не може перевищувати 100 символів.")]
        [Display(Name = "Керівник бібліотеки")]
        public string? LeaderLibrary { get; set; } 

        [Display(Name = "Відділи")]
        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
