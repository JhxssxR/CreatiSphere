using CreatiSphere.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CreatiSphere.Views.Creator
{
    public partial class AssetEditorPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private CreatorAsset _assetToEdit;
        private string _selectedImagePath = string.Empty;
        private bool _isEditMode = false;

        public AssetEditorPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _assetToEdit = new CreatorAsset();
            PageTitleLabel.Text = "Upload New Asset";
        }

        public AssetEditorPage(CreatorAsset assetToEdit)
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            _assetToEdit = assetToEdit;
            _isEditMode = true;
            PageTitleLabel.Text = "Edit Asset";
            SaveButton.Text = "Update Asset";
            
            TitleEntry.Text = _assetToEdit.Title;
            TypeEntry.Text = _assetToEdit.Type;
            PriceEntry.Text = _assetToEdit.Price.ToString("F2");
            
            if (!string.IsNullOrEmpty(_assetToEdit.ImagePath))
            {
                _selectedImagePath = _assetToEdit.ImagePath;
                ImagePathLabel.Text = Path.GetFileName(_selectedImagePath);
                PreviewImage.Source = _selectedImagePath;
                PreviewImage.IsVisible = true;
            }
        }

        private async void OnChooseImageClicked(object sender, EventArgs e)
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
                    ImagePathLabel.Text = result.FileName;
                    PreviewImage.Source = localPath;
                    PreviewImage.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to pick image: {ex.Message}", "OK");
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleEntry.Text) || string.IsNullOrWhiteSpace(TypeEntry.Text))
            {
                await DisplayAlert("Validation Error", "Title and Type are required.", "OK");
                return;
            }

            if (!decimal.TryParse(PriceEntry.Text, out decimal price))
            {
                await DisplayAlert("Validation Error", "Please enter a valid price.", "OK");
                return;
            }

            _assetToEdit.Title = TitleEntry.Text.Trim();
            _assetToEdit.Type = TypeEntry.Text.Trim();
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
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to save asset. Please try again.", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}