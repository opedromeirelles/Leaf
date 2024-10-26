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

// Função para calcular o valor total e atualizar o elemento #valorTotal
function calcularValorTotal() {
    let valorTotal = itemCompra.reduce((acc, item) => acc + (item.quantidade * item.valorUnitario), 0);

    // Função para formatar valores em moeda
    const formatarMoeda = (valor) => {
        return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor);
    };

    // Atualiza o valor total na view
    $('#valorTotal').text(formatarMoeda(valorTotal));

    // Atualiza a variável global de valor total
    valorTotalInsumos = valorTotal;
}
