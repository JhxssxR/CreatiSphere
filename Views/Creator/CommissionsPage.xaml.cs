using CreatiSphere.Services;
using System.Threading.Tasks;

namespace CreatiSphere.Views.Creator
{
    public partial class CommissionsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private List<CustomOrder> _allCommissions = new();
        private string _currentFilter = "All";
        private bool _showAll = false;
        private CustomOrder? _selectedCommission;

        private Image? _previewImage;
        private Label? _previewTitleLabel;
        private Label? _previewSubtitleLabel;
        private Label? _previewDescriptionLabel;
        private Label? _previewResolutionLabel;
        private Label? _previewFormatLabel;
        private Label? _viewAllLabel;

        public CommissionsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            InitializeNamedElements();
        }

        private void InitializeNamedElements()
        {
            _previewImage = (Image?)FindByName("PreviewImage");
            _previewTitleLabel = (Label?)FindByName("PreviewTitleLabel");
            _previewSubtitleLabel = (Label?)FindByName("PreviewSubtitleLabel");
            _previewDescriptionLabel = (Label?)FindByName("PreviewDescriptionLabel");
            _previewResolutionLabel = (Label?)FindByName("PreviewResolutionLabel");
            _previewFormatLabel = (Label?)FindByName("PreviewFormatLabel");
            _viewAllLabel = (Label?)FindByName("ViewAllLabel");
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
                _allCommissions = await _databaseService.GetCustomOrdersAsync();
                UpdateCounts();
                FilterCommissions();
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

        private void UpdateCounts()
        {
            CountAll.Text = _allCommissions.Count.ToString();
            CountBriefing.Text = _allCommissions.Count(c => c.Status == "Briefing").ToString();
            CountConcept.Text = _allCommissions.Count(c => c.Status == "Concept").ToString();
            CountRefining.Text = _allCommissions.Count(c => c.Status == "Refining").ToString();
            CountWaitingForPayment.Text = _allCommissions.Count(c => c.Status == "Waiting for Payment").ToString();
            CountDelivery.Text = _allCommissions.Count(c => c.Status == "Delivery").ToString();
        }

        private void FilterCommissions()
        {
            var filtered = _currentFilter == "All" 
                ? _allCommissions 
                : _allCommissions.Where(c => c.Status == _currentFilter).ToList();
            
            var displayList = _showAll ? filtered : filtered.Take(4).ToList();
            
            BindableLayout.SetItemsSource(CommissionsListContainer, displayList);

            if (_viewAllLabel != null)
            {
                if (filtered.Count > 4)
                {
                    _viewAllLabel.IsVisible = true;
                    _viewAllLabel.Text = _showAll ? "Show Less" : $"View All {filtered.Count} Commissions";
                }
                else
                {
                    _viewAllLabel.IsVisible = false;
                }
            }

            if (displayList.Count > 0)
            {
                SelectCommission(displayList[0]);
            }
        }

        private void OnCommissionSelected(object? sender, TappedEventArgs e)
        {
            var order = e.Parameter as CustomOrder ?? (sender as View)?.BindingContext as CustomOrder;
            if (order == null) return;
            
            SelectCommission(order);
        }

        private void SelectCommission(CustomOrder order)
        {
            _selectedCommission = order;
            
            if (_previewImage != null) _previewImage.Source = string.IsNullOrEmpty(order.ImagePath) ? "nebula_dreamscape.png" : order.ImagePath;
            if (_previewTitleLabel != null) _previewTitleLabel.Text = $"\"{order.Title}\"";
            if (_previewSubtitleLabel != null) _previewSubtitleLabel.Text = $"Commission for {order.ClientName}";
            if (_previewDescriptionLabel != null) _previewDescriptionLabel.Text = $"\"{order.Description}\"";
            if (_previewResolutionLabel != null) _previewResolutionLabel.Text = string.IsNullOrEmpty(order.Resolution) ? "4000 × 6000 px" : order.Resolution;
            if (_previewFormatLabel != null) _previewFormatLabel.Text = string.IsNullOrEmpty(order.Deliverables) ? "TIFF / PSD" : order.Deliverables;
        }

        private void OnViewAllTapped(object? sender, EventArgs e)
        {
            _showAll = !_showAll;
            FilterCommissions();
        }

        private void OnTabTapped(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is string filter)
            {
                _currentFilter = filter;
                
                // Reset all tabs
                ResetTab(TabAll, LblAll, BadgeAll, CountAll);
                ResetTab(TabBriefing, LblBriefing, BadgeBriefing, CountBriefing);
                ResetTab(TabConcept, LblConcept, BadgeConcept, CountConcept);
                ResetTab(TabRefining, LblRefining, BadgeRefining, CountRefining);
                ResetTab(TabWaitingForPayment, LblWaitingForPayment, BadgeWaitingForPayment, CountWaitingForPayment);
                ResetTab(TabDelivery, LblDelivery, BadgeDelivery, CountDelivery);

                // Set active tab
                var activeTab = filter switch
                {
                    "Briefing" => (TabBriefing, LblBriefing, BadgeBriefing, CountBriefing),
                    "Concept" => (TabConcept, LblConcept, BadgeConcept, CountConcept),
                    "Refining" => (TabRefining, LblRefining, BadgeRefining, CountRefining),
                    "Waiting for Payment" => (TabWaitingForPayment, LblWaitingForPayment, BadgeWaitingForPayment, CountWaitingForPayment),
                    "Delivery" => (TabDelivery, LblDelivery, BadgeDelivery, CountDelivery),
                    _ => (TabAll, LblAll, BadgeAll, CountAll)
                };
                
                SetActiveTab(activeTab.Item1, activeTab.Item2, activeTab.Item3, activeTab.Item4);
                
                FilterCommissions();
            }
        }

        private void ResetTab(Border tab, Label lbl, Border badge, Label count)
        {
            tab.BackgroundColor = Color.FromArgb("#E2E8F0");
            lbl.TextColor = Color.FromArgb("#475569");
            badge.BackgroundColor = Color.FromArgb("#CBD5E1");
            count.TextColor = Color.FromArgb("#475569");
        }

        private void SetActiveTab(Border tab, Label lbl, Border badge, Label count)
        {
            tab.BackgroundColor = Color.FromArgb("#065F46");
            lbl.TextColor = Colors.White;
            badge.BackgroundColor = Color.FromArgb("#047857");
            count.TextColor = Colors.White;
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void OnUpdateStatusClicked(object? sender, TappedEventArgs e)
        {
            var order = e.Parameter as CustomOrder ?? (sender as View)?.BindingContext as CustomOrder;
            if (order == null) return;

            string[] allStatuses = { "Briefing", "Concept", "Refining", "Waiting for Payment", "Delivery" };
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
        private async void OnNotificationClicked(object? sender, EventArgs e)
        {
            if (MockMessageService.HasNewMessageForCreator)
            {
                await this.DisplayAlert("Notifications", "You have a new message from a customer! Check your Messages.", "OK");
                MockMessageService.HasNewMessageForCreator = false; // Mark as read
            }
            else
            {
                await this.DisplayAlert("Notifications", "No new notifications.", "OK");
            }
        }

        private async void OnMessageClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatorMessagesPage");
        }

        private async void OnMessagesTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatorMessagesPage");
        }

        private async void OnProfileClicked(object? sender, EventArgs e)
        {
            await this.DisplayAlertAsync("Profile", "Opening creator profile settings...", "OK");
        }

        private async void OnUploadNewArtClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new AssetEditorPage());
        }
    }
}
