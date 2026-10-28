using System.ComponentModel.DataAnnotations;

namespace CocoaAtelierPlus.Models
{
    public class MaterialForm
    {
        public int? MaterialId { get; set; }

        [Display(Name = "Тип материала")]
        [Required(ErrorMessage = "Выберите тип материала из списка")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите тип материала из списка")]
        public int? MaterialTypeId { get; set; }

        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Укажите наименование материала")]
        [StringLength(150, ErrorMessage = "Наименование не должно превышать 150 символов")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Закупочная цена единицы материала")]
        [Required(ErrorMessage = "Укажите закупочную цену единицы материала")]
        [RegularExpression(@"^\d+([.,]\d{1,2})?$", ErrorMessage = "Закупочная цена может содержать не более двух знаков после запятой")]
        [Range(0, double.MaxValue, ErrorMessage = "Закупочная цена не может быть отрицательной")]
        public decimal? UnitPrice { get; set; }

        [Display(Name = "Единица измерения")]
        [Required(ErrorMessage = "Укажите единицу измерения")]
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;

        [Display(Name = "Количество в упаковке")]
        [Required(ErrorMessage = "Укажите количество в упаковке")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Количество в упаковке должно быть больше нуля")]
        public decimal? PackQty { get; set; }

        [Display(Name = "Количество на складе")]
        [Required(ErrorMessage = "Укажите количество на складе")]
        [Range(0, double.MaxValue, ErrorMessage = "Количество на складе не может быть отрицательным")]
        public decimal? StockQty { get; set; }

        [Display(Name = "Минимальное количество")]
        [Required(ErrorMessage = "Укажите минимальное количество")]
        [Range(0, double.MaxValue, ErrorMessage = "Минимальное количество не может быть отрицательным")]
        public decimal? MinQty { get; set; }

        public List<MaterialTypeItem> Types { get; set; } = new();
    }

    public class MaterialTypeItem
    {
        public int MaterialTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
