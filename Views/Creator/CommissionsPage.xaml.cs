using CreatiSphere.Services;
using System.Threading.Tasks;

namespace CreatiSphere.Views.Creator
{
    public partial class CommissionsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public CommissionsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCommissions();
        }

        private async Task LoadCommissions()
        {
            try
            {
                var commissions = await _databaseService.GetCustomOrdersAsync();
                BindableLayout.SetItemsSource(CommissionsListContainer, commissions);
            }
            catch (Exception ex)
            {
                await this.DisplayAlertAsync("Error", "Could not load commissions: " + ex.Message, "OK");
            }
        }

        private async void OnDashboardTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatorDashboardPage");
        }

        private async void OnAssetLibraryTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AssetLibraryPage");
        }

        private async void OnCatalogManagerTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CatalogManagerPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void OnUpdateStatusClicked(object? sender, TappedEventArgs e)
        {
            var order = e.Parameter as CustomOrder ?? (sender as View)?.BindingContext as CustomOrder;
            if (order == null) return;

            string[] allStatuses = { "Briefing", "Concept", "Refining", "Delivery" };
            int currentIndex = Array.IndexOf(allStatuses, order.Status ?? "");
            
            var availableStatuses = new System.Collections.Generic.List<string>();
            for (int i = currentIndex + 1; i < allStatuses.Length; i++)
            {
                availableStatuses.Add(allStatuses[i]);
            }

            if (availableStatuses.Count == 0)
            {
                await this.DisplayAlertAsync("Update Status", "Order is already at the final stage.", "OK");
                return;
            }

            string action = await this.DisplayActionSheetAsync($"Update Status for {order.Title}", "Cancel", null, 
                availableStatuses.ToArray());

            if (string.IsNullOrEmpty(action) || action == "Cancel") return;

            bool success = await _databaseService.UpdateCustomOrderStatusAsync(order.OrderID ?? "", action);
            if (success)
            {
                await this.DisplayAlertAsync("Success", $"Status updated to {action}", "OK");
                await LoadCommissions();
            }
            else
            {
                await this.DisplayAlertAsync("Error", "Failed to update status in database.", "OK");
            }
        }
    }
}
