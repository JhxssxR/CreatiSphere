using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Finance
{
    public partial class BusinessReportsPage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private ReportStats? _lastStats;
        private List<SaleTransaction> _allTransactions = new();
        private List<ChartDataPoint> _chartData = new();

        public BusinessReportsPage()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            HeaderProfileInitials.Text = !string.IsNullOrEmpty(UserSession.Username)
                ? UserSession.Username.Substring(0, 1).ToUpper() : "F";
            await LoadReportStats();
            await LoadChartData();
        }

        private async Task LoadReportStats()
        {
            try
            {
                _lastStats = await _dbService.GetReportStatsAsync();
                _allTransactions = await _dbService.GetRecentTransactionsAsync();

                TotalRevenueLabel.Text = $"₱{_lastStats.TotalRevenue:N2}";
                NewCreatorsLabel.Text = _lastStats.NewCreators.ToString("N0");
                TotalOrdersLabel.Text = _lastStats.TotalOrders.ToString("N0");
                ConversionRateLabel.Text = _lastStats.ConversionRate.ToString("F2") + "%";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadReportStats error: {ex.Message}");
            }
        }

        private async Task LoadChartData()
        {
            try
            {
                _chartData = await _dbService.GetWeeklyRevenueGrowthAsync();
                BindableLayout.SetItemsSource(ChartFlexLayout, _chartData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadChartData error: {ex.Message}");
            }
        }

        private async void OnExportClicked(object? sender, EventArgs e)
        {
            string? format = await DisplayActionSheetAsync(
                "Export Business Report",
                "Cancel",
                null,
                "Export as CSV (Spreadsheet)",
                "Export as Report (Text File)");

            if (format == null || format == "Cancel") return;

            if (format == "Export as CSV (Spreadsheet)")
                await ExportAsCsvAsync();
            else
                await ExportAsTextReportAsync();
        }

        private async Task ExportAsCsvAsync()
        {
            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"CreatiSphere_BusinessReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string filePath = Path.Combine(desktop, fileName);

                var sb = new StringBuilder();

                // Report header
                sb.AppendLine("CreatiSphere Business Analytics Report");
                sb.AppendLine($"Generated:,{DateTime.Now:MMMM dd, yyyy HH:mm}");
                sb.AppendLine($"Exported by:,{UserSession.Username ?? "Finance Staff"}");
                sb.AppendLine();

                // Summary section
                sb.AppendLine("=== SUMMARY METRICS ===");
                sb.AppendLine("Metric,Value");
                sb.AppendLine($"Total Revenue,\"{(_lastStats != null ? $"₱{_lastStats.TotalRevenue:N2}" : "N/A")}\"");
                sb.AppendLine($"New Creators,\"{_lastStats?.NewCreators:N0}\"");
                sb.AppendLine($"Total Orders,\"{_lastStats?.TotalOrders:N0}\"");
                sb.AppendLine($"Conversion Rate,\"{_lastStats?.ConversionRate:F2}%\"");
                sb.AppendLine();

                // Weekly revenue growth
                sb.AppendLine("=== WEEKLY REVENUE GROWTH ===");
                sb.AppendLine("Day,Last Week (PHP),This Week (PHP),Growth");
                foreach (var pt in _chartData)
                {
                    decimal growth = pt.LastWeekRevenue > 0
                        ? Math.Round(((pt.ThisWeekRevenue - pt.LastWeekRevenue) / pt.LastWeekRevenue) * 100, 1)
                        : 0;
                    string growthStr = growth >= 0 ? $"+{growth}%" : $"{growth}%";
                    sb.AppendLine($"\"{pt.Day}\",\"{pt.LastWeekRevenue:N2}\",\"{pt.ThisWeekRevenue:N2}\",\"{growthStr}\"");
                }
                sb.AppendLine();

                // Transactions
                sb.AppendLine("=== ALL TRANSACTIONS ===");
                sb.AppendLine("Transaction ID,Customer Name,Customer Email,Date,Amount (PHP),Status");
                foreach (var tx in _allTransactions)
                    sb.AppendLine($"\"{tx.TransactionID}\",\"{tx.CustomerName}\",\"{tx.CustomerEmail}\",\"{tx.Date}\",\"{tx.Amount:N2}\",\"{tx.Status}\"");

                await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);

                bool open = await DisplayAlertAsync("✅ CSV Exported",
                    $"Saved to Desktop:\n{fileName}\n\nOpen it now?",
                    "Open File", "Close");
                if (open)
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Export Failed", $"Could not export CSV: {ex.Message}", "OK");
            }
        }

        private async Task ExportAsTextReportAsync()
        {
            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"CreatiSphere_BusinessReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string filePath = Path.Combine(desktop, fileName);

                var sb = new StringBuilder();
                string line = new string('=', 65);
                string dash = new string('-', 65);

                sb.AppendLine(line);
                sb.AppendLine("           CREATISPHERE – BUSINESS ANALYTICS REPORT");
                sb.AppendLine(line);
                sb.AppendLine($"  Generated : {DateTime.Now:MMMM dd, yyyy  HH:mm}");
                sb.AppendLine($"  Prepared by: {UserSession.Username ?? "Finance Staff"} – Finance Portal");
                sb.AppendLine($"  Report Period: {DateTime.Now.AddMonths(-1):MMMM yyyy} – {DateTime.Now:MMMM yyyy}");
                sb.AppendLine(line);
                sb.AppendLine();

                // --- SUMMARY ---
                sb.AppendLine("  EXECUTIVE SUMMARY");
                sb.AppendLine(dash);
                sb.AppendLine();
                if (_lastStats != null)
                {
                    sb.AppendLine($"  {"Total Revenue",-30} ₱{_lastStats.TotalRevenue,12:N2}");
                    sb.AppendLine($"  {"Net Profit (65%)",-30} ₱{(_lastStats.TotalRevenue * 0.65m),12:N2}");
                    sb.AppendLine($"  {"Outstanding Payouts (15%)",-30} ₱{(_lastStats.TotalRevenue * 0.15m),12:N2}");
                    sb.AppendLine($"  {"New Creators (Last 30 Days)",-30} {_lastStats.NewCreators,13:N0}");
                    sb.AppendLine($"  {"Total Orders",-30} {_lastStats.TotalOrders,13:N0}");
                    sb.AppendLine($"  {"Conversion Rate",-30} {_lastStats.ConversionRate,12:F2}%");
                }
                sb.AppendLine();

                // --- WEEKLY GROWTH ---
                sb.AppendLine(dash);
                sb.AppendLine("  WEEKLY REVENUE GROWTH");
                sb.AppendLine(dash);
                sb.AppendLine();
                sb.AppendLine($"  {"Day",-10} {"Last Week",15} {"This Week",15} {"Change",10}");
                sb.AppendLine($"  {new string('-', 52)}");
                foreach (var pt in _chartData)
                {
                    decimal growth = pt.LastWeekRevenue > 0
                        ? Math.Round(((pt.ThisWeekRevenue - pt.LastWeekRevenue) / pt.LastWeekRevenue) * 100, 1)
                        : 0;
                    string growthStr = growth >= 0 ? $"▲ {growth}%" : $"▼ {Math.Abs(growth)}%";
                    sb.AppendLine($"  {pt.Day,-10} ₱{pt.LastWeekRevenue,13:N2} ₱{pt.ThisWeekRevenue,13:N2} {growthStr,10}");
                }
                sb.AppendLine();

                // --- TRANSACTIONS ---
                sb.AppendLine(dash);
                sb.AppendLine("  RECENT TRANSACTIONS");
                sb.AppendLine(dash);
                sb.AppendLine();
                sb.AppendLine($"  {"Transaction ID",-14} {"Customer",-22} {"Date",-14} {"Amount",12} {"Status"}");
                sb.AppendLine($"  {new string('-', 68)}");
                foreach (var tx in _allTransactions.Take(20))
                    sb.AppendLine($"  {tx.TransactionID,-14} {tx.CustomerName,-22} {tx.Date,-14} ₱{tx.Amount,10:N2} {tx.Status}");

                sb.AppendLine();
                sb.AppendLine(line);
                sb.AppendLine("  This report is generated automatically by CreatiSphere ERP.");
                sb.AppendLine($"  Confidential — For internal use only.");
                sb.AppendLine(line);

                await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);

                bool open = await DisplayAlertAsync("✅ Report Exported",
                    $"Saved to Desktop:\n{fileName}\n\nOpen it now?",
                    "Open File", "Close");
                if (open)
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Export Failed", $"Could not generate report: {ex.Message}", "OK");
            }
        }

        private async void OnRefreshDataClicked(object? sender, EventArgs e)
        {
            await LoadReportStats();
            await LoadChartData();
            await DisplayAlertAsync("✅ Refreshed", "Analytics data has been updated.", "OK");
        }

        private async void OnDashboardClicked(object? sender, TappedEventArgs e)
            => await Shell.Current.GoToAsync("//FinanceDashboardPage", false);

        private async void OnMonitorRevenueClicked(object? sender, TappedEventArgs e)
            => await Shell.Current.GoToAsync("//FinanceMonitorRevenuePage", false);

        private async void OnSignOutClicked(object? sender, TappedEventArgs e)
        {
            bool confirm = await DisplayAlertAsync("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");
            if (confirm)
                await Shell.Current.GoToAsync("//MainPage", false);
        }
    }
}
