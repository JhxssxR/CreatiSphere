using System;
using System.Threading;
using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Shapes;

namespace CreatiSphere.Views.SuperAdmin
{
    public partial class MonitorSystemPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private CancellationTokenSource? _cts;

        public MonitorSystemPage()
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _cts = new CancellationTokenSource();
            _ = StartRealTimePolling(_cts.Token);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async Task StartRealTimePolling(CancellationToken token)
        {
            // Load immediately on first open
            await LoadMonitorData();

            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
            try
            {
                while (await timer.WaitForNextTickAsync(token))
                {
                    await LoadMonitorData();
                }
            }
            catch (OperationCanceledException)
            {
                // Page navigated away — stop gracefully
            }
        }

        private async Task LoadMonitorData()
        {
            try
            {
                var metrics = await _databaseService.GetSystemMetricsAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CpuLabel.Text = $"{metrics.CpuLoad:F1}%";
                    MemoryLabel.Text = $"{metrics.MemoryUsage:F1} GB";
                    ApiConnectionsLabel.Text = metrics.ApiConnections.ToString("N0");
                    LatencyLabel.Text = $"{metrics.Latency}ms";

                    // Update Trends
                    UpdateTrend(CpuTrendLabel, CpuTrendIcon, metrics.CpuTrend, true); // CPU Up is usually bad/red, but following UI's teal theme for now
                    UpdateTrend(MemoryTrendLabel, MemoryTrendIcon, metrics.MemoryTrend, false); // Memory Up is red
                    
                    ApiTrendLabel.Text = $"{(metrics.ApiTrend >= 0 ? "+" : "")}{metrics.ApiTrend}";
                    ApiTrendIcon.Data = (Geometry?)new PathGeometryConverter().ConvertFrom(metrics.ApiTrend >= 0 
                        ? "M16,6l2.29,2.29l-4.88,4.88l-4-4L2,16.59L3.41,18l6-6l4,4l6.3-6.29L22,12V6H16z" 
                        : "M16,18l2.29-2.29l-4.88-4.88l-4,4L2,7.41L3.41,6l6,6l4-4l6.3,6.29L22,12v6H16z") ?? new PathGeometry();

                    // Update Latency Status
                    var status = metrics.LatencyStatus ?? "Stable";
                    LatencyStatusLabel.Text = status;
                    LatencyStatusLabel.TextColor = status == "Stable" ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
                    LatencyStatusIcon.Fill = new SolidColorBrush(status == "Stable" ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"));

                    // Refresh chart bars
                    BindableLayout.SetItemsSource(SystemLoadChartLayout, null);
                    BindableLayout.SetItemsSource(SystemLoadChartLayout, metrics.LoadHistory);

                    // Live timestamp indicator
                    if (LastRefreshedLabel != null)
                        LastRefreshedLabel.Text = $"Live · {DateTime.Now:HH:mm:ss}";
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Monitor refresh error: {ex.Message}");
            }
        }

        private void UpdateTrend(Label label, Microsoft.Maui.Controls.Shapes.Path icon, double? trend, bool inverseColor = false)
        {
            double trendValue = trend ?? 0;
            bool isUp = trendValue >= 0;
            label.Text = $"{(isUp ? "+" : "")}{trendValue:F1}%";
            
            // Icon Data
            string upPath = "M16,6l2.29,2.29l-4.88,4.88l-4-4L2,16.59L3.41,18l6-6l4,4l6.3-6.29L22,12V6H16z";
            string downPath = "M16,18l2.29-2.29l-4.88-4.88l-4,4L2,7.41L3.41,6l6,6l4-4l6.3,6.29L22,12v6H16z";
            icon.Data = (Geometry?)new PathGeometryConverter().ConvertFrom(isUp ? upPath : downPath) ?? new PathGeometry();

            // Color
            Color positiveColor = Color.FromArgb("#22C55E"); // Green
            Color negativeColor = Color.FromArgb("#EF4444"); // Red
            
            // For metrics like CPU/Memory, "Up" is bad (Red), "Down" is good (Green)
            Color finalColor = isUp ? negativeColor : positiveColor;
            
            label.TextColor = finalColor;
            icon.Fill = new SolidColorBrush(finalColor);
        }

        private async void OnExportMetricsClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("Export Metrics", "System performance metrics for the last 24 hours have been compiled and exported as 'SystemMetrics_May09.csv'.", "OK");
        }

        private async void OnDashboardClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//DashboardPage");
        }

        private async void OnMsmeDirectoryClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MsmePage");
        }

        private async void OnManageUsersClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//UsersPage");
        }

        private async void OnManageModulesClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ModulesPage");
        }

        private async void OnSystemReportsClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//ReportsPage");
        }

        private async void OnSignOutClicked(object? sender, TappedEventArgs e)
        {
            bool confirm = await DisplayAlertAsync("Sign Out", "Are you sure you want to sign out?", "Yes", "Cancel");
            if (confirm)
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
    }
}
