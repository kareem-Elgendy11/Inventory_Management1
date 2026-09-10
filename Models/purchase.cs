using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApp_v2.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public DateTime PurchaseDate { get; set; }

        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public decimal TotalAmount { get; set; }

        public List<PurchaseItem> PurchaseItems { get; set; } = new();
    }
}