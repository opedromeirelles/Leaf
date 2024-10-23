using Leaf.Models.Domain;
using Leaf.Models.ViewModels;
using Leaf.Services.Agentes;
using Leaf.Services.Facede;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leaf.Controllers.Ordens
{
    [Authorize]
    public class PedidoController : Controller
    {
        //Serviços
        private readonly PedidoFacedeServices _pedidoFacedeServices;
        private readonly UsuarioServices _usuarioServices;


        //Injection 
        public PedidoController(PedidoFacedeServices pedidoFacedeServices, UsuarioServices usuarioServices)
        {
            _usuarioServices = usuarioServices;
            _pedidoFacedeServices = pedidoFacedeServices;
        }

        public async Task<IActionResult> Index()
        {

            List<PedidoViewModel> pedidos = new List<PedidoViewModel>();
            try
            {
                pedidos = await _pedidoFacedeServices.GetPedidosAsync();

                if (pedidos != null && pedidos.Any())
                {
                    TempData["MensagemSucesso"] = "Dados atualizados.";
                    return View(pedidos);
                }
                else
                {
                    TempData["MensagemErro"] = "Não há pedidos lançado";
                    return View(pedidos);
                }

            }
            catch (Exception ex)
            {

                TempData["MensagemErro"] = "Não foi possivel estabelecer conexão, erro " + ex.Message;
                return View(new List<PedidoViewModel>());
            }


        }

        [HttpGet]
        public async Task<IActionResult> Buscar(int numeroPedido, string status)
        {
            List<PedidoViewModel> pedidos = new List<PedidoViewModel>();

            try
            {
                pedidos = await _pedidoFacedeServices.GetPedidoFiltroAsync(numeroPedido, status);

                if (pedidos != null && pedidos.Any())
                {
                    TempData["MensagemSucesso"] = "Dados atualizados.";
                    return View("Index", pedidos);
                }
                else
                {

                    TempData["MensagemErro"] = "Não há pedidos lançado";
                    return View("Index", pedidos);

                }
            }
            catch (Exception)
            {
                TempData["MensagemErro"] = "Não há registro com as informações estabelecidas.";
                return View("Index", new List<PedidoViewModel>());
            }

        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(int id)
        {
            GetEntregadores();

            try
            {
                PedidoViewModel pedidoViewModel = await _pedidoFacedeServices.GetPedidoAsync(id);
                if (pedidoViewModel != null)
                {
                    return View(pedidoViewModel);
                }

                TempData["MensagemErro"] = "Não foi possivel trazer os dados do pedido";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Não foi possivel acessar o pedido, erro: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarStatus(PedidoViewModel pedidoViewModel, int entregadores)
        {
            GetEntregadores();

            // Populo meu pedido da view com as informações do pedido em si
            pedidoViewModel = await _pedidoFacedeServices.GetPedidoAsync(pedidoViewModel.IdPedido);

            // Verificação de vazio
            if (pedidoViewModel == null && entregadores == 0)
            {
                TempData["MensagemErro"] = "Erro ao atualizar o pedido, selecione um entregador novamente.";

                // Carregar entregadores disponiveis
                List<Usuario> Entregadores = _usuarioServices.ListaEntregadores();
                if (Entregadores != null && Entregadores.Any())
                {
                    ViewData["Entregadores"] = Entregadores;
                }
                else
                {
                    ViewData["Entregadores"] = new List<Usuario> { new Usuario { Id = 0, Nome = "Desconhecido" } };
                }

                return View(pedidoViewModel);
            }
            else
            {
                //Preencho a lista se tiver de produtos de negativo - Atualzo o pedido caso há produtos em estoque
                List<Produto> produtosNegativos = _pedidoFacedeServices.AtualizarPedido(pedidoViewModel, entregadores);

                //Verifco a lista
                if (produtosNegativos != null && produtosNegativos.Any())
                {
                    TempData["MensagemErro"] = "Erro ao atualizar o pedido, há produtos em falta de estoque.";
                    TempData["ProdutosNegativos"] = produtosNegativos;
                    return View(pedidoViewModel);
                }
                else
                {
                    TempData["MensagemSucesso"] = "Pedido atualizado com êxito. Agora em rota de entrega!";
                    return RedirectToAction("Index");
                }
            }
        }


        //Popular ViewBags
        private void GetEntregadores()
        {
            // Carregar entregadores disponiveis
            List<Usuario> Entregadores = _usuarioServices.ListaEntregadores();
            if (Entregadores != null && Entregadores.Any())
            {
                ViewData["Entregadores"] = Entregadores;
            }
            else
            {
                ViewData["Entregadores"] = new List<Usuario> { new Usuario { Id = 0, Nome = "Desconhecido" } };
            }
        }




    }
}
