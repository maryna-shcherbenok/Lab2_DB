using System.ComponentModel.DataAnnotations;

namespace Lab2_DB.Models
{
    public partial class Librarian
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "ПІБ бібліотекаря є обов’язковим.")]
        [StringLength(100, ErrorMessage = "ПІБ не може перевищувати 100 символів.")]
        [Display(Name = "ПІБ бібліотекаря")]
        public string? FullNameLibrarian { get; set; }

        [Required(ErrorMessage = "Номер телефону є обов’язковим.")]
        [Phone(ErrorMessage = "Некоректний формат номера телефону.")]
        [Display(Name = "Номер телефону")]
        public string? PhoneNumberLibrarian { get; set; }

        [Required(ErrorMessage = "Email є обов’язковим.")]
        [EmailAddress(ErrorMessage = "Некоректний формат email адреси.")]
        [Display(Name = "Email")]
        public string? EmailLibrarian { get; set; }

        [Display(Name = "Запити")]
        public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}
