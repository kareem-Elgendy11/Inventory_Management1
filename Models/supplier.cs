namespace InventoryApp_v2.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public List<Purchase> Purchases { get; set; } = new();
    }
}