using Leaf.Models.Domain;

namespace Leaf.Models.DomainLog
{
	public class LoteProducaoLog
	{
        public int IdLog { get; set; }
        public string Lote { get; set; }

        public int QtdeAntiga { get; set; }
		public int QtdeNova { get; set; }

        public int IdUsuario { get; set; }

        public Usuario? Usuario { get; set; }

        public DateTime? DtaAlteracao { get; set; }


    }
}
