using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApp_v2.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        [ForeignKey("Purchase")]
        public int PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}