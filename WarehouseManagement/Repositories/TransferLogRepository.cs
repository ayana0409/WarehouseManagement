using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories;

public class TransferLogRepository : GenericRepository<TransferLog>, ITransferLogRepository
{
    public TransferLogRepository(ApplicationDbContext context) : base(context)
    {
    }

}
