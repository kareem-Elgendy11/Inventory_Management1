using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApp_v2.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? sku { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int LowStokThreshold { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<PurchaseItem>? purchaseItems { get; set; }
        public List<SaleItem>? saleItems { get; set; }
    }
}