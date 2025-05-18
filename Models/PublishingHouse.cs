using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab2_DB.Models
{
    public partial class PublishingHouse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва видавництва є обов’язковою.")]
        [StringLength(100, ErrorMessage = "Назва видавництва не може перевищувати 100 символів.")]
        [Display(Name = "Назва видавництва")]
        public string? TitlePh { get; set; }

        [Required(ErrorMessage = "Номер телефону є обов’язковим.")]
        [Phone(ErrorMessage = "Некоректний формат номера телефону.")]
        [Display(Name = "Номер телефону")]
        public string? PhoneNumberPh { get; set; }

        [Required(ErrorMessage = "Адреса є обов’язковою.")]
        [StringLength(200, ErrorMessage = "Адреса не може перевищувати 200 символів.")]
        [Display(Name = "Адреса видавництва")]
        public string? AddressPh { get; set; }

        [Required(ErrorMessage = "Email є обов’язковим.")]
        [EmailAddress(ErrorMessage = "Некоректний формат email адреси.")]
        [Display(Name = "Email видавництва")]
        public string? EmailPh { get; set; }

        [Display(Name = "Книги видавництва")]
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
