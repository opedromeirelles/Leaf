using Leaf.Data;
using Leaf.Models.Domain;
using Leaf.Repository.Agentes;

namespace Leaf.Services.Agentes
{
    public class PessoaServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        // Construtor que injeta a dependência de DbConnectionManager
        public PessoaServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        // Método para listar todas as pessoas
        public List<Pessoa> ListarPessoas()
        {
            PessoaRepository _pessoaRepository = new PessoaRepository(_dbConnectionManager);
            try
            {
                return _pessoaRepository.GetPessoas();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar as pessoas, erro: {ex.Message}");
            }
        }

        // Método para listar pessoas com filtros
        public List<Pessoa> ListarPessoasFiltradas(string nome, string tipo)
        {
            PessoaRepository _pessoaRepository = new PessoaRepository(_dbConnectionManager);
            try
            {
                return _pessoaRepository.GetPessoasFiltro(nome, tipo);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar as pessoas com filtro, erro: {ex.Message}");
            }
        }

        // Método para consultar apenas fornecedores - Pessoas que contém insumos vinculados
        public List<Pessoa> GetFornecedores()
        {
            PessoaRepository pessoaRepository = new PessoaRepository(_dbConnectionManager);
            return pessoaRepository.GetFornecedores();
        }


        // Método para consultar uma pessoa pelo ID
        public Pessoa GetPessoa(int idPessoa)
        {
            PessoaRepository _pessoaRepository = new PessoaRepository(_dbConnectionManager);
            try
            {
                return _pessoaRepository.GetPessoaById(idPessoa);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar pessoa, erro: {ex.Message}");
            }
        }

        // Buscar pessoa por com CNPJ
        public Pessoa PessoaComCnpj(string cnpj)
        {
            PessoaRepository _pessoaRepository = new PessoaRepository(_dbConnectionManager);
            try
            {
                return _pessoaRepository.GetPessoaComCnpj(cnpj);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar pessoa com CNPJ, erro: {ex.Message}");
            }
        }

        // Buscar pessoa por com CNPJ ou NOME
        public Pessoa PessoaNomeCnpj(string dadosPessoa)
        {
            PessoaRepository _pessoaRepository = new PessoaRepository(_dbConnectionManager);
            try
            {
                return _pessoaRepository.GetPessoaCnpjNome(dadosPessoa);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar pessoa com CNPJ e Nome, erro: {ex.Message}");
            }
        }


        // Método para atualizar o status de uma pessoa (por exemplo, tipo)
        public bool AtualizarStatusPessoa(int idPessoa, string tipo)
        {
            PessoaRepository _pessoaRepository = new PessoaRepository(_dbConnectionManager);
            try
            {
                _pessoaRepository.AtualizarStatusPessoa(idPessoa, tipo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Método para atualizar informações de uma pessoa
        public bool AtualizarPessoa(Pessoa pessoa)
        {
            PessoaRepository _pessoaRepository = new PessoaRepository(_dbConnectionManager);
            try
            {
                _pessoaRepository.AtualizarPessoa(pessoa);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Método para cadastrar uma nova pessoa
        public bool CadastrarPessoa(Pessoa pessoa)
        {
            PessoaRepository _pessoaRepository = new PessoaRepository(_dbConnectionManager);
            try
            {
                _pessoaRepository.CadastrarPessoa(pessoa);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
