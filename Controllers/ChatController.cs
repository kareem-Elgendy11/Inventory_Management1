using InventoryApp_v2.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Prog1_iti.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration config;

        public ChatController(ApplicationDbContext db, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            this.db = db;
            this.httpClientFactory = httpClientFactory;
            this.config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return Json(new { reply = "من فضلك اكتب سؤال." });
            }

            string context = BuildContext();

            string prompt = $@"أنت مساعد ذكي لنظام إدارة مخزون. 
جاوب على سؤال المستخدم بناءً على البيانات دي فقط، وبالعربي، وبإيجاز:

البيانات الحالية:
{context}

سؤال المستخدم: {request.Message}";

            string apiKey = config["GeminiApiKey"];
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.6-flash:generateContent?key={apiKey}";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = prompt } }
                    }
                }
            };

            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsync(url,
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                return Json(new { reply = "حصل خطأ في الاتصال بالـ AI: " + errorText });
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            string reply = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "معرفتش أجاوب على السؤال ده.";

            return Json(new { reply });
        }

        private string BuildContext()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"عدد المنتجات: {db.Products.Count()}");
            sb.AppendLine($"عدد الفئات: {db.Categories.Count()}");
            sb.AppendLine($"عدد الموردين: {db.Suppliers.Count()}");
            sb.AppendLine($"إجمالي الكمية في المخزون: {db.Products.Sum(p => (int?)p.Quantity) ?? 0}");

            var lowStock = db.Products.Where(p => p.Quantity <= p.LowStokThreshold).ToList();
            sb.AppendLine($"عدد المنتجات اللي مخزونها منخفض: {lowStock.Count}");
            if (lowStock.Any())
            {
                sb.AppendLine("المنتجات منخفضة المخزون: " + string.Join(", ", lowStock.Select(p => $"{p.Name} (متبقي {p.Quantity})")));
            }

            sb.AppendLine($"عدد المشتريات: {db.Purchases.Count()}");
            sb.AppendLine($"إجمالي قيمة المشتريات: {db.Purchases.Sum(p => (decimal?)p.TotalAmount) ?? 0}");

            sb.AppendLine($"عدد المبيعات: {db.Sales.Count()}");
            sb.AppendLine($"إجمالي الإيرادات: {db.Sales.Sum(s => (decimal?)s.Totalamount) ?? 0}");

            var topProducts = db.SaleItems
                .GroupBy(si => si.ProductId)
                .Select(g => new { g.First().Product.Name, Qty = g.Sum(si => si.Quantity) })
                .OrderByDescending(x => x.Qty)
                .Take(5)
                .ToList();

            if (topProducts.Any())
            {
                sb.AppendLine("الأكتر مبيعًا: " + string.Join(", ", topProducts.Select(p => $"{p.Name} ({p.Qty} قطعة)")));
            }

            return sb.ToString();
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = "";
    }
}
