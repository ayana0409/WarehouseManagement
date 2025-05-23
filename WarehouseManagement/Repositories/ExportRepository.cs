using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories
{
    public class ExportRepository : GenericRepository<Export>, IExportRepository
    {
        public ExportRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
