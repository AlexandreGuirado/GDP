**Gerenciamento de Pedidos**

Este projeto é uma aplicação web completa e robusta desenvolvida para simplificar o gerenciamento de clientes, produtos e pedidos em uma pequena loja. Construído do zero, ele demonstra um profundo conhecimento em desenvolvimento full-stack, seguindo as melhores práticas e especificações técnicas.

**Tecnologias Utilizadas**
Backend:
Linguagem: C#
Framework: ASP.NET Core MVC (com .NET 6)
Acesso a Dados: Dapper.NET (com padrão Repository)
Banco de Dados: SQL Server
Frontend:
Estrutura: HTML5
Estilização: CSS3, Bootstrap
Interatividade: jQuery, AJAX
Versionamento: Git (GitHub/GitLab)

**Arquitetura e Estilo de Projeto**
O projeto foi estruturado seguindo uma arquitetura em camadas desacoplada, promovendo organização, manutenibilidade e escalabilidade. As camadas incluem:
Apresentação: Responsável pela interface do usuário, utilizando ASP.NET Core MVC, HTML5, CSS3 e Bootstrap. A interatividade é aprimorada com jQuery e AJAX.
Domínio/Negócio: Contém a lógica de negócio central da aplicação, com classes e regras específicas para clientes, produtos e pedidos.
Infraestrutura/Dados: Gerencia o acesso ao banco de dados, abstraindo as operações com o Dapper.NET e implementando o padrão Repository.
Os princípios SOLID foram aplicados sempre que possível para garantir um código mais flexível, legível e testável.

**Funcionalidades Implementadas**
1. Gerenciamento de Clientes
CRUD: Criação, leitura, atualização e deleção de clientes.
Campos: ID, Nome, Email, Telefone, Data de Cadastro.
Listagem e Filtro: Tela para listar todos os clientes com busca/filtro por Nome ou Email.
2. Gerenciamento de Produtos
CRUD: Criação, leitura, atualização e deleção de produtos.
Campos: ID, Nome, Descrição, Preço, Quantidade em Estoque.
Listagem e Filtro: Tela para listar todos os produtos com busca/filtro por Nome.
3. Registro de Pedidos
Criar Novo Pedido:
Seleção de um cliente existente.
Adição de um ou mais produtos ao pedido, com especificação de quantidade.
Validação automática de estoque suficiente para os produtos adicionados.
Cálculo automático do valor total do pedido.
Salvamento seguro do pedido com seus respectivos itens.
Campos do Pedido: ID, ID do Cliente, Data do Pedido, Valor Total, Status (Ex: 'Novo', 'Processando', 'Finalizado').
Campos do Item do Pedido: ID, ID do Pedido, ID do Produto, Quantidade, Preço Unitário (no momento da compra).
Listagem de Pedidos:
Exibição de uma lista completa de pedidos, mostrando Cliente, Data, Valor Total e Status.
Funcionalidade de filtro por Cliente ou Status.
Visualizar Detalhes do Pedido: Ao clicar em um pedido, são exibidos seus detalhes completos, incluindo todos os produtos e itens associados.
Atualizar Status do Pedido: Permite a alteração do status de um pedido (ex: de 'Novo' para 'Processando').
4. Interface de Usuário
Interface limpa, intuitiva e responsiva, construída com Bootstrap.
Interações dinâmicas aprimoradas com jQuery e AJAX, proporcionando uma experiência de usuário fluida (ex: adição de produtos ao pedido sem recarregar a página).

**Qualidade de Software e Testes Unitários**
O projeto foi desenvolvido com foco na qualidade e manutenibilidade. Foram implementados testes unitários básicos para a camada de negócio (OrderService), garantindo a confiabilidade das funcionalidades críticas, como a validação de estoque.
Para executar os testes:
Abra a solução no Visual Studio.
Navegue até o "Test Explorer" (ou o explorador de testes da sua IDE).
Clique em "Run All Tests" para executar todos os testes unitários.

**Script do Banco de Dados**
O projeto inclui o script SQL para a criação das tabelas necessárias no SQL Server, facilitando a configuração inicial do ambiente.

**Colaboração e Versionamento**
Todo o código-fonte está hospedado em um repositório Git, seguindo as melhores práticas de versionamento para garantir o histórico e a colaboração eficiente do projeto.

Abaixo segue algumas imagens do projeto:
<img width="1917" height="904" alt="image" src="https://github.com/user-attachments/assets/1bc1f9bd-c97d-4fe2-abd7-849ea963673e" />
<img width="1914" height="922" alt="image" src="https://github.com/user-attachments/assets/7fa942b9-a6e7-4733-81ed-01b27a0449a7" />
<img width="1920" height="917" alt="image" src="https://github.com/user-attachments/assets/3f085b49-fd74-4bd8-ab4e-645182009855" />
<img width="1918" height="919" alt="image" src="https://github.com/user-attachments/assets/1ea6351d-8c76-4528-aee8-379176725a18" />
<img width="1916" height="918" alt="image" src="https://github.com/user-attachments/assets/dc1475f6-7d30-4e62-b019-9b02429ad91f" />
<img width="1920" height="921" alt="image" src="https://github.com/user-attachments/assets/2a2e0f91-afc2-4040-87a1-e1f474812c36" />
<img width="1920" height="919" alt="image" src="https://github.com/user-attachments/assets/e12371e5-a26c-4b9b-a587-67d7c912e78c" />
<img width="1920" height="921" alt="image" src="https://github.com/user-attachments/assets/fc405943-941b-4d1a-a7a2-58edbaf37b29" />
<img width="1918" height="915" alt="image" src="https://github.com/user-attachments/assets/528233bc-3530-4a76-a14f-8306a7b26e1e" />
