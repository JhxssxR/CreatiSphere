using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

class DatabaseCleaner
{
    static async Task RunCleanupAsync()
    {
        string connectionString = "Server=DESKTOP-H3Q23FS\\SQLEXPRESS;Database=CreatiSphere;Integrated Security=true;";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("✓ Connected to CreatiSphere database");
                Console.WriteLine("");

                // Drop CrmLeads table
                string[] tablesToDrop = { "CrmLeads", "SystemIncidents", "ReportArchives" };

                foreach (var tableName in tablesToDrop)
                {
                    string query = $"IF OBJECT_ID('dbo.{tableName}', 'U') IS NOT NULL DROP TABLE dbo.{tableName};";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                        Console.WriteLine($"✓ Dropped table: dbo.{tableName}");
                    }
                }

                Console.WriteLine("");
                Console.WriteLine("========================================");
                Console.WriteLine("Cleanup completed successfully!");
                Console.WriteLine("========================================");
                Console.WriteLine("");
                Console.WriteLine("Removed unused tables and all their data:");
                Console.WriteLine("  ✓ dbo.CrmLeads");
                Console.WriteLine("  ✓ dbo.SystemIncidents");
                Console.WriteLine("  ✓ dbo.ReportArchives");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
