using Microsoft.Data.Sqlite;
using System;
using System.Text;

class Program
{
    private string ConnectionString = "Data Source=storage.sqlite";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Program storageTable = new Program();
        storageTable.CreateSkladTable();

        using (SqliteConnection connection = new SqliteConnection(storageTable.ConnectionString))
        {
            try
            {
                connection.Open();
                storageTable.DisplayAllProducts(connection);
                storageTable.DisplayAllProductTypes(connection);
                storageTable.DisplayAllSuppliers(connection);
                storageTable.DisplayProductWithMaxQuantity(connection);
                storageTable.DisplayProductWithMinQuantity(connection);
                storageTable.DisplayProductWithMinCost(connection);
                storageTable.DisplayProductWithMaxCost(connection);
                storageTable.DisplayProductsByCategory(connection, "Клавіатура");
                storageTable.DisplayProductsBySupplier(connection, "Lenovo");
                storageTable.DisplayProductWithLongestStorage(connection);
                storageTable.DisplayAverageQuantityByProductType(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }

    public void CreateSkladTable()
    {
        string createTypesTable = "CREATE TABLE IF NOT EXISTS Types ( TypeId INTEGER UNIQUE PRIMARY KEY, TypeName NVARCHAR(255) NOT NULL);";

        string createSuppliersTable = "CREATE TABLE IF NOT EXISTS Suppliers ( SupplierId INTEGER UNIQUE PRIMARY KEY, SupplierName NVARCHAR(255) NOT NULL);";

        string createGoodsTable = @"CREATE TABLE IF NOT EXISTS Goods 
            (
                ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name NVARCHAR(255) UNIQUE NOT NULL,
                TypeId INTEGER NOT NULL,
                SupplierId INTEGER NOT NULL,
                Quantity INTEGER NOT NULL,
                CostPrice DECIMAL(18,2) NOT NULL,
                SupplyDate DATETIME NOT NULL,
                FOREIGN KEY(TypeId) REFERENCES Types(TypeId),
                FOREIGN KEY(SupplierId) REFERENCES Suppliers(SupplierId)
            );
        ";
        string insertTypes = "INSERT OR IGNORE INTO Types (TypeId, TypeName) VALUES (1, 'Монітор'), (2, 'Мишка'), (3, 'Клавіатура');";

        string insertSuppliers = "INSERT OR IGNORE INTO Suppliers (SupplierId, SupplierName) VALUES (1, 'Sumsung'), (2, 'Lenovo'), (3, 'Logitech');";

        string insertGoods = @"INSERT OR IGNORE INTO Goods 
            (Name, TypeId, SupplierId, Quantity, CostPrice, SupplyDate) VALUES 
            ('Монітор SAMSUNG LS43CG700NIXUA', 1, 1, 1, 29999, '2023-09-03'),
            ('Монітор LENOVO G27qc-30', 1, 2, 4, 9339, '2023-10-25'),
            ('Монітор SAMSUNG LS32BM500EIXUA', 1, 1, 3, 11299, '2022-08-27'),
            ('Мишка LOGITECH MX Master 3 Advanced', 2, 3, 2, 4899, '2023-11-01'),
            ('Мишка LENOVO Legion M500 RGB', 2, 2, 5, 2599, '2023-09-19'),
            ('Мишка LOGITECH G102 Lightsync', 2, 3, 1, 1099, '2023-04-29'),
            ('Клавіатура LOGITECH MX Keys Advanced', 3, 3, 2, 3999, '2023-04-02'),
            ('Клавіатура LENOVO Lenovo Legion K500', 3, 2, 3, 4999, '2023-07-25'),
            ('Клавіатура SAMSUNG Trio 500', 3, 1, 5, 1959, '2023-08-29');
        ";

        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            connection.Open();

            using (SqliteCommand command = new SqliteCommand(createTypesTable, connection))
            {
                command.ExecuteNonQuery();
            }

            using (SqliteCommand command = new SqliteCommand(createSuppliersTable, connection))
            {
                command.ExecuteNonQuery();
            }

            using (SqliteCommand command = new SqliteCommand(createGoodsTable, connection))
            {
                command.ExecuteNonQuery();
            }

            using (SqliteCommand command = new SqliteCommand(insertTypes, connection))
            {
                command.ExecuteNonQuery();
            }

            using (SqliteCommand command = new SqliteCommand(insertSuppliers, connection))
            {
                command.ExecuteNonQuery();
            }

            using (SqliteCommand command = new SqliteCommand(insertGoods, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void DisplayAllProducts(SqliteConnection connection)
    {
        string query = "SELECT * FROM Goods";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("\nВся інформація про товар:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]}, {reader["TypeId"]}, {reader["SupplierId"]}, {reader["Quantity"]}, {reader["CostPrice"]}, {reader["SupplyDate"]}");
            }
        }
    }

    public void DisplayAllProductTypes(SqliteConnection connection)
    {
        string query = "SELECT DISTINCT TypeName FROM Types";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("\nУсі типи товарів:");
            while (reader.Read())
            {
                Console.WriteLine(reader["TypeName"]);
            }
        }
    }

    public void DisplayAllSuppliers(SqliteConnection connection)
    {
        string query = "SELECT DISTINCT SupplierName FROM Suppliers";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("\nУсі постачальники:");
            while (reader.Read())
            {
                Console.WriteLine(reader["SupplierName"]);
            }
        }
    }

    public void DisplayProductWithMaxQuantity(SqliteConnection connection)
    {
        string query = "SELECT * FROM Goods ORDER BY Quantity DESC LIMIT 1";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("\nТовар з максимальною кількістю:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]}, {reader["Quantity"]}");
            }
        }
    }

    public void DisplayProductWithMinQuantity(SqliteConnection connection)
    {
        string query = "SELECT * FROM Goods ORDER BY Quantity ASC LIMIT 1";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("\nТовар з мінімальною кількістю:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]}, {reader["Quantity"]}");
            }
        }
    }

    public void DisplayProductWithMinCost(SqliteConnection connection)
    {
        string query = "SELECT * FROM Goods ORDER BY CostPrice ASC LIMIT 1";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("\nТовар з мінімальною собівартістю:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]}, {reader["CostPrice"]}");
            }
        }
    }

    public void DisplayProductWithMaxCost(SqliteConnection connection)
    {
        string query = "SELECT * FROM Goods ORDER BY CostPrice DESC LIMIT 1";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("\nТовар з максимальною собівартістю:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]}, {reader["CostPrice"]}");
            }
        }
    }

    public void DisplayProductsByCategory(SqliteConnection connection, string category)
    {
        string query = $"SELECT * FROM Goods INNER JOIN Types ON Goods.TypeId = Types.TypeId WHERE TypeName = '{category}'";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine($"\nТовари категорії '{category}':");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]}, {reader["TypeName"]}");
            }
        }
    }

    public void DisplayProductsBySupplier(SqliteConnection connection, string supplier)
    {
        string query = $"SELECT * FROM Goods INNER JOIN Suppliers ON Goods.SupplierId = Suppliers.SupplierId WHERE SupplierName = '{supplier}'";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine($"\nТовари постачальника '{supplier}':");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]}, {reader["SupplierName"]}");
            }
        }
    }

    public void DisplayProductWithLongestStorage(SqliteConnection connection)
    {
        string query = "SELECT * FROM Goods ORDER BY SupplyDate ASC LIMIT 1";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("\nТовар, який знаходиться на складі найдовше:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]}, {reader["SupplyDate"]}");
            }
        }
    }

    public void DisplayAverageQuantityByProductType(SqliteConnection connection)
    {
        string query = "SELECT TypeName, AVG(Quantity) AS AverageQuantity FROM Goods INNER JOIN Types ON Goods.TypeId = Types.TypeId GROUP BY TypeName";
        using (SqliteCommand command = new SqliteCommand(query, connection))
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("\nСередня кількість товарів за кожним типом:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["TypeName"]}, {reader["AverageQuantity"]}");
            }
        }
    }
}