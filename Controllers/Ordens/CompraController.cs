using Leaf.Models.Domain;
using Leaf.Services.Agentes;
using Leaf.Services.Compras;
using Leaf.Services.Materiais;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leaf.Controllers.Ordens
{
    [Authorize]
    public class CompraController : Controller
    {

        private readonly CompraServices _compraServices;
        private readonly ItemCompraServices _itemcompraServices;
        private readonly UsuarioServices _usuarioServices;
        private readonly PessoaServices _pessoaServices;
        private readonly InsumoServices _insumoServices;


        public CompraController(CompraServices compra, ItemCompraServices itemCompra, UsuarioServices usuario, InsumoServices insumo, PessoaServices pessoa)
        {
            _compraServices = compra;
            _itemcompraServices = itemCompra;
            _usuarioServices = usuario;
            _insumoServices = insumo;
            _pessoaServices = pessoa;
        }

        public IActionResult Index()
        {
            List<Compra> compra = new List<Compra>();
            return View(compra);
        }

        public IActionResult NovaCompra(Compra compra)
        {

            return View(compra);
        }

        [HttpPost]
        public IActionResult Emitir(Compra compra, int IdUsuario)
        {
            // Atribuindo um valor fixo ao ID do usuário enquanto não persiste o ID do login

            IdUsuario = 1;
            compra.IdUsuario = IdUsuario;
            compra.DtaEmissao = DateTime.Now;
            compra.Status = "EM";

            // Se a compra foi cadastrada com sucesso, redireciona para a Index
            if (_compraServices.NovaCompra(compra) != 0)
            {
                //Inserir os itens:

                TempData["MensagemSucesso"] = "Compra emitida com sucesso!";
                return RedirectToAction("Index"); // Redireciona para a lista de compras após sucesso
            }


            // Se houve erro na validação, retorna para a página de cadastro com uma mensagem de erro
            TempData["MensagemErro"] = "Não foi possível efetuar a nova compra.";
            return View("NovaCompra", compra); // Reexibe o formulário de NovaCompra com o estado do modelo
        }

        [HttpGet]
        public JsonResult ListarInsumos(int idPessoa)
        {
            List<Insumo> insumos = new List<Insumo>();

            // Verificar se o id é 0, significa que pessoa nao esta prenchida, então deve se esperar selecionar um inusmo
            if (idPessoa == 0)
            {
                insumos = _insumoServices.ListarInsumos();
                foreach (var insumo in insumos)
                {
                    insumo.Pessoa = _pessoaServices.GetPessoa(insumo.IdPessoa);
                }
                return Json(insumos);
            }
            // Se o id tiver valor, listaremos so os produtos dessa pessoa
            else if (idPessoa != 0)
            {
                insumos = _insumoServices.ListarInsumosForPessoa(idPessoa);
                foreach (var insumo in insumos)
                {
                    insumo.Pessoa = _pessoaServices.GetPessoa(insumo.IdPessoa);
                }
                return Json(insumos);
            }

            return Json(insumos); //se vier diferente, lista vazia

        }

        [HttpGet]
        public JsonResult ListarInsumoPessoa(int idPessoa, string descricao)
        {
            List<Insumo> insumos = new List<Insumo>();

            if (idPessoa == 0)
            {
                insumos = _insumoServices.ListarInsumos(descricao);
                foreach (var insumo in insumos)
                {
                    insumo.Pessoa = _pessoaServices.GetPessoa(insumo.IdPessoa);
                }
                return Json(insumos);
            }
            else if (idPessoa != 0)
            {
                insumos = _insumoServices.ListarInsumosForPessoa(idPessoa, descricao);
                foreach (var insumo in insumos)
                {
                    insumo.Pessoa = _pessoaServices.GetPessoa(insumo.IdPessoa);
                }
                return Json(insumos);
            }

            return Json(insumos);

        }








        /*
        [HttpGet]
        public JsonResult ListaInsumo(string descricaoInsumo)
        {
            List<Insumo> insumo = _insumoServices.ListarInsumosFiltrados(descricaoInsumo, "", 3);
            if (!insumo.Any())
            {
                insumo = _insumoServices.ListarInsumos();
            }
            return Json(insumo);
        }

        */



        /*
        [HttpPost]
        public JsonResult AdicionarInsumoList(int idInsumo, string quantidade)
        {
           
            ItemCompra itemCompra = new ItemCompra();
            Insumo insumo = new Insumo();

            itemCompra.IdInsumo = idInsumo;
            insumo = _insumoServices.GetInsumo(idInsumo);

            if (insumo.IdInsumo != idInsumo)
            {
                
                itemCompra.Quantidade = Convert.ToInt32(quantidade);
                itemCompra.SubTotal += itemCompra.Quantidade * insumo.ValorUnitario;
                itemCompra.itemCompras.Add(insumo);
                return Json(new { sucesso = true, mensagem = "Item adicionado com sucesso!"});
            }
            else
            {
                TempData["MensagemErro"] = "Insumo ja inserido!";
                return Json(new { sucesso = false, mensagem = "Erro ao adicionar item" });
            }
        }
        */

    }
}
