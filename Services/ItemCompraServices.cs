using Leaf.Data;
using Leaf.Models;


namespace Leaf.Services
{
    public class ItemCompraServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        public ItemCompraServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }
    }
}
