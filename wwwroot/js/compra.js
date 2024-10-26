/// AREA DE PESSOAS (FORNECEDORES):

//variaveis globlais para manipulação

// Pessoa model:
var idPessoa = 0;
var PessoaModel;
var pessoasModal = [];

// Criando um construtor de objeto
function PessoaModelView(id, nome, cnpj) {

    this.id = id;
    this.nome = `${nome}`;
    this.cnpj = `${cnpj}`;

}

//Função para manipular a pessoa selecionada - atualizar o id
function setIdPessoaSelecionada(id, nome, cnpj) {
    PessoaModel = new PessoaModelView(id, nome, cnpj);
}


// construir informações do fornecedor na tela
function poularHtmlPessoaModel(nome, cnpj) {
    $('#sessaoUsuario').html(`
    <br>
    <p><strong>Pessoa: </strong> ${nome}</p>
    <p><strong>CNPJ: </strong> ${cnpj}</p>
    `);
}

// listar pessoas na tabela do modal com ou sem filtro
function popularHtmlPessoasInModel(pessoa) {

    var isChecked = (pessoa.idPessoa === idPessoa) ? 'checked' : '';    

    $('#listaPessoa').append(`
        <tr>
            <td>${pessoa.nome}</td>
            <td>${pessoa.cnpj || 'N/A'}</td>
            <td class="text-center">${pessoa.telefone1 || 'N/A'}</td>
            <td class="text-center">
                <input type="radio" name="pessoaSelecionada" value="${pessoa.idPessoa}" onclick="setIdPessoaSelecionada(${pessoa.idPessoa}, '${pessoa.nome}', '${pessoa.cnpj}')" ${isChecked}>
            </td>
        </tr>
    `);

}

// Função para listar pessoas na tabela do modal e alimentar minha lista de pessoas para controle
function listarPessoas(listaPessoas) {
    $('#listaPessoa').html('');

    listaPessoas.forEach(function (pessoa) {
        popularHtmlPessoasInModel(pessoa);

        // Adiciona a pessoa a lista de pessoasModal se não estiver presente
        if (Array.isArray(pessoasModal) && pessoasModal.length > 0) {
            var contem = pessoasModal.find(p => p.idPessoa === pessoa.idPessoa);

            if (!contem) {
                pessoasModal.push(pessoa);
            }
        } else {
            // Garante que pessoasModal é um array e adiciona a pessoa
            pessoasModal = pessoasModal || []; // Caso esteja null ou undefined
            pessoasModal.push(pessoa);
        }
    });
}

//Função para bloquear o enter do teclado
function blockEnter(idFormulario) {

    var formulario = idFormulario;

    // Intercepta a tecla Enter
    $(formulario).on('keydown', function (event) {
        // Se a tecla for Enter (código 13)
        if (event.key === "Enter") {
            event.preventDefault(); // Impede o comportamento padrão (submeter o formulário)
        }
    });

}


// Função para buscar e listar fornecedores no banco
function buscarPessoas(id) {
    
    $.ajax({
        url: '/Compra/BuscarPessoa',
        method: 'GET',
        dataType: 'json',
        data: { id: id },
        success: function (pessoas) {
            listarPessoas(pessoas);
        },
        error: function (xhr, status, error) {
            console.error('Erro ao buscar pessoas:', xhr.responseText);
        },
        
    });
}

function getPessoa(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: '/Compra/BuscarPessoa',
            method: 'GET',
            dataType: 'json',
            data: { id: id },
            success: function (pessoas) {
                resolve(pessoas); // Resolve a Promise com os dados recebidos
            },
            error: function (xhr, status, error) {
                console.error('Erro ao buscar pessoas:', xhr.responseText);
                reject(error); // Rejeita a Promise em caso de erro
            }
        });
    });
}


//***********************************************/

//AREA DE INSUMOS

//Variaveis globais para controle

var insumosModal = [];
var itemCompra = [];
var insumo;
var InsumoModel;

function InsumoModelView(idInsumo, descricao, quantidade, valorUnitario, qtdeEstoque, insumoIdPessoa) {
    this.id = idInsumo;
    this.descricao = `${descricao}`;
    this.quantidade = quantidade;
    this.valorUnitario = `${valorUnitario}`;
    this.qtdeEstoque = `${qtdeEstoque}`;
    this.insumoIdPessoa = `${insumoIdPessoa}`;
}

function setInsumoModelView(idInsumo, descricao, quantidade, valorUnitario, qtdeEstoque, insumoIdPessoa) {
    InsumoModel = new InsumoModelView(idInsumo, descricao, quantidade, valorUnitario, qtdeEstoque, insumoIdPessoa)
}

// Função para construir insumos no modal
function popularHtmlInsumosInModel(insumos) {
    if (insumos && insumos.length > 0) {

        

        $('#listaInsumo').html('');

        insumos.forEach(function (insumo) {
            $('#listaInsumo').append(`
                <tr id="insumo-${insumo.idInsumo}">
                    <td>${insumo.descricao}</td>
                    <td class="text-center">${insumo.valorUnitario.toFixed(2)}</td>
                    <td class="text-center">
                        <input type="number" class="form-control text-center" id="quantidadeInsumo-${insumo.idInsumo}" value="1" min="1" max="${insumo.qtdeEstoque}">
                    </td>
                    <td class="text-center">${insumo.unidadeMedida}</td>
                    <td class="text-center">${insumo.qtdeEstoque}</td>
                    <td class="text-center">
                        <button type="button" class="btn btn-primary" onclick="adicionarInsumo(${insumo.idInsumo}, '${insumo.descricao}', ${insumo.valorUnitario}, ${insumo.qtdeEstoque}, ${insumo.idPessoa})">
                            +
                        </button>
                    </td>
                </tr>
            `);
        });
    }
}


// Função para buscar insumos vinculados SE fornecedor selecionado
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

/// Função para listar insumos no modal e armazenar a lista de insumos
function listarInsumos(insumos) {
    var html = '';

    // Filtra insumos que já foram adicionados ao itemCompra
    var insumosDisponiveis = insumos.filter(insumo => !itemCompra.some(item => item.id === insumo.idInsumo));

    // Verifica se há insumos disponíveis após o filtro
    if (insumosDisponiveis.length === 0) {
        html = '<tr><td colspan="6" class="text-center">Nenhum insumo disponível</td></tr>';
        $('#listaInsumo').html(html);
        return;
    }

    // Verifica se um idPessoa foi selecionado, se não, lista todos os insumos
    insumosDisponiveis.forEach(function (insumo) {
        // Se idPessoa for 0 (nenhum insumo selecionado ainda) ou o insumo pertence à idPessoa
        if (idPessoa === 0 || insumo.idPessoa === idPessoa) {
            if (insumo.qtdeEstoque > 0) { // Valida se o insumo está em estoque
                html += `
                    <tr id="insumo-${insumo.idInsumo}">
                        <td>${insumo.descricao}</td>
                        <td class="text-center">${insumo.valorUnitario.toFixed(2)}</td>
                        <td class="text-center"><input type="number" class="form-control text-center" id="quantidadeInsumo-${insumo.idInsumo}" value="1" min="1" max="999999"></td>
                        <td class="text-center">${insumo.unidadeMedida}</td>
                        <td class="text-center">${insumo.qtdeEstoque}</td>
                        <td class="text-center">
                            <button type="button" class="btn btn-primary" onclick="adicionarInsumo(${insumo.idInsumo}, '${insumo.descricao}', ${insumo.qtdeEstoque},${insumo.valorUnitario}, ${insumo.idPessoa})">
                                +
                            </button>
                        </td>
                    </tr>
                `;

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
        }
    });

    // Atualiza o HTML da tabela
    $('#listaInsumo').html(html);
}



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

    console.log(insumoSelecionado);
}

function addInsumoItem(insumo) {
    // Se insumo for válido, adiciona na lista e atualiza a view
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

function removerInsumoItem(idInsumo) {
    // Remove o item do array `itemCompra` com o ID correspondente
    itemCompra = itemCompra.filter(insumo => insumo.id !== idInsumo);

    // Remove a linha da tabela com o ID correspondente
    $(`#insumo-item-${idInsumo}`).remove();

    // Atualiza o valor total
    calcularValorTotal();
}







function vincularPessoaInsumo(idPessoa) {

    getPessoa(idPessoa)
        .then(pessoas => {
            if (pessoas && pessoas.length > 0) {
                // Acessa a primeira pessoa da lista
                var pessoa = pessoas[0];

                // Agora, cria o objeto PessoaModelView com a primeira pessoa da lista
                var fornecedor = new PessoaModelView(pessoa.idPessoa, pessoa.nome, pessoa.cnpj);
                pessoasModal = fornecedor;
                PessoaModel = fornecedor;
                console.log("PessoaModel:", PessoaModel);

                //Exibi na view as informações do fornecedor
                poularHtmlPessoaModel(fornecedor.nome, fornecedor.cnpj);

                //Exibi no modal o fornecedor vinculado
                $('#pessoaVinculada').text(`de ${fornecedor.nome}`);                


            } else {
                console.log("Nenhuma pessoa encontrada.");
            }
        })
        .catch(error => {
            console.log("Erro ao acessar pessoas:", error);
        });

}


// Função para calcular o valor total e atualizar o elemento #valorTotal
function calcularValorTotal() {
    let valorTotal = itemCompra.reduce((acc, item) => acc + (item.quantidade * item.valorUnitario), 0);

    // Função para formatar valores em moeda
    const formatarMoeda = (valor) => {
        return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor);
    };

    // Atualiza o valor total na view
    $('#valorTotal').text(formatarMoeda(valorTotal));
}





// Bloco $(document).ready() - quando o documento for lido
$(document).ready(function () {

    
    

    // Reinicar o processo de compra
    $('#btnReiniciarCompra').click(function () {
        if (confirm("Tem certeza que deseja reinicar a ordem de compra, todos os dados seram perdidos.")) {
           location.reload(true);
        }
    });


    //Bloquear o submit via enter
    blockEnter('#frmCompra');


    // Capturar o id da pessoa vinculada - dentro modal
    $('#btnVincularPessoa').click(function () {        

        var pessoaSelecionada = new PessoaModelView(PessoaModel.id, PessoaModel.nome, PessoaModel.cnpj);

        // verifico se o ID da pessoa ja esta zerado
        if (idPessoa === 0) {
            idPessoa = pessoaSelecionada.id;
            console.log(`Fornecedor atualizado para: ${pessoaSelecionada.nome}`);
            poularHtmlPessoaModel(pessoaSelecionada.nome, pessoaSelecionada.cnpj); // Construir informações na view
        } else {

            // Verifica se o ID da pessoa selecionada é diferente do idPessoa atual
            if (idPessoa !== pessoaSelecionada.id) {
                // Se for diferente, exibe uma mensagem de confirmação
                if (confirm("Ao trocar de fornecedor, você perderá todos os itens da ordem de compra. Deseja continuar ?")) {
                    // Se confirmado, atualiza idPessoa e limpa os insumos selecionados
                    idPessoa = pessoaSelecionada.id;

                    console.log(`Fornecedor atualizado para: ${pessoaSelecionada.nome}`);
                    poularHtmlPessoaModel(pessoaSelecionada.nome, pessoaSelecionada.cnpj); // Construir informações na view

                    //limpar lista
                    insumosModal = [];
                    insumo = '';
                    $('#listaInsumos').html('');


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

            console.log(pessoasEncontradas);

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
                alert("insumos não encontrados.");
            }
            console.log(insumosEncontrados);
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




    


   
});

