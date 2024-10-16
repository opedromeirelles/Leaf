using Leaf.Data;
using Leaf.Models;
using Leaf.Repository;

namespace Leaf.Services
{
	public class ItemLoteProducaoServices
	{
		private readonly DbConnectionManager _dbConnectionManager;

		public ItemLoteProducaoServices(DbConnectionManager dbConnectionManager)
		{
			_dbConnectionManager = dbConnectionManager;
		}
	}
}
