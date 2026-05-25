using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;

namespace CreatiSphere.Views.Creator
{
    public partial class AssetLibraryPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private Border? _featuredAssetCard;
        private CollectionView? _assetCardsCollectionView;
        private Label? _allAssetsLabel;
        private Label? _pngCountLabel;
        private Label? _svgCountLabel;
        private Label? _jpgCountLabel;
        private Label? _assetsCountLabel;
        private Label? _profileNameLabel;
        private Label? _profileRoleLabel;
        private Label? _profileInitialsLabel;
        private Label? _accountNameLabel;

        public AssetLibraryPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            BindUiReferences();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAssetsAsync();
            await LoadProfileAsync();
        }

        private async Task LoadAssetsAsync()
        {
            var assets = await _databaseService.GetCreatorAssetsAsync(UserSession.AccountID);
            var featured = assets.FirstOrDefault();
            if (_featuredAssetCard != null)
            {
                _featuredAssetCard.BindingContext = featured ?? new CreatorAsset();
            }

            if (_assetCardsCollectionView != null)
            {
                _assetCardsCollectionView.ItemsSource = assets.Skip(1).Take(4).ToList();
            }

            UpdateAssetCounts(assets);
        }

        private async Task LoadProfileAsync()
        {
            var user = await _databaseService.GetAccountInfoByIdAsync(UserSession.AccountID);
            if (user == null)
            {
                if (_profileNameLabel != null) _profileNameLabel.Text = string.Empty;
                if (_accountNameLabel != null) _accountNameLabel.Text = string.Empty;
                if (_profileRoleLabel != null) _profileRoleLabel.Text = string.Empty;
                if (_profileInitialsLabel != null) _profileInitialsLabel.Text = string.Empty;
                return;
            }

            string name = user.AccountName ?? string.Empty;
            if (_profileNameLabel != null) _profileNameLabel.Text = name;
            if (_accountNameLabel != null) _accountNameLabel.Text = name;
            if (_profileRoleLabel != null) _profileRoleLabel.Text = GetRoleLabel(user.RoleID);
            if (_profileInitialsLabel != null)
            {
                _profileInitialsLabel.Text = name.Length >= 2 ? name.Substring(0, 2).ToUpper() : string.Empty;
            }
        }

        private static string GetRoleLabel(int roleId)
        {
            return roleId switch
            {
                4 => "Creator",
                _ => string.Empty
            };
        }

        private void UpdateAssetCounts(IList<CreatorAsset> assets)
        {
            int total = assets.Count;
            int pngCount = assets.Count(a => string.Equals(a.Type, "PNG", StringComparison.OrdinalIgnoreCase));
            int svgCount = assets.Count(a => string.Equals(a.Type, "SVG", StringComparison.OrdinalIgnoreCase));
            int jpgCount = assets.Count(a => string.Equals(a.Type, "JPG", StringComparison.OrdinalIgnoreCase) || string.Equals(a.Type, "JPEG", StringComparison.OrdinalIgnoreCase));

            if (_allAssetsLabel != null) _allAssetsLabel.Text = $"All Assets ({total})";
            if (_pngCountLabel != null) _pngCountLabel.Text = $"PNG: {pngCount}";
            if (_svgCountLabel != null) _svgCountLabel.Text = $"SVG: {svgCount}";
            if (_jpgCountLabel != null) _jpgCountLabel.Text = $"JPG: {jpgCount}";
            if (_assetsCountLabel != null) _assetsCountLabel.Text = $"Showing {total} assets";
        }

        private void BindUiReferences()
        {
            _featuredAssetCard = (Border)FindByName("FeaturedAssetCard");
            _assetCardsCollectionView = (CollectionView)FindByName("AssetCardsCollectionView");
            _allAssetsLabel = (Label)FindByName("AllAssetsLabel");
            _pngCountLabel = (Label)FindByName("PngCountLabel");
            _svgCountLabel = (Label)FindByName("SvgCountLabel");
            _jpgCountLabel = (Label)FindByName("JpgCountLabel");
            _assetsCountLabel = (Label)FindByName("AssetsCountLabel");
            _profileNameLabel = (Label)FindByName("ProfileNameLabel");
            _profileRoleLabel = (Label)FindByName("ProfileRoleLabel");
            _profileInitialsLabel = (Label)FindByName("ProfileInitialsLabel");
            _accountNameLabel = (Label)FindByName("AccountNameLabel");
        }

        private async void OnDashboardTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatorDashboardPage");
        }

        private async void OnCommissionsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CommissionsPage");
        }

        private async void OnCatalogManagerTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CatalogManagerPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
