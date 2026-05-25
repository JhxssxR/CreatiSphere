using System;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using CreatiSphere.Services;
using System.Threading.Tasks;

namespace CreatiSphere.Views.Finance
{
    public partial class FinanceDashboardPage : ContentPage
    {
        private readonly DatabaseService _dbService = new();
        
        public string TotalRevenue { get; set; } = "$0";
        public string NetProfit { get; set; } = "$0";
        public string OutstandingPayouts { get; set; } = "$0";
        public string RevenueChartData { get; set; } = "M0,250 L1200,250"; // Default flat line
        public ObservableCollection<SaleTransaction> Transactions { get; set; } = new();

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
                var stats = await _dbService.GetReportStatsAsync();
                var transactions = await _dbService.GetRecentTransactionsAsync();

                TotalRevenue = stats.TotalRevenue.ToString("C0");
                NetProfit = (stats.TotalRevenue * 0.65m).ToString("C0"); // Simple calculation for profit
                OutstandingPayouts = (stats.TotalRevenue * 0.15m).ToString("C0"); // Simulating payouts

                Transactions.Clear();
                foreach (var tx in transactions)
                {
                    Transactions.Add(tx);
                }

                OnPropertyChanged(nameof(TotalRevenue));
                OnPropertyChanged(nameof(NetProfit));
                OnPropertyChanged(nameof(OutstandingPayouts));

                // Generate dynamic chart path
                var dashboardStats = await _dbService.GetDashboardStatsAsync();
                RevenueChartData = GenerateChartPath(dashboardStats.ActivityHistory);
                OnPropertyChanged(nameof(RevenueChartData));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading finance data: {ex.Message}");
            }
        }

        private string GenerateChartPath(List<double> history)
        {
            if (history == null || history.Count < 2) return "M0,250 L1200,250";
            
            double width = 1200;
            double height = 250;
            double max = 200; // Expected max value
            double stepX = width / (history.Count - 1);
            
            string path = $"M0,{height - (history[0] / max * height)}";
            for (int i = 1; i < history.Count; i++)
            {
                double x = i * stepX;
                double y = height - (history[i] / max * height);
                path += $" L{x},{y}";
            }
            return path;
        }

        private async void OnMonitorRevenueClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FinanceMonitorRevenuePage");
        }

        private async void OnBusinessReportsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FinanceBusinessReportsPage");
        }

        private async void OnSignOutClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
