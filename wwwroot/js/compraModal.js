// principais ids para controle:

// idPessoa
// btnModalInsumo   - botao clicar para abrir o modal
// pessoaVinculada  - label do nome da pessoa dentro do modal
// inputBuscaInsumo - input de busca do modal
// btnBuscaInsumo   - btn de buscar dentro do modal
// btnLimparBusca   - btn de limpar os dados dentro do modal
// listaInsumo      - tbody dentro do modal (tag do corpo da minha lista)
// mensagemControle - span dentro da lista para mensagens de controle
// btnDispensar     - btn de dispensar dentro do modal (fechar ele ou desistir)
// btnConcluir      - btn de confirmação dentro do modal (concluir a lista)



//MAPEAR UMA LISTA DE INSUMOS
function exibirListaInsumos(insumos, idTbody, idElementoControleMensagem)
{
    // Verifica se a lista está vazia
    if (insumos.length === 0) {
        $(`#${idElementoControleMensagem}`).html('<div class="alert alert-warning text-center">Nenhum insumo encontrado.</div>');
        return;
    }

    // Limpa a mensagem e a lista antes de popular
    $(`#${idElementoControleMensagem}`).html('');
    $(`#${listaElementoId}`).html('');


    // Popula a tabela com os insumos
    $.each(insumos, function (index, insumo) {
        $(`#${idTbody}`).append(`
                            <tr>
                                <td>${insumo.descricao}</td>
                                <td>R$ ${insumo.valorUnitario}</td>
                                <td><input type="number" class="w-50" id="idQuantidade_${index}" /></td>
                                <td>${insumo.pessoa?.nome ?? '-'}</td>
                                <td><a href="#" id="adicionarInsumo_${index}" class="btn btn-primary w-70">Adicionar</a></td>
                            </tr>
        `);
    });
}



function atualizarModal(idPessoa) {

}