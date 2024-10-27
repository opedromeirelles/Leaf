function OrdemCompraModel(idPessoa, idAdministrativo, itensCompra, valorTotal) {
    this.idPessoa = idPessoa;
    this.idAdministrativo = idAdministrativo;
    this.itensCompra = itensCompra;
    this.valorTotal = parseFloat(valorTotal).toFixed(2); // Valor total com duas casas decimais
}

// Modelo de Item de Compra
function ItemCompraModel(idInsumo, quantidade, valorUnitario) {
    this.idInsumo = idInsumo;
    this.quantidade = parseInt(quantidade, 10); // Quantidade como número inteiro
    this.valorUnitario = parseFloat(valorUnitario).toFixed(2); // Valor unitário com duas casas decimais
    this.subTotal = (this.quantidade * this.valorUnitario).toFixed(2); // Subtotal calculado e formatado
}

// Emitir compra com validações e formatação
function emitirCompra(idPessoa, idAdministrativo, itensCompra, valorTotal) {
    if (!idPessoa) {
        alert("Ops, parece que não há um fornecedor vinculado à ordem de compra.");
        return;
    }
    if (!idAdministrativo) {
        alert("Ops, parece que não há um gerente vinculado à ordem de compra.");
        return;
    }
    if (!itensCompra || itensCompra.length === 0) {
        alert("Ops, parece que não há itens vinculados à ordem de compra.");
        return;
    }
    if (!valorTotal) {
        alert("Ops, parece que não há um valor total para a ordem de compra.");
        return;
    }

    // Criação do objeto compra
    const compra = new OrdemCompraModel(idPessoa, idAdministrativo, itensCompra, valorTotal);

    console.log(compra);
    novaCompra(compra); //disparando a compra
}


// Envio da compra com AJAX
function novaCompra(compra) {
    if (!compra) {
        alert("Erro ao lançar compra: dados da compra estão vazios.");
        return;
    }

    $.ajax({
        url: '/Compra/NovaCompra',
        method: 'POST',
        data: JSON.stringify(compra),
        contentType: 'application/json',
        dataType: 'json',

        success: function (response) {

            console.log("Response from server:", response); // Para ver o que o servidor está retornando

            if (response.response) {

                alert(response.message || "Compra emitida com sucesso."); // Mensagem de sucesso
                setTimeout(function () {
                    window.location.href = "/Compra/Index";
                }, 500);

            } else {
                alert(response.message || "Erro ao processar a compra."); // Mensagem de erro específica
            }
        },

        error: function (xhr, status, error) {
            alert("Erro na requisição:", error);
            console.error("Erro na requisição:", xhr.responseText || error);
        }
    });


}


