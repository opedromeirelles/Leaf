using Leaf.Data;
using Leaf.Models.Domain;
using Leaf.Models.Domain.ErrorModel;
using Leaf.Repository.Materiais;
using Leaf.Services.Agentes;
using System.Globalization;

namespace Leaf.Services.Materiais
{
    public class InsumoServices
    {
        private readonly DbConnectionManager _dbConnectionManager;

        // Construtor que injeta a dependência de DbConnectionManager
        public InsumoServices(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        private Insumo MapearInsumo(Insumo insumo)
        {
            PessoaServices _pessoaService = new PessoaServices(_dbConnectionManager);
            insumo.Pessoa = _pessoaService.GetPessoa(insumo.IdPessoa);

            return insumo;
        } 

        private List<Insumo> MapearListaInsumo(List<Insumo> insumos)
        {
            foreach (var insumo in insumos)
            {
                MapearInsumo(insumo);
            }

            return insumos;
        }


        // Listar todos os insumos
        public List<Insumo> ListarInsumos()
        {
            InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
            try
            {
                List<Insumo> insumos = _insumoRepository.GetInsumos();
                insumos = MapearListaInsumo(insumos);
                return insumos.Any() ? insumos : new List<Insumo>();         
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao listar os insumos, erro: {ex.Message}");
            }
        }

        // Listar Insumos vinculado a uma pessoa (se tiver)
        public async Task<List<Insumo>> ListarInsumosFornecedoresAsync(int idFornecedor)
        {
            try
            {
                InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
                List<Insumo> insumos = await _insumoRepository.GetInsumosFornecedor(idFornecedor);
                insumos = MapearListaInsumo(insumos);

                return insumos.Any() ? insumos : new List<Insumo>();
            }
            catch (Exception ex)
            {

                throw new Exception("Não foi possivel listar os insumos, erro: " + ex.Message);
            }
        }

       
        public List<Insumo> BuscarInsumosFiltro(string descricao, int idFornecedor, int status)
        {
            try
            {
                InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
                List<Insumo> insumos = _insumoRepository.GetInsumosFiltro(descricao, idFornecedor, status);
                insumos = MapearListaInsumo(insumos);

                return insumos.Any() ? insumos : new List<Insumo>();

            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao listar insumos, " + ex.Message);
            }
        }


        // Consultar insumo por ID
        public Insumo GetInsumo(int idInsumo)
        {
            InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
            try
            {
                Insumo insumo = _insumoRepository.GetInsumoById(idInsumo);
                insumo = MapearInsumo(insumo);
                return insumo ?? null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao consultar insumo, erro: {ex.Message}");
            }
        }

        // Atualizar dados do insumo
        public bool AtualizarInsumo(Insumo insumo)
        {
            InsumoRepository _insumoRepository = new InsumoRepository(_dbConnectionManager);
            try
            {
                Insumo insumoBase = _insumoRepository.GetInsumoById(insumo.IdInsumo);
                insumo.CodBarras = insumoBase.CodBarras;

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
                //Ajustar valores
                insumo.Status = 1;
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
