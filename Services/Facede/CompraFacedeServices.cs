using Leaf.Models.Domain;
using Leaf.Models.ViewModels;
using Leaf.Services;

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

        var compra = await _compraServices.GetCompra(idCompra);
        if (compra == null)
        {
            return new CompraViewModel();
        }

        var compraViewModel = new CompraViewModel
        {
            IdCompra = compra.IdOc,
            Compra = compra,
            Fornecedor = _pessoaServices.GetPessoa(compra.IdPessoa),
            Vendedor =  _usuarioServices.GetUsuarioId(compra.IdUsuario),
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
