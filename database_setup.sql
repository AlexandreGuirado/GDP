IF OBJECT_ID('dbo.ItensPedido', 'U') IS NOT NULL DROP TABLE dbo.ItensPedido;
IF OBJECT_ID('dbo.Pedidos', 'U') IS NOT NULL DROP TABLE dbo.Pedidos;
IF OBJECT_ID('dbo.Produtos', 'U') IS NOT NULL DROP TABLE dbo.Produtos;
IF OBJECT_ID('dbo.Clientes', 'U') IS NOT NULL DROP TABLE dbo.Clientes;
GO

CREATE TABLE Clientes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(200) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Telefone NVARCHAR(20) NULL,
    DataCadastro DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Produtos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nome NVARCHAR(200) NOT NULL,
    Descricao NVARCHAR(500) NULL,
    Preco DECIMAL(18, 2) NOT NULL,
    Estoque INT NOT NULL CHECK (Estoque >= 0)
);
GO

CREATE TABLE Pedidos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ClienteId INT NOT NULL,
    DataPedido DATETIME2 NOT NULL DEFAULT GETDATE(),
    ValorTotal DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_Pedidos_Clientes FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);
GO

CREATE TABLE ItensPedido (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PedidoId INT NOT NULL,
    ProdutoId INT NOT NULL,
    Quantidade INT NOT NULL,
    PrecoUnitario DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_ItensPedido_Pedidos FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id),
    CONSTRAINT FK_ItensPedido_Produtos FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id)
);
GO

PRINT 'Tabelas criadas com sucesso.';
GO

INSERT INTO Clientes (Nome, Email, Telefone) VALUES
('João da Silva', 'joao.silva@email.com', '1198765-4321'),
('Maria Oliveira', 'maria.oliveira@email.com', '2191234-5678'),
('Carlos Pereira', 'carlos.pereira@email.com', '3195555-4444');

INSERT INTO Produtos (Nome, Descricao, Preco, Estoque) VALUES
('Notebook Pro 15"', 'Um notebook.', 7499.99, 50),
('Mouse Sem Fio', 'Mouse.', 149.90, 200),
('Teclado Mecânico', 'Teclado.', 420.00, 75),
('Monitor 4K 27"', 'Monitor.', 1850.50, 30);

PRINT 'Dados iniciais inseridos com sucesso.';
GO