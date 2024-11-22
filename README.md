# Leaf - Aplicação Web

**Leaf** é um sistema de gestão distribuído desenvolvido com foco em produtividade e eficiência, projetado para operar em diferentes dispositivos (desktop, web e mobile), cada um com responsabilidades específicas. O sistema é estruturado em uma arquitetura **MVC** com **ASP.NET**, **C#** e **SQL Server**, adotando conceitos modernos como injeção de dependências, responsividade e boas práticas de desenvolvimento. Este repositório contém a aplicação **web**, cuja responsabilidade principal é a emissão de relatórios e o gerenciamento de compras.

## Índice

- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Funcionalidades](#funcionalidades)
- [Arquitetura do Projeto](#arquitetura-do-projeto)
- [Considerações](#considerações)

## Tecnologias Utilizadas

- **ASP.NET**: Framework utilizado para o desenvolvimento da aplicação web.
- **C#**: Linguagem principal para a implementação da lógica de negócio.
- **SQL Server**: Banco de dados relacional para armazenamento e recuperação de dados.
- **JavaScript, JSON, AJAX**: Para validações no lado do cliente e integração com a interface de forma dinâmica.
- **LINQ**: Utilizado para consultas dinâmicas ao banco de dados.
- **Metodologia MVC**: Estrutura para organização do projeto, promovendo uma separação clara de responsabilidades.

## Funcionalidades

A aplicação **Leaf Web** é parte do sistema distribuído e desempenha um papel essencial no gerenciamento e controle das operações administrativas. Suas principais funcionalidades incluem:

- **Geração de Relatórios**: Emissão e visualização de relatórios detalhados de vendas e compras, auxiliando na análise e tomada de decisões.
- **Gerenciamento de Compras de Insumos**: Controle de pedidos de insumos, gerenciamento de fornecedores e atualização do status de compras.
- **Autenticação e Autorização**: Sistema de login seguro e gerenciamento de permissões de acesso dos usuários.
- **Validações de Dados**: Validações no lado do cliente com **AJAX** e **JSON** para uma experiência de usuário responsiva.
- **Consultas Otimizadas**: Utilização de **LINQ** para consultas eficientes, combinadas com métodos assíncronos para melhor desempenho.

## Arquitetura do Projeto

A aplicação **web** segue uma arquitetura **MVC** bem definida, garantindo modularidade e facilidade de manutenção:

- **View**: Componentes de interface do usuário, desenvolvidos para serem responsivos e intuitivos.
- **Controller**: Controla o fluxo de dados entre a View e o Model, processando solicitações e coordenando a lógica de negócios.
- **Model**: Representa as entidades do domínio e é responsável pela interação com o banco de dados.
- **Services**: Camada intermediária que implementa a lógica de negócio e promove a integração entre Controller e Model.
- **Repository**: Abstração do acesso ao banco de dados, utilizando **LINQ** e consultas personalizadas.

### Recursos Adicionais

- **Injeção de Dependências**: Implementada para facilitar a testabilidade e o desacoplamento entre as camadas.
- **Tratamento de Erros**: Inclusão de logs e mensagens de erro amigáveis para melhorar a experiência do usuário e a manutenção do sistema.
- **Controle de Versão**: Para monitoramento de atualizações e correções, garantindo um processo de desenvolvimento colaborativo e eficiente.

## Considerações

O projeto **Leaf** foi desenvolvido como um projeto acadêmico com o objetivo de aplicar e consolidar conhecimentos em desenvolvimento web e arquitetura de software. Este projeto simula um ambiente real de trabalho, permitindo a prática de conceitos avançados, como a arquitetura em camadas, injeção de dependências, e metodologias modernas de desenvolvimento em **ASP.NET** com **C#**.

---

Veja outras partes que complementam esse projeto:

- [Leaf Desktop - Gerenciamento de vendas e produção de produtos](https://github.com/opedromeirelles/Leaf-Mobile)
- [Leaf Mobile - Gerenciamento de entregas dos produtos vendidos](https://github.com/opedromeirelles/Leaf-WinForms)
