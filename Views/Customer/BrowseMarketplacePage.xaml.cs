using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace CreatiSphere.Views.Customer
{
    public partial class BrowseMarketplacePage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private const int CurrentUserId = 1002; // Mocked for demonstration
        private const string CartKey = "CartItems";
        private const string CartDelimiter = "|";
        private List<Product> _products = new();
        private List<Services.CartItem> _cartItems = new();
        private PaymentMethod _selectedPaymentMethod = PaymentMethod.Card;
        private Border? _productCard1;
        private Border? _productCard2;
        private Border? _productCard3;
        private Border? _productCard4;
        private Label? _cartTotalLabel;
        private Grid? _paymentOverlay;
        private Label? _paymentTotalLabel;
        private VerticalStackLayout? _paymentCardPanel;
        private VerticalStackLayout? _paymentWalletPanel;
        private Border? _paymentCardTab;
        private Border? _paymentWalletTab;

        public BrowseMarketplacePage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            BindUiReferences();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserInfo();
            await LoadProductsAsync();
            await UpdateCartBadgeAsync();
            SetPaymentMethod(_selectedPaymentMethod);
        }

        private async Task LoadUserInfo()
        {
            try
            {
                var user = await _databaseService.GetAccountInfoByIdAsync(GetCurrentUserId());
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
            await Shell.Current.GoToAsync("//CustomerDashboard");
        }

        private async void OnCustomOrdersTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//CustomOrdersPage");
        }

        private async void OnTrackOrdersTapped(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//TrackOrdersPage");
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

        private async void OnAddToCartTapped(object? sender, TappedEventArgs e)
        {
            var productId = e.Parameter as string;
            if (string.IsNullOrWhiteSpace(productId))
            {
                return;
            }

            var product = _products.FirstOrDefault(p => string.Equals(p.ProductID, productId, StringComparison.OrdinalIgnoreCase));
            if (product == null)
            {
                return;
            }

            var items = await GetCartItemIdsAsync();
            items.Add(product.ProductID ?? productId);
            await SecureStorage.Default.SetAsync(CartKey, string.Join(CartDelimiter, items));

            await RefreshCartAsync();
            UpdateCartBadge(items.Count);
        }

        private async void OnCartTapped(object? sender, EventArgs e)
        {
            await RefreshCartAsync();
            CartOverlay.IsVisible = true;
        }

        private void OnCloseCartTapped(object? sender, EventArgs e)
        {
            CartOverlay.IsVisible = false;
        }

        private async void OnCheckoutClicked(object? sender, EventArgs e)
        {
            await RefreshCartAsync();
            if (_cartItems.Count == 0)
            {
                await DisplayAlert("Checkout", "Your cart is empty.", "OK");
                return;
            }

            if (_paymentTotalLabel != null && _cartTotalLabel != null)
            {
                _paymentTotalLabel.Text = _cartTotalLabel.Text;
            }

            if (_paymentOverlay != null)
            {
                _paymentOverlay.IsVisible = true;
            }
        }

        private void OnClosePaymentTapped(object? sender, EventArgs e)
        {
            if (_paymentOverlay != null)
            {
                _paymentOverlay.IsVisible = false;
            }
        }

        private void OnPaymentCardTapped(object? sender, EventArgs e)
        {
            SetPaymentMethod(PaymentMethod.Card);
        }

        private void OnPaymentWalletTapped(object? sender, EventArgs e)
        {
            SetPaymentMethod(PaymentMethod.Wallet);
        }

        private async void OnConfirmPaymentClicked(object? sender, EventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                await DisplayAlert("Checkout", "Your cart is empty.", "OK");
                return;
            }

            var transactionId = await _databaseService.RecordSaleTransactionAsync(GetCurrentUserId(), _cartItems, _selectedPaymentMethod.ToString());
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                await DisplayAlert("Checkout", "Payment failed. Please try again.", "OK");
                return;
            }

            await SecureStorage.Default.SetAsync(CartKey, string.Empty);
            _cartItems.Clear();
            UpdateCartBadge(0);
            UpdateCartDrawer(_cartItems);
            if (_paymentOverlay != null)
            {
                _paymentOverlay.IsVisible = false;
            }
            CartOverlay.IsVisible = false;

            await DisplayAlert("Payment Successful", $"Checkout complete. Transaction {transactionId} recorded.", "OK");
        }

        private static async Task<List<string>> GetCartItemIdsAsync()
        {
            var raw = await SecureStorage.Default.GetAsync(CartKey);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return new List<string>();
            }

            return new List<string>(raw.Split(CartDelimiter, StringSplitOptions.RemoveEmptyEntries));
        }

        private async Task UpdateCartBadgeAsync()
        {
            await RefreshCartAsync();
            UpdateCartBadge(_cartItems.Count);
        }

        private void UpdateCartBadge(int count)
        {
            CartBadgeLabel.Text = count.ToString();
            CartBadge.IsVisible = count > 0;
        }

        private void UpdateCartDrawer(IList<Services.CartItem> items)
        {
            CartItemsCollectionView.ItemsSource = items;
            EmptyCartLabel.IsVisible = items.Count == 0;
            if (_cartTotalLabel != null)
            {
                _cartTotalLabel.Text = items.Sum(item => item.Price).ToString("C2");
            }
        }

        private async Task LoadProductsAsync()
        {
            _products = await _databaseService.GetProductsAsync();
            var categoryOrder = new[] { "Stickers", "Art Prints", "Textures", "Brushes" };
            var ordered = _products
                .Where(p => !string.IsNullOrWhiteSpace(p.Category))
                .GroupBy(p => p.Category)
                .OrderBy(g => Array.IndexOf(categoryOrder, g.Key ?? string.Empty))
                .Select(g => g.First())
                .ToList();

            if (ordered.Count > 0 && _productCard1 != null) _productCard1.BindingContext = ordered.ElementAtOrDefault(0);
            if (ordered.Count > 1 && _productCard2 != null) _productCard2.BindingContext = ordered.ElementAtOrDefault(1);
            if (ordered.Count > 2 && _productCard3 != null) _productCard3.BindingContext = ordered.ElementAtOrDefault(2);
            if (ordered.Count > 3 && _productCard4 != null) _productCard4.BindingContext = ordered.ElementAtOrDefault(3);
        }

        private async Task RefreshCartAsync()
        {
            var ids = await GetCartItemIdsAsync();
            _cartItems = ids
                .Select(id => _products.FirstOrDefault(p => string.Equals(p.ProductID, id, StringComparison.OrdinalIgnoreCase)))
                .Where(p => p != null)
                .Select(p => new Services.CartItem
                {
                    ProductId = p!.ProductID ?? string.Empty,
                    Name = p.Name ?? string.Empty,
                    Category = p.Category ?? string.Empty,
                    Price = p.Price
                })
                .ToList();

            UpdateCartDrawer(_cartItems);
        }

        private static int GetCurrentUserId()
        {
            return UserSession.AccountID != 0 ? UserSession.AccountID : CurrentUserId;
        }

        private void SetPaymentMethod(PaymentMethod method)
        {
            _selectedPaymentMethod = method;

            if (_paymentCardPanel != null)
            {
                _paymentCardPanel.IsVisible = method == PaymentMethod.Card;
            }

            if (_paymentWalletPanel != null)
            {
                _paymentWalletPanel.IsVisible = method == PaymentMethod.Wallet;
            }

            if (_paymentCardTab != null)
            {
                _paymentCardTab.StrokeThickness = method == PaymentMethod.Card ? 2 : 1;
                _paymentCardTab.Stroke = method == PaymentMethod.Card ? Color.FromArgb("#0D9488") : Color.FromArgb("#E2E8F0");
                _paymentCardTab.BackgroundColor = method == PaymentMethod.Card ? Color.FromArgb("#F0FDFA") : Colors.White;
            }

            if (_paymentWalletTab != null)
            {
                _paymentWalletTab.StrokeThickness = method == PaymentMethod.Wallet ? 2 : 1;
                _paymentWalletTab.Stroke = method == PaymentMethod.Wallet ? Color.FromArgb("#0D9488") : Color.FromArgb("#E2E8F0");
                _paymentWalletTab.BackgroundColor = method == PaymentMethod.Wallet ? Color.FromArgb("#F0FDFA") : Colors.White;
            }
        }

        private void BindUiReferences()
        {
            _productCard1 = this.FindByName<Border>("ProductCard1");
            _productCard2 = this.FindByName<Border>("ProductCard2");
            _productCard3 = this.FindByName<Border>("ProductCard3");
            _productCard4 = this.FindByName<Border>("ProductCard4");
            _cartTotalLabel = this.FindByName<Label>("CartTotalLabel");
            _paymentOverlay = this.FindByName<Grid>("PaymentOverlay");
            _paymentTotalLabel = this.FindByName<Label>("PaymentTotalLabel");
            _paymentCardPanel = this.FindByName<VerticalStackLayout>("PaymentCardPanel");
            _paymentWalletPanel = this.FindByName<VerticalStackLayout>("PaymentWalletPanel");
            _paymentCardTab = this.FindByName<Border>("PaymentCardTab");
            _paymentWalletTab = this.FindByName<Border>("PaymentWalletTab");
        }
    }

    public enum PaymentMethod
    {
        Card,
        Wallet
    }
}
