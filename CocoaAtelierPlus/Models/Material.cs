namespace CocoaAtelierPlus.Models
{
    public class Material
    {
        public int MaterialId { get; set; }
        public int MaterialTypeId { get; set; }
        public string MaterialTypeName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal StockQty { get; set; }
        public decimal MinQty { get; set; }
        public decimal PackQty { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal MinBatchQty { get; set; }
        public decimal MinBatchCost { get; set; }
        public decimal RequiredQty { get; set; }
    }
}
