using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // Modal overlay controls
        private Grid? _assetEditorModal;
        private Label? _modalTitleLabel;
        private Entry? _titleEntry;
        private Entry? _typeEntry;
        private Entry? _priceEntry;
        private Label? _imagePathLabel;
        private Image? _previewImage;
        private Button? _saveButton;

        // Modal state
        private CreatorAsset? _assetToEdit;
        private string _selectedImagePath = string.Empty;
        private bool _isEditMode = false;

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

            // Modal overlay binding
            _assetEditorModal = (Grid)FindByName("AssetEditorModal");
            _modalTitleLabel = (Label)FindByName("ModalTitleLabel");
            _titleEntry = (Entry)FindByName("TitleEntry");
            _typeEntry = (Entry)FindByName("TypeEntry");
            _priceEntry = (Entry)FindByName("PriceEntry");
            _imagePathLabel = (Label)FindByName("ImagePathLabel");
            _previewImage = (Image)FindByName("PreviewImage");
            _saveButton = (Button)FindByName("SaveButton");
        }

        private async void OnDashboardTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatorDashboardPage");
        }

        private async void OnCommissionsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CommissionsPage");
        }

        private async void OnMessagesTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CreatorMessagesPage");
        }

        private async void OnCatalogManagerTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CatalogManagerPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private void OnBrowseFilesTapped(object? sender, EventArgs e)
        {
            OpenAssetEditorModal(null);
        }

        private void OnUploadNewArtClicked(object? sender, EventArgs e)
        {
            OpenAssetEditorModal(null);
        }

        private async void OnAssetOptionsTapped(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is CreatorAsset asset)
            {
                string action = await DisplayActionSheet($"Options for {asset.Title}", "Cancel", "Delete", "Edit");
                if (action == "Edit")
                {
                    OpenAssetEditorModal(asset);
                }
                else if (action == "Delete")
                {
                    bool confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete '{asset.Title}'?", "Yes, Delete", "Cancel");
                    if (confirm)
                    {
                        bool success = await _databaseService.DeleteCreatorAssetAsync(asset.AssetID);
                        if (success)
                        {
                            await LoadAssetsAsync(); // Reload UI
                        }
                        else
                        {
                            await DisplayAlert("Error", "Could not delete asset.", "OK");
                        }
                    }
                }
            }
        }

        private void OpenAssetEditorModal(CreatorAsset? asset = null)
        {
            if (_assetEditorModal == null || _modalTitleLabel == null || _titleEntry == null || 
                _typeEntry == null || _priceEntry == null || _imagePathLabel == null || 
                _previewImage == null || _saveButton == null)
            {
                return;
            }

            if (asset != null)
            {
                _assetToEdit = asset;
                _isEditMode = true;
                _modalTitleLabel.Text = "Edit Asset";
                _saveButton.Text = "Update Asset";
                
                _titleEntry.Text = _assetToEdit.Title;
                _typeEntry.Text = _assetToEdit.Type;
                _priceEntry.Text = _assetToEdit.Price.ToString("F2");
                
                if (!string.IsNullOrEmpty(_assetToEdit.ImagePath))
                {
                    _selectedImagePath = _assetToEdit.ImagePath;
                    _imagePathLabel.Text = System.IO.Path.GetFileName(_selectedImagePath);
                    _previewImage.Source = _selectedImagePath;
                    _previewImage.IsVisible = true;
                }
                else
                {
                    _selectedImagePath = string.Empty;
                    _imagePathLabel.Text = "No file selected";
                    _previewImage.Source = null;
                    _previewImage.IsVisible = false;
                }
            }
            else
            {
                _assetToEdit = new CreatorAsset();
                _isEditMode = false;
                _modalTitleLabel.Text = "Upload New Asset";
                _saveButton.Text = "Save Asset";
                
                _titleEntry.Text = string.Empty;
                _typeEntry.Text = string.Empty;
                _priceEntry.Text = string.Empty;
                _selectedImagePath = string.Empty;
                _imagePathLabel.Text = "No file selected";
                _previewImage.Source = null;
                _previewImage.IsVisible = false;
            }
            
            _assetEditorModal.IsVisible = true;
        }

        private async void OnChooseImageClicked(object? sender, EventArgs e)
        {
            if (_imagePathLabel == null || _previewImage == null) return;

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
                    var localPath = System.IO.Path.Combine(localFolder, result.FileName);

                    using var stream = await result.OpenReadAsync();
                    using var localStream = System.IO.File.OpenWrite(localPath);
                    await stream.CopyToAsync(localStream);

                    _selectedImagePath = localPath;
                    _imagePathLabel.Text = result.FileName;
                    _previewImage.Source = localPath;
                    _previewImage.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
            }
        }

        private void OnCancelModalClicked(object? sender, EventArgs e)
        {
            if (_assetEditorModal != null)
            {
                _assetEditorModal.IsVisible = false;
            }
        }

        private async void OnSaveAssetClicked(object? sender, EventArgs e)
        {
            if (_titleEntry == null || _typeEntry == null || _priceEntry == null || _assetEditorModal == null) return;

            if (string.IsNullOrWhiteSpace(_titleEntry.Text) || string.IsNullOrWhiteSpace(_typeEntry.Text))
            {
                await DisplayAlert("Validation Error", "Title and Type are required.", "OK");
                return;
            }

            if (!decimal.TryParse(_priceEntry.Text, out decimal price))
            {
                await DisplayAlert("Validation Error", "Please enter a valid price.", "OK");
                return;
            }

            if (_assetToEdit == null)
            {
                _assetToEdit = new CreatorAsset();
            }

            _assetToEdit.Title = _titleEntry.Text.Trim();
            _assetToEdit.Type = _typeEntry.Text.Trim();
            _assetToEdit.Price = price;
            _assetToEdit.ImagePath = _selectedImagePath;

            bool success;
            if (_isEditMode)
            {
                success = await _databaseService.UpdateCreatorAssetAsync(_assetToEdit);
            }
            else
            {
                success = await _databaseService.AddCreatorAssetAsync(UserSession.AccountID, _assetToEdit);
            }

            if (success)
            {
                await DisplayAlert("Success", "Asset saved successfully.", "OK");
                _assetEditorModal.IsVisible = false;
                await LoadAssetsAsync(); // Reload UI
            }
            else
            {
                await DisplayAlert("Error", "Failed to save asset. Please try again.", "OK");
            }
        }

        private async void OnLoadMoreClicked(object? sender, EventArgs e)
        {
            await DisplayAlert("Load More", "Fetching more assets from the library...", "OK");
        }
    }
}
