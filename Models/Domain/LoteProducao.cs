using System.ComponentModel.DataAnnotations;

namespace Leaf.Models.Domain
{
    public class LoteProducao
    {
        [Required(ErrorMessage = "O código do lote é obrigatório")]
        public string IdLote { get; set; }

        public int? Estagio { get; set; }

		// Propriedade calculada que retorna a descrição do estágio
		public string DescricaoEstagio
		{
			get
			{
				return Enum.IsDefined(typeof(EstagioProducao), Estagio) ? ((EstagioProducao)Estagio).ToString() : "Estágio Desconhecido";
			}
		}

		[Required(ErrorMessage = "O código do produto é obrigatório")]
        public int IdProduto { get; set; }
        public Produto? Produto { get; set; }
        public int? Qtde { get; set; }
        public DateTime? DtaSemeadura { get; set; }
        public DateTime? DtaCrescimento { get; set; }
        public DateTime? DtaPlantio { get; set; }
        public DateTime? DtaColheita { get; set; }

        [Required(ErrorMessage = "O código do Usuário responsável é obrigatório")]
        public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }


        // Enun de estagio padrao
		public enum EstagioProducao
		{
			Semeadura = 1,
			Crescimento = 2,
			Desenvolvimento = 3,
			Colheita = 4
		}

		// Método que obtém a descrição do estágio
		public string ObterDescricaoEstagio()
		{
			return Enum.IsDefined(typeof(EstagioProducao), Estagio) ? Estagio.ToString() : "Estágio Desconhecido";
		}

        public LoteProducao()
        {
        }

    }
}
