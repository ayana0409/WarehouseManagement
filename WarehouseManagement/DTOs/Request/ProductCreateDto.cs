using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.DTOs.Request
{
    public class ProductCreateDto
    {
        [Required]
        public int ManuId { get; set; }

        [Required]
        public int CateId { get; set; }

        [Required]
        public string ProName { get; set; }

        public string? Image { get; set; }
        public string? Unit { get; set; }
        public int? Expiry { get; set; }
        [Required]
        public double ImportPrice { get; set; }
        [Required]
        public double ExportPrice { get; set; }
    }

}
