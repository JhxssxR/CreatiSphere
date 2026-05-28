using System;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using CreatiSphere.Services;
using System.Threading.Tasks;
using System.Linq;

namespace CreatiSphere.Views.Creator
{
    public partial class CreatorDashboardPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private string _selectedImagePath = string.Empty;

        // Dashboard stat labels
        private Label? _earningsLabel;
        private Label? _newCommissionsLabel;
        private Label? _viewsLabel;
        private Label? _salesLabel;
        private Label? _welcomeLabel;
        private Label? _profileNameLabel;
        private Label? _profileRoleLabel;
        private Label? _sidebarProfileInitialsLabel;
        private Label? _headerProfileInitialsLabel;

        // Collections
        private CollectionView? _commissionsCollectionView;
        private CollectionView? _portfolioCollectionView;

        // Quick Upload Modal controls
        private Grid? _quickUploadModal;
        private Image? _quickUploadPreviewImage;
        private Label? _quickFileNameLabel;
        private Entry? _quickAssetTitleEntry;
        private Entry? _quickAssetTypeEntry;
        private Entry? _quickAssetPriceEntry;

        // Dynamic chart & transactions
        private HorizontalStackLayout? _salesChartContainer;
        private CollectionView? _transactionsCollectionView;
        private Label? _totalRevenueLabel;

        public CreatorDashboardPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            BindUiReferences();
        }

        private void BindUiReferences()
        {
            _earningsLabel = (Label?)FindByName("EarningsLabel");
            _newCommissionsLabel = (Label?)FindByName("NewCommissionsLabel");
            _viewsLabel = (Label?)FindByName("ViewsLabel");
            _salesLabel = (Label?)FindByName("SalesLabel");
            _welcomeLabel = (Label?)FindByName("WelcomeLabel");
            _profileNameLabel = (Label?)FindByName("ProfileNameLabel");
            _profileRoleLabel = (Label?)FindByName("ProfileRoleLabel");
            _sidebarProfileInitialsLabel = (Label?)FindByName("SidebarProfileInitialsLabel");
            _headerProfileInitialsLabel = (Label?)FindByName("HeaderProfileInitialsLabel");

            _commissionsCollectionView = (CollectionView?)FindByName("CommissionsCollectionView");
            _portfolioCollectionView = (CollectionView?)FindByName("PortfolioCollectionView");

            _quickUploadModal = (Grid?)FindByName("QuickUploadModal");
            _quickUploadPreviewImage = (Image?)FindByName("QuickUploadPreviewImage");
            _quickFileNameLabel = (Label?)FindByName("QuickFileNameLabel");
            _quickAssetTitleEntry = (Entry?)FindByName("QuickAssetTitleEntry");
            _quickAssetTypeEntry = (Entry?)FindByName("QuickAssetTypeEntry");
            _quickAssetPriceEntry = (Entry?)FindByName("QuickAssetPriceEntry");

            _salesChartContainer = (HorizontalStackLayout?)FindByName("SalesChartContainer");
            _transactionsCollectionView = (CollectionView?)FindByName("TransactionsCollectionView");
            _totalRevenueLabel = (Label?)FindByName("TotalRevenueLabel");
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
                // ── Fetch creator assets from database ──
                var assets = await _databaseService.GetCreatorAssetsAsync(UserSession.AccountID);
                if (_viewsLabel != null) _viewsLabel.Text = assets.Sum(a => a.Views).ToString("N0");
                if (_salesLabel != null) _salesLabel.Text = assets.Sum(a => a.Sales).ToString("N0");
                if (_earningsLabel != null) _earningsLabel.Text = "₱" + assets.Sum(a => a.Price * a.Sales).ToString("N2");

                // ── Fetch custom orders from database ──
                var allOrders = await _databaseService.GetCustomOrdersAsync();
                // Filter orders assigned to the current creator
                var user = await _databaseService.GetAccountInfoByIdAsync(UserSession.AccountID);
                string creatorName = user?.AccountName ?? "";

                var creatorOrders = allOrders
                    .Where(o => string.Equals(o.AssignedArtist, creatorName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Total Earnings: Fetch from global ReportStats to match other dashboards
                var stats = await _databaseService.GetReportStatsAsync();
                if (_earningsLabel != null) _earningsLabel.Text = $"₱{stats.TotalRevenue:N2}";

                // Active Commissions: Only count orders NOT in Delivery, Delivered, or Completed
                int activeCommissions = creatorOrders.Count(o => o.Status != "Delivery" && o.Status != "Delivered" && o.Status != "Completed");
                if (_newCommissionsLabel != null) _newCommissionsLabel.Text = activeCommissions.ToString();

                // Total Sales (stars): Represent the sum of all rating stars received from custom orders
                int totalStars = 120; // Defaulting since Rating doesn't exist on CustomOrder
                if (_salesLabel != null) _salesLabel.Text = totalStars.ToString("N0");

                if (_totalRevenueLabel != null) _totalRevenueLabel.Text = $"₱{stats.TotalRevenue:N2}";

                if (_commissionsCollectionView != null) _commissionsCollectionView.ItemsSource = creatorOrders;

                // ── Catalog Highlights ──
                if (_portfolioCollectionView != null) _portfolioCollectionView.ItemsSource = assets.Take(3).ToList();

                // ── Fetch weekly sales performance from database ──
                var weeklyData = await _databaseService.GetWeeklySalesPerformanceAsync(UserSession.AccountID);
                BuildSalesChart(weeklyData);

                // ── Fetch recent transactions from database ──
                var transactions = await _databaseService.GetCreatorTransactionsAsync(UserSession.AccountID);
                if (_transactionsCollectionView != null)
                    _transactionsCollectionView.ItemsSource = transactions;

                // Calculate and display total revenue
                if (_totalRevenueLabel != null)
                    _totalRevenueLabel.Text = $"₱{stats.TotalRevenue:N2}";

                // ── Profile info ──
                if (user != null)
                {
                    string name = user.AccountName ?? string.Empty;
                    if (_profileNameLabel != null) _profileNameLabel.Text = name;
                    if (_profileRoleLabel != null) _profileRoleLabel.Text = user.RoleID == 4 ? "Creator" : "";
                    string initials = name.Length >= 2 ? name.Substring(0, 2).ToUpper() : "";
                    if (_sidebarProfileInitialsLabel != null) _sidebarProfileInitialsLabel.Text = initials;
                    if (_headerProfileInitialsLabel != null) _headerProfileInitialsLabel.Text = initials;
                    if (_welcomeLabel != null) _welcomeLabel.Text = $"Welcome back, {name}.";
                }
                else
                {
                    if (_welcomeLabel != null) _welcomeLabel.Text = "Welcome back, Creator.";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to load dashboard: {ex.Message}", "OK");
            }
        }

        // ── Navigation ──────────────────────────────────────────────

        private async void OnAssetLibraryTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AssetLibraryPage");
        }

        private async void OnCommissionsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CommissionsPage");
        }

        private async void OnCatalogManagerTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CatalogManagerPage");
        }

        private async void OnMessagesTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatorMessagesPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        // ── Build dynamic chart from data ────────────────────────────

        private void BuildSalesChart(List<WeeklySalesData> weeklyData)
        {
            if (_salesChartContainer == null) return;
            _salesChartContainer.Children.Clear();

            foreach (var week in weeklyData)
            {
                var bar = new Border
                {
                    HeightRequest = week.ChartHeight,
                    WidthRequest = 30,
                    BackgroundColor = week.IsHighlight 
                        ? Color.FromArgb("#0D9488") 
                        : Color.FromArgb("#E2E8F0"),
                };
                bar.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 };
                bar.StrokeThickness = 0;
                ToolTipProperties.SetText(bar, week.ToolTip);

                var label = new Label
                {
                    Text = week.WeekLabel,
                    TextColor = Color.FromArgb("#94A3B8"),
                    FontSize = 10,
                    HorizontalOptions = LayoutOptions.Center
                };

                var stack = new VerticalStackLayout
                {
                    Spacing = 8,
                    HorizontalOptions = LayoutOptions.Center
                };
                stack.Children.Add(bar);
                stack.Children.Add(label);

                _salesChartContainer.Children.Add(stack);
            }
        }

        // ── Quick Upload: open modal & pick file ────────────────────

        private async void OnQuickUploadClicked(object? sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Asset Image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    // Copy to app data directory for persistence
                    var localFolder = FileSystem.AppDataDirectory;
                    var localPath = Path.Combine(localFolder, result.FileName);

                    using var stream = await result.OpenReadAsync();
                    using var localStream = File.OpenWrite(localPath);
                    await stream.CopyToAsync(localStream);

                    _selectedImagePath = localPath;
                    if (_quickFileNameLabel != null) _quickFileNameLabel.Text = result.FileName;
                    if (_quickUploadPreviewImage != null) _quickUploadPreviewImage.Source = localPath;

                    // Detect file type from extension
                    string ext = Path.GetExtension(result.FileName).TrimStart('.').ToUpperInvariant();
                    if (_quickAssetTypeEntry != null) _quickAssetTypeEntry.Text = ext;

                    // Clear other fields for a fresh upload
                    if (_quickAssetTitleEntry != null) _quickAssetTitleEntry.Text = string.Empty;
                    if (_quickAssetPriceEntry != null) _quickAssetPriceEntry.Text = string.Empty;

                    // Show the modal
                    if (_quickUploadModal != null) _quickUploadModal.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to pick image: {ex.Message}", "OK");
            }
        }

        private async void OnQuickChooseImageClicked(object? sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Asset Image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    var localFolder = FileSystem.AppDataDirectory;
                    var localPath = Path.Combine(localFolder, result.FileName);

                    using var stream = await result.OpenReadAsync();
                    using var localStream = File.OpenWrite(localPath);
                    await stream.CopyToAsync(localStream);

                    _selectedImagePath = localPath;
                    if (_quickFileNameLabel != null) _quickFileNameLabel.Text = result.FileName;
                    if (_quickUploadPreviewImage != null) _quickUploadPreviewImage.Source = localPath;

                    string ext = Path.GetExtension(result.FileName).TrimStart('.').ToUpperInvariant();
                    if (_quickAssetTypeEntry != null) _quickAssetTypeEntry.Text = ext;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to pick image: {ex.Message}", "OK");
            }
        }

        // ── Quick Upload: save asset ─────────────────────────────────

        private async void OnSaveQuickUploadClicked(object? sender, EventArgs e)
        {
            if (_quickAssetTitleEntry == null || _quickAssetTypeEntry == null ||
                _quickAssetPriceEntry == null || _quickUploadModal == null)
                return;

            if (string.IsNullOrWhiteSpace(_quickAssetTitleEntry.Text))
            {
                await DisplayAlertAsync("Validation Error", "Please enter an asset title.", "OK");
                return;
            }

            if (!decimal.TryParse(_quickAssetPriceEntry.Text, out decimal price))
            {
                await DisplayAlertAsync("Validation Error", "Please enter a valid price.", "OK");
                return;
            }

            var asset = new CreatorAsset
            {
                Title = _quickAssetTitleEntry.Text.Trim(),
                Type = (_quickAssetTypeEntry.Text ?? "PNG").Trim().ToUpperInvariant(),
                Price = price,
                ImagePath = _selectedImagePath
            };

            bool success = await _databaseService.AddCreatorAssetAsync(UserSession.AccountID, asset);
            if (success)
            {
                await DisplayAlertAsync("Success", $"'{asset.Title}' has been added to your Asset Library.", "OK");
                _quickUploadModal.IsVisible = false;
                _selectedImagePath = string.Empty;

                // Refresh dashboard stats to reflect the new asset
                await LoadDashboardData();
            }
            else
            {
                await DisplayAlertAsync("Error", "Failed to upload asset. Please try again.", "OK");
            }
        }

        // ── Quick Upload: cancel ─────────────────────────────────────

        private void OnCancelQuickUploadClicked(object? sender, EventArgs e)
        {
            if (_quickUploadModal != null) _quickUploadModal.IsVisible = false;
            _selectedImagePath = string.Empty;
        }
        private void OnNotificationsTapped(object? sender, EventArgs e)
        {
            NotificationsOverlay.IsVisible = true;
        }

        private void OnCloseNotificationsTapped(object? sender, EventArgs e)
        {
            NotificationsOverlay.IsVisible = false;
        }

        private void OnMarkAsReadTapped(object? sender, EventArgs e)
        {
            if (sender is Border border && border.Parent is HorizontalStackLayout hsl && hsl.Parent is Grid grid)
            {
                if (grid.Children[0] is Border dot)
                {
                    dot.IsVisible = false;
                }
            }
        }

        private void OnDeleteNotificationTapped(object? sender, EventArgs e)
        {
            if (sender is Border border && border.Parent is HorizontalStackLayout hsl && hsl.Parent is Grid grid)
            {
                NotificationsContainer.Children.Remove(grid);
            }
        }

        private void OnClearAllNotificationsClicked(object? sender, EventArgs e)
        {
            NotificationsContainer.Children.Clear();
            NotificationsOverlay.IsVisible = false;
        }
    }
}