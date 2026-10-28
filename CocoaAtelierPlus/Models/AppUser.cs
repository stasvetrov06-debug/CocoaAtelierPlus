using System.ComponentModel.DataAnnotations;

namespace CocoaAtelierPlus.Models
{
    public class LoginModel
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Укажите логин")]
        public string Login { get; set; } = string.Empty;

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Укажите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class AppUser
    {
        public int UserId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class UserForm
    {
        public int? UserId { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Укажите логин")]
        [StringLength(50)]
        public string Login { get; set; } = string.Empty;

        [Display(Name = "ФИО")]
        [Required(ErrorMessage = "Укажите ФИО")]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Укажите email")]
        [EmailAddress(ErrorMessage = "Укажите корректный адрес электронной почты")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Укажите пароль")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Пароль должен содержать от 5 до 100 символов")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Роль")]
        [Required(ErrorMessage = "Выберите роль из списка")]
        [Range(1, 3, ErrorMessage = "Выберите роль из списка")]
        public int? RoleId { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        public List<RoleItem> Roles { get; set; } = new();
    }
}
