using Leaf.Models.Domain;
using Leaf.Models.ItensDomain;
using Leaf.Models.ViewModels;
using Leaf.Services.Agentes;
using Leaf.Services.Compras;
using Leaf.Services.Materiais;

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

    //Processo de compra:

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
    
    public bool EmitirCompra(CompraViewModel compraViewModel)
    {
        //Alimento minha compra
        Compra compra = compraViewModel.Compra;

        try
        {
            //Emito a compra e retorno o id correspondente a ela
            if (_compraServices.NovaCompra(compra) != 0)
            {
                List<ItemCompra> itensNaoInseridos = new List<ItemCompra>();

                try
                {
                    foreach (var item in compraViewModel.ItensCompra)
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
            Gerente =  _usuarioServices.GetUsuarioId(compra.IdUsuario),
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
}
