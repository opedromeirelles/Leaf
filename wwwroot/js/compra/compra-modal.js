function OrdemCompraModel(idPessoa, idAdministrativo, itensCompra, valorTotal) {
    this.idPessoa = idPessoa;
    this.idAdministrativo = idAdministrativo;
    this.itensCompra = itensCompra;
    this.valorTotal = valorTotal;
}

function ItemCompraModel(idInsumo, quantidade, valorUnitario) {
    this.idInsumo = idInsumo;
    this.quantidade = quantidade;
    this.valorUnitario = valorUnitario;
    this.subTotal = quantidade * valorUnitario;
}

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

    // Envio da requisição AJAX para o backend
    novaCompra(compra);
}

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
            alert("COMPRA EMITIDA COM SUCESSO - Redirecionando para o incio.");

            setTimeout(function () {
                window.location.href = "/Compra/Index";
            }, 500); 

        },
        error: function (xhr, status, error) {
            alert("Erro na requisição:", error);
            console.error("Erro na requisição:", xhr.responseText || error);
        }
    });
}


