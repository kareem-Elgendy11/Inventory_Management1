using System;
using System.Collections.Generic;

namespace InventoryApp_v2.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalStockQuantity { get; set; }
        public int LowStockProductsCount { get; set; }

        public int TotalPurchasesCount { get; set; }
        public decimal TotalPurchasesValue { get; set; }

        public int TotalSalesCount { get; set; }
        public decimal TotalSalesRevenue { get; set; }

        public List<ActivityLogItem> RecentActivity { get; set; } = new();
        public List<TopProductItem> MostSoldProducts { get; set; } = new();
    }

    public class ActivityLogItem
    {
        public string Type { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class TopProductItem
    {
        public string ProductName { get; set; } = "";
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
