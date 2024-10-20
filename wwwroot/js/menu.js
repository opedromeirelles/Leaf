// ANIMAÇÃO MENU

// Seleciona todos os itens da lista e a barra lateral de navegação
const list = document.querySelectorAll('.list');
const navegation = document.querySelector('.navegation');

// Função para adicionar a classe 'active' ao item clicado
function activeLink() {
    list.forEach((item) => {
        item.classList.remove('active');
        // Remove submenu de relatórios, se existir
        const submenu = item.querySelector('.submenu');
        if (submenu) {
            submenu.remove();
        }
    });

    this.classList.add('active');
    // Adiciona o submenu dinamicamente se for o item "Relatórios"
    if (this.classList.contains('relatorios')) {
        createSubmenu(this);
        navegation.classList.add('expanded');
    } else {
        navegation.classList.remove('expanded');
    }
}

// Função para criar o submenu dinamicamente
function createSubmenu(parentItem) {
    const submenuHTML = `
        <ul class="submenu">
            <li><a href="/relatorio-vendas">Vendas</a></li>
            <li><a href="/relatorio-producao">Produção</a></li>
            <li><a href="/relatorio-entregas">Entregas</a></li>
            <li><a href="/relatorio-compras">Compras</a></li>
        </ul>`;
    parentItem.insertAdjacentHTML('beforeend', submenuHTML);
}

// Adiciona o evento de clique a todos os itens da lista
list.forEach((item) => item.addEventListener('click', activeLink));

// Detecta a URL atual e ajusta o menu ativo ao carregar a página
document.addEventListener("DOMContentLoaded", () => {
    const currentPath = window.location.pathname.toLowerCase();

    list.forEach((item) => {
        const link = item.querySelector('a');
        if (link && link.getAttribute('href').toLowerCase() === currentPath) {
            item.classList.add('active');
            // Adiciona o submenu se o item "Relatórios" estiver ativo ao carregar a página
            if (item.classList.contains('relatorios')) {
                createSubmenu(item);
                navegation.classList.add('expanded');
            }
        }
    });
});

// Função para mostrar/ocultar o submenu ao clicar no item "Relatórios"
document.querySelectorAll('.relatorios-toggle').forEach(item => {
    item.addEventListener('click', (e) => {
        e.preventDefault(); // Evita redirecionamento ao clicar
        const parent = item.closest('.relatorios');
        if (parent) {
            const submenu = parent.querySelector('.submenu');
            if (submenu) {
                submenu.remove();
                navegation.classList.remove('expanded');
            } else {
                createSubmenu(parent);
                navegation.classList.add('expanded');
            }
        }
    });
});

// Fecha a barra lateral ao clicar fora dela
document.addEventListener('click', (e) => {
    // Verifica se o clique foi fora da navegação expandida e fora do item "Relatórios"
    if (!navegation.contains(e.target) && !e.target.closest('.relatorios')) {
        navegation.classList.remove('expanded'); // Remove a classe de expansão

        // Verifica apenas o item "Relatórios" e mantém ele ativo, se já estava ativo
        list.forEach((item) => {
            if (item.classList.contains('relatorios')) {
                const submenu = item.querySelector('.submenu');
                if (submenu) {
                    submenu.remove(); // Remove o submenu
                }
                // Se o item "Relatórios" já estava ativo antes, mantém ele ativo
                if (item.classList.contains('active')) {
                    item.classList.add('active');
                }
            }
        });
    }
});
