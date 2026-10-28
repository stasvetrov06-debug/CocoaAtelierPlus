namespace CocoaAtelierPlus.Models
{
    public class ProductItem
    {
        public string Name { get; set; } = string.Empty;
        public decimal QtyPerUnit { get; set; }
    }

    public class SupplierItem
    {
        public int SupplierId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public decimal PurchasePrice { get; set; }
        public int DeliveryDays { get; set; }
    }
}
