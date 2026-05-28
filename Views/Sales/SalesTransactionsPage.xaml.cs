using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Sales
{
    public partial class SalesTransactionsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public string PendingApprovalCount { get; set; } = "0";
        public string ActiveNowCount { get; set; } = "0";

        public SalesTransactionsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTransactions();
        }

        private async Task LoadTransactions()
        {
            try
            {
                var allTransactions = await _databaseService.GetRecentTransactionsAsync();
                
                var customOrders = await _databaseService.GetCustomOrdersAsync();
                
                // Calculate dynamic stats exactly from the Custom Orders database table
                int pendingCustomOrders = customOrders.Count(o => o.Status?.Equals("Pending", StringComparison.OrdinalIgnoreCase) == true);
                int ongoingCustomOrders = customOrders.Count(o => 
                    o.Status?.Equals("Processing", StringComparison.OrdinalIgnoreCase) == true || 
                    o.Status?.Equals("In Progress", StringComparison.OrdinalIgnoreCase) == true ||
                    o.Status?.Equals("Ongoing", StringComparison.OrdinalIgnoreCase) == true);

                PendingApprovalCount = pendingCustomOrders.ToString();
                ActiveNowCount = ongoingCustomOrders.ToString();

                OnPropertyChanged(nameof(PendingApprovalCount));
                OnPropertyChanged(nameof(ActiveNowCount));

                // Show only completed and pending
                var filteredTransactions = allTransactions
                    .Where(t => t.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true || 
                                t.Status?.Equals("Pending", StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();

                TransactionsList.ItemsSource = filteredTransactions;
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load transactions: " + ex.Message, "OK");
            }
        }

        private async void OnDashboardClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SalesDashboardPage");

        private async void OnCustomersClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SalesCustomersPage");

        private async void OnPerformanceClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SalesPerformancePage");

        private async void OnCrmClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SalesCrmPage");

        private async void OnReportsClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//SalesReportsPage");

        private async void OnLogoutClicked(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//MainPage");
    }
}
