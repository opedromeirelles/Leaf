using Leaf.Models.Domain;
using Leaf.Models.ItensDomain;

namespace Leaf.Models.ViewModels
{
	public class LoteProducaoViewModel
	{
        public LoteProducao LoteProducao { get; set; }
        public List<ItemLoteProducao> ItemLote { get; set; }
        public List<Insumo> InsumosLote { get; set; }

    }
}
