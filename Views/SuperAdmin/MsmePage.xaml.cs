using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CreatiSphere.Views.SuperAdmin
{
    public partial class MsmePage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private List<Msme> _allMsmes = new();
        private string _currentFilter = "All";

        public MsmePage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadUserData();
        }

        private void LoadUserData()
        {
            UserNameLabel.Text = UserSession.Username ?? "Super Admin";
            UserRoleLabel.Text = UserSession.IsSuperAdmin ? "SUPER ADMIN" : "ADMIN";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadMsmes();
        }

        private async Task LoadMsmes()
        {
            try
            {
                _allMsmes = await _databaseService.GetMsmesAsync();
                ApplyFilters();

                var stats = await _databaseService.GetMsmeStatsAsync();
                TotalMsmesLabel.Text = stats.TotalMsmes.ToString("N0");
                ActiveMsmesLabel.Text = stats.ActiveMsmes.ToString("N0");
                OnTrialMsmesLabel.Text = stats.OnTrialMsmes.ToString("N0");
                GrowthLabel.Text = stats.Growth;
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to load MSMEs: {ex.Message}", "OK");
            }
        }

        private void ApplyFilters()
        {
            var filtered = _allMsmes.AsEnumerable();

            // Status Filter
            if (_currentFilter != "All")
            {
                filtered = filtered.Where(m => m.Status?.Equals(_currentFilter, StringComparison.OrdinalIgnoreCase) == true);
            }

            // Search Filter
            string searchText = SearchEntry.Text ?? "";
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(m => 
                    (m.BusinessName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true) || 
                    (m.Niche?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true));
            }

            var list = filtered.ToList();
            MsmesCollectionView.Children.Clear();

            foreach (var msme in list)
            {
                // Divider
                var divider = new BoxView { HeightRequest = 1, BackgroundColor = Color.FromArgb("#F8FAFC") };

                // Status badge colors
                bool isInactive = msme.Status?.Equals("INACTIVE", StringComparison.OrdinalIgnoreCase) == true
                               || msme.Status?.Equals("Inactive", StringComparison.OrdinalIgnoreCase) == true;
                var badgeBg = isInactive ? Color.FromArgb("#FEE2E2") : Color.FromArgb("#CCFBF1");
                var badgeFg = isInactive ? Color.FromArgb("#EF4444") : Color.FromArgb("#0F766E");

                // Initials badge
                var initials = new Label
                {
                    Text = msme.Initials,
                    TextColor = Color.FromArgb("#0F766E"),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };
                var initialsBorder = new Border
                {
                    HeightRequest = 40, WidthRequest = 40,
                    BackgroundColor = Color.FromArgb("#E0F2F1"),
                    StrokeThickness = 0,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Content = initials
                };

                // Business name cell
                var businessNameLabel = new Label { Text = msme.BusinessName, TextColor = Color.FromArgb("#0F172A"), FontSize = 14, FontAttributes = FontAttributes.Bold };
                var uidLabel = new Label { Text = "UID: MSME-XXXX", TextColor = Color.FromArgb("#94A3B8"), FontSize = 11 };
                var businessStack = new HorizontalStackLayout { Spacing = 12 };
                businessStack.Children.Add(initialsBorder);
                var nameStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center };
                nameStack.Children.Add(businessNameLabel);
                nameStack.Children.Add(uidLabel);
                businessStack.Children.Add(nameStack);

                // Niche chip
                var nicheLabel = new Label { Text = msme.Niche, TextColor = Color.FromArgb("#475569"), FontSize = 11 };
                var nicheBorder = new Border
                {
                    BackgroundColor = Color.FromArgb("#F1F5F9"),
                    Padding = new Thickness(8, 4),
                    StrokeThickness = 0,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    Content = nicheLabel
                };

                // Owner name
                var ownerLabel = new Label { Text = msme.OwnerName, TextColor = Color.FromArgb("#0F172A"), FontSize = 13, VerticalOptions = LayoutOptions.Center };

                // ERP tier
                var tierLabel = new Label { Text = msme.ErpTier, TextColor = Color.FromArgb("#0F172A"), FontSize = 13, VerticalOptions = LayoutOptions.Center };

                // Active users
                var usersLabel = new Label { Text = msme.ActiveUsers, TextColor = Color.FromArgb("#475569"), FontSize = 13, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

                // Status badge
                var statusLabel = new Label { Text = msme.Status, TextColor = badgeFg, FontSize = 10, FontAttributes = FontAttributes.Bold };
                var statusBorder = new Border
                {
                    BackgroundColor = badgeBg,
                    Padding = new Thickness(12, 4),
                    StrokeThickness = 0,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 20 },
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = statusLabel
                };

                // Options button
                var optionsLabel = new Label { Text = "⋮", TextColor = Color.FromArgb("#94A3B8"), FontSize = 18, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                var tap = new TapGestureRecognizer();
                tap.CommandParameter = msme;
                tap.Tapped += OnMsmeOptionsClicked;
                optionsLabel.GestureRecognizers.Add(tap);

                // Row grid
                var row = new Grid
                {
                    Padding = new Thickness(24, 16),
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = new GridLength(25, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(15, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(15, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(15, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(5, GridUnitType.Star) }
                    }
                };

                Grid.SetColumn(businessStack, 0);
                Grid.SetColumn(nicheBorder, 1);
                Grid.SetColumn(ownerLabel, 2);
                Grid.SetColumn(tierLabel, 3);
                Grid.SetColumn(usersLabel, 4);
                Grid.SetColumn(statusBorder, 5);
                Grid.SetColumn(optionsLabel, 6);

                row.Children.Add(businessStack);
                row.Children.Add(nicheBorder);
                row.Children.Add(ownerLabel);
                row.Children.Add(tierLabel);
                row.Children.Add(usersLabel);
                row.Children.Add(statusBorder);
                row.Children.Add(optionsLabel);

                var rowWrapper = new VerticalStackLayout();
                rowWrapper.Children.Add(row);
                rowWrapper.Children.Add(divider);
                MsmesCollectionView.Children.Add(rowWrapper);
            }

            UpdateFilterButtonStyles();
        }

        private void OnFilterClicked(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                _currentFilter = btn.Text;
                ApplyFilters();
            }
        }

        private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private string? _editingMsmeName = null;

        private void OnCancelModalClicked(object? sender, EventArgs e)
        {
            MsmeModal.IsVisible = false;
        }

        private async void OnSaveMsmeClicked(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BusinessNameEntry.Text))
            {
                await DisplayAlertAsync("Error", "Business Name is required.", "OK");
                return;
            }

            string tier = TierPicker.SelectedItem?.ToString() ?? "Starter";
            int limit = Services.UserSession.GetLimitForTier(tier);

            int current = 1;
            if (_editingMsmeName != null)
            {
                var existing = _allMsmes.FirstOrDefault(m => m.BusinessName == _editingMsmeName)?.ActiveUsers;
                if (existing != null && existing.Contains("/"))
                {
                    int.TryParse(existing.Split('/')[0].Trim(), out current);
                }
            }

            var msme = new Msme
            {
                BusinessName = BusinessNameEntry.Text,
                Niche = NicheEntry.Text ?? "Creative Business",
                OwnerName = OwnerNameEntry.Text ?? "Unknown Owner",
                ErpTier = tier,
                Status = StatusPicker.SelectedItem?.ToString() ?? "ACTIVE",
                ActiveUsers = $"{current} / {limit}"
            };


            bool success;
            if (_editingMsmeName == null)
            {
                // Add New
                success = await _databaseService.AddMsmeAsync(msme);
                if (success && !string.IsNullOrWhiteSpace(EmailEntry.Text) && !string.IsNullOrWhiteSpace(PasswordEntry.Text))
                {
                    // Create an Admin account for the new MSME (RoleID = 2 typically denotes Admin)
                    await _databaseService.CreateAccountAsync(msme.OwnerName ?? "Admin", EmailEntry.Text, PasswordEntry.Text, 2);
                }
            }
            else
            {
                // Update Existing
                success = await _databaseService.UpdateMsmeAsync(msme, _editingMsmeName);
            }

            if (success)
            {
                MsmeModal.IsVisible = false;
                await LoadMsmes();
                await DisplayAlertAsync("Success", _editingMsmeName == null ? "MSME registered successfully." : "MSME profile updated.", "OK");
            }
            else
            {
                await DisplayAlertAsync("Error", "Failed to save record to database.", "OK");
            }
        }

        private void OnAddMsmeClicked(object? sender, EventArgs e)
        {
            _editingMsmeName = null;
            ModalTitle.Text = "Register New MSME";
            BusinessNameEntry.Text = "";
            NicheEntry.Text = "";
            OwnerNameEntry.Text = "";
            EmailEntry.Text = "";
            PasswordEntry.Text = "";
            AccountFieldsLayout.IsVisible = true;
            TierPicker.SelectedIndex = 0;
            StatusPicker.SelectedIndex = 0;
            MsmeModal.IsVisible = true;
        }

        private void ShowEditModal(Msme msme)
        {
            _editingMsmeName = msme.BusinessName;
            ModalTitle.Text = $"Edit {msme.BusinessName}";
            BusinessNameEntry.Text = msme.BusinessName;
            NicheEntry.Text = msme.Niche;
            OwnerNameEntry.Text = msme.OwnerName;
            AccountFieldsLayout.IsVisible = false;
            TierPicker.SelectedItem = msme.ErpTier;
            StatusPicker.SelectedItem = msme.Status?.ToUpper();
            MsmeModal.IsVisible = true;
        }

        private enum MsmeAction { Deactivate, Delete }
        private MsmeAction _pendingAction;
        private Msme? _pendingMsme;

        private void OnCancelConfirmationClicked(object? sender, EventArgs e)
        {
            ConfirmationModal.IsVisible = false;
        }

        private async void OnConfirmActionClicked(object? sender, EventArgs e)
        {
            if (_pendingMsme == null) return;

            bool success = false;
            string actionName = "";

            if (_pendingAction == MsmeAction.Deactivate)
            {
                success = await _databaseService.UpdateMsmeStatusAsync(_pendingMsme.BusinessName ?? "", "INACTIVE");
                actionName = "deactivated";
            }
            else if (_pendingAction == MsmeAction.Delete)
            {
                success = await _databaseService.DeleteMsmeAsync(_pendingMsme.BusinessName ?? "");
                actionName = "deleted";
            }

            ConfirmationModal.IsVisible = false;

            if (success)
            {
                await LoadMsmes();
                await DisplayAlertAsync("Success", $"Account has been {actionName}.", "OK");
            }
            else
            {
                await DisplayAlertAsync("Error", "Action failed. Please check database connection.", "OK");
            }
        }

        private void ShowConfirmation(Msme msme, MsmeAction action)
        {
            _pendingMsme = msme;
            _pendingAction = action;

            if (action == MsmeAction.Deactivate)
            {
                ConfirmationTitle.Text = "Deactivate Account";
                ConfirmationMessage.Text = $"Are you sure you want to deactivate {msme.BusinessName}? The users will no longer be able to access the ERP.";
                ConfirmActionButton.Text = "Deactivate";
                ConfirmActionButton.BackgroundColor = Color.FromArgb("#F59E0B"); // Orange
                IconContainer.BackgroundColor = Color.FromArgb("#FFF7ED");
                ConfirmationIcon.Fill = Color.FromArgb("#F59E0B");
            }
            else
            {
                ConfirmationTitle.Text = "Delete Record";
                ConfirmationMessage.Text = $"This action is permanent. All data for {msme.BusinessName} will be removed from the system.";
                ConfirmActionButton.Text = "Delete Permanently";
                ConfirmActionButton.BackgroundColor = Color.FromArgb("#EF4444"); // Red
                IconContainer.BackgroundColor = Color.FromArgb("#FEF2F2");
                ConfirmationIcon.Fill = Color.FromArgb("#EF4444");
            }

            ConfirmationModal.IsVisible = true;
        }

        private async void OnMsmeOptionsClicked(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is Msme msme)
            {
                string action = await DisplayActionSheetAsync($"Options for {msme.BusinessName}", "Cancel", null, "View Details", "Edit Profile", "Change ERP Tier", "Deactivate", "Delete Record");
                
                switch (action)
                {
                    case "Edit Profile":
                        ShowEditModal(msme);
                        break;
                    case "Change ERP Tier":
                        string newTier = await DisplayActionSheetAsync("Select New ERP Tier", "Cancel", null, "Starter", "Standard", "Enterprise Plus");
                        if (newTier != "Cancel" && !string.IsNullOrEmpty(newTier))
                        {
                            bool success = await _databaseService.UpdateMsmeTierAsync(msme.BusinessName ?? "", newTier);
                            if (success)
                            {
                                await DisplayAlertAsync("Success", $"Tier updated to {newTier} for {msme.BusinessName}.", "OK");
                                await LoadMsmes();
                            }
                            else
                            {
                                await DisplayAlertAsync("Error", "Failed to update tier in database.", "OK");
                            }
                        }
                        break;
                    case "Deactivate":
                        ShowConfirmation(msme, MsmeAction.Deactivate);
                        break;
                    case "Delete Record":
                        ShowConfirmation(msme, MsmeAction.Delete);
                        break;
                }
            }
        }

        private void UpdateFilterButtonStyles()
        {
            var buttons = new Dictionary<string, Button>
            {
                { "All", FilterAllBtn },
                { "Active", FilterActiveBtn },
                { "Trial", FilterTrialBtn },
                { "Inactive", FilterInactiveBtn }
            };

            foreach (var kvp in buttons)
            {
                if (kvp.Key == _currentFilter)
                {
                    kvp.Value.BackgroundColor = Color.FromArgb("#F1F5F9"); // Slate 50
                    kvp.Value.TextColor = Color.FromArgb("#0F172A");       // Slate 900
                    kvp.Value.FontAttributes = FontAttributes.Bold;
                }
                else
                {
                    kvp.Value.BackgroundColor = Colors.Transparent;
                    kvp.Value.TextColor = Color.FromArgb("#64748B");       // Slate 500
                    kvp.Value.FontAttributes = FontAttributes.None;
                }
            }
        }

        private async void OnDashboardClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//DashboardPage");
        }

        private async void OnManageUsersClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//UsersPage");
        }

        private async void OnManageModulesClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ModulesPage");
        }

        private async void OnMonitorSystemClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MonitorSystemPage");
        }

        private async void OnSystemReportsClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ReportsPage");
        }

        private async void OnSignOutClicked(object? sender, TappedEventArgs e)
        {
            bool answer = await DisplayAlertAsync("Sign Out", "Are you sure you want to sign out?", "Yes", "No");
            if (answer)
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
    }
}
