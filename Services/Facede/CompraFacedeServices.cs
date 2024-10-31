using Leaf.Models.Domain;
using Leaf.Models.Domain.ErrorModel;
using Leaf.Models.ItensDomain;
using Leaf.Models.ViewModels;
using Leaf.Models.ViewModels.Json;
using Leaf.Services.Agentes;
using Leaf.Services.Compras;
using Leaf.Services.Materiais;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

public class CompraFacedeServices
{
    // Serviços
    private readonly CompraServices _compraServices;
    private readonly ItemCompraServices _itemCompraServices;
    private readonly UsuarioServices _usuarioServices;
    private readonly PessoaServices _pessoaServices;
    private readonly InsumoServices _insumoServices;

    public CompraFacedeServices(CompraServices compra, ItemCompraServices itemCompra, UsuarioServices usuario, PessoaServices pessoa, InsumoServices insumo)
    {
        _compraServices = compra;
        _itemCompraServices = itemCompra;
        _usuarioServices = usuario;
        _pessoaServices = pessoa;
        _insumoServices = insumo;
    }


    
    public async Task<CompraViewModel> MapearCompraAsync(int idCompra)
    {
        if (idCompra <= 0)
        {
            return new CompraViewModel();
        }

        Compra compra = await _compraServices.GetCompra(idCompra);
        if (compra == null)
        {
            return new CompraViewModel();
        }

        CompraViewModel compraViewModel = new CompraViewModel
        {
            IdCompra = compra.IdOc,
            Compra = compra,
            Fornecedor = _pessoaServices.GetPessoa(compra.IdPessoa),
            Administrativo = _usuarioServices.GetUsuarioId(compra.IdUsuario),
            ItensCompra = _itemCompraServices.GetItemCompras(compra.IdOc)
        };

        foreach (var itemCompra in compraViewModel.ItensCompra)
        {
            itemCompra.insumo = _insumoServices.GetInsumo(itemCompra.IdInsumo);
            if (itemCompra.insumos == null)
            {
                itemCompra.insumos = new List<Insumo>();
            }
            itemCompra.insumos.Add(itemCompra.insumo);
        }

        return compraViewModel;
    }


    //Get lista fornecedor
    public async Task<List<Pessoa>> FornecedoresInsumos(int idFornecedor)
    {
        List<Pessoa> fornecedores = await _pessoaServices.GetFornecedoresInsumosAsync(idFornecedor);
        return fornecedores.Any() ? fornecedores : new List<Pessoa>();
    } 

    //Get lista insumos
    public async Task<List<Insumo>> InsumosFornecedores(int idFornecedor)
    {
        List<Insumo> insumos = await _insumoServices.ListarInsumosFornecedoresAsync(idFornecedor);
        return insumos.Any() ? insumos : new List<Insumo>();
    }


    //Get Compras filtro
    public async Task<List<CompraViewModel>> GetCompras(int idUser, string status, int idOc)
    {
        List<CompraViewModel> compraViewModels = new List<CompraViewModel>();
        

        List<Compra> compras = _compraServices.GetCompras(status, idOc);
		compras = ListaByUsuario(compras, idUser);

		if (compras == null)
        {
            return new List<CompraViewModel>();
        }

        foreach(var compra in compras){

            CompraViewModel compraView = await MapearCompraAsync(compra.IdOc);
            compraViewModels.Add(compraView);
        };

        return compraViewModels.Any() ? compraViewModels : new List<CompraViewModel>();

    }
    public async Task<List<CompraViewModel>> GetCompras(int idUser)
    {
        List<CompraViewModel> compraViewModels = new List<CompraViewModel>();

        int numeroCompra = 0;
        string allStatus = "";

        List<Compra> compras = _compraServices.GetCompras(allStatus, numeroCompra);
        compras = ListaByUsuario(compras, idUser);

        if (compras == null)
        {
            return new List<CompraViewModel>();
        }

        foreach (var compra in compras)
        {

            CompraViewModel compraView = await MapearCompraAsync(compra.IdOc);
            compraViewModels.Add(compraView);
        };

        return compraViewModels.Any() ? compraViewModels : new List<CompraViewModel>();

    }


    //Validar lista por usuario
    public List<Compra> ListaByUsuario(List<Compra> compras, int id)
    {
        try
        {
			List<Compra> comprasByUsuario = new List<Compra>();

			//Filtra usuario para conferir se ele pertence ao departamento de administrativo
			Usuario usuario = _usuarioServices.GetUsuarioId(id);

            if (usuario.Departamento?.Descricao == "ADMINISTRATIVO")
            {

				foreach (var compra in compras)
				{
					if (compra.IdUsuario == id)
					{
						comprasByUsuario.Add(compra);
					}
				}
                return comprasByUsuario.Any() ? comprasByUsuario : new List<Compra>();

			}

            return compras;
        }
        catch (Exception ex)
        {

            throw new Exception("Não foi possivel acessar compras com associação. " + ex.Message);
        }
    }



    //Processar compra
    public ProcessarCompraResult ProcessarCompra(CompraJsonView compraJson)
    {
        //validar se a compra existe
        if (compraJson == null)
        {
            return new ProcessarCompraResult(false, "Compra inválida");
        }

        // Validar Solicitante
        if (compraJson.IdAdministrativo == 0)
        {
            return new ProcessarCompraResult(false, "Não há solicitantes vinculado a compra.");
        }


        //validar se todos os insumos fazem parte do mesmo fornecedor       
        Pessoa pessoa = _pessoaServices.GetPessoa(compraJson.IdPessoa);

        foreach (var item in compraJson.ItensCompra)
        {
            Insumo insumo = _insumoServices.GetInsumo(item.IdInsumo);
            if (pessoa.IdPessoa != insumo.IdPessoa)
            {
                return new ProcessarCompraResult(false, $"O insumo: '{insumo.Descricao}', não pertence há o fornecedor: '{pessoa.Nome}'.");
            };                
        }

        //Validar valor total
        decimal valorTotalBack = compraJson.ItensCompra.Sum(i => i.ValorUnitario * i.Quantidade);

        if (valorTotalBack != compraJson.ValorTotal)
        {
            return new ProcessarCompraResult(false, $"O valor final da compra de '{compraJson.ValorTotal}', foi diferente do valor calclado internamente de '{valorTotalBack}'");
        }

        try
        {
            if (EmitirCompra(compraJson))
            {
                return new ProcessarCompraResult(true, "Compra emitida.");
            }
            else
            {
                return new ProcessarCompraResult(false, "Erro ao lançar compra no banco");
            }
        }
        catch (Exception ex)
        {

            return new ProcessarCompraResult(false, ex.Message);
        }
        
        
    }

    //Validar compra
    public async Task<DomainErrorModel> ValidarCompra(int idCompra)
    {
        if (idCompra != 0)
        {
            try
            {
				CompraViewModel compra = await MapearCompraAsync(idCompra);

				//validar se todos os insumos fazem parte do mesmo fornecedor       
				Pessoa pessoa = _pessoaServices.GetPessoa(compra.Compra.IdPessoa);

				foreach (var item in compra.ItensCompra)
				{
					Insumo insumo = _insumoServices.GetInsumo(item.IdInsumo);
					if (pessoa.IdPessoa != insumo.IdPessoa)
					{
						return new DomainErrorModel(false, $"O insumo: '{insumo.Descricao}', não pertence há o fornecedor: '{pessoa.Nome}'.");
					};
				}

				return new DomainErrorModel(true, "Compra Validada");
			}
            catch (Exception ex)
            {

                new DomainErrorModel(false, "Erro Validação ao validar compra", "Validação", ex.Message);
            }

		}
        else
        {
            return new DomainErrorModel(false, "Compra invalida.");
        }

        return new DomainErrorModel(false, "Erro ao validar compra");
    }

    // Nova compra
    private bool EmitirCompra(CompraJsonView compraView)
    {
        //Alimento minha compra
        Compra compra = new Compra
        {
            IdPessoa = compraView.IdPessoa,
            IdUsuario = compraView.IdAdministrativo,
            DtaEmissao = DateTime.Now,
            Status = "EM",
            ValorTotal = compraView.ValorTotal
        };


        try
        {
            //Emito a compra e retorno o id correspondente a ela
            if (_compraServices.NovaCompra(compra) != 0)
            {

                List<ItemCompra> itensNaoInseridos = new List<ItemCompra>();

                try
                {
                    foreach (var item in compraView.ItensCompra)
                    {
                        //Popula meus itens com a lista enviada da view
                        ItemCompra itemCompra = new ItemCompra
                        {
                            IdInsumo = item.IdInsumo,
                            Quantidade = item.Quantidade,
                            SubTotal = item.SubTotal,
                            IdOc = compra.IdOc
                        };

                        // Se não conseguir inserir armazena o item:
                        if (!_itemCompraServices.NovaCompraItens(compra.IdOc, itemCompra))
                        {
                            itensNaoInseridos.Add(itemCompra);
                        }
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception($"Erro ao cadastrar Item a compra id: { compra.IdOc }, erro: { ex.Message }");
                }
                

                //Valida se houve algum item inserido
                if (itensNaoInseridos != null)
                {
                    //Nova compra realizada com sucesso
                    return true;
                }
            }

        }
        catch (Exception ex)
        {

            throw new Exception("Erro ao emitir uma nova compra, erro: " + ex.Message);
        }
        

        return false;
    }

    public DomainErrorModel BaixarCompra(int idCompra, string status)
    {
        try
        {
            _compraServices.BaixarCompra(idCompra, status);
            return new DomainErrorModel(true, "Compra baixada");
        }
        catch (Exception ex)
        {

            return new DomainErrorModel(false, "Erro ao baixar compra", "Validação", ex.Message);
        }
    }



    // Relatório de compras com filtros assíncrono
    public async Task<List<CompraViewModel>> GetComprasFiltrosAsync(DateTime dataInicio, DateTime dataFim, int idFornecedor, string status, string numeroCompra, int idSolicitante)
    {
        List<Compra> compras = await _compraServices.GetListComprasPeriodo(dataInicio, dataFim, idFornecedor, status, numeroCompra, idSolicitante);

        if (compras == null || !compras.Any())
        {
            return new List<CompraViewModel>();
        }

        List<CompraViewModel> compraViewModels = new List<CompraViewModel>();

        foreach (var compra in compras)
        {
            try
            {
                CompraViewModel compraViewModel = await MapearCompraAsync(compra.IdOc);

                if (compraViewModel != null)
                {
                    compraViewModels.Add(compraViewModel);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao mapear compra com ID {compra.IdOc}: {ex.Message}");
            }
        }

        return compraViewModels;
    }

}
