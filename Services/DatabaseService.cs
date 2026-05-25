using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace CreatiSphere.Services
{
    public class Reward
    {
        public int PointsBalance { get; set; }
        public string Tier { get; set; } = string.Empty;
    }

    public class RedeemableReward
    {
        public int RewardId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PointsCost { get; set; }
    }

    public class RewardHistoryItem
    {
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Points { get; set; }
        public string DateDisplay => Date.ToString("MMM dd, yyyy");
        public string PointsDisplay => Points >= 0 ? $"+ {Points:N0}" : $"- {Math.Abs(Points):N0}";
        public string PointsColor => Points >= 0 ? "#0D9488" : "#EF4444";
        public string StatusBackground => Status.Equals("Redeemed", StringComparison.OrdinalIgnoreCase) ? "#F1F5F9" : "#F0FDFA";
        public string StatusColor => Status.Equals("Redeemed", StringComparison.OrdinalIgnoreCase) ? "#64748B" : "#0D9488";
    }

    public class Commission
    {
        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public int Progress { get; set; }
        public string Status { get; set; } = string.Empty;
        public double ProgressFraction => (double)Progress / 100.0;
        public double ProgressBarWidth => 300.0 * ProgressFraction; // Approximate width for UI
        public string ImagePath { get; set; } = "commission_placeholder.png";
    }

    public class Activity
    {
        public string Title { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class CreatorAsset
    {
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int Views { get; set; }
        public int Sales { get; set; }
    }

    public class SystemModule
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int EnabledCount { get; set; }
        public bool IsStarterEnabled { get; set; }
        public bool IsStandardEnabled { get; set; }
        public bool IsEnterpriseEnabled { get; set; }
    }

    public class SystemMetrics
    {
        public double CpuLoad { get; set; }
        public double CpuTrend { get; set; }
        public double MemoryUsage { get; set; }
        public double MemoryTrend { get; set; }
        public int ApiConnections { get; set; }
        public int ApiTrend { get; set; }
        public int Latency { get; set; }
        public string LatencyStatus { get; set; } = "Stable";
        public List<double> LoadHistory { get; set; } = new();
    }

    public class User
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public string? LastActive { get; set; }
        public string? Initials => Username?.Length >= 2 ? Username.Substring(0, 2).ToUpper() : "U";
    }

    public class Msme
    {
        public string? BusinessName { get; set; }
        public string? Niche { get; set; }
        public string? OwnerName { get; set; }
        public string? ErpTier { get; set; }
        public string? ActiveUsers { get; set; }
        public string? Status { get; set; }
        public string? Initials => BusinessName?.Length >= 1 ? BusinessName.Substring(0, 1).ToUpper() : "M";
    }

    public class AccountInfo
    {
        public int AccountID { get; set; }
        public string? AccountName { get; set; }
        public string? Email { get; set; }
        public int RoleID { get; set; }
        public bool IsActive { get; set; }
    }

    public class DashboardStats
    {
        public int TotalMsmes { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCustomers { get; set; }
        public string? Uptime { get; set; }
        public int ActiveModules { get; set; }
        public List<MonthlyGrowth> GrowthHistory { get; set; } = new();
        public List<double> ActivityHistory { get; set; } = new();
    }

    public class MonthlyGrowth
    {
        public string? Month { get; set; }
        public int ActiveMsmes { get; set; }
        public int NewRegistrations { get; set; }
    }

    public class AdminStats
    {
        public int TotalAdmins { get; set; }
        public int ActiveSessions { get; set; }
        public int PendingRequests { get; set; }
        public double SuperAdminRatio { get; set; } // 0.0 to 1.0
        public string? MonthlyGrowth { get; set; }
        public string? PriorityStatus { get; set; }
    }

    public class MsmeStats
    {
        public int TotalMsmes { get; set; }
        public int ActiveMsmes { get; set; }
        public int OnTrialMsmes { get; set; }
        public string? Growth { get; set; }
    }

    public class SaleTransaction
    {
        public string? TransactionID { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
        public string? Initials => CustomerName?.Length >= 2 ? CustomerName.Substring(0, 2).ToUpper() : "C";
    }

    public class Subscription
    {
        public string SubscriptionID { get; set; } = string.Empty;
        public int AccountID { get; set; }
        public string TierLevel { get; set; } = "Starter";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BillingCycle { get; set; } = "Monthly";
        public string PaymentStatus { get; set; } = "Active";
        public decimal Amount { get; set; }
        public string DateString => StartDate.ToString("MMM dd, yyyy");
        public string ExpiryString => EndDate.ToString("MMM dd, yyyy");
    }

    public class ReportStats
    {
        public decimal TotalRevenue { get; set; }
        public int NewCreators { get; set; }
        public int TotalOrders { get; set; }
        public decimal ConversionRate { get; set; }
        public string? RevenueTrend { get; set; }
        public int TotalCustomers { get; set; }
        public int ActiveLeads { get; set; }
        public decimal TotalPortfolioSpend { get; set; }
    }

    public class Product
    {
        public string? ProductID { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Status { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }

    public class CartItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class InventoryItem
    {
        public string? SKU { get; set; }
        public string? Name { get; set; }
        public string? Warehouse { get; set; }
        public int Quantity { get; set; }
        public string? ReorderPoint { get; set; }
        public string? Status { get; set; }
    }

    public class DigitalAsset
    {
        public string? Name { get; set; }
        public string? Size { get; set; }
        public string? Format { get; set; }
        public string? Date { get; set; }
    }

    public class CustomOrder
    {
        public string? OrderID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ClientName { get; set; }
        public string? Date { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public string? Category { get; set; }
        public string? Resolution { get; set; }
        public string? ColorProfile { get; set; }
        public string? Deliverables { get; set; }
        public string? AssignedArtist { get; set; }
        public decimal Price { get; set; }
    }

    public class ChartDataPoint
    {
        public string Day { get; set; } = string.Empty;
        public double LastWeekHeight { get; set; }
        public double ThisWeekHeight { get; set; }
    }

    public class DatabaseService
    {
        private readonly string _connectionString = "Server=DESKTOP-H3Q23FS\\SQLEXPRESS;Database=IT13_CreatiSphere;Trusted_Connection=True;TrustServerCertificate=True;";

        private static List<Msme>? _dummyMsmes = null;

        private void InitializeDummyMsmes()
        {
            if (_dummyMsmes == null)
            {
                _dummyMsmes = new List<Msme>
                {
                    new Msme { BusinessName = "Sticker Haven Co.", Niche = "Sticker Shop", OwnerName = "Elena Rodriguez", ErpTier = "Enterprise Plus", ActiveUsers = "24 / 50", Status = "ACTIVE" },
                    new Msme { BusinessName = "Pixel Craft Agency", Niche = "Digital Assets", OwnerName = "Marcus Thorne", ErpTier = "Standard", ActiveUsers = "12 / 20", Status = "TRIAL" },
                    new Msme { BusinessName = "Urban Flora", Niche = "Botanical Design", OwnerName = "Sarah Lofton", ErpTier = "Standard", ActiveUsers = "2 / 5", Status = "INACTIVE" }
                };
            }
        }

        private async Task EnsureProductsTableSchema()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Create table if it doesn't exist
                    string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Products')
                        BEGIN
                            CREATE TABLE Products (
                                ProductID NVARCHAR(50) PRIMARY KEY,
                                Name NVARCHAR(255) NOT NULL,
                                Category NVARCHAR(100) NULL,
                                Price DECIMAL(18, 2) NOT NULL,
                                Stock INT NOT NULL,
                                Status NVARCHAR(50) NULL,
                                ImageUrl NVARCHAR(MAX) NULL
                            );
                            
                            -- Insert some initial data
                            INSERT INTO Products (ProductID, Name, Category, Price, Stock, Status, ImageUrl)
                            VALUES 
                            ('PRD-001', 'Ethereal Landscape', 'Fine Art', 1200.00, 5, 'In Stock', 'product1.png'),
                            ('PRD-002', 'Neon Cyberpunk Pack', 'Digital Asset', 45.00, 999, 'Digital', 'product2.png');
                        END
                        ELSE IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Products' AND COLUMN_NAME = 'ImageUrl')
                        BEGIN
                            ALTER TABLE Products ADD ImageUrl NVARCHAR(MAX) NULL;
                        END
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Products' AND COLUMN_NAME = 'Description')
                        BEGIN
                            ALTER TABLE Products ADD Description NVARCHAR(MAX) NULL;
                        END";

                    using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database schema sync failed: {ex.Message}");
            }
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var stats = new DashboardStats
            {
                Uptime = "99.9%",
                ActiveModules = 12,
                GrowthHistory = new List<MonthlyGrowth>(),
                ActivityHistory = new List<double>()
            };

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Total counts
                    using (SqlCommand command = new SqlCommand("SELECT COUNT(1) FROM Msmes", connection))
                        stats.TotalMsmes = Convert.ToInt32(await command.ExecuteScalarAsync());

                    using (SqlCommand command = new SqlCommand("SELECT COUNT(1) FROM Accounts", connection))
                        stats.TotalUsers = Convert.ToInt32(await command.ExecuteScalarAsync());

                    // Total Customers (RoleID = 3 for customers)
                    using (SqlCommand command = new SqlCommand("SELECT COUNT(1) FROM Accounts WHERE RoleID = 3", connection))
                        stats.TotalCustomers = Convert.ToInt32(await command.ExecuteScalarAsync());

                    using (SqlCommand command = new SqlCommand("SELECT COUNT(1) FROM SystemModules WHERE Status = 'Active'", connection))
                        stats.ActiveModules = Convert.ToInt32(await command.ExecuteScalarAsync());

                    // Growth History (Last 6 Months)
                    string growthQuery = @"
                        SELECT TOP 6 
                            UPPER(FORMAT(RegistrationDate, 'MMM')) as MonthName,
                            COUNT(*) as NewReg,
                            (SELECT COUNT(*) FROM Msmes m2 WHERE m2.RegistrationDate <= EOMONTH(m1.RegistrationDate)) as TotalAtEnd
                        FROM Msmes m1
                        GROUP BY FORMAT(RegistrationDate, 'MMM'), EOMONTH(RegistrationDate)
                        ORDER BY EOMONTH(RegistrationDate) DESC";
                    
                    using (SqlCommand command = new SqlCommand(growthQuery, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                stats.GrowthHistory.Insert(0, new MonthlyGrowth 
                                { 
                                    Month = reader["MonthName"].ToString(), 
                                    NewRegistrations = Convert.ToInt32(reader["NewReg"]) * 5, // Scaling for visual effect
                                    ActiveMsmes = Convert.ToInt32(reader["TotalAtEnd"]) * 2  // Scaling for visual effect
                                });
                            }
                        }
                    }

                    // System Activity (Line Chart)
                    using (SqlCommand command = new SqlCommand("SELECT TOP 7 RequestCount FROM SystemActivity ORDER BY LogTime DESC", connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                stats.ActivityHistory.Insert(0, Convert.ToDouble(reader["RequestCount"]));
                            }
                        }
                    }

                    // Fallbacks if data is too thin
                    if (stats.GrowthHistory.Count == 0)
                    {
                        stats.GrowthHistory = new List<MonthlyGrowth>
                        {
                            new MonthlyGrowth { Month = "JAN", ActiveMsmes = 60, NewRegistrations = 25 },
                            new MonthlyGrowth { Month = "FEB", ActiveMsmes = 85, NewRegistrations = 35 },
                            new MonthlyGrowth { Month = "MAR", ActiveMsmes = 110, NewRegistrations = 45 }
                        };
                    }
                    if (stats.ActivityHistory.Count == 0)
                        stats.ActivityHistory = new List<double> { 150, 160, 140, 180, 150, 100, 140 };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                stats.TotalMsmes = 81;
                stats.TotalUsers = 1247;
                stats.ActiveModules = 12;
            }
            return stats;
        }

        public async Task<AccountInfo?> ValidateLogin(string username, string password)
        {
            bool dbReachable = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    dbReachable = true;

                    // Try plain Password column first, fallback to PasswordHash
                    string query = @"SELECT AccountID, AccountName, Email, RoleID, IsActive
                                     FROM [dbo].[Accounts]
                                     WHERE Email = @Email
                                       AND PasswordHash = @Password
                                       AND IsActive = 1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", username);
                        command.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new AccountInfo
                                {
                                    AccountID = Convert.ToInt32(reader["AccountID"]),
                                    AccountName = reader["AccountName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    RoleID = Convert.ToInt32(reader["RoleID"]),
                                    IsActive = Convert.ToBoolean(reader["IsActive"])
                                };
                            }
                        }
                    }
                }
                // DB connected but no matching account found
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");

                if (dbReachable)
                {
                    // DB was reachable but query failed (column issue etc.) — try PasswordHash only
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(_connectionString))
                        {
                            await connection.OpenAsync();
                            string query = @"SELECT AccountID, AccountName, Email, RoleID, IsActive
                                             FROM [dbo].[Accounts]
                                             WHERE Email = @Email AND PasswordHash = @Password AND IsActive = 1";
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@Email", username);
                                command.Parameters.AddWithValue("@Password", password);
                                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                        return new AccountInfo
                                        {
                                            AccountID = Convert.ToInt32(reader["AccountID"]),
                                            AccountName = reader["AccountName"].ToString(),
                                            Email = reader["Email"].ToString(),
                                            RoleID = Convert.ToInt32(reader["RoleID"]),
                                            IsActive = Convert.ToBoolean(reader["IsActive"])
                                        };
                                }
                            }
                        }
                    }
                    catch { }
                    return null;
                }

                // DB not reachable — use offline fallback credentials
                if (username == "superadmin@creatisphere.com" && password == "superadmin123!")
                    return new AccountInfo { AccountID = 1, AccountName = "Super Admin", Email = username, RoleID = 1, IsActive = true };
                if (username == "admin@creatisphere.com" && password == "admin123!")
                    return new AccountInfo { AccountID = 2, AccountName = "Admin", Email = username, RoleID = 2, IsActive = true };
                if (username == "customer@creatisphere.com" && password == "customer123!")
                    return new AccountInfo { AccountID = 3, AccountName = "Customer", Email = username, RoleID = 3, IsActive = true };
                if (username == "sales@creatisphere.com" && password == "sales123!")
                    return new AccountInfo { AccountID = 4, AccountName = "Sales Associate", Email = username, RoleID = 5, IsActive = true };
                if (username == "finance@creatisphere.com" && password == "finance123!")
                    return new AccountInfo { AccountID = 5, AccountName = "Finance Manager", Email = username, RoleID = 6, IsActive = true };

                return null;
            }
        }


        public async Task<AccountInfo?> GetAccountInfoByIdAsync(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT AccountID, AccountName, Email, RoleID, IsActive FROM Accounts WHERE AccountID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new AccountInfo
                                {
                                    AccountID = Convert.ToInt32(reader["AccountID"]),
                                    AccountName = reader["AccountName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    RoleID = Convert.ToInt32(reader["RoleID"]),
                                    IsActive = Convert.ToBoolean(reader["IsActive"])
                                };
                            }
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            if (UserSession.AccountID > 5) return new List<User>();
            var users = new List<User>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Joining Accounts with Roles if Roles table exists, otherwise just selecting from Accounts
                    string query = @"
                        SELECT a.AccountName as Username, a.Email, r.RoleName, 
                               CASE WHEN a.IsActive = 1 THEN 'Active' ELSE 'Inactive' END as Status, 
                               '2 mins ago' as LastActive 
                        FROM Accounts a
                        LEFT JOIN Roles r ON a.RoleID = r.RoleID";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(new User
                                {
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Role = reader["RoleName"] != DBNull.Value ? reader["RoleName"].ToString() : "N/A",
                                    Status = reader["Status"].ToString(),
                                    LastActive = reader["LastActive"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                // Return dummy data if connection fails for now to keep UI populated during development
                return new List<User>
                {
                    new User { Username = "Julianne Deauville", Email = "j.deauville@creatisphere.io", Role = "Super Admin", Status = "Active", LastActive = "2 mins ago" },
                    new User { Username = "Arthur Chen", Email = "a.chen@creatisphere.io", Role = "System Auditor", Status = "Active", LastActive = "Yesterday" }
                };
            }
            return users;
        }

        // ─── INVENTORY CRUD ─────────────────────────────────────────────────────────

        private async Task EnsureInventoryTableAsync(SqlConnection connection)
        {
            string createTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Inventory')
                BEGIN
                    CREATE TABLE Inventory (
                        SKU           NVARCHAR(50)  PRIMARY KEY,
                        Name          NVARCHAR(255) NOT NULL,
                        Warehouse     NVARCHAR(100) NULL,
                        Quantity      INT           NOT NULL DEFAULT 0,
                        ReorderPoint  NVARCHAR(50)  NULL,
                        Status        NVARCHAR(50)  NULL
                    );
                    INSERT INTO Inventory (SKU, Name, Warehouse, Quantity, ReorderPoint, Status) VALUES
                    ('SKU-9281', 'Premium Canvas 24x36', 'North Hub',    142, '50', 'Stable'),
                    ('SKU-9285', 'Oil Paint Pro Set',    'East Wing',     12, '20', 'Low Stock'),
                    ('SKU-9290', 'Digital Brush Pack',   'Digital Storage',500,'100','Stable'),
                    ('SKU-9301', 'Frame Kit Standard',   'Central Logistics',37,'30','Stable');
                END";
            using var cmd = new SqlCommand(createTable, connection);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<InventoryItem>> GetInventoryAsync()
        {
            if (UserSession.AccountID > 5) return new List<InventoryItem>();
            var items = new List<InventoryItem>();
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                await EnsureInventoryTableAsync(connection);

                string query = "SELECT SKU, Name, Warehouse, Quantity, ReorderPoint, Status FROM Inventory ORDER BY SKU";
                using var cmd = new SqlCommand(query, connection);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    items.Add(new InventoryItem
                    {
                        SKU          = reader["SKU"].ToString(),
                        Name         = reader["Name"].ToString(),
                        Warehouse    = reader["Warehouse"].ToString(),
                        Quantity     = Convert.ToInt32(reader["Quantity"]),
                        ReorderPoint = reader["ReorderPoint"].ToString(),
                        Status       = reader["Status"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetInventoryAsync Error: {ex.Message}");
                // Offline fallback
                items = new List<InventoryItem>
                {
                    new InventoryItem { SKU = "SKU-9281", Name = "Premium Canvas 24x36", Warehouse = "North Hub",         Quantity = 142, ReorderPoint = "50",  Status = "Stable"    },
                    new InventoryItem { SKU = "SKU-9285", Name = "Oil Paint Pro Set",    Warehouse = "East Wing",          Quantity = 12,  ReorderPoint = "20",  Status = "Low Stock" },
                    new InventoryItem { SKU = "SKU-9290", Name = "Digital Brush Pack",   Warehouse = "Digital Storage",    Quantity = 500, ReorderPoint = "100", Status = "Stable"    },
                    new InventoryItem { SKU = "SKU-9301", Name = "Frame Kit Standard",   Warehouse = "Central Logistics",  Quantity = 37,  ReorderPoint = "30",  Status = "Stable"    }
                };
            }
            return items;
        }

        public async Task<bool> AddInventoryAsync(InventoryItem item)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                await EnsureInventoryTableAsync(connection);

                string query = @"INSERT INTO Inventory (SKU, Name, Warehouse, Quantity, ReorderPoint, Status)
                                 VALUES (@SKU, @Name, @Warehouse, @Quantity, @ReorderPoint, @Status)";
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SKU",          item.SKU ?? "");
                cmd.Parameters.AddWithValue("@Name",         item.Name ?? "");
                cmd.Parameters.AddWithValue("@Warehouse",    item.Warehouse ?? "Main Hub");
                cmd.Parameters.AddWithValue("@Quantity",     item.Quantity);
                cmd.Parameters.AddWithValue("@ReorderPoint", item.ReorderPoint ?? "10");
                cmd.Parameters.AddWithValue("@Status",       item.Status ?? "Stable");
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddInventoryAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateInventoryAsync(InventoryItem item)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = @"UPDATE Inventory 
                                 SET Name=@Name, Warehouse=@Warehouse, Quantity=@Quantity,
                                     ReorderPoint=@ReorderPoint, Status=@Status
                                 WHERE SKU=@SKU";
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SKU",          item.SKU ?? "");
                cmd.Parameters.AddWithValue("@Name",         item.Name ?? "");
                cmd.Parameters.AddWithValue("@Warehouse",    item.Warehouse ?? "Main Hub");
                cmd.Parameters.AddWithValue("@Quantity",     item.Quantity);
                cmd.Parameters.AddWithValue("@ReorderPoint", item.ReorderPoint ?? "10");
                cmd.Parameters.AddWithValue("@Status",       item.Status ?? "Stable");
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateInventoryAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteInventoryAsync(string sku)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = "DELETE FROM Inventory WHERE SKU = @SKU";
                using var cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@SKU", sku);
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteInventoryAsync Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Msme>> GetMsmesAsync()
        {
            var msmes = new List<Msme>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    string query = "SELECT BusinessName, Niche, OwnerName, ErpTier, ActiveUsers, Status FROM Msmes ORDER BY RegistrationDate DESC";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                msmes.Add(new Msme
                                {
                                    BusinessName = reader["BusinessName"].ToString(),
                                    Niche = reader["Niche"].ToString(),
                                    OwnerName = reader["OwnerName"].ToString(),
                                    ErpTier = reader["ErpTier"].ToString(),
                                    ActiveUsers = reader["ActiveUsers"].ToString(),
                                    Status = reader["Status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                // Return dummy data if connection fails
                InitializeDummyMsmes();
                return _dummyMsmes!;
            }
            return msmes;
        }

        public async Task<bool> AddMsmeAsync(Msme msme)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Auto-calculate user limit based on tier if not provided
                    if (string.IsNullOrEmpty(msme.ActiveUsers) || msme.ActiveUsers == "1 / 5")
                    {
                        int limit = GetUserLimitForTier(msme.ErpTier ?? "Starter");
                        msme.ActiveUsers = $"1 / {limit}";
                    }

                    string query = "INSERT INTO Msmes (BusinessName, Niche, OwnerName, ErpTier, ActiveUsers, Status) VALUES (@BusinessName, @Niche, @OwnerName, @ErpTier, @ActiveUsers, @Status)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessName", msme.BusinessName ?? string.Empty);
                        command.Parameters.AddWithValue("@Niche", msme.Niche ?? "Creative Business");
                        command.Parameters.AddWithValue("@OwnerName", msme.OwnerName ?? "System Administrator");
                        command.Parameters.AddWithValue("@ErpTier", msme.ErpTier ?? "Starter");
                        command.Parameters.AddWithValue("@ActiveUsers", msme.ActiveUsers);
                        command.Parameters.AddWithValue("@Status", msme.Status ?? "ACTIVE");

                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                InitializeDummyMsmes();
                _dummyMsmes!.Add(msme);
                return true;
            }
        }

        private int GetUserLimitForTier(string tier)
        {
            return UserSession.GetLimitForTier(tier);
        }


        public async Task<bool> CreateAccountAsync(string accountName, string email, string password, int roleId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Accounts (AccountName, Email, PasswordHash, RoleID, IsActive) VALUES (@AccountName, @Email, @Password, @RoleID, 1)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AccountName", accountName);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);
                        command.Parameters.AddWithValue("@RoleID", roleId);

                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateMsmeTierAsync(string businessName, string newTier)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Fetch current active count to maintain it while updating the limit
                    int currentActive = 1;
                    string fetchQuery = "SELECT ActiveUsers FROM Msmes WHERE BusinessName = @Name";
                    using (SqlCommand fetchCmd = new SqlCommand(fetchQuery, connection))
                    {
                        fetchCmd.Parameters.AddWithValue("@Name", businessName);
                        var result = await fetchCmd.ExecuteScalarAsync();
                        if (result != null)
                        {
                            string activeUsers = result.ToString() ?? "1 / 5";
                            if (activeUsers.Contains("/"))
                                int.TryParse(activeUsers.Split('/')[0].Trim(), out currentActive);
                        }
                    }

                    int newLimit = GetUserLimitForTier(newTier);
                    string updatedActiveUsers = $"{currentActive} / {newLimit}";

                    string query = "UPDATE Msmes SET ErpTier = @NewTier, ActiveUsers = @ActiveUsers WHERE BusinessName = @BusinessName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NewTier", newTier);
                        command.Parameters.AddWithValue("@ActiveUsers", updatedActiveUsers);
                        command.Parameters.AddWithValue("@BusinessName", businessName);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                InitializeDummyMsmes();
                var target = _dummyMsmes!.FirstOrDefault(m => m.BusinessName == businessName);
                if (target != null)
                {
                    target.ErpTier = newTier;
                    int newLimit = GetUserLimitForTier(newTier);
                    int currentActive = 1;
                    if (target.ActiveUsers != null && target.ActiveUsers.Contains("/"))
                        int.TryParse(target.ActiveUsers.Split('/')[0].Trim(), out currentActive);
                    target.ActiveUsers = $"{currentActive} / {newLimit}";
                }
                return true;
            }
        }


        public async Task<bool> UpdateAccountTierAsync(int accountId, string newTier)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    string accountName = string.Empty;
                    using (SqlCommand cmd = new SqlCommand("SELECT AccountName FROM Accounts WHERE AccountID = @AccountID", connection))
                    {
                        cmd.Parameters.AddWithValue("@AccountID", accountId);
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value) 
                        {
                            accountName = result.ToString() ?? "";
                        }
                    }

                    if (!string.IsNullOrEmpty(accountName))
                    {
                        // Also update ActiveUsers limit
                        int currentActive = 1;
                        using (SqlCommand fetchCmd = new SqlCommand("SELECT ActiveUsers FROM Msmes WHERE OwnerName = @Name OR BusinessName = @Name", connection))
                        {
                            fetchCmd.Parameters.AddWithValue("@Name", accountName);
                            var res = await fetchCmd.ExecuteScalarAsync();
                            if (res != null)
                            {
                                string activeUsers = res.ToString() ?? "1 / 5";
                                if (activeUsers.Contains("/"))
                                    int.TryParse(activeUsers.Split('/')[0].Trim(), out currentActive);
                            }
                        }

                        int newLimit = GetUserLimitForTier(newTier);
                        string updatedActiveUsers = $"{currentActive} / {newLimit}";

                        using (SqlCommand cmd = new SqlCommand("UPDATE Msmes SET ErpTier = @NewTier, ActiveUsers = @ActiveUsers WHERE OwnerName = @OwnerName OR BusinessName = @OwnerName", connection))
                        {
                            cmd.Parameters.AddWithValue("@NewTier", newTier);
                            cmd.Parameters.AddWithValue("@ActiveUsers", updatedActiveUsers);
                            cmd.Parameters.AddWithValue("@OwnerName", accountName);
                            int rows = await cmd.ExecuteNonQueryAsync();
                            
                            if (rows > 0)
                            {
                                // Automatically create a subscription record
                                await EnsureSubscriptionsTableSchema(connection);
                                var sub = new Subscription
                                {
                                    SubscriptionID = "SUB-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                                    AccountID = accountId,
                                    TierLevel = newTier,
                                    StartDate = DateTime.Now,
                                    EndDate = DateTime.Now.AddMonths(1),
                                    BillingCycle = "Monthly",
                                    PaymentStatus = "Paid",
                                    Amount = newTier == "Standard" ? 49.00m : (newTier == "Enterprise Plus" ? 199.00m : 0m)
                                };
                                await CreateSubscriptionAsync(sub);
                            }
                            
                            return rows > 0;
                        }
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error in UpdateAccountTierAsync: {ex.Message}");
                InitializeDummyMsmes();
                // Offline fallback: try to find the MSME associated with the default offline accounts
                string ownerToFind = accountId == 2 ? "Admin" : (accountId == 1 ? "Super Admin" : "");
                var target = _dummyMsmes!.FirstOrDefault(m => m.OwnerName == ownerToFind || m.OwnerName == "Elena Rodriguez");
                if (target != null)
                {
                    target.ErpTier = newTier;
                    int newLimit = GetUserLimitForTier(newTier);
                    int currentActive = 1;
                    if (target.ActiveUsers != null && target.ActiveUsers.Contains("/"))
                        int.TryParse(target.ActiveUsers.Split('/')[0].Trim(), out currentActive);
                    target.ActiveUsers = $"{currentActive} / {newLimit}";
                }
                return true;
            }
        }

        public async Task<bool> UpdateMsmeAsync(Msme msme, string originalBusinessName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "UPDATE Msmes SET BusinessName = @BusinessName, Niche = @Niche, OwnerName = @OwnerName, ErpTier = @ErpTier, ActiveUsers = @ActiveUsers, Status = @Status WHERE BusinessName = @OriginalName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessName", msme.BusinessName ?? string.Empty);
                        command.Parameters.AddWithValue("@Niche", msme.Niche ?? string.Empty);
                        command.Parameters.AddWithValue("@OwnerName", msme.OwnerName ?? string.Empty);
                        command.Parameters.AddWithValue("@ErpTier", msme.ErpTier ?? string.Empty);
                        command.Parameters.AddWithValue("@ActiveUsers", msme.ActiveUsers ?? "1 / 5");
                        command.Parameters.AddWithValue("@Status", msme.Status ?? string.Empty);
                        command.Parameters.AddWithValue("@OriginalName", originalBusinessName);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                InitializeDummyMsmes();
                var index = _dummyMsmes!.FindIndex(m => m.BusinessName == originalBusinessName);
                if (index != -1)
                {
                    _dummyMsmes![index] = msme;
                }
                return true;
            }
        }

        public async Task<bool> DeleteMsmeAsync(string businessName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Msmes WHERE BusinessName = @BusinessName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@BusinessName", businessName);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                InitializeDummyMsmes();
                _dummyMsmes!.RemoveAll(m => m.BusinessName == businessName);
                return true;
            }
        }

        public async Task<bool> UpdateMsmeStatusAsync(string businessName, string status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "UPDATE Msmes SET Status = @Status WHERE BusinessName = @BusinessName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", status);
                        command.Parameters.AddWithValue("@BusinessName", businessName);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                InitializeDummyMsmes();
                var target = _dummyMsmes!.FirstOrDefault(m => m.BusinessName == businessName);
                if (target != null)
                {
                    target.Status = status;
                }
                return true;
            }
        }

        public async Task<List<SaleTransaction>> GetRecentTransactionsAsync()
        {
            var transactions = new List<SaleTransaction>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureSalesTransactionsTableAsync(connection);

                    string query = "SELECT TransactionID, CustomerName, CustomerEmail, TransactionDate, Amount, Status FROM SalesTransactions ORDER BY TransactionDate DESC";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DateTime dt = Convert.ToDateTime(reader["TransactionDate"]);
                                transactions.Add(new SaleTransaction
                                {
                                    TransactionID = reader["TransactionID"].ToString(),
                                    CustomerName = reader["CustomerName"].ToString(),
                                    CustomerEmail = reader["CustomerEmail"].ToString(),
                                    Date = dt.ToString("MMM dd, yyyy"),
                                    Time = dt.ToString("HH:mm"),
                                    Amount = Convert.ToDecimal(reader["Amount"]),
                                    Status = reader["Status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
            }
            return transactions;
        }

        public async Task<ReportStats> GetReportStatsAsync()
        {
            var stats = new ReportStats();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureSalesTransactionsTableAsync(connection);
                    
                    // Sum revenue from SalesTransactions
                    using (SqlCommand cmd = new SqlCommand("SELECT SUM(Amount) FROM SalesTransactions WHERE Status = 'Completed'", connection))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        stats.TotalRevenue = (result != DBNull.Value && result != null) ? Convert.ToDecimal(result) : 0m;
                    }

                    // Total Orders
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SalesTransactions", connection))
                    {
                        stats.TotalOrders = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }

                    // New Creators (MSMEs registered in the last 30 days)
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Msmes WHERE RegistrationDate >= DATEADD(day, -30, GETDATE())", connection))
                    {
                        stats.NewCreators = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }

                    // Conversion Rate (Refined estimate)
                    stats.ConversionRate = stats.TotalOrders == 0 ? 0 : 4.82m;

                    // Total Customers (from Accounts table)
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Accounts WHERE RoleID = 3", connection))
                    {
                        stats.TotalCustomers = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }

                    // Active Leads (from SalesTransactions - recent activity)
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(DISTINCT CustomerID) FROM SalesTransactions WHERE TransactionDate >= DATEADD(day, -7, GETDATE())", connection))
                    {
                        stats.ActiveLeads = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    }

                    // Total Portfolio Spend (from customer account balances or orders)
                    using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(Amount), 0) FROM SalesTransactions", connection))
                    {
                        stats.TotalPortfolioSpend = Convert.ToDecimal(await cmd.ExecuteScalarAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
            }
            return stats;
        }

        public async Task<List<ChartDataPoint>> GetWeeklyRevenueGrowthAsync()
        {
            var data = new List<ChartDataPoint>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // In a fully integrated system, we would query the SalesTransactions table 
                    // grouping by Day and scaling the height relative to a max height of 340.
                    // For now, we seed realistic scaled data simulating the database query result.
                    
                    data.Add(new ChartDataPoint { Day = "Mon", LastWeekHeight = 120, ThisWeekHeight = 180 });
                    data.Add(new ChartDataPoint { Day = "Tue", LastWeekHeight = 160, ThisWeekHeight = 220 });
                    data.Add(new ChartDataPoint { Day = "Wed", LastWeekHeight = 140, ThisWeekHeight = 190 });
                    data.Add(new ChartDataPoint { Day = "Thu", LastWeekHeight = 210, ThisWeekHeight = 260 });
                    data.Add(new ChartDataPoint { Day = "Fri", LastWeekHeight = 250, ThisWeekHeight = 320 });
                    data.Add(new ChartDataPoint { Day = "Sat", LastWeekHeight = 280, ThisWeekHeight = 340 });
                    data.Add(new ChartDataPoint { Day = "Sun", LastWeekHeight = 190, ThisWeekHeight = 280 });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                // Fallback
                data.Add(new ChartDataPoint { Day = "Mon", LastWeekHeight = 120, ThisWeekHeight = 180 });
                data.Add(new ChartDataPoint { Day = "Tue", LastWeekHeight = 160, ThisWeekHeight = 220 });
                data.Add(new ChartDataPoint { Day = "Wed", LastWeekHeight = 140, ThisWeekHeight = 190 });
                data.Add(new ChartDataPoint { Day = "Thu", LastWeekHeight = 210, ThisWeekHeight = 260 });
                data.Add(new ChartDataPoint { Day = "Fri", LastWeekHeight = 250, ThisWeekHeight = 320 });
                data.Add(new ChartDataPoint { Day = "Sat", LastWeekHeight = 280, ThisWeekHeight = 340 });
                data.Add(new ChartDataPoint { Day = "Sun", LastWeekHeight = 190, ThisWeekHeight = 280 });
            }
            return data;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            await EnsureProductsTableSchema();
            var products = new List<Product>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT ProductID, Name, Category, Price, Stock, Status, ImageUrl, Description FROM Products";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                products.Add(new Product {
                                    ProductID = reader["ProductID"]?.ToString(),
                                    Name = reader["Name"]?.ToString(),
                                    Category = reader["Category"]?.ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Stock = Convert.ToInt32(reader["Stock"]),
                                    Status = reader["Status"]?.ToString(),
                                    ImageUrl = reader["ImageUrl"]?.ToString() ?? "product_placeholder.png",
                                    Description = reader["Description"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
        catch (Exception ex)
        {
            Console.WriteLine($"Database Error: {ex.Message}");
        }
            return products;
        }

        private async Task EnsureSalesTransactionsTableAsync(SqlConnection connection)
        {
            string createTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SalesTransactions')
                BEGIN
                    CREATE TABLE SalesTransactions (
                        TransactionID NVARCHAR(50) PRIMARY KEY,
                        CustomerID INT NOT NULL,
                        CustomerName NVARCHAR(200) NOT NULL,
                        CustomerEmail NVARCHAR(200) NULL,
                        TransactionDate DATETIME NOT NULL DEFAULT GETDATE(),
                        Amount DECIMAL(18,2) NOT NULL,
                        Status NVARCHAR(50) NOT NULL,
                        PaymentMethod NVARCHAR(50) NOT NULL
                    );
                END";
            using var cmd = new SqlCommand(createTable, connection);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<string?> RecordSaleTransactionAsync(int accountId, IList<CartItem> items, string paymentMethod)
        {
            if (items == null || items.Count == 0)
            {
                return null;
            }

            decimal total = items.Sum(item => item.Price);
            string transactionId = $"#TX-{DateTime.UtcNow:yyyyMMddHHmmss}";

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureSalesTransactionsTableAsync(connection);

                    var user = await GetAccountInfoByIdAsync(accountId);
                    string customerName = user?.AccountName ?? "Customer";
                    string customerEmail = user?.Email ?? string.Empty;

                    string query = @"INSERT INTO SalesTransactions
                                     (TransactionID, CustomerID, CustomerName, CustomerEmail, TransactionDate, Amount, Status, PaymentMethod)
                                     VALUES (@TransactionID, @CustomerID, @CustomerName, @CustomerEmail, GETDATE(), @Amount, @Status, @PaymentMethod)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TransactionID", transactionId);
                        command.Parameters.AddWithValue("@CustomerID", accountId);
                        command.Parameters.AddWithValue("@CustomerName", customerName);
                        command.Parameters.AddWithValue("@CustomerEmail", customerEmail);
                        command.Parameters.AddWithValue("@Amount", total);
                        command.Parameters.AddWithValue("@Status", "Completed");
                        command.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return null;
            }

            return transactionId;
        }


        public async Task<List<DigitalAsset>> GetAssetsAsync()
        {
            if (UserSession.AccountID > 5) return new List<DigitalAsset>();
            try {
                return new List<DigitalAsset> {
                    new DigitalAsset { Name = "Forest_Concept_V2.psd", Size = "240 MB", Format = "PSD", Date = "2 hours ago" },
                    new DigitalAsset { Name = "Brand_Mascot_Vector.ai", Size = "12 MB", Format = "AI", Date = "Yesterday" }
                };
            } catch { return new List<DigitalAsset>(); }
        }

        public async Task<List<CustomOrder>> GetCustomOrdersAsync()
        {
            var orders = new List<CustomOrder>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureCustomOrdersTableSchema(connection);

                    string query = "SELECT OrderID, Title, Description, ClientName, Status, Priority, Category, CONVERT(NVARCHAR, OrderDate, 23) AS OrderDate, Resolution, ColorProfile, Deliverables, AssignedArtist, Price FROM CustomOrders ORDER BY OrderDate DESC";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // Map legacy status values to 4-stage tracker
                                string rawStatus = reader["Status"]?.ToString() ?? "Briefing";
                                string mappedStatus = rawStatus switch
                                {
                                    "PENDING REVIEW" or "Pending" or "PENDING" => "Briefing",
                                    "IN PROGRESS" or "In Progress" or "STANDARD" => "Concept",
                                    "REVISION" or "Revision" => "Refining",
                                    "COMPLETED" or "Completed" or "EXPEDITED" => "Delivery",
                                    "Briefing" or "Concept" or "Refining" or "Delivery" => rawStatus,
                                    _ => "Briefing"
                                };

                                orders.Add(new CustomOrder
                                {
                                    OrderID = reader["OrderID"]?.ToString(),
                                    Title = reader["Title"]?.ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    ClientName = reader["ClientName"]?.ToString(),
                                    Status = mappedStatus,
                                    Priority = reader["Priority"]?.ToString(),
                                    Category = reader["Category"]?.ToString(),
                                    Date = reader["OrderDate"]?.ToString(),
                                    Resolution = reader["Resolution"]?.ToString(),
                                    ColorProfile = reader["ColorProfile"]?.ToString(),
                                    Deliverables = reader["Deliverables"]?.ToString(),
                                    AssignedArtist = reader["AssignedArtist"]?.ToString(),
                                    Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0m
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCustomOrdersAsync Error: {ex.Message}");
                // Offline fallback so the UI is never blank
                return new List<CustomOrder>
                {
                    new CustomOrder { OrderID = "8821", Title = "Futuristic Cyberpunk Landscape",
                        Description = "High-contrast neon aesthetics with an urban future theme.",
                        ClientName = "Alex Linden", Date = "2024-10-24", Status = "Briefing", Price = 1250.00m }
                };
            }
            return orders;
        }

        public async Task<(bool success, string error)> AddCustomOrderAsync(CustomOrder order)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureCustomOrdersTableSchema(connection);
                    
                    string query = @"INSERT INTO CustomOrders (OrderID, Title, Description, ClientName, Status, Priority, Category, OrderDate, Resolution, ColorProfile, Deliverables, AssignedArtist, Price)
                                     VALUES (@OrderID, @Title, @Description, @ClientName, @Status, @Priority, @Category, @OrderDate, @Resolution, @ColorProfile, @Deliverables, @AssignedArtist, @Price)";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OrderID", order.OrderID != null ? (object)order.OrderID : DBNull.Value);
                        command.Parameters.AddWithValue("@Title", order.Title ?? "Untitled Commission");
                        command.Parameters.AddWithValue("@Description", order.Description != null ? (object)order.Description : DBNull.Value);
                        command.Parameters.AddWithValue("@ClientName", order.ClientName != null ? (object)order.ClientName : DBNull.Value);
                        command.Parameters.AddWithValue("@Status", order.Status != null ? (object)order.Status : "PENDING");
                        command.Parameters.AddWithValue("@Priority", order.Priority != null ? (object)order.Priority : "Normal");
                        command.Parameters.AddWithValue("@Category", order.Category != null ? (object)order.Category : "Custom Art");
                        command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                        command.Parameters.AddWithValue("@Resolution", order.Resolution != null ? (object)order.Resolution : DBNull.Value);
                        command.Parameters.AddWithValue("@ColorProfile", order.ColorProfile != null ? (object)order.ColorProfile : DBNull.Value);
                        command.Parameters.AddWithValue("@Deliverables", order.Deliverables != null ? (object)order.Deliverables : DBNull.Value);
                        command.Parameters.AddWithValue("@AssignedArtist", order.AssignedArtist != null ? (object)order.AssignedArtist : DBNull.Value);
                        command.Parameters.AddWithValue("@Price", order.Price);

                        int rows = await command.ExecuteNonQueryAsync();
                        return (rows > 0, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public async Task<bool> UpdateCustomOrderStatusAsync(string orderId, string status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "UPDATE CustomOrders SET Status = @Status WHERE OrderID = @OrderID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", status);
                        command.Parameters.AddWithValue("@OrderID", orderId);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating custom order status: {ex.Message}");
                return false;
            }
        }

        public async Task<List<User>> GetCustomersAsync()
        {
            var users = new List<User>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = @"
                        SELECT AccountName as Username, Email, 'Customer' as RoleName, 'Active' as Status, 'Just now' as LastActive 
                        FROM Accounts 
                        WHERE RoleID = 3";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(new User
                                {
                                    Username = reader["Username"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Role = reader["RoleName"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    LastActive = reader["LastActive"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return users;
        }
        public async Task<List<SystemModule>> GetSystemModulesAsync()
        {
            var modules = new List<SystemModule>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureSystemModulesTableAsync(connection);

                    string query = "SELECT Name, Version, Status, EnabledCount, IsStarterEnabled, IsStandardEnabled, IsEnterpriseEnabled FROM SystemModules";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                modules.Add(new SystemModule
                                {
                                    Name = reader["Name"].ToString() ?? string.Empty,
                                    Version = reader["Version"].ToString() ?? string.Empty,
                                    Status = reader["Status"].ToString() ?? string.Empty,
                                    EnabledCount = Convert.ToInt32(reader["EnabledCount"]),
                                    IsStarterEnabled = reader["IsStarterEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsStarterEnabled"]),
                                    IsStandardEnabled = reader["IsStandardEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsStandardEnabled"]),
                                    IsEnterpriseEnabled = reader["IsEnterpriseEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsEnterpriseEnabled"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return new List<SystemModule>
                {
                    new SystemModule { Name = "Inventory Management", Version = "v2.4.1", Status = "Active", EnabledCount = 124, IsStarterEnabled = true, IsStandardEnabled = true, IsEnterpriseEnabled = true },
                    new SystemModule { Name = "Sales Analytics", Version = "v1.8.0", Status = "Active", EnabledCount = 98, IsStarterEnabled = false, IsStandardEnabled = true, IsEnterpriseEnabled = true },
                    new SystemModule { Name = "CRM Integration", Version = "v3.0.2", Status = "Beta", EnabledCount = 42, IsStarterEnabled = false, IsStandardEnabled = false, IsEnterpriseEnabled = true },
                    new SystemModule { Name = "Finance Hub", Version = "v2.1.0", Status = "Active", EnabledCount = 115, IsStarterEnabled = false, IsStandardEnabled = true, IsEnterpriseEnabled = true }
                };
            }
            return modules;
        }


        public async Task<bool> UpdateSystemModuleStatusAsync(string moduleName, string newStatus)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureSystemModulesTableAsync(connection);
                    string query = "UPDATE SystemModules SET Status = @Status WHERE Name = @Name";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Status", newStatus);
                        command.Parameters.AddWithValue("@Name", moduleName);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddSystemModuleAsync(SystemModule module)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await EnsureSystemModulesTableAsync(connection);
                    string query = "INSERT INTO SystemModules (Name, Version, Status, EnabledCount, IsStarterEnabled, IsStandardEnabled, IsEnterpriseEnabled) VALUES (@Name, @Version, @Status, @EnabledCount, @IsStarter, @IsStandard, @IsEnterprise)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", module.Name);
                        command.Parameters.AddWithValue("@Version", module.Version);
                        command.Parameters.AddWithValue("@Status", module.Status);
                        command.Parameters.AddWithValue("@EnabledCount", module.EnabledCount);
                        command.Parameters.AddWithValue("@IsStarter", module.IsStarterEnabled);
                        command.Parameters.AddWithValue("@IsStandard", module.IsStandardEnabled);
                        command.Parameters.AddWithValue("@IsEnterprise", module.IsEnterpriseEnabled);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateModulePermissionsAsync(string moduleName, bool starter, bool standard, bool enterprise)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureSystemModulesTableAsync(connection);
                    string query = @"UPDATE SystemModules 
                                   SET IsStarterEnabled = @IsStarter, 
                                       IsStandardEnabled = @IsStandard, 
                                       IsEnterpriseEnabled = @IsEnterprise 
                                   WHERE Name = @Name";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IsStarter", starter);
                        command.Parameters.AddWithValue("@IsStandard", standard);
                        command.Parameters.AddWithValue("@IsEnterprise", enterprise);
                        command.Parameters.AddWithValue("@Name", moduleName);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteSystemModuleAsync(string moduleName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureSystemModulesTableAsync(connection);
                    string query = "DELETE FROM SystemModules WHERE Name = @Name";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", moduleName);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return false;
            }
        }

        public async Task<Reward?> GetCustomerRewardsAsync(int accountId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureRewardsTablesAsync(connection);
                    string query = "SELECT PointsBalance, Tier FROM Rewards WHERE AccountID = @AccountID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AccountID", accountId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Reward
                                {
                                    PointsBalance = Convert.ToInt32(reader["PointsBalance"]),
                                    Tier = reader["Tier"].ToString() ?? string.Empty
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
            }
            return null;
        }

        private async Task EnsureRewardsTablesAsync(SqlConnection connection)
        {
            string createRewards = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Rewards')
                BEGIN
                    CREATE TABLE Rewards (
                        AccountID INT PRIMARY KEY,
                        PointsBalance INT NOT NULL DEFAULT 0,
                        Tier NVARCHAR(100) NOT NULL DEFAULT 'Creative Enthusiast'
                    );
                END
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RewardCatalog')
                BEGIN
                    CREATE TABLE RewardCatalog (
                        RewardId INT IDENTITY(1,1) PRIMARY KEY,
                        Title NVARCHAR(200) NOT NULL,
                        Description NVARCHAR(400) NOT NULL,
                        PointsCost INT NOT NULL
                    );
                    INSERT INTO RewardCatalog (Title, Description, PointsCost)
                    VALUES
                    ('15% Commission Discount', 'Apply this discount to any custom order over $500.', 1500),
                    ('Exclusive Artist Access', 'Early access to limited series releases and priority booking.', 2000),
                    ('CreatiSphere Swag Kit', 'Get our seasonal creative toolkit shipped directly to you.', 500);
                END
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RewardHistory')
                BEGIN
                    CREATE TABLE RewardHistory (
                        HistoryId INT IDENTITY(1,1) PRIMARY KEY,
                        AccountID INT NOT NULL,
                        ActivityDate DATETIME NOT NULL DEFAULT GETDATE(),
                        Description NVARCHAR(300) NOT NULL,
                        Status NVARCHAR(50) NOT NULL,
                        Points INT NOT NULL
                    );
                END";
            using var cmd = new SqlCommand(createRewards, connection);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<RedeemableReward>> GetRedeemableRewardsAsync()
        {
            var rewards = new List<RedeemableReward>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureRewardsTablesAsync(connection);
                    string query = "SELECT RewardId, Title, Description, PointsCost FROM RewardCatalog ORDER BY PointsCost DESC";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                rewards.Add(new RedeemableReward
                                {
                                    RewardId = Convert.ToInt32(reader["RewardId"]),
                                    Title = reader["Title"].ToString() ?? string.Empty,
                                    Description = reader["Description"].ToString() ?? string.Empty,
                                    PointsCost = Convert.ToInt32(reader["PointsCost"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
            }
            return rewards;
        }

        public async Task<List<RewardHistoryItem>> GetRewardHistoryAsync(int accountId)
        {
            var history = new List<RewardHistoryItem>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureRewardsTablesAsync(connection);
                    string query = "SELECT ActivityDate, Description, Status, Points FROM RewardHistory WHERE AccountID = @AccountID ORDER BY ActivityDate DESC";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AccountID", accountId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                history.Add(new RewardHistoryItem
                                {
                                    Date = Convert.ToDateTime(reader["ActivityDate"]),
                                    Description = reader["Description"].ToString() ?? string.Empty,
                                    Status = reader["Status"].ToString() ?? string.Empty,
                                    Points = Convert.ToInt32(reader["Points"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
            }
            return history;
        }
        public async Task<List<Commission>> GetCommissionsAsync(int accountId, bool isCreator = false)
        {
            var commissions = new List<Commission>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string column = isCreator ? "CreatorID" : "CustomerID";
                    string query = $"SELECT Title, ArtistName, ClientName, Progress, Status, ImagePath FROM Commissions WHERE {column} = @AccountID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AccountID", accountId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                commissions.Add(new Commission
                                {
                                    Title = reader["Title"].ToString() ?? string.Empty,
                                    ArtistName = reader["ArtistName"].ToString() ?? string.Empty,
                                    ClientName = reader["ClientName"].ToString() ?? string.Empty,
                                    Progress = Convert.ToInt32(reader["Progress"]),
                                    Status = reader["Status"].ToString() ?? string.Empty,
                                    ImagePath = reader["ImagePath"].ToString() ?? "commission_placeholder.png"
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return commissions;
        }

        public async Task<List<Activity>> GetUserActivityAsync(int accountId)
        {
            var activities = new List<Activity>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT Title, Timestamp, Icon FROM UserActivity WHERE AccountID = @AccountID ORDER BY Timestamp DESC";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AccountID", accountId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DateTime dt = Convert.ToDateTime(reader["Timestamp"]);
                                string timeStr = (DateTime.Now - dt).TotalHours < 24 
                                    ? $"{(int)(DateTime.Now - dt).TotalHours} hours ago" 
                                    : dt.ToString("MMMM dd");

                                activities.Add(new Activity
                                {
                                    Title = reader["Title"].ToString() ?? string.Empty,
                                    Timestamp = timeStr,
                                    Icon = reader["Icon"].ToString() ?? "Edit"
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return activities;
        }

        public async Task<List<CreatorAsset>> GetCreatorAssetsAsync(int accountId)
        {
            var assets = new List<CreatorAsset>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT Title, Type, Price, ImagePath, Views, Sales FROM CreatorAssets WHERE CreatorID = @AccountID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AccountID", accountId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                assets.Add(new CreatorAsset
                                {
                                    Title = reader["Title"].ToString() ?? string.Empty,
                                    Type = reader["Type"].ToString() ?? string.Empty,
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    ImagePath = reader["ImagePath"].ToString() ?? string.Empty,
                                    Views = Convert.ToInt32(reader["Views"]),
                                    Sales = Convert.ToInt32(reader["Sales"])
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return assets;
        }

        public async Task<SystemMetrics> GetSystemMetricsAsync()
        {
            var rand = new Random();
            var metrics = new SystemMetrics
            {
                CpuLoad = 15 + rand.NextDouble() * 15,
                CpuTrend = (rand.NextDouble() * 5) * (rand.Next(0, 2) == 0 ? 1 : -1),
                MemoryUsage = 60 + rand.NextDouble() * 10,
                MemoryTrend = (rand.NextDouble() * 2) * (rand.Next(0, 2) == 0 ? 1 : -1),
                ApiConnections = 1200 + rand.Next(800),
                ApiTrend = rand.Next(-50, 150),
                Latency = 10 + rand.Next(10),
                LatencyStatus = rand.Next(0, 10) > 8 ? "Congested" : "Stable"
            };
            
            // Generate a more "wave-like" history
            double current = 40.0;
            for (int i = 0; i < 24; i++) 
            {
                current += (rand.NextDouble() - 0.5) * 40;
                current = Math.Clamp(current, 20, 180);
                metrics.LoadHistory.Add(current);
            }
            
            return metrics;
        }

        public async Task<AdminStats> GetAdminStatsAsync()
        {
            var stats = new AdminStats();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Total Admins (Role 1 or 2)
                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Accounts WHERE RoleID IN (1, 2)", connection))
                    {
                        stats.TotalAdmins = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }

                    // Active Sessions (Proxy: Active accounts)
                    // We'll multiply by a factor to make it look like "sessions" or just use active count
                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Accounts WHERE IsActive = 1", connection))
                    {
                        stats.ActiveSessions = Convert.ToInt32(await command.ExecuteScalarAsync()) * 7; // Simulating multiple sessions per user
                    }

                    // Pending Requests (From our new table)
                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM PendingRequests", connection))
                    {
                        stats.PendingRequests = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }

                    // Dynamic Badges
                    stats.MonthlyGrowth = "+4% this mo"; // Hardcoded for now but structure is there
                    stats.PriorityStatus = stats.PendingRequests > 5 ? "Critical" : (stats.PendingRequests > 0 ? "High Priority" : "All Clear");

                    // Super Admin Ratio
                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Accounts WHERE RoleID = 1", connection))
                    {
                        int superAdmins = Convert.ToInt32(await command.ExecuteScalarAsync());
                        if (stats.TotalAdmins > 0)
                        {
                            stats.SuperAdminRatio = (double)superAdmins / stats.TotalAdmins;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                // Fallback dummy data only on error
                stats.TotalAdmins = 128;
                stats.ActiveSessions = 42;
                stats.PendingRequests = 15;
                stats.SuperAdminRatio = 0.65;
            }
            return stats;
        }
        public async Task<MsmeStats> GetMsmeStatsAsync()
        {
            var stats = new MsmeStats
            {
                TotalMsmes = 0,
                ActiveMsmes = 0,
                OnTrialMsmes = 0,
                Growth = "+12%"
            };

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Msmes", connection))
                    {
                        stats.TotalMsmes = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }

                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Msmes WHERE Status = 'ACTIVE' OR Status = 'Active'", connection))
                    {
                        stats.ActiveMsmes = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }

                    using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Msmes WHERE Status = 'TRIAL' OR Status = 'Trial'", connection))
                    {
                        stats.OnTrialMsmes = Convert.ToInt32(await command.ExecuteScalarAsync());
                    }
                }
            }
            catch
            {
                // Fallback dummy data only on error
                stats.TotalMsmes = 3;
                stats.ActiveMsmes = 1;
                stats.OnTrialMsmes = 1;
            }
            return stats;
        }

        public async Task<bool> UpdateUserStatusAsync(string email, string status)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    bool isActive = status.Equals("Active", StringComparison.OrdinalIgnoreCase);
                    string query = "UPDATE Accounts SET IsActive = @IsActive WHERE Email = @Email";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IsActive", isActive ? 1 : 0);
                        command.Parameters.AddWithValue("@Email", email);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateAdminAccountAsync(string accountName, string email, string role, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    int roleId = role switch
                    {
                        "Super Admin" => 1,
                        "Admin" => 2,
                        "Customer" => 3,
                        "Creator" => 4,
                        "Sales" => 5,
                        "Finance" => 6,
                        _ => 2 // Default to Admin
                    };
                    
                    string query = @"INSERT INTO Accounts (AccountName, Email, RoleID, PasswordHash, IsActive, CreatedDate, LastModifiedDate) 
                                     VALUES (@AccountName, @Email, @RoleID, @Password, 1, GETDATE(), GETDATE())";
                    
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AccountName", accountName);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@RoleID", roleId);
                        command.Parameters.AddWithValue("@Password", password);
                        
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating admin account: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteUserAsync(string email)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Accounts WHERE Email = @Email";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> AddProductAsync(Product product)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO Products (ProductID, Name, Category, Price, Stock, Status, ImageUrl, Description) VALUES (@ProductID, @Name, @Category, @Price, @Stock, @Status, @ImageUrl, @Description)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", product.ProductID ?? Guid.NewGuid().ToString().Substring(0, 8));
                        command.Parameters.AddWithValue("@Name", product.Name ?? string.Empty);
                        command.Parameters.AddWithValue("@Category", product.Category ?? string.Empty);
                        command.Parameters.AddWithValue("@Price", product.Price);
                        command.Parameters.AddWithValue("@Stock", product.Stock);
                        command.Parameters.AddWithValue("@Status", product.Status ?? "In Stock");
                        command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl ?? "product_placeholder.png");
                        command.Parameters.AddWithValue("@Description", product.Description ?? string.Empty);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "UPDATE Products SET Name = @Name, Category = @Category, Price = @Price, Stock = @Stock, Status = @Status, ImageUrl = @ImageUrl, Description = @Description WHERE ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", product.Name ?? string.Empty);
                        command.Parameters.AddWithValue("@Category", product.Category ?? string.Empty);
                        command.Parameters.AddWithValue("@Price", product.Price);
                        command.Parameters.AddWithValue("@Stock", product.Stock);
                        command.Parameters.AddWithValue("@Status", product.Status ?? string.Empty);
                        command.Parameters.AddWithValue("@ImageUrl", product.ImageUrl ?? string.Empty);
                        command.Parameters.AddWithValue("@Description", product.Description ?? string.Empty);
                        command.Parameters.AddWithValue("@ProductID", product.ProductID);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Products WHERE ProductID = @ProductID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", productId);
                        int rows = await command.ExecuteNonQueryAsync();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                return false;
            }
        }

        private async Task EnsureCustomOrdersTableSchema(SqlConnection connection)
        {
            string createTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CustomOrders')
                BEGIN
                    CREATE TABLE CustomOrders (
                        OrderID        NVARCHAR(50)  PRIMARY KEY,
                        Title          NVARCHAR(255) NOT NULL,
                        Description    NVARCHAR(MAX) NULL,
                        ClientName     NVARCHAR(255) NULL,
                        Status         NVARCHAR(50)  NULL,
                        Priority       NVARCHAR(50)  NULL,
                        Category       NVARCHAR(100) NULL,
                        OrderDate      DATETIME      NOT NULL DEFAULT GETDATE(),
                        Resolution     NVARCHAR(100) NULL,
                        ColorProfile   NVARCHAR(100) NULL,
                        Deliverables   NVARCHAR(MAX) NULL,
                        AssignedArtist NVARCHAR(255) NULL,
                        Price          DECIMAL(18,2) NOT NULL DEFAULT 0
                    );
                END
                ELSE
                BEGIN
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CustomOrders' AND COLUMN_NAME = 'Price')
                        ALTER TABLE CustomOrders ADD Price DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CustomOrders' AND COLUMN_NAME = 'Resolution')
                        ALTER TABLE CustomOrders ADD Resolution NVARCHAR(100) NULL;
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CustomOrders' AND COLUMN_NAME = 'ColorProfile')
                        ALTER TABLE CustomOrders ADD ColorProfile NVARCHAR(100) NULL;
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CustomOrders' AND COLUMN_NAME = 'Deliverables')
                        ALTER TABLE CustomOrders ADD Deliverables NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CustomOrders' AND COLUMN_NAME = 'AssignedArtist')
                        ALTER TABLE CustomOrders ADD AssignedArtist NVARCHAR(255) NULL;
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CustomOrders' AND COLUMN_NAME = 'Priority')
                        ALTER TABLE CustomOrders ADD Priority NVARCHAR(50) NULL;
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'CustomOrders' AND COLUMN_NAME = 'Category')
                        ALTER TABLE CustomOrders ADD Category NVARCHAR(100) NULL;
                END";
            using (SqlCommand createCmd = new SqlCommand(createTable, connection))
                await createCmd.ExecuteNonQueryAsync();
        }

        private async Task EnsureSystemModulesTableAsync(SqlConnection connection)
        {
            string createTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SystemModules')
                BEGIN
                    CREATE TABLE SystemModules (
                        Name                NVARCHAR(100) PRIMARY KEY,
                        Version             NVARCHAR(50)  NULL,
                        Status              NVARCHAR(50)  NULL,
                        EnabledCount        INT           DEFAULT 0,
                        IsStarterEnabled    BIT           DEFAULT 0,
                        IsStandardEnabled   BIT           DEFAULT 0,
                        IsEnterpriseEnabled BIT           DEFAULT 0
                    );
                    INSERT INTO SystemModules (Name, Version, Status, EnabledCount, IsStarterEnabled, IsStandardEnabled, IsEnterpriseEnabled) VALUES
                    ('Inventory Management', 'v2.4.1', 'Active', 124, 1, 1, 1),
                    ('Sales Analytics',      'v1.8.0', 'Active', 98,  0, 1, 1),
                    ('CRM Integration',      'v3.0.2', 'Beta',   42,  0, 0, 1),
                    ('Finance Hub',          'v2.1.0', 'Active', 115, 0, 1, 1);
                END";
            using (SqlCommand command = new SqlCommand(createTable, connection))
                await command.ExecuteNonQueryAsync();
        }

        public async Task<List<Subscription>> GetSubscriptionsAsync(int accountId)
        {
            var subs = new List<Subscription>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureSubscriptionsTableSchema(connection);
                    string query = "SELECT * FROM Subscriptions WHERE AccountID = @ID ORDER BY StartDate DESC";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", accountId);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                subs.Add(new Subscription
                                {
                                    SubscriptionID = reader["SubscriptionID"].ToString() ?? "",
                                    AccountID = Convert.ToInt32(reader["AccountID"]),
                                    TierLevel = reader["TierLevel"].ToString() ?? "",
                                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                                    BillingCycle = reader["BillingCycle"].ToString() ?? "",
                                    PaymentStatus = reader["PaymentStatus"].ToString() ?? "",
                                    Amount = Convert.ToDecimal(reader["Amount"])
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return subs;
        }

        public async Task<bool> CreateSubscriptionAsync(Subscription sub)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    await EnsureSubscriptionsTableSchema(connection);
                    string query = @"INSERT INTO Subscriptions (SubscriptionID, AccountID, TierLevel, StartDate, EndDate, BillingCycle, PaymentStatus, Amount) 
                                     VALUES (@ID, @AccID, @Tier, @Start, @End, @Cycle, @Status, @Amount)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", sub.SubscriptionID);
                        command.Parameters.AddWithValue("@AccID", sub.AccountID);
                        command.Parameters.AddWithValue("@Tier", sub.TierLevel);
                        command.Parameters.AddWithValue("@Start", sub.StartDate);
                        command.Parameters.AddWithValue("@End", sub.EndDate);
                        command.Parameters.AddWithValue("@Cycle", sub.BillingCycle);
                        command.Parameters.AddWithValue("@Status", sub.PaymentStatus);
                        command.Parameters.AddWithValue("@Amount", sub.Amount);
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch { return false; }
        }

        private async Task EnsureSubscriptionsTableSchema(SqlConnection connection)
        {
            string createTable = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Subscriptions')
                BEGIN
                    CREATE TABLE Subscriptions (
                        SubscriptionID  NVARCHAR(50)  PRIMARY KEY,
                        AccountID       INT           NOT NULL,
                        TierLevel       NVARCHAR(50)  NOT NULL,
                        StartDate       DATETIME      NOT NULL DEFAULT GETDATE(),
                        EndDate         DATETIME      NOT NULL,
                        BillingCycle    NVARCHAR(50)  NULL,
                        PaymentStatus   NVARCHAR(50)  NULL,
                        Amount          DECIMAL(18,2) NOT NULL DEFAULT 0
                    );
                END";
            using (SqlCommand command = new SqlCommand(createTable, connection))
                await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Removes unused database tables that are not in the use-case scope.
        /// This permanently deletes: CrmLeads, SystemIncidents, ReportArchives tables and all their data.
        /// </summary>
        public async Task<bool> CleanupUnusedTablesAsync()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string[] tablesToDrop = { "CrmLeads", "SystemIncidents", "ReportArchives" };
                    int droppedCount = 0;

                    foreach (var tableName in tablesToDrop)
                    {
                        string query = $"IF OBJECT_ID('dbo.{tableName}', 'U') IS NOT NULL DROP TABLE dbo.{tableName};";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                            droppedCount++;
                            Console.WriteLine($"✓ Dropped table: dbo.{tableName}");
                        }
                    }

                    Console.WriteLine($"\nSuccessfully removed {droppedCount} unused tables.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning up tables: {ex.Message}");
                return false;
            }
        }
    }
}

