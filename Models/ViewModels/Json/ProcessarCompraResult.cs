namespace Leaf.Models.ViewModels.Json
{
    public class ProcessarCompraResult
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }

        public ProcessarCompraResult(bool sucesso, string mensagem)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
        }
    }
}
