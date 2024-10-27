
//AREA DE INSUMOS

//Variaveis globais para controle

var insumosModal = [];
var itemCompra = [];
var insumo;
var InsumoModel;


// meu objeto de controle
function InsumoModelView(idInsumo, descricao, quantidade, valorUnitario, qtdeEstoque, insumoIdPessoa) {
    this.id = idInsumo;
    this.descricao = `${descricao}`;
    this.quantidade = quantidade;
    this.valorUnitario = `${valorUnitario}`;
    this.qtdeEstoque = `${qtdeEstoque}`;
    this.insumoIdPessoa = `${insumoIdPessoa}`;
}

// set valores
function setInsumoModelView(idInsumo, descricao, quantidade, valorUnitario, qtdeEstoque, insumoIdPessoa) {
    InsumoModel = new InsumoModelView(idInsumo, descricao, quantidade, valorUnitario, qtdeEstoque, insumoIdPessoa)
}



// Função para buscar insumos vinculados SE fornecedor selecionado - CHAMADA AJAX
function buscarInsumos(idPessoa) {
    $.ajax({
        url: '/Compra/BuscarInsumo',
        method: 'GET',
        dataType: 'json',
        data: { idPessoa: idPessoa },
        success: function (insumos) {
            listarInsumos(insumos);
        },
        error: function (xhr, status, error) {
            console.error('Erro ao buscar insumos:', xhr.responseText);
        }
    });
}



// Função para construir insumos no modal
function popularHtmlInsumosInModel(insumos) {

    if (insumos && insumos.length > 0) {
        $('#listaInsumo').html('');

        insumos.forEach(function (insumo) {

            var negativo = insumo.qtdeEstoque === 0 ? 'text-danger' : '';

            $('#listaInsumo').append(`
                        <tr id="insumo-${insumo.idInsumo}">
                            <td>${insumo.descricao}</td>
                            <td class="text-center">${insumo.valorUnitario.toFixed(2)}</td>
                            <td class="text-center"><input type="number" class="form-control text-center" id="quantidadeInsumo-${insumo.idInsumo}" value="1" min="1" max="999999"></td>
                            <td class="text-center">${insumo.unidadeMedida}</td>
                            <td class="text-center ${negativo}">${insumo.qtdeEstoque}</td>
                            <td class="text-center">
                                <button type="button" class="btn btn-primary" onclick="adicionarInsumo(${insumo.idInsumo}, '${insumo.descricao}', ${insumo.qtdeEstoque},${insumo.valorUnitario}, ${insumo.idPessoa})">
                                    +
                                </button>
                            </td>
                        </tr>
                `);
        });
    }

    
}


/// Função para listar insumos no modal e armazenar a lista de insumos
function listarInsumos(insumos) {

    if (idPessoa !== 0) {
        // Exibe no modal o fornecedor vinculado
        $('#pessoaVinculada').html(`de <b>${pessoaNomeAtual}</b> - <a href="#" onclick="desvincularPessoa()">desvincular</a>`);
    }

    // Filtra insumos que já foram adicionados ao itemCompra para não duplicá-los no pedido
    var insumosDisponiveis = insumos.filter(insumo => !itemCompra.some(item => item.id === insumo.idInsumo));

    // Filtra os insumos disponíveis para armazenar no vetor de visualização insumosModal
    insumosModal = insumosDisponiveis.filter(insumo => idPessoa === 0 || insumo.idPessoa === idPessoa);

    // Verifica se há insumos disponíveis no vetor de visualização
    if (insumosModal.length === 0) {
        $('#listaInsumo').html('<tr><td colspan="6" class="text-center">Nenhum insumo disponível</td></tr>');
        return;
    }

    // Limpa a lista antes de exibir os insumos disponíveis
    $('#listaInsumo').html('');

    // Itera sobre os insumos disponíveis e exibe-os no modal
    insumosModal.forEach(function (insumo) {

        var negativo = insumo.qtdeEstoque === 0 ? 'text-danger' : '';

        $('#listaInsumo').append(`
            <tr id="insumo-${insumo.idInsumo}">
                <td>${insumo.descricao}</td>
                <td class="text-center">${parseFloat(insumo.valorUnitario).toFixed(2)}</td>
                <td class="text-center"><input type="number" class="form-control text-center" id="quantidadeInsumo-${insumo.idInsumo}" value="1" min="1" max="999999"></td>
                <td class="text-center">${insumo.unidadeMedida}</td>
                <td class="text-center ${negativo}">${insumo.qtdeEstoque}</td>
                <td class="text-center">
                    <button type="button" class="btn btn-primary" onclick="adicionarInsumo(${insumo.idInsumo}, '${insumo.descricao}', ${insumo.qtdeEstoque}, ${insumo.valorUnitario}, ${insumo.idPessoa})">
                        +
                    </button>
                </td>
            </tr>
        `);
    });
}


// Adicionar insumo ao meu ItemCompra
function adicionarInsumo(idInsumo, descricao, valorUnitario, qtdeEstoque, insumoIdPessoa) {
    var insumoSelecionado;
    var quantidade;

    // Captura o valor de quantidade do campo input
    quantidade = parseInt($(`#quantidadeInsumo-${idInsumo}`).val(), 10) || 1; // Usa 1 como valor padrão caso não consiga capturar o valor

    // Se o id da pessoa for zero, o primeiro vínculo acontece aqui
    if (idPessoa === 0) {
        idPessoa = insumoIdPessoa;  // Alimento meu id global

        // Limpo a lista de insumos do modal
        $('#listaInsumo').html('');
        insumosModal = [];

        // Buscar insumos e pessoa associada
        buscarInsumos(idPessoa);
        buscarPessoas(idPessoa);
        vincularPessoaInsumo(idPessoa);

        // Permitir o reinício da compra
        $('#btnControles').addClass('d-flex');
        $('#btnReiniciarCompra').removeAttr('hidden').show();
    } else {

        // Atualizar insumos modais
        insumosModal = [];
        // Buscar insumos e pessoa associada
        buscarInsumos(idPessoa);
    }

    // Adiciona o insumo ao vetor `itemCompra` e atualiza a view
    setInsumoModelView(idInsumo, descricao, valorUnitario, qtdeEstoque, insumoIdPessoa);
    insumoSelecionado = InsumoModel;

    // Adiciona a quantidade e calcula o subtotal
    insumoSelecionado.quantidade = quantidade;
    insumoSelecionado.subTotal = quantidade * valorUnitario;

    addInsumoItem(insumoSelecionado);

    // Ocultar o insumo no modal
    $(`#insumo-${idInsumo}`).hide();

    // log para ver insumo selecionado -> console.log(insumoSelecionado);
    
}


// Se insumo for válido, adiciona na lista e atualiza a view
function addInsumoItem(insumo) {

   if (insumo) {
        insumo.valorUnitario = parseFloat(insumo.valorUnitario); // Garantindo que seja numérico
        insumo.subTotal = insumo.quantidade * insumo.valorUnitario;
        itemCompra.push(insumo);

        $('#listaInsumos').append(`
            <tr id="insumo-item-${insumo.id}">
                <td>${insumo.descricao}</td>
                <td class="text-center">${insumo.quantidade}</td>
                <td class="text-center">${formatarMoeda(insumo.valorUnitario)}</td>
                <td class="text-center">${formatarMoeda(insumo.subTotal)}</td>
                <td class="text-center">
                    <button type="button" class="btn btn-danger" onclick="removerInsumoItem(${insumo.id})">X</button>
                </td>
            </tr>
        `);

        calcularValorTotal(); // Atualiza o total após adicionar item
    }
}


// função para remover insumo da lista da view
function removerInsumoItem(idInsumo) {
    // Remove o item do array `itemCompra` com o ID correspondente
    itemCompra = itemCompra.filter(insumo => insumo.id !== idInsumo);

    // Remove a linha da tabela com o ID correspondente
    $(`#insumo-item-${idInsumo}`).remove();

    // Atualiza o valor total
    calcularValorTotal();
}




