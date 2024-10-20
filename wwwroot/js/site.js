
//Evento carreagamento
function mostrarCarregando() {
    document.getElementById('loadingSpinner').style.display = 'flex';
}


//Botao Drop Custom:

// Função para abrir/fechar o dropdown
function toggleDropdown() {
    const dropdownMenu = document.getElementById("dropdownMenuCustom");
    dropdownMenu.classList.toggle("show");
}

// Função para capturar o item selecionado, mostrar no botão e fechar o dropdown
function selectItem(department) {
   
    document.getElementById("dropdownButton").innerHTML = department;

    
    toggleDropdown();
}

window.onclick = function (event) {
    if (!event.target.matches('.btn-secondary')) {
        const dropdowns = document.getElementsByClassName("dropdown-menu");
        for (let i = 0; i < dropdowns.length; i++) {
            const openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}


// Trabalhando com Sweetalert para confirmação de tomada de desição:

//VER SENHA:

document.addEventListener('DOMContentLoaded', function () {
    var mostrarSenhaCheckbox = document.getElementById('mostrarSenha');
    var senhaInput = document.getElementById('senhaUsuario');
    var confirmaSenhaInput = document.getElementById('confirmaSenha');

    if (mostrarSenhaCheckbox) {
        mostrarSenhaCheckbox.addEventListener('change', function () {
            if (this.checked) {
                senhaInput.type = 'text';
                confirmaSenhaInput.type = 'text';
            } else {
                senhaInput.type = 'password';
                confirmaSenhaInput.type = 'password';
            }
        });
    }
});


// Validação de ponto para virgula

document.addEventListener('DOMContentLoaded', function () {
    // Obtenha o campo de valor unitário
    var valorUnitarioInput = document.querySelector('input[name="ValorUnitario"]');

    // Adicione o evento 'blur' ao campo de valor unitário (acionado quando o campo perde o foco)
    if (valorUnitarioInput) {
        valorUnitarioInput.addEventListener('blur', function () {
            // Substituir ponto por vírgula ao perder o foco
            valorUnitarioInput.value = valorUnitarioInput.value.replace(".", ",");
        });
    }

    // Adicione o evento 'submit' ao formulário para garantir que a conversão ocorra também no envio
    var form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (event) {
            // Substituir ponto por vírgula antes do envio, se houver
            valorUnitarioInput.value = valorUnitarioInput.value.replace(".", ",");
        });
    }
});


// Tratando unidade de medida
document.addEventListener('DOMContentLoaded', function () {
    // Seleciona o campo pelo ID (ou use name se preferir)
    const unidadeMedidaInput = document.getElementById('unidadeMedida');

    if (unidadeMedidaInput) {
        // Adiciona um evento 'input' para transformar o valor em maiúsculas enquanto o usuário digita
        unidadeMedidaInput.addEventListener('input', function () {
            this.value = this.value.toUpperCase();
        });
    }
});


