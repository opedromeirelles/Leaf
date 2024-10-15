using Leaf.Models;
using Leaf.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leaf.Controllers
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

        public IActionResult Index()
        {
           
            List<PedidoViewModel> pedidos = new List<PedidoViewModel>();
            try
            {
                pedidos = _pedidoFacedeServices.PreencherPedidos();

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
        public IActionResult Buscar(int numeroPedido, string status)
        {
            List<PedidoViewModel> pedidos = new List<PedidoViewModel>();

            try
            {
                pedidos = _pedidoFacedeServices.PreencherPedidos(numeroPedido, status);

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
        public IActionResult Atualizar(int id)
        {
            try
            {
                PedidoViewModel pedidoViewModel = _pedidoFacedeServices.PreencherPedido(id);
                if (pedidoViewModel != null)
                {
                    // Carregar entregadores disponiveis
                    List<Usuario> Entregadores = _usuarioServices.ListaEntregadores();
                    if (Entregadores.Any())
                    {
                        ViewData["Entregadores"] = Entregadores;
                    }
                    else
                    {
                        ViewData["Entregadores"] = new List<Usuario> { new Usuario { Id = 0, Nome = "Desconhecido" } };
                    }

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

        public  IActionResult AtualizarStatus(PedidoViewModel pedidoViewModel, int entregadores)
        {
            // Populo meu pedido da view com as informações do pedido em si
            pedidoViewModel = _pedidoFacedeServices.PreencherPedido(pedidoViewModel.IdPedido);

            // Verificação de vazio
            if (pedidoViewModel == null && entregadores == 0 )
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
                if(produtosNegativos != null && produtosNegativos.Any())
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






    }
}
