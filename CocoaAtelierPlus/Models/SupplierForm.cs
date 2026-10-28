using System.ComponentModel.DataAnnotations;

namespace CocoaAtelierPlus.Models
{
    public class SupplierForm
    {
        public int? SupplierId { get; set; }

        [Display(Name = "Тип поставщика")]
        [Required(ErrorMessage = "Укажите тип поставщика")]
        public string SupplierType { get; set; } = "ООО";

        [Display(Name = "Наименование поставщика")]
        [Required(ErrorMessage = "Укажите наименование поставщика")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "ИНН")]
        [Required(ErrorMessage = "Укажите ИНН")]
        [RegularExpression(@"^(\d{10}|\d{12})$", ErrorMessage = "ИНН должен содержать 10 или 12 цифр")]
        public string Inn { get; set; } = string.Empty;

        [Display(Name = "Рейтинг")]
        [Required(ErrorMessage = "Укажите рейтинг поставщика")]
        [Range(1, 5, ErrorMessage = "Рейтинг должен быть в диапазоне от 1 до 5")]
        public int? Rating { get; set; } = 5;

        [Display(Name = "Дата начала работы")]
        [Required(ErrorMessage = "Укажите дату начала работы")]
        [RegularExpression(@"^\d{2}\.\d{2}\.\d{4}$", ErrorMessage = "Укажите дату в формате ДД.ММ.ГГГГ")]
        public string StartDate { get; set; } = string.Empty;
    }
}
