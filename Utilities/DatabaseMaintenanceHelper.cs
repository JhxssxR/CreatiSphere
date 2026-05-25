using System;
using System.Threading.Tasks;
using CreatiSphere.Services;

namespace CreatiSphere.Utilities
{
    /// <summary>
    /// Database maintenance utility for cleaning up unused tables.
    /// </summary>
    public class DatabaseMaintenanceHelper
    {
        private readonly DatabaseService _databaseService;

        public DatabaseMaintenanceHelper()
        {
            _databaseService = new DatabaseService();
        }

        /// <summary>
        /// Cleans up unused database tables that are not in the use-case scope.
        /// Call this once after updating to the new codebase.
        /// 
        /// Tables removed:
        /// - dbo.CrmLeads (CRM Lead tracking - not in use case)
        /// - dbo.SystemIncidents (System Incident tracking - not in use case)
        /// - dbo.ReportArchives (Report Archive management - not in use case)
        /// </summary>
        public async Task CleanupUnusedTablesAsync()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("CreatiSphere Database Cleanup Process");
            Console.WriteLine("========================================");
            Console.WriteLine("");
            Console.WriteLine("Removing unused tables...");
            Console.WriteLine("");

            bool success = await _databaseService.CleanupUnusedTablesAsync();

            Console.WriteLine("");
            if (success)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("✓ Cleanup Completed Successfully!");
                Console.WriteLine("========================================");
                Console.WriteLine("");
                Console.WriteLine("Removed tables and their data:");
                Console.WriteLine("  ✓ dbo.CrmLeads");
                Console.WriteLine("  ✓ dbo.SystemIncidents");
                Console.WriteLine("  ✓ dbo.ReportArchives");
                Console.WriteLine("");
                Console.WriteLine("Your database now contains only use-case-aligned tables.");
            }
            else
            {
                Console.WriteLine("✗ Cleanup failed. Check the error messages above.");
            }
        }
    }
}
