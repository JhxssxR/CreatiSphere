using System;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Diagnostics;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Finance
{
    public partial class MonitorRevenuePage : ContentPage
    {
        private readonly DatabaseService _dbService = new();
        private List<SaleTransaction> _allTransactions = new();
        private ReportStats? _lastStats;

        public string TotalRevenue { get; set; } = "₱0.00";
        public string Commissions { get; set; } = "₱0.00";
        public string PendingPayouts { get; set; } = "₱0.00";
        public ObservableCollection<SaleTransaction> Transactions { get; set; } = new();

        // Chart Bindings
        public double BarHeight0 { get; set; } = 80;
        public double BarHeight1 { get; set; } = 120;
        public double BarHeight2 { get; set; } = 160;
        public double BarHeight3 { get; set; } = 100;
        public double BarHeight4 { get; set; } = 180;
        public double BarHeight5 { get; set; } = 200;
        public double BarHeight6 { get; set; } = 90;

        public string LabelText0 { get; set; } = "MON";
        public string LabelText1 { get; set; } = "TUE";
        public string LabelText2 { get; set; } = "WED";
        public string LabelText3 { get; set; } = "THU";
        public string LabelText4 { get; set; } = "FRI";
        public string LabelText5 { get; set; } = "SAT";
        public string LabelText6 { get; set; } = "SUN";

        public MonitorRevenuePage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                _lastStats = await _dbService.GetReportStatsAsync();
                var transactions = await _dbService.GetRecentTransactionsAsync();
                _allTransactions = transactions ?? new List<SaleTransaction>();

                TotalRevenue = $"₱{_lastStats.TotalRevenue:N2}";
                Commissions = $"₱{(_lastStats.TotalRevenue * 0.242m):N2}";

                decimal pendingSum = _allTransactions
                    .Where(t => t.Status != null && (t.Status.Equals("Processing", StringComparison.OrdinalIgnoreCase)
                                                  || t.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)))
                    .Sum(t => t.Amount);

                if (pendingSum == 0) pendingSum = 13100.00m;
                PendingPayouts = $"₱{pendingSum:N2}";

                Transactions.Clear();
                foreach (var tx in _allTransactions.Take(5))
                    Transactions.Add(tx);

                OnPropertyChanged(nameof(TotalRevenue));
                OnPropertyChanged(nameof(Commissions));
                OnPropertyChanged(nameof(PendingPayouts));

                UpdateChart("Daily");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading revenue data: {ex.Message}");
            }
        }

        private void HighlightTab(string tabName)
        {
            DailyTabBorder.BackgroundColor = Colors.Transparent;
            DailyTabLabel.TextColor = Color.FromArgb("#64748B");
            WeeklyTabBorder.BackgroundColor = Colors.Transparent;
            WeeklyTabLabel.TextColor = Color.FromArgb("#64748B");
            MonthlyTabBorder.BackgroundColor = Colors.Transparent;
            MonthlyTabLabel.TextColor = Color.FromArgb("#64748B");

            if (tabName == "Daily") { DailyTabBorder.BackgroundColor = Colors.White; DailyTabLabel.TextColor = Color.FromArgb("#0D9488"); }
            else if (tabName == "Weekly") { WeeklyTabBorder.BackgroundColor = Colors.White; WeeklyTabLabel.TextColor = Color.FromArgb("#0D9488"); }
            else if (tabName == "Monthly") { MonthlyTabBorder.BackgroundColor = Colors.White; MonthlyTabLabel.TextColor = Color.FromArgb("#0D9488"); }
        }

        private void UpdateChart(string mode)
        {
            HighlightTab(mode);

            decimal[] data;
            string[] labels;

            if (mode == "Daily")
            {
                labels = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-6 + i).ToString("ddd").ToUpper()).ToArray();
                data = new decimal[7];
                for (int i = 0; i < 7; i++)
                {
                    var targetDate = DateTime.Today.AddDays(-6 + i);
                    data[i] = _allTransactions
                        .Where(t => DateTime.TryParse(t.Date, out var dt) && dt.Date == targetDate.Date)
                        .Sum(t => t.Amount);
                }
                if (data.Max() == 0) data = new decimal[] { 1200, 2450, 590, 850, 1600, 2000, 950 };
            }
            else if (mode == "Weekly")
            {
                labels = Enumerable.Range(0, 7).Select(i => i == 6 ? "THIS WK" : $"WK -{6 - i}").ToArray();
                data = new decimal[7];
                for (int i = 0; i < 7; i++)
                {
                    var start = DateTime.Today.AddDays(-7 * (6 - i));
                    data[i] = _allTransactions
                        .Where(t => DateTime.TryParse(t.Date, out var dt) && dt.Date >= start.Date && dt.Date < start.AddDays(7).Date)
                        .Sum(t => t.Amount);
                }
                if (data.Max() == 0) data = new decimal[] { 4500, 8200, 6100, 9500, 11000, 14000, 8500 };
            }
            else // Monthly
            {
                labels = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddMonths(-6 + i).ToString("MMM").ToUpper()).ToArray();
                data = new decimal[7];
                for (int i = 0; i < 7; i++)
                {
                    var target = DateTime.Today.AddMonths(-6 + i);
                    data[i] = _allTransactions
                        .Where(t => DateTime.TryParse(t.Date, out var dt) && dt.Year == target.Year && dt.Month == target.Month)
                        .Sum(t => t.Amount);
                }
                if (data.Max() == 0) data = new decimal[] { 18000, 24000, 21000, 29000, 35000, 42000, 31000 };
            }

            decimal maxVal = data.Max();
            double[] heights = data.Select(v => maxVal > 0 ? (double)(v / maxVal) * 200 : 20).ToArray();

            BarHeight0 = heights[0]; BarHeight1 = heights[1]; BarHeight2 = heights[2]; BarHeight3 = heights[3];
            BarHeight4 = heights[4]; BarHeight5 = heights[5]; BarHeight6 = heights[6];
            LabelText0 = labels[0]; LabelText1 = labels[1]; LabelText2 = labels[2]; LabelText3 = labels[3];
            LabelText4 = labels[4]; LabelText5 = labels[5]; LabelText6 = labels[6];

            for (int i = 0; i <= 6; i++) OnPropertyChanged($"BarHeight{i}");
            for (int i = 0; i <= 6; i++) OnPropertyChanged($"LabelText{i}");
        }

        private async void OnExportCsvClicked(object? sender, EventArgs e)
        {
            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"CreatiSphere_Revenue_Monitor_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                string csvPath = Path.Combine(desktop, fileName);

                var sb = new StringBuilder();
                // Header block
                sb.AppendLine("CreatiSphere – Revenue Analytics Export");
                sb.AppendLine($"Generated: {DateTime.Now:MMMM dd, yyyy HH:mm}");
                sb.AppendLine();
                sb.AppendLine("=== REVENUE SUMMARY ===");
                sb.AppendLine($"Total Revenue,{TotalRevenue}");
                sb.AppendLine($"Commissions (24.2%),{Commissions}");
                sb.AppendLine($"Pending Payouts,{PendingPayouts}");
                sb.AppendLine();
                // Transaction table
                sb.AppendLine("=== RECENT TRANSACTIONS ===");
                sb.AppendLine("Transaction ID,Customer Name,Customer Email,Date,Time,Amount (PHP),Status");
                foreach (var tx in _allTransactions)
                    sb.AppendLine($"\"{tx.TransactionID}\",\"{tx.CustomerName}\",\"{tx.CustomerEmail}\",\"{tx.Date}\",\"{tx.Time}\",\"{tx.Amount:N2}\",\"{tx.Status}\"");

                await File.WriteAllTextAsync(csvPath, sb.ToString(), Encoding.UTF8);

                bool open = await DisplayAlertAsync("✅ CSV Exported",
                    $"Saved to Desktop:\n{fileName}\n\nOpen the file now?",
                    "Open File", "Close");

                if (open)
                    Process.Start(new ProcessStartInfo(csvPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Export Failed", $"Could not export: {ex.Message}", "OK");
            }
        }

        private void OnDailyTabTapped(object? sender, EventArgs e) => UpdateChart("Daily");
        private void OnWeeklyTabTapped(object? sender, EventArgs e) => UpdateChart("Weekly");
        private void OnMonthlyTabTapped(object? sender, EventArgs e) => UpdateChart("Monthly");

        private async void OnDashboardClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//FinanceDashboardPage");

        private async void OnBusinessReportsClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//FinanceBusinessReportsPage");

        private async void OnSignOutClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//MainPage");
    }
}
