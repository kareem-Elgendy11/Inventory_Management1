using InventoryApp_v2.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prog1_iti.viiewmodels.productviewmodel
{
    public class productviewmodel
    {

        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Range(.001,double.MaxValue)]
        public decimal Price { get; set; }
        [Required]
        [Range(0,int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string? sku { get; set; }
        public int LowStokThreshold { get; set; }
        public string? categoryname { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
