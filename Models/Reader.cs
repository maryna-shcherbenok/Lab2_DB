using System.ComponentModel.DataAnnotations;

namespace Lab2_DB.Models
{
    public partial class Reader
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "ПІБ читача є обов’язковим.")]
        [StringLength(100, ErrorMessage = "ПІБ не може перевищувати 100 символів.")]
        [Display(Name = "ПІБ читача")]
        public string? FullNameReader { get; set; }

        [Required(ErrorMessage = "Номер телефону є обов’язковим.")]
        [Phone(ErrorMessage = "Некоректний формат номера телефону.")]
        [Display(Name = "Номер телефону")]
        public string? PhoneNumberReader { get; set; }

        [Required(ErrorMessage = "Email є обов’язковим.")]
        [EmailAddress(ErrorMessage = "Некоректний формат email адреси.")]
        [Display(Name = "Email")]
        public string? EmailReader { get; set; }

        [Required(ErrorMessage = "Роль є обов’язковою.")]
        [Display(Name = "Роль (студент, викладач, інше)")]
        [StringLength(100, ErrorMessage = "Роль не може перевищувати 100 символів.")]
        public string? RoleReader { get; set; }

        [Required(ErrorMessage = "Місце навчання або роботи є обов’язковим.")]
        [Display(Name = "Місце навчання або роботи")]
        [StringLength(200, ErrorMessage = "Назва місця не може перевищувати 200 символів.")]
        public string? PlaceStudyOrWorkReader { get; set; }

        [Display(Name = "Запити")]
        public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    }
}
