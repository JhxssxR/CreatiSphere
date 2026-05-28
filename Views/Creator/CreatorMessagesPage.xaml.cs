using System;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Threading.Tasks;

namespace CreatiSphere.Views.Creator
{
    public partial class CreatorMessagesPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private const int CurrentUserId = 1; // Default creator

        public CreatorMessagesPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserInfo();
            
            ChatCollectionView.ItemsSource = MockMessageService.ChatHistory;
            MockMessageService.HasNewMessageForCreator = false; // Mark as read
            
            // Update conversation list visibility
            if (MockMessageService.ChatHistory.Count > 0)
            {
                ConversationItem.IsVisible = true;
                EmptyChatState.IsVisible = false;
                var lastMsg = MockMessageService.ChatHistory[MockMessageService.ChatHistory.Count - 1];
                LastMessagePreview.Text = lastMsg.Text.Length > 35 ? lastMsg.Text.Substring(0, 35) + "..." : lastMsg.Text;
                LastMessageTime.Text = lastMsg.Timestamp.ToString("h:mm tt");
            }
            else
            {
                ConversationItem.IsVisible = false;
                EmptyChatState.IsVisible = true;
            }
            
            ScrollToBottom();
        }

        private void OnConversationTapped(object? sender, EventArgs e)
        {
            // Already on the messages page with the chat open, just scroll to bottom
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (MockMessageService.ChatHistory.Count > 0)
            {
                ChatCollectionView.ScrollTo(MockMessageService.ChatHistory.Count - 1, position: ScrollToPosition.End, animate: false);
            }
        }

        private void OnSendMessageTapped(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageEntry.Text))
            {
                MockMessageService.AddMessage("Elena Vance (Creator)", MessageEntry.Text, false);
                MessageEntry.Text = string.Empty;
                ScrollToBottom();
            }
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

        private async void OnCommissionsTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CommissionsPage");
        }

        private async void OnSignOutTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private async void OnUploadNewArtClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new AssetEditorPage());
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