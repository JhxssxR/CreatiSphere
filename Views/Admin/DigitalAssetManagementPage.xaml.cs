using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Admin
{
    public class AssetFolder
    {
        public string Name { get; set; } = string.Empty;
        public string ItemCount { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string BgColor { get; set; } = string.Empty;
        public string IconColor { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string Avatar1 { get; set; } = string.Empty;
        public string Avatar2 { get; set; } = string.Empty;
        public string PlusCount { get; set; } = string.Empty;
    }

    public partial class DigitalAssetManagementPage : ContentPage
    {
        private readonly Services.DatabaseService _dbService;
        public ObservableCollection<AssetFolder> Folders { get; set; } = new ObservableCollection<AssetFolder>();

        public DigitalAssetManagementPage()
        {
            InitializeComponent();
            _dbService = new Services.DatabaseService();

            Folders.Add(new AssetFolder { Name = "Brush Sets", ItemCount = "1,248", Size = "4.2 GB", BgColor = "#7FFFD4", IconColor = "#0D9488", IconPath = "M3,17.25V21h3.75L17.81,9.94l-3.75-3.75L3,17.25z M20.71,7.04c0.39-0.39,0.39-1.02,0-1.41l-2.34-2.34 c-0.39-0.39-1.02-0.39-1.41,0l-1.83,1.83l3.75,3.75L20.71,7.04z", Avatar1 = "avatar.png", Avatar2 = "artist1.png", PlusCount = "+12" });
            Folders.Add(new AssetFolder { Name = "Stickers", ItemCount = "856", Size = "1.8 GB", BgColor = "#1E293B", IconColor = "White", IconPath = "M18,2H6C4.9,2,4,2.9,4,4v16c0,1.1,0.9,2,2,2h12c1.1,0,2-0.9,2-2V4C20,2.9,19.1,2,18,2z M18,20H6V4h12V20z M14,14H8v2h6V14z M16,10H8v2h8V10z M16,6H8v2h8V6z", Avatar1 = "artist2.png", Avatar2 = "artist3.png", PlusCount = "+8" });
            Folders.Add(new AssetFolder { Name = "Templates", ItemCount = "324", Size = "12.5 GB", BgColor = "#7FFFD4", IconColor = "#0D9488", IconPath = "M4,11h5V5H4V11z M4,18h5v-5H4V18z M10,18h5v-6h-5V18z M16,18h5v-7h-5V18z M10,10h11V5H10V10z", Avatar1 = "avatar.png", Avatar2 = "artist2.png", PlusCount = "+5" });
            
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            HeaderProfileInitials.Text = !string.IsNullOrEmpty(UserSession.Username) ? UserSession.Username.Substring(0, 1).ToUpper() : "U";
            await LoadAssets();
        }

        private async Task LoadAssets()
        {
            try
            {
                if (UserSession.AccountID > 5)
                {
                    Folders.Clear();
                    BindableLayout.SetItemsSource(AssetsList, new List<Services.DigitalAsset>());
                    return;
                }

                var assets = await _dbService.GetAssetsAsync();
                BindableLayout.SetItemsSource(AssetsList, assets);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "Failed to load assets: " + ex.Message, "OK");
            }
        }

        private async void OnNewFolderClicked(object? sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("New Folder", "Enter the name for the new folder:", "Create", "Cancel", "Untitled Folder");
            
            if (!string.IsNullOrWhiteSpace(result))
            {
                Folders.Add(new AssetFolder 
                { 
                    Name = result, 
                    ItemCount = "0", 
                    Size = "0 KB", 
                    BgColor = "#7FFFD4", 
                    IconColor = "#0D9488", 
                    IconPath = "M10 4H4c-1.1 0-1.99.9-1.99 2L2 18c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V8c0-1.1-.9-2-2-2h-8l-2-2z", 
                    Avatar1 = "avatar.png", 
                    Avatar2 = "artist1.png", 
                    PlusCount = "+1" 
                });
                await DisplayAlertAsync("Success", $"Folder '{result}' has been created successfully.", "OK");
            }
        }

        private async void OnFolderTapped(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is AssetFolder folder)
            {
                // Update breadcrumb to show current folder
                CurrentFolderLabel.Text = folder.Name;
                
                // Show a dummy item to simulate opening the folder
                var dummyAssets = new List<Services.DigitalAsset>
                {
                    new Services.DigitalAsset { Name = $"{folder.Name}_Asset_1.png", Size = "2.4 MB", Format = "PNG", Date = "Just now" },
                    new Services.DigitalAsset { Name = $"{folder.Name}_Asset_2.png", Size = "1.1 MB", Format = "PNG", Date = "Just now" }
                };
                BindableLayout.SetItemsSource(AssetsList, dummyAssets);
            }
        }

        private async void OnUploadAssetsClicked(object? sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "Select Assets to Upload"
                });

                if (result != null && result.Any())
                {
                    await DisplayAlertAsync("Upload Success", $"{result.Count()} files selected for upload.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", "File selection failed: " + ex.Message, "OK");
            }
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
            // Already on this page
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
            await Shell.Current.GoToAsync("//CrmManagementPage", false);
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
            bool confirm = await DisplayAlertAsync("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//MainPage", false);
            }
        }

        private async void OnTabTapped(object? sender, TappedEventArgs e)
        {
            string tab = e.Parameter?.ToString() ?? "Recent";
            
            // Update UI
            RecentFilesLabel.TextColor = (Color)Application.Current!.Resources["Slate500"];
            RecentFilesLabel.FontAttributes = FontAttributes.None;
            RecentFilesIndicator.IsVisible = false;
            
            FavoritesLabel.TextColor = (Color)Application.Current!.Resources["Slate500"];
            FavoritesLabel.FontAttributes = FontAttributes.None;
            FavoritesIndicator.IsVisible = false;
            
            TrashLabel.TextColor = (Color)Application.Current!.Resources["Slate500"];
            TrashLabel.FontAttributes = FontAttributes.None;
            TrashIndicator.IsVisible = false;
            
            switch (tab)
            {
                case "Recent":
                    RecentFilesLabel.TextColor = (Color)Application.Current!.Resources["DashboardTeal"];
                    RecentFilesLabel.FontAttributes = FontAttributes.Bold;
                    RecentFilesIndicator.IsVisible = true;
                    break;
                case "Favorites":
                    FavoritesLabel.TextColor = (Color)Application.Current!.Resources["DashboardTeal"];
                    FavoritesLabel.FontAttributes = FontAttributes.Bold;
                    FavoritesIndicator.IsVisible = true;
                    break;
                case "Trash":
                    TrashLabel.TextColor = (Color)Application.Current!.Resources["DashboardTeal"];
                    TrashLabel.FontAttributes = FontAttributes.Bold;
                    TrashIndicator.IsVisible = true;
                    break;
            }
            
            await LoadAssets();
        }

        private void OnViewToggleTapped(object? sender, TappedEventArgs e)
        {
            string view = e.Parameter?.ToString() ?? "List";
            
            GridViewBtn.BackgroundColor = Colors.Transparent;
            GridViewIcon.Fill = (Color)Application.Current!.Resources["Slate400"];
            
            ListViewBtn.BackgroundColor = Colors.Transparent;
            ListViewIcon.Fill = (Color)Application.Current!.Resources["Slate400"];
            
            if (view == "Grid")
            {
                GridViewBtn.BackgroundColor = (Color)Application.Current!.Resources["Slate100"];
                GridViewIcon.Fill = (Color)Application.Current!.Resources["DashboardTeal"];
            }
            else
            {
                ListViewBtn.BackgroundColor = (Color)Application.Current!.Resources["Slate100"];
                ListViewIcon.Fill = (Color)Application.Current!.Resources["DashboardTeal"];
            }
        }
    }
}

