// Bloco $(document).ready() - quando o documento for lido - EVENTOS:
$(document).ready(function () {

    // Eventos e tratativas:

    //Bloquear o submit via enter
    blockEnter('#frmCompra');

    // Reinicar o processo de compra
    $('#btnReiniciarCompra').click(function () {
        if (confirm("Tem certeza que deseja reinicar a ordem de compra, todos os dados seram perdidos.")) {
           location.reload(true);
        }
    });


    // **************   Eventos de Pessoa   ************* //

    // Capturar o id da pessoa vinculada - dentro modal 
    $('#btnVincularPessoa').click(function () {        

        var pessoaSelecionada = new PessoaModelView(PessoaModel.id, PessoaModel.nome, PessoaModel.cnpj);

        // verifico se o ID da pessoa ja esta zerado
        if (idPessoa === 0) {

            idPessoa = pessoaSelecionada.id;
            // log fornecedor atualizado -> console.log(`Fornecedor atualizado para: ${pessoaSelecionada.nome}`);
            poularHtmlPessoaModel(pessoaSelecionada.nome, pessoaSelecionada.cnpj); // Construir informações na view

            // Atualiza nome global
            pessoaNomeAtual = pessoaSelecionada.nome; 

            

        } else {

            // Verifica se o ID da pessoa selecionada é diferente do idPessoa atual
            if (idPessoa !== pessoaSelecionada.id) {
                // Se for diferente, exibe uma mensagem de confirmação
                if (confirm("Ao trocar de fornecedor, você perderá todos os itens da ordem de compra. Deseja continuar ?")) {

                    // Se confirmado, atualiza idPessoa e limpa os insumos selecionados
                    idPessoa = pessoaSelecionada.id;

                    // log fornecedor atualizado -> console.log(`Fornecedor atualizado para: ${pessoaSelecionada.nome}`);
                    poularHtmlPessoaModel(pessoaSelecionada.nome, pessoaSelecionada.cnpj); // Construir informações na view

                    //limpa lista item compra
                    itemCompra = [];

                    //Limpa visual lista item compra
                    $('#listaInsumos').html('');

                    // Atualiza nome global
                    pessoaNomeAtual = pessoaSelecionada.nome; 

                    // Atualiza o valor total na view
                    $('#valorTotal').text('R$ 0');


                } else {
                    console.log("Ação cancelada.");
                }
            } else {
                console.log("Fornecedor já vinculado.");
            }
        }

    });


    // Evento de busca do fornecedor - na barra de busca do modal
    $('#btnBuscaPessoa').click(function () {
        // Verifica se o array pessoasModal não está vazio
        if (pessoasModal && pessoasModal.length > 0) {

            // ler inuput
            var nomePessoa = $('#inputBuscarPessoa').val();

            var pessoasEncontradas = pessoasModal.filter(pessoa =>
                pessoa.nome.toLowerCase().includes(nomePessoa.toLowerCase())
            );

            // log de pessoas encontradas console.log(pessoasEncontradas);

            //Verifica se econtrou
            if (pessoasEncontradas && pessoasEncontradas.length > 0) {

                //Lipar model view lista
                $('#listaPessoa').html('');

                //Constroi a lista
                pessoasEncontradas.forEach(function (pessoa) {

                    popularHtmlPessoasInModel(pessoa);
                    
                })
            }
            else {
                alert("Fornecdor não econtrado");
                $('#inputBuscarPessoa').val('');
            }

        } else {
            console.log("Nenhuma pessoa disponível para busca.");
        }
    });

    //Evento para listar as pessoas ao abrir o modal
    $('#btnAdicionarPessoa').click(function () {
        $('#listaPessoa').html('');
        $('#inputBuscarPessoa').val('');
        buscarPessoas();
    });

    //Evento para limpar busca dentro da model
    $('#btnLimparBuscaPessoa').click(function () {
        $('#inputBuscarPessoa').val('');
        buscarPessoas();
    })

    
   // **************   Eventos de insumos   ************* //


    // Buscar insumos ao abrir o modal de insumos
    $('#btnModalInsumo').click(function () {
        $('#inputBuscaInsumo').val('');        
        buscarInsumos(idPessoa);        
    });


    // Evento de busca dentro do modal
    $('#btnBuscaInsumo').click(function () {
        //verifca se meu array de insumos nao esta vazio

        if (insumosModal && insumosModal.length > 0) {

            // ler input
            var nomeInsumo = $('#inputBuscaInsumo').val();

            // buscar insumos
            var insumosEncontrados = insumosModal.filter(insumo =>
                insumo.descricao.toLowerCase().includes(nomeInsumo.toLowerCase()));


            // verifica se encontrou
            if (insumosEncontrados && insumosEncontrados.length > 0) {
               
                popularHtmlInsumosInModel(insumosEncontrados);
            }
            else {
                alert("Insumos não encontrados.");
                $('#inputBuscaInsumo').val('');
            }
            // log de insumos encontrados - > console.log(insumosEncontrados);
        }
        else {
            alert("Insumos indisponíveis no momento.")
        }
    });


    // Evento de limpar busca dentro do modal
    $('#btnLimparBusca').click(function () {
        insumosModal = [];
        $('#inputBuscaInsumo').val('');
        buscarInsumos(idPessoa);
    });



    // **************   Evento Emitir Compra   ************* //

    $('#btnConfirmar').click(function (event) {
        event.preventDefault();

        // Calcula o valor total dos itens na compra
        calcularValorTotal();

        const idAdministrativo = $('#administrativo').text();
        const idPessoaVinculada = idPessoa;
        const valorTotalCompra = valorTotalInsumos;

        

        // Montando os itens da compra a partir da lista `itemCompra`
        const itensCompra = itemCompra.map(item => {
            return new ItemCompraModel(item.id, item.quantidade, item.valorUnitario);
        });

        // Emite a compra com os dados processados
        emitirCompra(idPessoaVinculada, idAdministrativo, itensCompra, valorTotalCompra);
    });


});




