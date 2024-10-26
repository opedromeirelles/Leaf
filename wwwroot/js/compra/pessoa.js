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

// pegar pessoa expecifica - CHAMADA AJAX
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

                // Exibe no modal o fornecedor vinculado
                $('#pessoaVinculada').html(`de <b>${fornecedor.nome}</b> - <a href="#" onclick="desvincularPessoa()">desvincular</a>`);                

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

