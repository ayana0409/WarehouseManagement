using WarehouseManagement.Share;
using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.Model
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
        public bool Gender { get; set; }
        public int? Tel { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public RoleEnum Role { get; set; }
        public bool? IsActive { get; set; }
    }
}
