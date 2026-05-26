using Microsoft.Maui.Controls;
using System.Linq;

namespace CreatiSphere.Views.Admin
{
    public partial class AdminDashboardPage : ContentPage
    {
        private readonly Services.DatabaseService _dbService;

        public AdminDashboardPage()
        {
            InitializeComponent();
            _dbService = new Services.DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDashboardData();
            ApplyCurrentPlanUI();
        }

        private void ApplyCurrentPlanUI()
        {
            string currentPlan = Services.UserSession.Tier ?? "Starter";
            int limit = Services.UserSession.MaxUsers;
            
            // Update the tier and capacity labels added to the MSME stat card
            if (TierLimitLabel != null)
                TierLimitLabel.Text = $"{currentPlan} (Limit: {limit})";
            
            // Calculate progress based on active count vs limit
            int activeUsers = 1; // Default fallback
            if (ActiveCreatorsLabel != null && int.TryParse(ActiveCreatorsLabel.Text, System.Globalization.NumberStyles.AllowThousands, null, out int parsed))
                activeUsers = parsed;
            
            double progress = (double)activeUsers / limit;
            if (CapacityProgress != null)
                CapacityProgress.Progress = Math.Min(progress, 1.0);
            
            if (CapacityPercentLabel != null)
                CapacityPercentLabel.Text = $"{(int)(Math.Min(progress, 1.0) * 100)}%";

            // Reset all badges and buttons to default
            StarterCurrentBadge.IsVisible  = false;
            StandardCurrentBadge.IsVisible  = false;
            EnterpriseCurrentBadge.IsVisible = false;

            if (StandardSubscribeBtn  != null) StandardSubscribeBtn.IsVisible  = true;
            if (EnterpriseSubscribeBtn != null) EnterpriseSubscribeBtn.IsVisible = true;

            switch (currentPlan)
            {
                case "Standard":
                    StandardCurrentBadge.IsVisible = true;
                    if (StandardSubscribeBtn != null) StandardSubscribeBtn.IsVisible = false;
                    break;
                case "Enterprise Plus":
                    EnterpriseCurrentBadge.IsVisible = true;
                    if (EnterpriseSubscribeBtn != null) EnterpriseSubscribeBtn.IsVisible = false;
                    break;
                default: // Starter
                    StarterCurrentBadge.IsVisible = true;
                    break;
            }
        }

        private async void OnViewSalesClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ViewSalesPage", false);
        }

        private async void OnSignOutClicked(object? sender, TappedEventArgs e)
        {
            bool confirm = await DisplayAlertAsync("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//MainPage", false);
            }
        }

        private async Task LoadDashboardData()
        {
            try
            {
                // Fetch Stats
                var stats = await _dbService.GetDashboardStatsAsync();
                var orders = await _dbService.GetCustomOrdersAsync();
                var assets = await _dbService.GetAssetsAsync();

                if (Services.UserSession.AccountID > 5)
                {
                    ActiveCreatorsLabel.Text = "0";
                    ActiveOrdersLabel.Text = "0";
                    DigitalAssetsLabel.Text = "0";
                    CrmLeadsLabel.Text = "0";

                    BindableLayout.SetItemsSource(RevenueChartLayout, new List<double> { 0, 0, 0, 0, 0, 0, 0 });
                    BindableLayout.SetItemsSource(RecentSalesList, new List<Services.SaleTransaction>());
                    
                    if (ArtSalesLabel != null) ArtSalesLabel.Text = "$0.00";
                    if (DigitalCommLabel != null) DigitalCommLabel.Text = "$0.00";
                    if (GraphicsDistributionLabel != null) GraphicsDistributionLabel.Text = "0";
                    if (MotionDistributionLabel != null) MotionDistributionLabel.Text = "0";
                    if (CrmLeadsContainer != null) CrmLeadsContainer.Children.Clear();
                }
                else
                {
                    // Update Labels
                    ActiveCreatorsLabel.Text = stats.TotalMsmes.ToString("N0");
                    ActiveOrdersLabel.Text = orders.Count.ToString();
                    DigitalAssetsLabel.Text = assets.Count.ToString();
                    CrmLeadsLabel.Text = stats.TotalCustomers.ToString(); // Use customer count from stats instead

                    // Update Charts
                    // Scaling ActivityHistory for visual height (max 180)
                    var chartData = stats.ActivityHistory.Select(h => Math.Max(20, (h / stats.ActivityHistory.Max()) * 180)).ToList();
                    BindableLayout.SetItemsSource(RevenueChartLayout, chartData);

                    // Update Recent Sales
                    var recentSales = await _dbService.GetRecentTransactionsAsync();
                    BindableLayout.SetItemsSource(RecentSalesList, recentSales);
                }
            }
            catch (Exception ex)
            {
                // Silence or log error
                System.Diagnostics.Debug.WriteLine($"Dashboard Load Error: {ex.Message}");
            }
        }
        private void OnTierTabTapped(object? sender, TappedEventArgs e)
        {
            var parameter = (e as TappedEventArgs)?.Parameter?.ToString();
            if (string.IsNullOrEmpty(parameter)) return;

            // Reset all tabs
            StarterTab.BackgroundColor = Colors.Transparent;
            StandardTab.BackgroundColor = Colors.Transparent;
            EnterpriseTab.BackgroundColor = Colors.Transparent;
            
            StarterIndicator.IsVisible = false;
            StandardIndicator.IsVisible = false;
            EnterpriseIndicator.IsVisible = false;

            var setLabelColor = (Border tab, Color color) => {
                if (tab?.Content is Grid grid && grid.Children.Count > 1 && grid.Children[1] is Label lbl)
                    lbl.TextColor = color;
            };

            setLabelColor(StarterTab, Color.FromArgb("#94A3B8"));
            setLabelColor(StandardTab, Color.FromArgb("#94A3B8"));
            setLabelColor(EnterpriseTab, Color.FromArgb("#94A3B8"));

            // Hide all details
            StarterDetails.IsVisible = false;
            StandardDetails.IsVisible = false;
            EnterprisePlusDetails.IsVisible = false;

            // Activate selected
            if (parameter == "Starter")
            {
                if (StarterTab != null) { StarterTab.BackgroundColor = Color.FromArgb("#1E293B"); setLabelColor(StarterTab, Colors.White); }
                StarterIndicator.IsVisible = true;
                StarterDetails.IsVisible = true;
            }
            else if (parameter == "Standard")
            {
                if (StandardTab != null) { StandardTab.BackgroundColor = Color.FromArgb("#1E293B"); setLabelColor(StandardTab, Colors.White); }
                StandardIndicator.IsVisible = true;
                StandardDetails.IsVisible = true;
            }
            else if (parameter == "EnterprisePlus")
            {
                if (EnterpriseTab != null) { EnterpriseTab.BackgroundColor = Color.FromArgb("#1E293B"); setLabelColor(EnterpriseTab, Colors.White); }
                EnterpriseIndicator.IsVisible = true;
                EnterprisePlusDetails.IsVisible = true;
            }
        }

        private string _selectedPlan = "";
        private string _selectedPlanPrice = "";

        private void OnSubscribeClicked(object? sender, EventArgs e)
        {
            var btn = sender as Button;
            var plan = btn?.CommandParameter?.ToString() ?? "Plan";
            _selectedPlan = plan;

            if (plan == "Starter") _selectedPlanPrice = "$0.00";
            else if (plan == "Standard") _selectedPlanPrice = "$49.00 / mo";
            else if (plan == "Enterprise Plus") _selectedPlanPrice = "$199.00 / mo";
            else _selectedPlanPrice = "$99.00 / mo";

            SummaryPlanNameLabel.Text = _selectedPlan;
            SummaryPlanPriceLabel.Text = _selectedPlanPrice;
            SummaryTotalLabel.Text = _selectedPlanPrice.Replace(" / mo", "");

            SubscriptionPaymentModal.IsVisible = true;
        }

        private void OnClosePaymentModalTapped(object? sender, TappedEventArgs e)
        {
            SubscriptionPaymentModal.IsVisible = false;
        }

        private void OnCreditCardTabTapped(object? sender, TappedEventArgs e)
        {
            if (CreditCardTabPayment != null)
            {
                CreditCardTabPayment.BackgroundColor = Color.FromArgb("#F0FDFA");
                CreditCardTabPayment.Stroke = Color.FromArgb("#0D9488");
                CreditCardTabPayment.StrokeThickness = 2;
            }

            if (DigitalWalletTabPayment != null)
            {
                DigitalWalletTabPayment.BackgroundColor = Colors.White;
                DigitalWalletTabPayment.Stroke = Color.FromArgb("#E2E8F0");
                DigitalWalletTabPayment.StrokeThickness = 1;
            }

            if (CreditCardPanel != null) CreditCardPanel.IsVisible = true;
            if (DigitalWalletPanel != null) DigitalWalletPanel.IsVisible = false;
        }

        private void OnDigitalWalletTabTapped(object? sender, TappedEventArgs e)
        {
            if (DigitalWalletTabPayment != null)
            {
                DigitalWalletTabPayment.BackgroundColor = Color.FromArgb("#F0FDFA");
                DigitalWalletTabPayment.Stroke = Color.FromArgb("#0D9488");
                DigitalWalletTabPayment.StrokeThickness = 2;
            }

            if (CreditCardTabPayment != null)
            {
                CreditCardTabPayment.BackgroundColor = Colors.White;
                CreditCardTabPayment.Stroke = Color.FromArgb("#E2E8F0");
                CreditCardTabPayment.StrokeThickness = 1;
            }

            if (CreditCardPanel != null) CreditCardPanel.IsVisible = false;
            if (DigitalWalletPanel != null) DigitalWalletPanel.IsVisible = true;
        }

        private void OnGCashSelected(object? sender, TappedEventArgs e)
        {
            if (GCashOption != null)
            {
                GCashOption.Stroke = Color.FromArgb("#0D9488");
                GCashOption.StrokeThickness = 2;
                GCashOption.BackgroundColor = Color.FromArgb("#F0FDFA");
            }
            if (GCashCheck != null) GCashCheck.IsVisible = true;
            if (GCashUncheck != null) GCashUncheck.IsVisible = false;

            if (PayMayaOption != null)
            {
                PayMayaOption.Stroke = Color.FromArgb("#E2E8F0");
                PayMayaOption.StrokeThickness = 1;
                PayMayaOption.BackgroundColor = Colors.White;
            }
            if (PayMayaCheck != null) PayMayaCheck.IsVisible = false;
            if (PayMayaUncheck != null) PayMayaUncheck.IsVisible = true;
        }

        private void OnPayMayaSelected(object? sender, TappedEventArgs e)
        {
            if (PayMayaOption != null)
            {
                PayMayaOption.Stroke = Color.FromArgb("#0D9488");
                PayMayaOption.StrokeThickness = 2;
                PayMayaOption.BackgroundColor = Color.FromArgb("#F0FDFA");
            }
            if (PayMayaCheck != null) PayMayaCheck.IsVisible = true;
            if (PayMayaUncheck != null) PayMayaUncheck.IsVisible = false;

            if (GCashOption != null)
            {
                GCashOption.Stroke = Color.FromArgb("#E2E8F0");
                GCashOption.StrokeThickness = 1;
                GCashOption.BackgroundColor = Colors.White;
            }
            if (GCashCheck != null) GCashCheck.IsVisible = false;
            if (GCashUncheck != null) GCashUncheck.IsVisible = true;
        }

        private async void OnConfirmPaymentClicked(object? sender, EventArgs e)
        {
            if (ConfirmPaymentButton != null) ConfirmPaymentButton.IsEnabled = false;

            await DisplayAlertAsync("Processing", $"Processing payment for {_selectedPlan}...", "OK");
            
            // Simulate network delay
            await Task.Delay(1500);

            try
            {
                // Update tier in DB first
                bool success = await _dbService.UpdateAccountTierAsync(Services.UserSession.AccountID, _selectedPlan);
                
                if (success)
                {
                    // Sync session ONLY on success
                    Services.UserSession.Tier = _selectedPlan;
                    
                    await DisplayAlertAsync("Payment Successful", $"You have successfully subscribed to the {_selectedPlan} tier!", "OK");
                    
                    // Refresh all dashboard data to reflect potential capacity changes
                    await LoadDashboardData();
                    if (Sidebar != null)
                    {
                        Sidebar.ApplyTierRestrictions();
                    }
                }
                else
                {
                    await DisplayAlertAsync("Update Failed", "We processed your payment but could not update your tier. Please contact support.", "OK");
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to update tier: {ex.Message}");
                await DisplayAlertAsync("Error", "An error occurred while updating your subscription.", "OK");
            }

            if (SubscriptionPaymentModal != null) SubscriptionPaymentModal.IsVisible = false;
            if (ConfirmPaymentButton != null) ConfirmPaymentButton.IsEnabled = true;

            // Final UI refresh
            ApplyCurrentPlanUI();
        }
    }
}
