# Leaf

Leaf é um sistema de gestão distribuído desenvolvido com foco em produtividade e eficiência, projetado para operar em diferentes dispositivos (desktop, web e mobile), cada um com responsabilidades específicas. O sistema é estruturado em uma arquitetura MVC em ASP.NET, com C# e SQL Server, adotando conceitos modernos de injeção de dependências, responsividade, separação de responsabilidades e boas práticas de desenvolvimento. Este repositório contém a aplicação **web**, responsável pela geração de relatórios e o gerenciamento de compras.

## Índice

- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Funcionalidades](#funcionalidades)
- [Arquitetura do Projeto](#arquitetura-do-projeto)
- [Instalação](#instalação)
- [Uso](#uso)
- [Contribuição](#contribuição)
- [Licença](#licença)

## Tecnologias Utilizadas

- **ASP.NET** para desenvolvimento da aplicação web
- **C#** como linguagem principal
- **SQL Server** para banco de dados
- **JavaScript, JSON, AJAX** para validações e integrações no front-end
- **LINQ** para consultas dinamicas
- **Metodologia MVC** para organização do projeto

## Funcionalidades

A aplicação **Leaf** é distribuída em três dispositivos com responsabilidades distintas:

- **Desktop**: Gestão de produção e vendas com emissão de lotes.
- **Web** (esta aplicação): Responsável por relatórios e gerenciamento de compras.
- **Mobile**: Parte da aplicação que será integrada para acesso remoto ao pedidos, destinados as entregas do mesmo (em desenvolvimento).

### Funcionalidades da aplicação web

- **Relatórios de Vendas e Compras**: Geração e visualização de relatórios detalhados.
- **Gerenciamento de Compras**: Controle de pedidos, recebimento e atualização de status.
- **Autenticação e Autorização**: Controle de login e acesso de usuários.
- **Validações de Dados**: Validações de entrada com JSON/AJAX integrado.
- **Consultas Dinâmicas e LINQ**: Utilização de consultas eficientes ao banco de dados.
- **Métodos Assíncronos** para otimização de consultas e respostas.

## Arquitetura do Projeto

A aplicação foi estruturada em camadas bem definidas:

- **View**: Interface do usuário e componentes de apresentação.
- **Controller**: Lógica de controle das requisições e navegação entre páginas.
- **Services**: Implementação da lógica de negócio e integrações entre camadas.
- **Repository**: Interface com o banco de dados usando consultas LINQ e métodos dinâmicos.

Além disso, o projeto utiliza **injeção de dependência** para gerenciar as dependências entre camadas, **tratamento de erros** com logs e mensagens amigáveis ao usuário, **validação de dados** tanto no lado do cliente quanto no servidor, e **controle de versão** para rastreamento de atualizações e correções.


## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues, enviar pull requests ou sugerir melhorias. Por favor, siga as boas práticas de desenvolvimento e inclua descrições detalhadas ao enviar suas contribuições.


## Considerações

Leaf é um sistema de gestão distribuído desenvolvido como um projeto acadêmico, com o objetivo de aplicar e consolidar conhecimentos em desenvolvimento web e de software. Este projeto busca simular um ambiente real de trabalho, permitindo a prática de conceitos avançados como arquitetura em camadas, injeção de dependências, controle de versionamento, e metodologias de desenvolvimento em ASP.NET com C#.

