# ⚡ Quick Start: Delete Unused Tables (2-5 minutes)

## The Situation

✅ **Code**: All cleaned and updated
❌ **Database**: Still has 3 unused tables with old data

## The Solution (Pick ONE)

---

## 🥇 Method 1: SSMS GUI (Easiest for Beginners)

**Time: 2 minutes**

```
Step 1: Open SQL Server Management Studio
Step 2: Connect to: DESKTOP-H3Q23FS\SQLEXPRESS
Step 3: Navigate: Databases → CreatiSphere → Tables
Step 4: Right-click dbo.CrmLeads → Delete
Step 5: Right-click dbo.SystemIncidents → Delete
Step 6: Right-click dbo.ReportArchives → Delete
Done! ✓
```

---

## 🥈 Method 2: Copy-Paste SQL (Fastest)

**Time: 1 minute**

### In SQL Server Management Studio:

1. **Open new query** (Ctrl + N)
2. **Copy this entire script:**

```sql
USE CreatiSphere;
GO

IF OBJECT_ID('dbo.CrmLeads', 'U') IS NOT NULL 
	DROP TABLE dbo.CrmLeads;

IF OBJECT_ID('dbo.SystemIncidents', 'U') IS NOT NULL 
	DROP TABLE dbo.SystemIncidents;

IF OBJECT_ID('dbo.ReportArchives', 'U') IS NOT NULL 
	DROP TABLE dbo.ReportArchives;

PRINT 'Cleanup complete! Deleted 3 unused tables.';
```

3. **Paste into query window**
4. **Press F5** (Execute)
5. **Done!** ✓

---

## 🥉 Method 3: Pre-Made Script (No Copy-Paste)

**Time: 1 minute**

1. Open SSMS
2. File → Open File
3. Browse to: `C:\IT13_CreatiSphere\DatabaseCleanup_DeleteTables.sql`
4. Open
5. Press **F5**
6. Done! ✓

---

## 🎯 Method 4: From Your Application (Code-Based)

**Time: 1-2 minutes**

Add this to any page that loads early (e.g., `MainPage.xaml.cs`):

```csharp
protected override async void OnAppearing()
{
	base.OnAppearing();

	// One-time cleanup on first app load
	var dbService = new DatabaseService();
	bool success = await dbService.CleanupUnusedTablesAsync();

	if (success)
	{
		await DisplayAlert("Success", "Database cleanup complete!", "OK");
	}
}
```

Then just run the app once. Done! ✓

---

## ✅ Verify It Worked

After cleanup, run this query to confirm:

```sql
SELECT COUNT(*) as TotalTables
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';
```

**Expected result: 20** (not 23)

---

## Summary

| Method | Time | Skill Level | Steps |
|--------|------|-------------|-------|
| **GUI** | 2 min | Beginner | Right-click 3 times |
| **SQL** | 1 min | Intermediate | Copy, paste, F5 |
| **Script** | 1 min | Intermediate | Open file, F5 |
| **Code** | 1-2 min | Advanced | Add 1 method |

---

## What Gets Deleted

```
Permanent deletion:
❌ dbo.CrmLeads table + all data (10 records)
❌ dbo.SystemIncidents table + all data (5 records)
❌ dbo.ReportArchives table + all data (3 records)

What stays:
✅ All 20 other tables with your real data
✅ Your application code (already updated)
✅ Your business data (Products, Sales, Accounts, etc.)
```

---

## After Cleanup

Your app will:
- ✅ Run normally
- ✅ No references to deleted tables
- ✅ Use only real, live business data
- ✅ Database is clean and lean

---

## 🚀 Ready?

**Pick a method above and execute it. That's it!**

No code changes needed. Just run the cleanup script once.

---

**Got questions? See `DATABASE_CLEANUP_INSTRUCTIONS.md` for details.**
