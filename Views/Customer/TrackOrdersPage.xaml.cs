using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Threading.Tasks;

namespace CreatiSphere.Views.Customer
{
    public partial class TrackOrdersPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private const int CurrentUserId = 1002;
        private List<CustomOrder> _allCommissions = new();
        private string _currentFilter = "Active";

        public TrackOrdersPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserInfo();
            await LoadCommissions();
        }

        private async Task LoadUserInfo()
        {
            try
            {
                var user = await _databaseService.GetAccountInfoByIdAsync(CurrentUserId);
                if (user != null)
                {
                    ProfileNameLabel.Text = user.AccountName;
                    ProfileInitialsLabel.Text = user.AccountName?.Length >= 2 ? user.AccountName.Substring(0, 2).ToUpper() : "U";
                    ProfileRoleLabel.Text = "Artisanal Collector";
                }
            }
            catch { }
        }

        private async Task LoadCommissions()
        {
            try
            {
                _allCommissions = await _databaseService.GetCustomOrdersAsync();
                FilterCommissions();
            }
            catch (Exception ex)
            {
                await this.DisplayAlertAsync("Error", "Could not load commissions: " + ex.Message, "OK");
            }
        }

        private async void OnDashboardTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomerDashboard");
        }

        private async void OnExploreTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//BrowseMarketplacePage");
        }

        private async void OnCustomOrdersTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomOrdersPage");
        }

        private async void OnMessagesTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MessagesPage");
        }

        private async void OnFeedbackTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FeedbackPage");
        }

        private async void OnRewardsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//RewardsPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private void OnViewDetailsClicked(object? sender, EventArgs e)
        {
            var button = sender as Button;
            var order = button?.CommandParameter as CustomOrder ?? (button?.BindingContext as CustomOrder);
            
            if (order != null)
            {
                OrderDetailsModal.BindingContext = order;
                OrderDetailsModal.IsVisible = true;
            }
        }

        private void OnCloseModalTapped(object? sender, TappedEventArgs e)
        {
            OrderDetailsModal.IsVisible = false;
        }

        private async void OnMessageArtistClicked(object? sender, EventArgs e)
        {
            var button = sender as Button;
            var order = button?.CommandParameter as CustomOrder ?? (button?.BindingContext as CustomOrder);
            
            if (order != null)
            {
                string message = await DisplayPromptAsync("Message Artist", $"Send a message about '{order.Title}'", "Send", "Cancel", "Type your message here...", -1, null, "");
                if (!string.IsNullOrWhiteSpace(message))
                {
                    MockMessageService.AddMessage("Alex (Customer)", message, true);
                    await DisplayAlertAsync("Success", "Message sent to artist!", "OK");
                    await Shell.Current.GoToAsync("//MessagesPage");
                }
            }
        }

        private void FilterCommissions()
        {
            var filtered = _currentFilter switch
            {
                "Active" => _allCommissions.Where(c => c.Status != "Completed" && c.Status != "Delivered").ToList(),
                "Completed" => _allCommissions.Where(c => c.Status == "Completed" || c.Status == "Delivered").ToList(),
                "Waiting for Payment" => _allCommissions.Where(c => c.Status == "Waiting for Payment").ToList(),
                "Drafts" => new List<CustomOrder>(), // Drafts not currently supported by mock DB
                _ => _allCommissions
            };

            BindableLayout.SetItemsSource(CommissionsContainer, filtered);
        }

        private void OnTabTapped(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is string filter)
            {
                _currentFilter = filter;
                
                ResetTab(TabActive, LblActive);
                ResetTab(TabWaitingForPayment, LblWaitingForPayment);
                ResetTab(TabCompleted, LblCompleted);
                ResetTab(TabDrafts, LblDrafts);

                var activeTab = filter switch
                {
                    "Waiting for Payment" => (TabWaitingForPayment, LblWaitingForPayment),
                    "Completed" => (TabCompleted, LblCompleted),
                    "Drafts" => (TabDrafts, LblDrafts),
                    _ => (TabActive, LblActive)
                };
                
                SetActiveTab(activeTab.Item1, activeTab.Item2);
                FilterCommissions();
            }
        }

        private void ResetTab(Border tab, Label lbl)
        {
            tab.BackgroundColor = Colors.Transparent;
            lbl.TextColor = Color.FromArgb("#64748B");
            lbl.FontAttributes = FontAttributes.None;
        }

        private void SetActiveTab(Border tab, Label lbl)
        {
            tab.BackgroundColor = Colors.White;
            lbl.TextColor = Color.FromArgb("#0D9488");
            lbl.FontAttributes = FontAttributes.Bold;
        }

        private async void OnPayBalanceClicked(object? sender, EventArgs e)
        {
            var button = sender as Button;
            var order = button?.CommandParameter as CustomOrder ?? (button?.BindingContext as CustomOrder);
            
            if (order != null)
            {
                bool confirm = await DisplayAlertAsync("Pay Balance", $"Would you like to pay the remaining balance for '{order.Title}' to proceed to Delivery?", "Pay Now", "Cancel");
                if (confirm)
                {
                    // Update status in DB to "Delivery"
                    bool success = await _databaseService.UpdateCustomOrderStatusAsync(order.OrderID ?? "", "Delivery");
                    if (success)
                    {
                        await DisplayAlertAsync("Payment Successful", "Your payment has been processed. The creator will now deliver your custom order.", "OK");
                        await LoadCommissions();
                    }
                    else
                    {
                        await DisplayAlertAsync("Error", "Payment failed to process. Please try again.", "OK");
                    }
                }
            }
        }
    }
}
