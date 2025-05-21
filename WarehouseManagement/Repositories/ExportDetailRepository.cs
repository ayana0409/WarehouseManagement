using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories;

public class ExportDetailRepository : GenericRepository<ExportDetail>, IExportDetailRepository
{
    public ExportDetailRepository(ApplicationDbContext context) : base(context)
    {
    }

}
