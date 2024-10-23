﻿using Leaf.Models.Domain;
using Leaf.Models.ViewModels;
using Leaf.Services.Agentes;
using Leaf.Services.Facede;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leaf.Controllers.Relatorios
{
    [Authorize]
    public class RelatorioVendasController : Controller
    {
        private readonly string _pathIndex = "~/Views/Relatorios/Vendas/Index.cshtml";
        private readonly string _pathDetalhes = "~/Views/Relatorios/Vendas/Detalhes.cshtml";
        private readonly string _pathImprimir = "~/Views/Relatorios/Vendas/Imprimir.cshtml";

        private readonly UsuarioServices _usuarioServices;
        private readonly PedidoFacedeServices _pedidoFacedeServices;

        public RelatorioVendasController(UsuarioServices usuarioServices, PedidoFacedeServices pedidoFacedeServices)
        {
            _pedidoFacedeServices = pedidoFacedeServices;
            _usuarioServices = usuarioServices;
        }

        [Route("relatorio-vendas")]
        public IActionResult Index()
        {
            GetVendedores();

            return View(_pathIndex);
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(string numeroPedido, string status, string vendedor, string dataInicio, string dataFim)
        {
            //Pupular entregadores
            GetVendedores();

            //Converter os parametros para o tipo certo
            var (idPedido, idVendedor, inicio, fim) = ConverterParametrosBusca(numeroPedido, vendedor, dataInicio, dataFim);

            if (fim < inicio)
            {
                TempData["MensagemErro"] = "A data de fim não pode ser menor que a data de início.";
                return RedirectToAction("Index");
            }

            try
            {
                //Listar pedidos ja com filtro
                List<PedidoViewModel> pedidos = await _pedidoFacedeServices.GetRelatorioVendas(inicio, fim, idVendedor, idPedido, status);

                if (pedidos.Any())
                {
                    // Armazena os filtros
                    ViewBag.NumeroPedido = numeroPedido;
                    ViewBag.Vendedor = vendedor;
                    ViewBag.DataInicio = dataInicio;
                    ViewBag.DataFim = dataFim;
                    ViewBag.Status = status;


                    TempData["MensagemSucesso"] = "Dados encontrados.";
                    return View(_pathIndex, pedidos);
                }
                else
                {
                    TempData["MensagemErro"] = "Não há pedidos lançados com os filtros aplicados.";
                    return View(_pathIndex, new List<PedidoViewModel>());
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Não foi possível acessar o banco de dados, erro: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Route("relatorio-vendas/imprimir")]
        public async Task<IActionResult> Imprimir(string numeroPedido, string status, string vendedor, string dataInicio, string dataFim)
        {
            var (idPedido, idVendedor, inicio, fim) = ConverterParametrosBusca(numeroPedido, vendedor, dataInicio, dataFim);

            List<PedidoViewModel> pedidoViewModel = await _pedidoFacedeServices.GetRelatorioVendas(inicio, fim, idVendedor, idPedido, status);


            //Retorna os filtros:
            ViewBag.NumeroPedido = numeroPedido;
            ViewBag.Vendedor = vendedor;
            ViewBag.DataInicio = dataInicio;
            ViewBag.DataFim = dataFim;
            ViewBag.Status = status;


            if (idVendedor != 0)
            {
                Usuario usuario = _usuarioServices.GetUsuarioId(idVendedor);
                ViewBag.NomeVendedor = usuario?.Nome ?? "Entregador não encontrado";
            }

            return View(_pathImprimir, pedidoViewModel);
        }


        [HttpGet]
        [Route("relatorio-vendas/detalhes/{id}")]
        public async Task<IActionResult> Detalhes(int id)
        {
            try
            {
                // Buscar os detalhes do pedido pelo ID
                PedidoViewModel pedidoViewModel = await _pedidoFacedeServices.GetPedidoAsync(id);

                if (pedidoViewModel == null)
                {
                    TempData["MensagemErro"] = "Pedido não encontrado.";
                    return RedirectToAction("Index");
                }

                return View(_pathDetalhes, pedidoViewModel);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Ocorreu um erro ao tentar buscar os detalhes do pedido: " + ex.Message;
                return RedirectToAction("Index");
            }
        }


        //Converter parametros:
        public (int idPedido, int idEntregador, DateTime inicio, DateTime fim) ConverterParametrosBusca(string numeroPedido, string vendedor, string dataInicio, string dataFim)
        {
            int idVendedor = int.TryParse(vendedor, out int vendedorInt) ? vendedorInt : 0;
            int idPedido = int.TryParse(numeroPedido, out int pedidoInt) ? pedidoInt : 0;

            DateTime? dtaInicio = DateTime.TryParse(dataInicio, out DateTime parsedDataInicio) ? parsedDataInicio : null;
            DateTime? dtaFim = DateTime.TryParse(dataFim, out DateTime parsedDataFim) ? parsedDataFim : null;

            DateTime inicio = dtaInicio ?? new DateTime(1800, 1, 1);
            DateTime fim = dtaFim ?? DateTime.Now;

            return (idPedido, idVendedor, inicio, fim);
        }



        //Popularizar VIEWBAGS
        private void GetVendedores()
        {
            List<Usuario> usuarios = _usuarioServices.ListaVendedores();
            if (usuarios == null || !usuarios.Any())
            {
                usuarios = new List<Usuario> { new Usuario { Id = 0, Nome = "SEM VENDEDOR" } };
            }

            ViewBag.Vendedores = usuarios;
        }


    }
}
