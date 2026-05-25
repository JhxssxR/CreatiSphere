using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Sales
{
    public partial class SalesReportsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private Label? _totalRevenueLabel;
        private Label? _newLeadsLabel;
        private Label? _conversionRateLabel;
        private Label? _avgDealSizeLabel;

        public SalesReportsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            BindUiReferences();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadReportMetrics();
        }

        private async Task LoadReportMetrics()
        {
            try
            {
                var stats = await _databaseService.GetReportStatsAsync();
                if (_totalRevenueLabel != null)
                {
                    _totalRevenueLabel.Text = stats.TotalRevenue.ToString("C0");
                }

                if (_newLeadsLabel != null)
                {
                    _newLeadsLabel.Text = stats.ActiveLeads.ToString("N0");
                }

                if (_conversionRateLabel != null)
                {
                    _conversionRateLabel.Text = stats.ConversionRate.ToString("F1") + "%";
                }

                if (_avgDealSizeLabel != null)
                {
                    _avgDealSizeLabel.Text = (stats.TotalOrders == 0 ? 0 : stats.TotalRevenue / stats.TotalOrders).ToString("C0");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading sales reports: " + ex.Message);
            }
        }

        private void BindUiReferences()
        {
            _totalRevenueLabel = (Label)FindByName("TotalRevenueLabel");
            _newLeadsLabel = (Label)FindByName("NewLeadsLabel");
            _conversionRateLabel = (Label)FindByName("ConversionRateLabel");
            _avgDealSizeLabel = (Label)FindByName("AvgDealSizeLabel");
        }

        private async void OnDashboardClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesDashboardPage");
        }

        private async void OnCustomersClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesCustomersPage");
        }

        private async void OnTransactionsClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesTransactionsPage");
        }

        private async void OnPerformanceClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesPerformancePage");
        }

        private async void OnCrmClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SalesCrmPage");
        }
        private async void OnLogoutClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}


