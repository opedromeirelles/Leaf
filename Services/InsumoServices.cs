using Leaf.Data;
using Leaf.Models;
using Leaf.Repository;

namespace Leaf.Services
{
    public class InsumoServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        // Construtor que injeta a dependência de DbConnectionManager
        public InsumoServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        // Listar todos os insumos
        public List<Insumo> ListarInsumos()
        {
            InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
            try
            {
                return _insumoRepository.GetInsumos();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar os insumos, erro: {ex.Message}");
            }
        }

        // Listar insumos filtrados por descrição, pessoa e status
        public List<Insumo> ListarInsumosFiltrados(string descricao, string pessoaNome, int status)
        {
            InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
            try
            {
                return _insumoRepository.GetInsumosFiltro(descricao, pessoaNome, status);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar os insumos com filtro, erro: {ex.Message}");
            }
        }

        // Consultar insumo por ID
        public Insumo GetInsumo(int idInsumo)
        {
            InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
            try
            {
                return _insumoRepository.GetInsumoById(idInsumo);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar insumo, erro: {ex.Message}");
            }
        }

        // Atualizar status do insumo
        public bool AtualizarStatusInsumo(int idInsumo, int novoStatus)
        {
            InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
            try
            {
                _insumoRepository.AtualizarStatusInsumo(idInsumo, novoStatus);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Atualizar dados do insumo
        public bool AtualizarInsumo(Insumo insumo)
        {
            InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
            try
            {
                _insumoRepository.AtualizarInsumo(insumo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Cadastrar um novo insumo
        public bool CadastrarInsumo(Insumo insumo)
        {
            InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
            try
            {
                _insumoRepository.CadastrarInsumo(insumo);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
