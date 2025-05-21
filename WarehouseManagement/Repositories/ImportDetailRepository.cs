using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories;

public class ImportDetailRepository : GenericRepository<ImportDetail>, IImportDetailRepository
{
    public ImportDetailRepository(ApplicationDbContext context) : base(context)
    {
    }

}

