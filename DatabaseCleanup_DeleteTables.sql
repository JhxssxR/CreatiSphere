-- CreatiSphere Database Cleanup Script
-- PERMANENTLY REMOVES unused tables and their data
-- Tables being deleted: CrmLeads, SystemIncidents, ReportArchives

USE CreatiSphere;
GO

SET NOCOUNT ON;

PRINT '========================================';
PRINT 'CreatiSphere Database Cleanup Process';
PRINT '========================================';
PRINT '';

-- Step 1: Drop CrmLeads table and all its data
IF OBJECT_ID('dbo.CrmLeads', 'U') IS NOT NULL 
BEGIN
	PRINT 'Dropping table [dbo.CrmLeads] and all data...';
	DROP TABLE dbo.CrmLeads;
	PRINT '✓ Table [dbo.CrmLeads] deleted successfully.';
END
ELSE
BEGIN
	PRINT '✓ Table [dbo.CrmLeads] does not exist.';
END
GO

-- Step 2: Drop SystemIncidents table and all its data
IF OBJECT_ID('dbo.SystemIncidents', 'U') IS NOT NULL 
BEGIN
	PRINT 'Dropping table [dbo.SystemIncidents] and all data...';
	DROP TABLE dbo.SystemIncidents;
	PRINT '✓ Table [dbo.SystemIncidents] deleted successfully.';
END
ELSE
BEGIN
	PRINT '✓ Table [dbo.SystemIncidents] does not exist.';
END
GO

-- Step 3: Drop ReportArchives table and all its data
IF OBJECT_ID('dbo.ReportArchives', 'U') IS NOT NULL 
BEGIN
	PRINT 'Dropping table [dbo.ReportArchives] and all data...';
	DROP TABLE dbo.ReportArchives;
	PRINT '✓ Table [dbo.ReportArchives] deleted successfully.';
END
ELSE
BEGIN
	PRINT '✓ Table [dbo.ReportArchives] does not exist.';
END
GO

-- Step 4: Verify remaining tables
PRINT '';
PRINT '========================================';
PRINT 'Remaining Tables in CreatiSphere';
PRINT '========================================';
PRINT '';

SELECT 
	TABLE_SCHEMA,
	TABLE_NAME,
	(SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) AS ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

PRINT '';
PRINT '========================================';
PRINT 'Cleanup Process Completed Successfully!';
PRINT '========================================';
PRINT '';
PRINT 'Removed unused tables:';
PRINT '  ✓ dbo.CrmLeads (with all data)';
PRINT '  ✓ dbo.SystemIncidents (with all data)';
PRINT '  ✓ dbo.ReportArchives (with all data)';
PRINT '';
PRINT 'Your database now contains only use-case-aligned tables.';
