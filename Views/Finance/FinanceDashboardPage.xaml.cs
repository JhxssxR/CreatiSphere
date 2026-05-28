using System;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using CreatiSphere.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CreatiSphere.Views.Finance
{
    public class FinanceChartDataPoint
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public double BarHeight { get; set; }
        public string RevenueFormatted => $"₱{Revenue:N0}";
    }

    public partial class FinanceDashboardPage : ContentPage
    {
        private readonly DatabaseService _dbService = new();
        private List<SaleTransaction> _allTransactions = new();
        private ReportStats? _lastStats;
        private string _currentFilter = "Last 12 Months";

        public string TotalRevenue { get; set; } = "₱0";
        public string NetProfit { get; set; } = "₱0";
        public string OutstandingPayouts { get; set; } = "₱0";

        public ObservableCollection<SaleTransaction> Transactions { get; set; } = new();
        public ObservableCollection<FinanceChartDataPoint> ChartData { get; set; } = new();

        public FinanceDashboardPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDashboardData();
        }

        private async Task LoadDashboardData()
        {
            try
            {
                _lastStats = await _dbService.GetReportStatsAsync();
                _allTransactions = await _dbService.GetRecentTransactionsAsync();

                TotalRevenue = $"₱{_lastStats.TotalRevenue:N0}";
                NetProfit = $"₱{(_lastStats.TotalRevenue * 0.65m):N0}";
                OutstandingPayouts = $"₱{(_lastStats.TotalRevenue * 0.15m):N0}";

                OnPropertyChanged(nameof(TotalRevenue));
                OnPropertyChanged(nameof(NetProfit));
                OnPropertyChanged(nameof(OutstandingPayouts));

                ApplyFilter();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading finance data: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            Transactions.Clear();
            List<SaleTransaction> filteredTx;

            if (_currentFilter == "Last 30 Days")
                filteredTx = _allTransactions.Where(t => DateTime.TryParse(t.Date, out var d) && (DateTime.Now - d).TotalDays <= 30).ToList();
            else if (_currentFilter == "Last 6 Months")
                filteredTx = _allTransactions.Where(t => DateTime.TryParse(t.Date, out var d) && (DateTime.Now - d).TotalDays <= 180).ToList();
            else
                filteredTx = _allTransactions;

            foreach (var tx in filteredTx.Take(10))
                Transactions.Add(tx);

            ChartData.Clear();
            int monthsCount = _currentFilter switch { "Last 30 Days" => 4, "Last 6 Months" => 6, _ => 12 };
            var now = DateTime.Now;
            var points = new List<FinanceChartDataPoint>();
            decimal baseRevenue = _allTransactions.Count > 0 ? _allTransactions.Sum(t => t.Amount) : 22000m;

            for (int i = monthsCount - 1; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);
                string label = _currentFilter == "Last 30 Days" ? $"Wk {4 - i}" : monthDate.ToString("MMM");
                decimal multiplier = 0.5m + (decimal)Math.Sin(i * 1.5) * 0.25m + (i == 0 ? 0.3m : 0.1m);
                decimal monthRevenue = Math.Round(baseRevenue * multiplier / 3) * 3;
                if (monthRevenue < 5000) monthRevenue = 8400;
                points.Add(new FinanceChartDataPoint { Month = label.ToUpper(), Revenue = monthRevenue });
            }

            decimal maxRevenue = points.Max(p => p.Revenue);
            if (maxRevenue == 0) maxRevenue = 10000;
            foreach (var pt in points)
            {
                pt.BarHeight = Math.Max(15.0, (double)(pt.Revenue / maxRevenue) * 180.0);
                ChartData.Add(pt);
            }
        }

        private async void OnExportCsvClicked(object? sender, EventArgs e)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"CreatiSphere_Finance_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string csvPath = Path.Combine(desktopPath, fileName);

                var sb = new StringBuilder();
                sb.AppendLine("CreatiSphere Finance Dashboard Export");
                sb.AppendLine($"Generated: {DateTime.Now:MMMM dd, yyyy HH:mm}");
                sb.AppendLine($"Filter: {_currentFilter}");
                sb.AppendLine();
                sb.AppendLine("=== SUMMARY ===");
                sb.AppendLine($"Total Revenue,{TotalRevenue}");
                sb.AppendLine($"Net Profit,{NetProfit}");
                sb.AppendLine($"Outstanding Payouts,{OutstandingPayouts}");
                sb.AppendLine();
                sb.AppendLine("=== TRANSACTIONS ===");
                sb.AppendLine("Transaction ID,Customer Name,Customer Email,Date,Amount (PHP),Status");
                foreach (var tx in _allTransactions)
                    sb.AppendLine($"\"{tx.TransactionID}\",\"{tx.CustomerName}\",\"{tx.CustomerEmail}\",\"{tx.Date}\",\"{tx.Amount:N2}\",\"{tx.Status}\"");

                await File.WriteAllTextAsync(csvPath, sb.ToString(), Encoding.UTF8);

                bool open = await DisplayAlertAsync("✅ CSV Exported Successfully",
                    $"File saved to Desktop:\n{fileName}\n\nWould you like to open it now?",
                    "Open File", "Close");

                if (open)
                    Process.Start(new ProcessStartInfo(csvPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Export Failed", $"Could not export data: {ex.Message}", "OK");
            }
        }

        private async void OnFilterClicked(object? sender, EventArgs e)
        {
            string action = await DisplayActionSheetAsync("Select Time Frame", "Cancel", null, "Last 12 Months", "Last 6 Months", "Last 30 Days");
            if (action != "Cancel" && !string.IsNullOrEmpty(action))
            {
                _currentFilter = action;
                FilterLabel.Text = $"{action} ▾";
                ApplyFilter();
            }
        }

        private async void OnMonitorRevenueClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//FinanceMonitorRevenuePage");

        private async void OnBusinessReportsClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//FinanceBusinessReportsPage");

        private async void OnSignOutClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//MainPage");
    }
}
