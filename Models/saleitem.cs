using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApp_v2.Models
{
    public class SaleItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        [ForeignKey("Sale")]

        public int SaleId { get; set; }
        public Sale? Sale { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}