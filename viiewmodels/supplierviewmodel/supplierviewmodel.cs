using InventoryApp_v2.Models;
using System.ComponentModel.DataAnnotations;

namespace Prog1_iti.viiewmodels.supplierviewmodel
{
    public class supplierviewmodel
    {
        public int? IdS { get; set; }
        [Required]
        public string SupplierNameS { get; set; } = null!;
        [Required]
        public string? ContactNameS { get; set; }
        [RegularExpression("^01[0125][0-9]{8}$")]
        public string PhoneS { get; set; }
        [Required]
        public string EmailS { get; set; }
        [Required]
        public string AddressS { get; set; }
        public List<Purchase>? purchases { get; set; }
        //public List<Product>? products { get; set; }
    }
}
