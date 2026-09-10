//using System.Net.ServerSentEvents;

namespace InventoryApp_v2.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal Totalamount { get; set; }
        public string CustomerInfo { get; set; }
        public List<SaleItem> SaleItems { get; set; } = new();
    }
}