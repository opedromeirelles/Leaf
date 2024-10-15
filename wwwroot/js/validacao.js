// Validar PESSOA

//Mascara
function mascaraCnpj(idInuput) {

    var inpCnpjPessoa = $(`#${idInuput}`);

    // Adicionar máscara de CNPJ enquanto o usuário digita
    inpCnpjPessoa.on('input', function () {
        let value = $(this).val().replace(/\D/g, ''); // Remover caracteres não numéricos
        if (value.length > 14) value = value.substring(0, 14); // Limitar o valor a 14 dígitos
        value = value.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, "$1.$2.$3/$4-$5");
        $(this).val(value);
    });
}

function validarPessoa(idInpCnpj) {

    var cnpjControle = $(`${idInpCnpj}`);

    // Realiza a chamada AJAX para validação
    $.ajax({
        url: '/Pessoa/ValidarPessoa',
        type: 'GET',
        data: { cnpj: cnpjControle },

        beforeSend: function () {
            $btnValidarCnpj.html('Validando...').prop('disabled', true);
        },

        success: function (pessoa) {
            if (pessoa) {
                $spnValidacao.html('Pessoa verificada.').removeClass('text-danger').addClass('text-success');
                $spnPessoaNome.html(`Nome: ${pessoa.nome}`);
                $idPessoa.val(pessoa.idpessoa);
                $inpCnpj.val(pessoa.cnpj);
                $btnSubmit.prop('disabled', false);
            } else {
                $spnValidacao.html('Pessoa Inválida.').removeClass('text-success').addClass('text-danger');
                $spnPessoaNome.html('');
                $btnSubmit.prop('disabled', true);
            }
        },
        complete: function () {
            $btnValidarCnpj.html('Validar Pessoa').prop('disabled', false);
        }
    });
}

//Rotina
$(document).ready(function () {
    // Definir a rotina de validação em uma função

    window.iniciarValidacaoPessoa = function (submitButtonId, cnpjInputId, validarButtonId, validacaoLabelId, nomeLabelId, idPessoaInputId) {

        const $btnSubmit = $(`#${submitButtonId}`);
        const $inpCnpj = $(`#${cnpjInputId}`);
        const $btnValidarCnpj = $(`#${validarButtonId}`);
        const $spnValidacao = $(`#${validacaoLabelId}`);
        const $spnPessoaNome = $(`#${nomeLabelId}`);
        const $idPessoa = $(`#${idPessoaInputId}`);

        // Desabilitar o botão de confirmação por padrão
        $btnSubmit.prop('disabled', true);


        mascaraCnpj("inpCnpj");
        

        // Validação de pessoa ao clicar no botão
        $btnValidarCnpj.click(function () {
            var cnpjPessoa = $inpCnpj.val();

            if (cnpjPessoa.trim() === "" || cnpjPessoa.length !== 18) {
                $spnValidacao.html('Por favor, digite um CNPJ válido.').removeClass('text-success').addClass('text-danger');
                $spnPessoaNome.html('');
                $btnSubmit.prop('disabled', true);
                return;
            }

            validarPessoa("cnpjPessoa");

        });

        // Se o valor do CNPJ for alterado, desabilitar o botão de confirmação
        $inpCnpj.on('input', function () {
            $btnSubmit.prop('disabled', true);
            $spnValidacao.html('*Validação Obrigatória').removeClass('text-success').addClass('text-danger');
            $spnPessoaNome.html('');
        });
    };
});
