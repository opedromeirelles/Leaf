var valorTotalInsumos = 0;

// Função para bloquear o enter do teclado
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

// Função de formatação de moeda em reais
function formatarMoeda(valor) {
    return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor);
}



// Função para calcular o valor total de todos os itens
function calcularValorTotal() {
    let valorTotal = itemCompra.reduce((acc, item) => acc + (item.quantidade * item.valorUnitario), 0);
    $('#valorTotal').text(formatarMoeda(valorTotal));

    valorTotalInsumos = valorTotal;

    return valorTotal;
}
