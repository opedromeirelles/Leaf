
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
    
    
    // Filtra insumos que já foram adicionados ao itemCompra
    var insumosDisponiveis = insumos.filter(insumo => !itemCompra.some(item => item.id === insumo.idInsumo));

    // Verifica se há insumos disponíveis após o filtro
    if (insumosDisponiveis.length === 0) {
        html = '<tr><td colspan="6" class="text-center">Nenhum insumo disponível</td></tr>';
        $('#listaInsumo').html(html);
        return;
    }

    $('#listaInsumo').html('');

    // Verifica se um idPessoa foi selecionado, se não, lista todos os insumos
    insumosDisponiveis.forEach(function (insumo) {
        // Se idPessoa for 0 (nenhum insumo selecionado ainda) ou o insumo pertence à idPessoa
        if (idPessoa === 0 || insumo.idPessoa === idPessoa) {

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
           

            // Alimenta o vetor de controle de insumos
            if (insumosModal && insumosModal.length > 0) {
                var contemInsumo = insumosModal.find(i => i.idInsumo === insumo.idInsumo);
                if (!contemInsumo) {
                    insumosModal.push(insumo);
                }
            } else {
                insumosModal.push(insumo);
            }
        }    
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
        itemCompra.push(insumo);

        // Calcula o total do insumo e formata como moeda
        var total = insumo.quantidade * insumo.valorUnitario;

        // Função para formatar valores em moeda
        const formatarMoeda = (valor) => {
            return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor);
        };

        // Atualiza a lista de insumos na view
        $('#listaInsumos').append(`
            <tr id="insumo-item-${insumo.id}">
                <td>${insumo.descricao}</td>
                <td class="text-center">${insumo.quantidade}</td>
                <td class="text-center">${formatarMoeda(insumo.valorUnitario)}</td>
                <td class="text-center">${formatarMoeda(total)}</td>
                <td class="text-center">
                    <button type="button" class="btn btn-danger" onclick="removerInsumoItem(${insumo.id})">X</button>
                </td>
            </tr>
        `);

        // Atualiza o valor total
        calcularValorTotal();
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




