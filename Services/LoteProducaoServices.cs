using Leaf.Data;
using Leaf.Models;
using Leaf.Repository;

namespace Leaf.Services
{
	public class LoteProducaoServices
	{
		private readonly DbConnectionManager _dbConnectionManager;

		public LoteProducaoServices(DbConnectionManager dbConnectionManager)
		{
			_dbConnectionManager = dbConnectionManager;
		}
	}
}
