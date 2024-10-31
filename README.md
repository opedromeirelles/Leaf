# Leaf

Leaf � um sistema de gest�o distribu�do desenvolvido com foco em produtividade e efici�ncia, projetado para operar em diferentes dispositivos (desktop, web e mobile), cada um com responsabilidades espec�ficas. O sistema � estruturado em uma arquitetura MVC em ASP.NET, com C# e SQL Server, adotando conceitos modernos de inje��o de depend�ncias, responsividade, separa��o de responsabilidades e boas pr�ticas de desenvolvimento. Este reposit�rio cont�m a aplica��o **web**, respons�vel pela gera��o de relat�rios e o gerenciamento de compras.

## �ndice

- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Funcionalidades](#funcionalidades)
- [Arquitetura do Projeto](#arquitetura-do-projeto)
- [Instala��o](#instala��o)
- [Uso](#uso)
- [Contribui��o](#contribui��o)
- [Licen�a](#licen�a)

## Tecnologias Utilizadas

- **ASP.NET** para desenvolvimento da aplica��o web
- **C#** como linguagem principal
- **SQL Server** para banco de dados
- **JavaScript, JSON, AJAX** para valida��es e integra��es no front-end
- **LINQ** para consultas dinamicas
- **Metodologia MVC** para organiza��o do projeto

## Funcionalidades

A aplica��o **Leaf** � distribu�da em tr�s dispositivos com responsabilidades distintas:

- **Desktop**: Gest�o de produ��o e vendas com emiss�o de lotes.
- **Web** (esta aplica��o): Respons�vel por relat�rios e gerenciamento de compras.
- **Mobile**: Parte da aplica��o que ser� integrada para acesso remoto ao pedidos, destinados as entregas do mesmo (em desenvolvimento).

### Funcionalidades da aplica��o web

- **Relat�rios de Vendas e Compras**: Gera��o e visualiza��o de relat�rios detalhados.
- **Gerenciamento de Compras**: Controle de pedidos, recebimento e atualiza��o de status.
- **Autentica��o e Autoriza��o**: Controle de login e acesso de usu�rios.
- **Valida��es de Dados**: Valida��es de entrada com JSON/AJAX integrado.
- **Consultas Din�micas e LINQ**: Utiliza��o de consultas eficientes ao banco de dados.
- **M�todos Ass�ncronos** para otimiza��o de consultas e respostas.

## Arquitetura do Projeto

A aplica��o foi estruturada em camadas bem definidas:

- **View**: Interface do usu�rio e componentes de apresenta��o.
- **Controller**: L�gica de controle das requisi��es e navega��o entre p�ginas.
- **Services**: Implementa��o da l�gica de neg�cio e integra��es entre camadas.
- **Repository**: Interface com o banco de dados usando consultas LINQ e m�todos din�micos.

Al�m disso, o projeto utiliza **inje��o de depend�ncia** para gerenciar as depend�ncias entre camadas, **tratamento de erros** com logs e mensagens amig�veis ao usu�rio, **valida��o de dados** tanto no lado do cliente quanto no servidor, e **controle de vers�o** para rastreamento de atualiza��es e corre��es.


## Contribui��o

Contribui��es s�o bem-vindas! Sinta-se � vontade para abrir issues, enviar pull requests ou sugerir melhorias. Por favor, siga as boas pr�ticas de desenvolvimento e inclua descri��es detalhadas ao enviar suas contribui��es.


## Considera��es

Leaf � um sistema de gest�o distribu�do desenvolvido como um projeto acad�mico, com o objetivo de aplicar e consolidar conhecimentos em desenvolvimento web e de software. Este projeto busca simular um ambiente real de trabalho, permitindo a pr�tica de conceitos avan�ados como arquitetura em camadas, inje��o de depend�ncias, controle de versionamento, e metodologias de desenvolvimento em ASP.NET com C#.

