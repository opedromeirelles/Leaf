namespace Leaf.Models.Domain.ErrorModel
{
    public class DomainErrorModel
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public string CodErro { get; set; }
        public string Detalhes { get; set; }

        public DateTime  DataError { get; set; }


        // Erro detalhado
        public DomainErrorModel(bool sucesso, string mensagem, string codErro, string detalhes)
        {
            this.Sucesso = sucesso;
            this.Mensagem = mensagem;
            this.CodErro = codErro;
            this.Detalhes = detalhes;
            this.DataError = DateTime.Now;
        }

        // Erro simples
        public DomainErrorModel(bool sucesso, string mensagem)
        {
            this.Sucesso = sucesso;
            this.Mensagem = mensagem;
            this.DataError = DateTime.Now;

        }

    }
}
