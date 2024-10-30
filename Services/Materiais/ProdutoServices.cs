using Leaf.Data;
using Leaf.Models.Domain;
using Leaf.Models.Domain.ErrorModel;
using Leaf.Repository.Materiais;
using System.Globalization;

namespace Leaf.Services.Materiais
{
    public class ProdutoServices
    {
        private readonly DbConnectionManager _dbConnectionManager;
        // Construtor que injeta a dependência de DbConnectionManager
        public ProdutoServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        // Validção:
        public DomainErrorModel ValidarPrdouto(Produto produto)
        {
            // converto os valores:
            if (!decimal.TryParse(produto.ValorUnitario.ToString().Replace(".", ","), NumberStyles.Number, new CultureInfo("pt-BR"), out _))
            {
                return new DomainErrorModel(false, "Valor unitário invalido.");
            }

            if (produto.ValorUnitario <= 0)
            {
                return new DomainErrorModel(false, "O valor unitário tem que ser numérico e não pode ser menor ou igual a zero");
            }

            try
            {

                if (produto.IdProduto != 0)
                {
                    return new DomainErrorModel(true, "Produto atualizado com sucesso");
                }

                else
                {
                    ProdutoRepository _produtoRepository = new ProdutoRepository(_dbConnectionManager);
                    List<Produto> produtosBase = _produtoRepository.GetProdutos();

                    //Valido se ja existe
                    foreach (var prodBase in produtosBase)
                    {
                        if (prodBase.Descricao.Equals(produto.Descricao, StringComparison.OrdinalIgnoreCase))
                        {
                            return new DomainErrorModel(false, $"Produto: {produto.Descricao.ToUpper()} já cadastrado.");
                        }
                    }

                    return new DomainErrorModel(true, "Produto cadastrado com sucesso!");
                }
                
            }
            catch (Exception ex)
            {
                return new DomainErrorModel(false, "Erro ao validar produto", "Validação", ex.Message);
            }
        }


        // Listas
        public List<Produto> ListarProdutos()
        {
            ProdutoRepository _produtosRepository = new ProdutoRepository(_dbConnectionManager);
            try
            {
                return _produtosRepository.GetProdutos();
            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao listar os produtos, erro: {ex.Message}");
            }
        }

        public List<Produto> ListarProdutosFiltrados(string descricao, int status)
        {
            ProdutoRepository _produtosRepository = new ProdutoRepository(_dbConnectionManager);
            try
            {
                return _produtosRepository.GetProdutosFiltro(descricao, status);
            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao listar os produtos, erro: {ex.Message}");
            }
        }


        // GET
        public int GetQuantidadeEstoque(int idProduto)
        {
            ProdutoRepository _produtosRepository = new ProdutoRepository(_dbConnectionManager);
            return _produtosRepository.ConsultaEstoqueProduto(idProduto);
        }

        public Produto GetProduto(int idProduto)
        {
            ProdutoRepository _produtosRepository = new ProdutoRepository(_dbConnectionManager);
            try
            {
                return _produtosRepository.GetProdutoById(idProduto);
            }
            catch (Exception ex)
            {

                throw new Exception($"Erro ao consultar produto, erro: {ex.Message}");
            }
        }


        //AÇÔES

        public void AtualizarProduto(Produto produto)
        {
            ProdutoRepository _produtoRepository = new ProdutoRepository(_dbConnectionManager);
            _produtoRepository.AtualizarProduto(produto);

        }

        public void CadastrarProduto(Produto produto)
        {
            ProdutoRepository _produtoRepository = new ProdutoRepository(_dbConnectionManager);
            _produtoRepository.CadastrarProduto(produto);

        }

    }
}
