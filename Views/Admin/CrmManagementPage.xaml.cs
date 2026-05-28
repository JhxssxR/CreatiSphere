using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Linq;
using System.Collections.Generic;
namespace CreatiSphere.Views.Admin
{
    public class CrmLead
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public partial class CrmManagementPage : ContentPage
    {
        private readonly Services.DatabaseService _dbService;

        public CrmManagementPage()
        {
            InitializeComponent();
            _dbService = new Services.DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            HeaderProfileInitials.Text = !string.IsNullOrEmpty(UserSession.Username) ? UserSession.Username.Substring(0, 1).ToUpper() : "U";
            await LoadStats();
        }

        private async Task LoadStats()
        {
            try
            {
                var msmes = await _dbService.GetMsmesAsync();
                var leads = msmes.Select(m => new CrmLead
                {
                    Name = m.BusinessName ?? "Unknown",
                    Email = m.OwnerName ?? "No Owner",
                    Status = m.Status ?? "ACTIVE",
                    Value = m.ErpTier == "Enterprise Plus" ? 9999m : (m.ErpTier == "Standard" ? 2499m : 0m)
                }).ToList();

                BindableLayout.SetItemsSource(LeadsList, leads);
                string customerWord = leads.Count == 1 ? "customer" : "customers";
                PaginationLabel.Text = $"Showing {leads.Count} of {leads.Count} {customerWord}";

                // Update stat cards from real MSME data
                int total = leads.Count;
                int active = leads.Count(l => l.Status?.ToUpper() == "ACTIVE");
                decimal totalSpend = leads.Sum(l => l.Value);

                TotalCustomersLabel.Text = total.ToString("N0");
                ActiveLeadsLabel.Text = active.ToString("N0");
                TotalPortfolioSpendLabel.Text = totalSpend >= 1000000
                    ? "₱" + (totalSpend / 1000000m).ToString("F1") + "M"
                    : "₱" + (totalSpend / 1000m).ToString("F1") + "K";

                // Update right profile panel with first MSME
                if (msmes.Count > 0)
                {
                    var first = msmes[0];
                    string bname = first.BusinessName ?? "MSME";
                    ProfileInitialsLabel.Text = bname.Length >= 2
                        ? bname.Substring(0, 2).ToUpper()
                        : bname.Substring(0, 1).ToUpper();
                    ProfileNameLabel.Text = bname;
                    ProfileLocationLabel.Text = first.Niche ?? "Creative Business";
                    ProfileContactLabel.Text = first.OwnerName ?? "Owner";
                    ProfileRoleLabel.Text = (first.ErpTier ?? "Starter") + " Plan";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stats: {ex.Message}");
            }
        }

        private async void OnFilterClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Filter", "CRM filtering options will be available soon.", "OK");
        }

        private async void OnAddCustomerClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Add Customer", "New customer registration form opened.", "OK");
        }

        private async void OnDashboardClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//AdminDashboard", false);
        }

        private async void OnManageInventoryClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageInventoryPage", false);
        }

        private async void OnDigitalAssetManagementClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//DigitalAssetManagementPage", false);
        }

        private async void OnManageProductsClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageProductsPage", false);
        }

        private async void OnManageUsersClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ManageUsersPage", false);
        }

        private async void OnCrmManagementClicked(object? sender, TappedEventArgs e)
        {
            // Already on this page
        }

        private async void OnCustomOrderManagementClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomOrderManagementPage", false);
        }

        private async void OnViewSalesClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ViewSalesPage", false);
        }

        private async void OnBusinessReportsClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//BusinessReportsPage", false);
        }

        private async void OnSignOutClicked(object? sender, TappedEventArgs e)
        {
            bool answer = await DisplayAlertAsync("Logout", "Are you sure you want to logout?", "Yes", "No");
            if (answer)
            {
                await Shell.Current.GoToAsync("//MainPage", false);
            }
        }
    }
}

