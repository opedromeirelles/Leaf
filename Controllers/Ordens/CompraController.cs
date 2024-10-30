using Leaf.Models.Domain;
using Leaf.Models.ViewModels.Json;
using Leaf.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Leaf.Services.Facede;
using Leaf.Services.Agentes;
using Leaf.Models.Domain.ErrorModel;
using System.Security.Claims;

namespace Leaf.Controllers.Ordens
{
    [Authorize]
    public class CompraController : Controller
    {
        private readonly CompraFacedeServices _compraFacedeServices;

        public CompraController(CompraFacedeServices compraFacedeServices)
        {
            _compraFacedeServices = compraFacedeServices;
        }
        
        

        public async Task<IActionResult> Index(List<CompraViewModel> compraViewModels)
        {
            try
            {
                int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

				compraViewModels = await _compraFacedeServices.GetCompras(idUser);

                if (compraViewModels != null && compraViewModels.Any())
                {
                    TempData["MensagemSucesso"] = "Compras encontradas.";
                }
                else
                {
                    TempData["MensagemErro"] = "Não há compras lançadas.";
                }

                return View("Index", compraViewModels);
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao carregar as compras: " + ex.Message;
                return View("Index", new List<CompraViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Buscar(string numeroConta, string status)
        {
            List<CompraViewModel> compraViewModels = new List<CompraViewModel>();

            try
            {
                int idOc = int.TryParse(numeroConta, out int parsedId) ? parsedId : 0;
				int idUser = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);


				compraViewModels = await _compraFacedeServices.GetCompras(idUser, status, idOc);
                if (compraViewModels != null && compraViewModels.Any())
                {
                    TempData["MensagemSucesso"] = "Dados filtrados encontrados.";
                    return View("Index", compraViewModels);
                }
                else
                {
                    TempData["MensagemErro"] = "Não há compras lançadas com os filtros atuais.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao buscar compras: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Atualizar(int id)
        {
            try
            {
                CompraViewModel compra = await _compraFacedeServices.MapearCompraAsync(id);

                if (compra != null)
                {
                    return View(compra);
                }

                TempData["MensagemErro"] = "Não foi possível trazer os dados da compra.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao acessar a compra: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Imprimir(int id)
        {
            try
            {
                CompraViewModel compra = await _compraFacedeServices.MapearCompraAsync(id);

                if (compra != null && compra.IdCompra != 0)
                {
                    TempData["MensagemSucesso"] = "Dados encontrados.";
                    return View(compra);
                }
                else
                {
                    TempData["MensagemErro"] = "Não foi possível encontrar os dados da compra.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = "Erro ao solicitar impressão: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Baixar(int idCompra, string status)
        {
            if (status == "EM")
            {
                TempData["MensagemErro"] = "Mude o status para continuar.";
                return RedirectToAction("Atualizar", new { id = idCompra });
            }

            if (idCompra != 0 && !string.IsNullOrEmpty(status))
            {
                try
                {
                    // Mapeia a compra para a atualização
                    CompraViewModel compra = await _compraFacedeServices.MapearCompraAsync(idCompra);
                    if (compra == null)
                    {
                        TempData["MensagemErro"] = "Compra não encontrada.";
                        return RedirectToAction("Index");
                    }

                    // Valida a compra antes de prosseguir
                    DomainErrorModel domainError = await _compraFacedeServices.ValidarCompra(compra.IdCompra);
                    if (domainError.Sucesso)
                    {
                        domainError = _compraFacedeServices.BaixarCompra(compra.IdCompra, status);
                        if (domainError.Sucesso)
                        {
                            TempData["MensagemSucesso"] = "Compra baixada com sucesso. Os insumos já estão disponíveis no estoque.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["MensagemErro"] = $"{domainError.Mensagem} - {domainError.Detalhes}";
                            return View("Atualizar", compra);
                        }
                    }
                    else
                    {
                        TempData["MensagemErro"] = $"{domainError.Mensagem} - {domainError.Detalhes}";
                        return View("Atualizar", compra);
                    }
                }
                catch (Exception ex)
                {
                    TempData["MensagemErro"] = $"Erro crítico ao baixar a compra: {ex.Message}";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["MensagemErro"] = "Número da compra ou status inválido.";
                return RedirectToAction("Atualizar", new { id = idCompra });
            }
        }


        [HttpGet]
        public IActionResult Emitir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult NovaCompra([FromBody] CompraJsonView compra)
        {
            try
            {
                if (compra == null || compra.ItensCompra == null || !compra.ItensCompra.Any())
                {
                    return Json(new { Response = false, Message = "Dados da compra estão incompletos." });
                }

                ProcessarCompraResult resultado = _compraFacedeServices.ProcessarCompra(compra);

                if (resultado.Sucesso)
                {
                    return Json(new { Response = true, Message = resultado.Mensagem ?? "Compra emitida com sucesso." });
                }
                else
                {
                    return Json(new { Response = false, Message = resultado.Mensagem ?? "Erro desconhecido ao emitir compra." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Response = false, Message = "Erro no processamento da compra.", Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> BuscarPessoa(int id)
        {
            try
            {
                List<Pessoa> pessoas = await _compraFacedeServices.FornecedoresInsumos(id);
                return Json(pessoas);
            }
            catch (Exception ex)
            {
				TempData["MensagemErro"] = "Erro ao buscar fornecedores: " + ex.Message;
				return Json(new { Response = false, Message = "Erro ao buscar pessoa.", Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> BuscarInsumo(int id)
        {
            try
            {
                List<Insumo> insumos = await _compraFacedeServices.InsumosFornecedores(id);
                return Json(insumos);
            }
            catch (Exception ex)
            {
				TempData["MensagemErro"] = "Erro ao buscar insumos: " + ex.Message;

				return Json(new { Response = false, Message = "Erro ao buscar insumo.", Error = ex.Message });
            }
        }
    }
}
