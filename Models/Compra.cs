using System.ComponentModel.DataAnnotations;

namespace Leaf.Models
{
    public class Compra
    {
        public int IdOc { get; set; }
        public string status { get; set; }
        public Decimal ValorTotal { get; set; }

        [Required(ErrorMessage = "O código da pessoa é obrigatório")]
        public int IdPessoa { get; set; }
        public Pessoa? Pessoa { get; set; }
        public DateTime DtaEmissao { get; set; }
        public DateTime? DtaBaixa { get; set; }
        public DateTime? DtaCancelamento { get; set; }

        [Required(ErrorMessage = "O código do Usuario é obrigatório")]
        public int IdUsuario { get; set; }
        public Usuario? usuario { get; set; }

        public Compra(DateTime dtaEmissao)
        {
            status = "EM";
            DtaEmissao = dtaEmissao;
            DtaBaixa = null;
            DtaCancelamento = null;
        }
        public Compra()
        {
            
        }

    }
}
