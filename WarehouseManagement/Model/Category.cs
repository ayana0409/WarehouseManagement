using WarehouseManagement.Share;

namespace WarehouseManagement.Model
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }
    }
}
