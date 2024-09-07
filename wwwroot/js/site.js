// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// ANIMAÇÃO MENU
const list = document.querySelectorAll('.list');
function activeLink() {
    list.forEach((item) =>
        item.classList.remove('active'));
    this.classList.add('active');
}
list.forEach((item) =>
    item.addEventListener('click', activeLink));

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


