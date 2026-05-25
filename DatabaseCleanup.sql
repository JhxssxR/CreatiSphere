-- CreatiSphere Database Cleanup Script
-- Removes tables and data that are outside the current use case scope
-- Tables to be dropped: CrmLeads, SystemIncidents, ReportArchives
-- 
-- USE CASE SCOPE:
-- KEEP: Products, Inventory, SalesTransactions, Rewards, RewardCatalog, RewardHistory, CustomOrders, 
--       SystemModules, Subscriptions, Accounts, Msmes, CreatorAssets
-- REMOVE: CrmLeads, SystemIncidents, ReportArchives

USE CreatiSphere;
GO

PRINT '========================================';
PRINT 'CreatiSphere Database Cleanup Process';
PRINT '========================================';
PRINT '';

-- Drop CrmLeads table if it exists
IF OBJECT_ID('dbo.CrmLeads', 'U') IS NOT NULL 
BEGIN
	PRINT 'Dropping table [CrmLeads]...';
	DROP TABLE dbo.CrmLeads;
	PRINT 'SUCCESS: Table [CrmLeads] dropped.';
END
ELSE
BEGIN
	PRINT 'INFO: Table [CrmLeads] does not exist.';
END
GO

-- Drop SystemIncidents table if it exists
IF OBJECT_ID('dbo.SystemIncidents', 'U') IS NOT NULL 
BEGIN
	PRINT 'Dropping table [SystemIncidents]...';
	DROP TABLE dbo.SystemIncidents;
	PRINT 'SUCCESS: Table [SystemIncidents] dropped.';
END
ELSE
BEGIN
	PRINT 'INFO: Table [SystemIncidents] does not exist.';
END
GO

-- Drop ReportArchives table if it exists
IF OBJECT_ID('dbo.ReportArchives', 'U') IS NOT NULL 
BEGIN
	PRINT 'Dropping table [ReportArchives]...';
	DROP TABLE dbo.ReportArchives;
	PRINT 'SUCCESS: Table [ReportArchives] dropped.';
END
ELSE
BEGIN
	PRINT 'INFO: Table [ReportArchives] does not exist.';
END
GO

-- Verify remaining tables
PRINT '';
PRINT '========================================';
PRINT 'Remaining tables in CreatiSphere';
PRINT '========================================';
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' 
ORDER BY TABLE_NAME;
GO

PRINT '';
PRINT 'Cleanup completed successfully!';
PRINT 'The application will now only initialize tables for the use-case features.';
