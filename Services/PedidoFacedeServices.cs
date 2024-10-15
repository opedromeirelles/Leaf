using Leaf.Models;

namespace Leaf.Services
{
    public class PedidoFacedeServices
    {
        //Serviços
        private readonly PedidoServices _pedidoService;
        private readonly ItemPedidoServices _itemPedidoService;
        private readonly UsuarioServices _usuarioService;
        private readonly PessoaServices _pessoaService;
        private readonly ProdutoServices _produtoService;

        //Injection de services
        public PedidoFacedeServices(PedidoServices pedidoServices, ItemPedidoServices itemPedidoServices, UsuarioServices usuarioServices, PessoaServices pessoaServices, ProdutoServices produtoServices)
        {
            _pedidoService = pedidoServices;
            _itemPedidoService = itemPedidoServices;
            _usuarioService = usuarioServices;
            _pessoaService = pessoaServices;
            _produtoService = produtoServices;
        }



        // Listar Pedidos e Popular eles
        public List<PedidoViewModel> PreencherPedidos()
        {
            //Lista de pedidos
            List<Pedido> pedidos = GetPedidos();

            //Verfico se há algum pedido, ou seja se nao esta vazia
            if (pedidos != null && pedidos.Any())
            {
                List<PedidoViewModel> listPedidoPreenchido = new List<PedidoViewModel>();

                foreach (var item in pedidos)
                {
                     // Busca os itens desse pedido encontrado
                List<ItemPedido> itensPedido = GetItemPedidos(item.IdPedido);

                // Popular os produtos dentro dos itens
                itensPedido = PopularProdutosEmItens(itensPedido);

                // Monta um objeto central, passando o pedido e os itens
                PedidoViewModel pedidoPreenchido = new PedidoViewModel(item, itensPedido);

                // Popular Entidades: Pessoa, Vendedor e Entregador para ter acesso a suas informações
                pedidoPreenchido = PopularElementos(pedidoPreenchido, item.IdPessoa, item.IdVendedor, item.IdEntregador);

                //Adiciono na lista de pedidos preechidos
                listPedidoPreenchido.Add(pedidoPreenchido);

                }
                if (listPedidoPreenchido != null && listPedidoPreenchido.Any())
                {
                    return listPedidoPreenchido;
                }
                return new List<PedidoViewModel>();
                
            }
            else
            {
                return new List<PedidoViewModel>();
            }

        }
        public List<PedidoViewModel> PreencherPedidos(int idPedido, string status)
        {
            //Lista de pedidos
            List<Pedido> pedidos = GetPedidosFiltro(idPedido, status);

            //Verfico se há algum pedido, ou seja se nao esta vazia
            if (pedidos != null && pedidos.Any())
            {
                List<PedidoViewModel> listPedidoPreenchido = new List<PedidoViewModel>();

                foreach (var item in pedidos)
                {
                    // Busca os itens desse pedido encontrado
                    List<ItemPedido> itensPedido = GetItemPedidos(item.IdPedido);

                    // Popular os produtos dentro dos itens
                    itensPedido = PopularProdutosEmItens(itensPedido);

                    // Monta um objeto central, passando o pedido e os itens
                    PedidoViewModel pedidoPreenchido = new PedidoViewModel(item, itensPedido);

                    // Popular Entidades: Pessoa, Vendedor e Entregador para ter acesso a suas informações
                    pedidoPreenchido = PopularElementos(pedidoPreenchido, item.IdPessoa, item.IdVendedor, item.IdEntregador);

                    //Adiciono na lista de pedidos preechidos
                    listPedidoPreenchido.Add(pedidoPreenchido);

                }
                if (listPedidoPreenchido != null && listPedidoPreenchido.Any())
                {
                    return listPedidoPreenchido;
                }
                return new List<PedidoViewModel>();

            }
            else
            {
                return new List<PedidoViewModel>();
            }

        }

        //Prencher um unico pedido
        public PedidoViewModel PreencherPedido(int idPedido)
        {
            // Busca o pedido
            Pedido pedido = GetPedido(idPedido);

            if (pedido != null)
            {
                // Busca os itens desse pedido encontrado
                List<ItemPedido> itensPedido = GetItemPedidos(pedido.IdPedido);

                // Popular os produtos dentro dos itens
                itensPedido = PopularProdutosEmItens(itensPedido);

                // Monta um objeto central, passando o pedido e os itens
                PedidoViewModel pedidoPreenchido = new PedidoViewModel(pedido, itensPedido);

                // Popular Entidades: Pessoa, Vendedor e Entregador para ter acesso a suas informações
                pedidoPreenchido = PopularElementos(pedidoPreenchido, pedido.IdPessoa, pedido.IdVendedor, pedido.IdEntregador);

                return pedidoPreenchido;
            }
            else
            {
                return new PedidoViewModel();
            }
        }



        //Atualizar Pedido tratativas de regra de negocio
        public List<Produto> AtualizarPedido(PedidoViewModel pedido, int idEntregador)
        {

            //Para cada item do meu pedido, preciso verificar se há a quantidade em estoque para fazer a liberação
            

            List<Produto> produtosNegativos = new List<Produto>(); //listar os produtos negativos em minha View

            foreach (var item in pedido.ItensPedido)
            {
                //Capto a quantidade daquele produto em estoque
                int quantidadeEstoque = _produtoService.GetQuantidadeEstoque(item.IdProduto);

                //Verifico se a quantidade solicidada é maior do que tenho em estoque
                if (item.Quantidade > quantidadeEstoque)
                {
                    // se verdaderio adiciona o produto em minha lista
                    produtosNegativos.Add(_produtoService.GetProduto(item.IdProduto));
                }
            }

            //Após o laço de repetição verificando produto a produto, verifico se há algo em minha lista
            if (produtosNegativos != null && produtosNegativos.Any())
            {
                return produtosNegativos; // Passo para minha view os produtos negativos
            }
            else if (_pedidoService.AtualizaStatusPedido(pedido.IdPedido, idEntregador)) // Vejo se foi possivel atualizar
            {
                return new List<Produto>(); // Retorno lista vazia
            }

            return produtosNegativos; // Passo para minha view os produtos negativos
        }

       


        //Buscar Pedido
        public Pedido GetPedido(int idPedido)
        {
            Pedido pedido = _pedidoService.GetPedido(idPedido);
            if (pedido != null)
            {
                return pedido;
            }
            return new Pedido();
        }

        //Buscar Pedido com filtro
        public List<Pedido> GetPedidosFiltro(int idPedido, string status)
        {
            List<Pedido> pedidos = _pedidoService.GetPedidosFiltro(idPedido, status);
            if (pedidos.Any() && pedidos != null)
            {
                return pedidos;
            }
            return new List<Pedido>();
        }


        //Buscando lista de Pedidos
        public List<Pedido> GetPedidos()
        {
            List<Pedido> pedidos = _pedidoService.GetPedidos();
            if (pedidos != null && pedidos.Any())
            {
                return pedidos;
            }
            return new List<Pedido>();
        }

        //Popular elementos e adicionar ao pedido
        public PedidoViewModel PopularElementos(PedidoViewModel pedido ,int idPessoa, int idVendedor, int idEntregador)
        {
            pedido.Pedido.Pessoa = GetPessoaPedido(idPessoa);
            pedido.Pedido.Vendedor = GetUsuarioPedido(idVendedor);
            pedido.Pedido.Entregador = GetUsuarioPedido(idEntregador);

            return pedido;
        }

        //Retornar os itens associado ao meu pedido
        public List<ItemPedido> GetItemPedidos(int idPedido)
        {
            List<ItemPedido> itensPedido = _itemPedidoService.GetItensPedido(idPedido);

            if (itensPedido != null && itensPedido.Any())
            {
                // Popular os produtos para cada item de pedido
                return PopularProdutosEmItens(itensPedido);
            }
            else
            {
                Console.WriteLine($"Nenhum item encontrado para o pedido {idPedido}.");
                return new List<ItemPedido>();
            }
        }

        //Retornar Popular lista de produtos vigente aos itens do pedido
        public List<ItemPedido> PopularProdutosEmItens(List<ItemPedido> itemPedidos)
        {
            foreach (var item in itemPedidos)
            {
                // Obtenha o produto associado ao item
                Produto produto = GetProdutoPedido(item.IdProduto);
                if (produto != null)
                {
                    item.Produto = produto; // Defina o produto no item
                }
                else
                {
                    item.Produto = new Produto { Descricao = "Produto não encontrado", ValorUnitario = 0 };
                }
            }
            return itemPedidos;

        }

        //Retornar usuarios associado ao pedido
        public Usuario GetUsuarioPedido(int idUsuario)
        {
            Usuario usuario = _usuarioService.GetUsuarioId(idUsuario);
            if (usuario != null)
            {
                return usuario;
            }

            return new Usuario();

        }

        //Retornar pessoa associada ao pedido
        public Pessoa GetPessoaPedido(int idPessoa)
        {
            Pessoa pessoa = _pessoaService.GetPessoa(idPessoa);
            if (pessoa != null)
            {
                return pessoa;
            }

            return new Pessoa();
        }

        //Retornar produto associado ao item do pedido
        public Produto GetProdutoPedido(int idProduto)
        {
            Produto produto = _produtoService.GetProduto(idProduto);
            if (produto != null)
            {
                return produto;
            }

            return new Produto();
        }

    }
}
