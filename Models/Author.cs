using System.ComponentModel.DataAnnotations;

namespace Lab2_DB.Models
{
    public partial class Author
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "ПІБ автора є обов’язковим.")]
        [StringLength(100, ErrorMessage = "ПІБ автора не може перевищувати 100 символів.")]
        [Display(Name = "ПІБ автора")]
        public string? FullNameAuthor { get; set; }

        [Required(ErrorMessage = "Номер телефону є обов’язковим.")]
        [Phone(ErrorMessage = "Некоректний формат номера телефону.")]
        [Display(Name = "Номер телефону")]
        public string? PhoneNumberAuthor { get; set; }

        [Required(ErrorMessage = "Email є обов’язковим.")]
        [EmailAddress(ErrorMessage = "Некоректний формат email адреси.")]
        [Display(Name = "Email")]
        public string? EmailAuthor { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
