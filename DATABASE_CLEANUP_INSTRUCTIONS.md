# How to Clean Up Unused Database Tables

Your CreatiSphere database contains 3 unused tables that should be deleted:
- ❌ `dbo.CrmLeads` (CRM Lead data - not in use case)
- ❌ `dbo.SystemIncidents` (System Incident data - not in use case)
- ❌ `dbo.ReportArchives` (Report Archive data - not in use case)

## Method 1: Using SQL Server Management Studio (Easiest) ✅ RECOMMENDED

### Steps:
1. **Open SQL Server Management Studio (SSMS)**
2. **Connect to**: `DESKTOP-H3Q23FS\SQLEXPRESS`
3. **Navigate to**: CreatiSphere → Databases → Tables
4. **Right-click on** `dbo.CrmLeads` → Delete
5. **Right-click on** `dbo.SystemIncidents` → Delete
6. **Right-click on** `dbo.ReportArchives` → Delete

**Done!** The unused tables and all their data will be permanently deleted.

---

## Method 2: Using SQL Query (T-SQL Script)

### Option A: Copy and paste into SSMS Query Window

1. Open SQL Server Management Studio
2. Connect to: `DESKTOP-H3Q23FS\SQLEXPRESS`
3. Select database: `CreatiSphere`
4. Open new query window (Ctrl+N)
5. Copy and paste this script:

```sql
-- Delete unused tables from CreatiSphere database
USE CreatiSphere;
GO

-- Drop CrmLeads table and all its data
IF OBJECT_ID('dbo.CrmLeads', 'U') IS NOT NULL 
BEGIN
	DROP TABLE dbo.CrmLeads;
	PRINT 'Dropped: dbo.CrmLeads';
END
GO

-- Drop SystemIncidents table and all its data
IF OBJECT_ID('dbo.SystemIncidents', 'U') IS NOT NULL 
BEGIN
	DROP TABLE dbo.SystemIncidents;
	PRINT 'Dropped: dbo.SystemIncidents';
END
GO

-- Drop ReportArchives table and all its data
IF OBJECT_ID('dbo.ReportArchives', 'U') IS NOT NULL 
BEGIN
	DROP TABLE dbo.ReportArchives;
	PRINT 'Dropped: dbo.ReportArchives';
END
GO

PRINT 'Cleanup completed!';
```

6. Press **F5** or click **Execute**

**Done!** You'll see confirmation messages for each table deleted.

---

## Method 3: Using the DatabaseCleanup_DeleteTables.sql Script

The file `DatabaseCleanup_DeleteTables.sql` is already created in your project.

### To execute it:

1. Open SQL Server Management Studio
2. File → Open → File
3. Navigate to: `C:\IT13_CreatiSphere\DatabaseCleanup_DeleteTables.sql`
4. Click Open
5. Press **F5** to execute

**Done!**

---

## Method 4: Using C# Code (From Your Application)

Add this code to any page (like AdminDashboardPage) to clean up when the app runs:

```csharp
// Add this to your page's initialization (e.g., MainPage.xaml.cs OnAppearing)
var dbService = new DatabaseService();
bool success = await dbService.CleanupUnusedTablesAsync();

if (success)
{
	await DisplayAlert("Success", "Database cleanup completed!", "OK");
}
else
{
	await DisplayAlert("Error", "Cleanup failed. Check the logs.", "OK");
}
```

---

## Verification: Check If Tables Are Deleted

After cleanup, verify the tables are gone:

### In SSMS:
1. Refresh the Tables folder in CreatiSphere database
2. You should NO LONGER see:
   - dbo.CrmLeads ✓ GONE
   - dbo.SystemIncidents ✓ GONE
   - dbo.ReportArchives ✓ GONE

### With SQL Query:
Run this query to list all remaining tables:

```sql
USE CreatiSphere;
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

You should see 20+ tables, but NOT the 3 that were deleted.

---

## What Gets Deleted?

| Table | Data Deleted | Impact |
|-------|-------------|--------|
| dbo.CrmLeads | All lead records (names, emails, interests, values) | Lead tracking feature disabled |
| dbo.SystemIncidents | All incident records (timestamps, severity, status) | Incident monitoring disabled |
| dbo.ReportArchives | All archived reports (names, formats, dates) | Report archives feature disabled |

---

## What Stays Intact?

✅ All your business data:
- Products & inventory
- Customer accounts & transactions
- Orders & custom orders
- Rewards & loyalty data
- Creator assets
- Sales & financial data

---

## Safety Note

⚠️ **WARNING**: This operation is **permanent**. The deleted data CANNOT be recovered.

However, since these features (CRM Leads, System Incidents, Report Archives) are not part of your use-case, the deletion is safe.

---

## I Already Cleaned Up the Code, So...

Your application code has already been updated to:
- ✓ NOT call the deleted table creation methods
- ✓ NOT reference CrmLead, SystemIncident, or ReportArchive models
- ✓ Use live Accounts & SalesTransactions data instead

The database cleanup is just the final step to remove the orphaned tables.

---

## Quick Checklist

- [ ] Choose cleanup method (1, 2, 3, or 4 above)
- [ ] Execute the cleanup script
- [ ] Verify tables are deleted using the verification query
- [ ] Run your application - it will work normally
- [ ] All metrics now derive from live transaction data ✓

**That's it! Your database is now clean and aligned with your use case.** 🎉
