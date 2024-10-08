﻿
// ANIMAÇÃO MENU
const list = document.querySelectorAll('.list');

// Função para adicionar a classe 'active' ao item clicado
function activeLink() {
    list.forEach((item) => item.classList.remove('active'));
    this.classList.add('active');
}

// Adiciona o evento de clique a todos os itens da lista
list.forEach((item) => item.addEventListener('click', activeLink));

// Detecta a URL atual e ajusta o menu ativo
document.addEventListener("DOMContentLoaded", () => {
    const currentPath = window.location.pathname.toLowerCase();

    list.forEach((item) => {
        const link = item.querySelector('a');
        if (link && link.getAttribute('href').toLowerCase() === currentPath) {
            item.classList.add('active');
        }
    });
});

// ANIMAÇÃO DE EVENTOS DE MENSAGEM

//ERRO:
// Função para fechar a notificação
document.addEventListener('DOMContentLoaded', function () {
    const alertBox = document.getElementById('eventoErro');

    if (alertBox) {
        // Revela o alerta ao carregar a página
        alertBox.style.opacity = '1';

        // Fecha o alerta após 7 segundos
        setTimeout(() => {
            closeAlert();
        }, 7000); // 7 segundos
    }
});

function closeAlert() {
    const alertBox = document.getElementById('eventoErro');
    if (alertBox) {
        // Oculta o alerta com transição suave
        alertBox.style.opacity = '0';
        setTimeout(() => {
            alertBox.style.display = 'none';
        }, 500); // 0,5 segundos após iniciar o fade-out
    }
}

//SUCESSO
document.addEventListener('DOMContentLoaded', function () {
    const successBox = document.getElementById('eventoSucesso');

    if (successBox) {
        
        successBox.style.opacity = '1';

        
        setTimeout(() => {
            closeSuccessAlert();
        }, 7000); // 7 segundos
    }
});

function closeSuccessAlert() {
    const successBox = document.getElementById('eventoSucesso');
    if (successBox) {
        
        successBox.style.opacity = '0';
        setTimeout(() => {
            successBox.style.display = 'none';
        }, 500); 
    }
}

//ALTERAÇÃO - MANIPULAÇÃO
document.addEventListener('DOMContentLoaded', function () {
    const alteracaoBox = document.getElementById('eventoAlteracao');

    if (alteracaoBox) {
        
        alteracaoBox.style.opacity = '1';

        
        setTimeout(() => {
            closeAlteracaoAlert();
        }, 7000); // 7 segundos
    }
});

function closeAlteracaoAlert() {
    const alteracaoBox = document.getElementById('eventoAlteracao');
    if (alteracaoBox) {
        
        alteracaoBox.style.opacity = '0';
        setTimeout(() => {
            alteracaoBox.style.display = 'none';
        }, 500); 
    }
}

//botao drop
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

//Confrimação:

function confirmarAcao(url, opcao) {
    // Corrige a declaração da variável opcao
    if (opcao === "negativo") {
        // Define a função para o caso negativo
        Swal.fire({
            title: 'Você tem certeza?',
            text: "Você não poderá reverter isso!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sim, continuar!',
            cancelButtonText: 'Desistir'
        }).then((result) => {
            if (result.isConfirmed) {
                // Se confirmado, redireciona para a URL
                window.location.href = url;
            } else {
                console.log('Usuário cancelou a ação');
            }
        });

    } else if (opcao === "positivo") {
        // Define a função para o caso positivo
        Swal.fire({
            title: 'Você tem certeza?',
            text: "Revise os dados inseridos!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Tudo certo, continuar!',
            cancelButtonText: 'Desistir'
        }).then((result) => {
            if (result.isConfirmed) {
                // Se confirmado, redireciona para a URL
                window.location.href = url;
            } else {
                console.log('Usuário cancelou a ação');
            }
        });

    } else {
        // Caso não seja nem positivo nem negativo, mostrar mensagem de erro
        Swal.fire({
            title: 'Algo deu errado',
            text: "Não conseguimos prosseguir, tente novamente!",
            icon: 'error',
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Confirmar'
        });
    }
}

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


