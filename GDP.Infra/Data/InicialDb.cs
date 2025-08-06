using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GDP.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(IConfiguration configuration, ILogger<DatabaseInitializer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Starting database initialization...");
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var builder = new SqlConnectionStringBuilder(connectionString);
        var dbName = builder.InitialCatalog;

        builder.InitialCatalog = "master";
        using (var masterConnection = new SqlConnection(builder.ConnectionString))
        {
            await masterConnection.OpenAsync();
            var records = await masterConnection.QueryAsync("SELECT * FROM sys.databases WHERE name = @name", new { name = dbName });
            if (!records.Any())
            {
                _logger.LogInformation("Database '{DbName}' not found. Creating...", dbName);
                await masterConnection.ExecuteAsync($"CREATE DATABASE {dbName}");
            }
        }

        try
        {
            var script = @"
                IF OBJECT_ID('dbo.Customers', 'U') IS NULL
                    EXEC('
                        CREATE TABLE Customers (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Name NVARCHAR(200) NOT NULL,
                            Email NVARCHAR(150) NOT NULL UNIQUE,
                            Phone NVARCHAR(20) NULL,
                            RegistrationDate DATETIME2 NOT NULL DEFAULT GETDATE()
                        );
                    ');

                IF OBJECT_ID('dbo.Products', 'U') IS NULL
                    EXEC('
                        CREATE TABLE Products (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Name NVARCHAR(200) NOT NULL,
                            Description NVARCHAR(500) NULL,
                            Price DECIMAL(18, 2) NOT NULL,
                            Stock INT NOT NULL,
                            ReservedStock INT NOT NULL DEFAULT 0
                        );
                    ');

                IF OBJECT_ID('dbo.Orders', 'U') IS NULL
                    EXEC('
                        CREATE TABLE Orders (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            CustomerId INT NOT NULL,
                            OrderDate DATETIME2 NOT NULL DEFAULT GETDATE(),
                            TotalAmount DECIMAL(18, 2) NOT NULL,
                            Status NVARCHAR(50) NOT NULL,
                            CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
                        );
                    ');

                IF OBJECT_ID('dbo.OrderItems', 'U') IS NULL
                    EXEC('
                        CREATE TABLE OrderItems (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            OrderId INT NOT NULL,
                            ProductId INT NOT NULL,
                            Quantity INT NOT NULL,
                            UnitPrice DECIMAL(18, 2) NOT NULL,
                            CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id),
                            CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
                        );
                    ');

                IF NOT EXISTS (SELECT 1 FROM dbo.Customers)
                BEGIN
                    INSERT INTO Customers (Name, Email, Phone) VALUES
                    ('Ana Julia', 'anajuria@emailficticio.com', '11 98877-6655'),
                    ('Carlos Souza', 'carlos.souza88@emailficticio.com.br', '21 91122-3344'),
                    ('Beatriz Lima', 'bia.lima@emailficticio.com', NULL);

                    INSERT INTO Products (Name, Description, Price, Stock, ReservedStock) VALUES
                    ('Mouse Gamer RGB', 'Mouse com 6 botões', 249.90, 75, 0),
                    ('Teclado Semi-Mecânico ABNT2', 'Bom para jogos', 320.00, 40, 0),
                    ('Monitor Curvo 24\""', 'Monitor Full HD', 950.50, 25, 0),
                    ('Headset com Microfone', 'Fone de ouvido', 180.00, 120, 0);
                END;
            ";

            using (var dbConnection = new SqlConnection(connectionString))
            {
                _logger.LogInformation("Executing embedded setup script...");
                await dbConnection.ExecuteAsync(script);
                _logger.LogInformation("Database setup script executed successfully.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute database setup script.");
            throw;
        }
    }
}