using System;

namespace CreatiSphere.Services
{
    public static class UserSession
    {
        public static int AccountID { get; set; }
        public static string? Username { get; set; }
        public static string? Email { get; set; }
        public static int RoleID { get; set; }
        public static string Tier { get; set; } = "Starter"; // Default to Starter
        
        public static bool IsSuperAdmin => RoleID == 1;
        public static bool IsAdmin => RoleID == 2;

        public static int MaxUsers => GetLimitForTier(Tier);
        
        public static int GetLimitForTier(string tier) => tier switch
        {
            "Starter" => 5,
            "Standard" => 20,
            "Enterprise Plus" => 50,
            _ => 5
        };

        public static bool CanCreateSuperAdmin => Tier == "Enterprise Plus";
        public static bool CanBulkExport => Tier != "Starter";
    }
}
