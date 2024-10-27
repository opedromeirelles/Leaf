/// AREA DE PESSOAS (FORNECEDORES):

// Pessoa model:
var idPessoa = 0;
var pessoaNomeAtual = "";
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



// Função para buscar e listar as pessoas no banco - CHAMADA AJAX
function buscarPessoas(id) {
    $('#listaPessoa').html('<tr><td colspan="4" class="text-center">Carregando...</td></tr>'); // Exibe o loading antes de iniciar a chamada AJAX

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
            $('#listaPessoa').html('<tr><td colspan="4" class="text-center text-danger">Erro ao carregar pessoas</td></tr>');
        },
    });
}

// Função para exibir uma pessoa na tabela do modal
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

// Função para listar pessoas na tabela do modal e alimentar a lista de controle pessoasModal
function listarPessoas(listaPessoas) {
    $('#listaPessoa').html(''); // Limpa o conteúdo atual

    if (!Array.isArray(listaPessoas) || listaPessoas.length === 0) {
        // Caso não haja pessoas na lista, exibe a mensagem "Nenhuma pessoa disponível"
        $('#listaPessoa').html('<tr><td colspan="4" class="text-center">Nenhuma pessoa disponível</td></tr>');
        return;
    }

    // Caso haja pessoas, popula a lista
    listaPessoas.forEach(function (pessoa) {
        popularHtmlPessoasInModel(pessoa);

        // Adiciona a pessoa à lista de pessoasModal se ainda não estiver presente
        if (!pessoasModal.some(p => p.idPessoa === pessoa.idPessoa)) {
            pessoasModal.push(pessoa);
        }
    });
}

// Função para obter uma pessoa específica - CHAMADA AJAX
function getPessoa(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: '/Compra/BuscarPessoa',
            method: 'GET',
            dataType: 'json',
            data: { id: id },
            success: function (pessoas) {
                resolve(pessoas); 
            },
            error: function (xhr, status, error) {
                console.error('Erro ao buscar pessoas:', xhr.responseText);
                reject(error); 
            }
        });
    });
}



// construir informações do fornecedor na tela
function poularHtmlPessoaModel(nome, cnpj) {
    $('#sessaoUsuario').html(`
    <br>
    <p><strong>Pessoa: </strong> ${nome}</p>
    <p><strong>CNPJ: </strong> ${cnpj}</p>
    `);

    // Permitir o reinício da compra
    $('#btnControles').addClass('d-flex');
    $('#btnReiniciarCompra').removeAttr('hidden').show();
}


// vincular pessoa ao insumo, caso o insumo for selecionado primeiro
function vincularPessoaInsumo(idPessoa) {
    getPessoa(idPessoa)
        .then(pessoas => {
            if (pessoas && pessoas.length > 0) {
                // Acessa a primeira pessoa da lista
                var pessoa = pessoas[0];

                // Cria o objeto PessoaModelView com a primeira pessoa da lista
                var fornecedor = new PessoaModelView(pessoa.idPessoa, pessoa.nome, pessoa.cnpj);

                // Atualiza a variável PessoaModel, mantendo pessoasModal como um array
                PessoaModel = fornecedor;

                // Log de pessoa -> console.log("PessoaModel:", PessoaModel);

                // Exibe na view as informações do fornecedor
                poularHtmlPessoaModel(fornecedor.nome, fornecedor.cnpj);

                pessoaNomeAtual = fornecedor.nome;

                // Exibe no modal o fornecedor vinculado
                $('#pessoaVinculada').html(`de <b>${pessoaNomeAtual}</b> - <a href="#" onclick="desvincularPessoa()">desvincular</a>`);                

                // Permitir o reinício da compra
                $('#btnControles').addClass('d-flex');
                $('#btnReiniciarCompra').removeAttr('hidden').show();
            } else {
                console.log("Nenhuma pessoa encontrada.");
            }
        })
        .catch(error => {
            console.log("Erro ao acessar pessoas:", error);
        });
}

function desvincularPessoa() {
    if (confirm("Ao desvincular forncedor atual você perderá os itens já adicionado a lista, tem certeza que deseja continuar?"))
    {
        // zerar todos os dados:
        idPessoa = 0;
        pessoasModal = [];
        itemCompra = [];
        PessoaModel = '';
        insumo = '';
        insumosModal = [];
        InsumoModel = '';

        // apagar todos os componentes:
        $('#listaInsumos').html('');
        $('#pessoaVinculada').html('');
        $('#sessaoUsuario').html('');
        // Atualiza o valor total na view
        $('#valorTotal').text("R$ 0");

        

        // atualizando listas
        buscarPessoas();
        buscarInsumos();

    }
    else {
        console.log("Ação de desvincular cancelada.");
    }
    

}

