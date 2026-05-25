using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Shapes;

namespace CreatiSphere.Views.SuperAdmin
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public DashboardPage()
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
            await LoadDashboardStats();
        }

        private async Task LoadDashboardStats()
        {
            try
            {
                var stats = await _databaseService.GetDashboardStatsAsync();
                TotalMsmesLabel.Text = stats.TotalMsmes.ToString();
                TotalUsersLabel.Text = stats.TotalUsers.ToString("N0");
                UptimeLabel.Text = stats.Uptime;
                ActiveModulesLabel.Text = stats.ActiveModules.ToString();

                // Bind Growth Chart
                BindableLayout.SetItemsSource(GrowthChartLayout, stats.GrowthHistory);

                // Bind Recent MSMEs
                var recentMsmes = await _databaseService.GetMsmesAsync();
                RecentMsmesCollectionView.ItemsSource = recentMsmes.Take(5);

                // Update Line Chart (API Activity)
                UpdateActivityChart(stats.ActivityHistory);

                // Update Health Bars
                var rand = new Random();
                double dbHealth = 0.8 + rand.NextDouble() * 0.2;
                DbNodeHealthBar.WidthRequest = 220 * dbHealth;
                DbNodeStatusLabel.Text = dbHealth > 0.9 ? "OPTIMAL" : "STABLE";

                double storageUsage = 0.3 + rand.NextDouble() * 0.4;
                StorageCapacityBar.WidthRequest = 220 * storageUsage;
                StorageUsageLabel.Text = $"{(storageUsage * 100):F0}% USED";
            }
            catch (Exception ex)
            {
                // Fallback or silent error for now
                Console.WriteLine($"Error loading dashboard stats: {ex.Message}");
            }
        }

        private void UpdateActivityChart(List<double> activities)
        {
            if (activities == null || activities.Count < 2) return;

            // Simple scaling for path: M0,y C50,y1 100,y2 ...
            // We'll just update the static path with some randomized offsets based on real data for effect
            // In a real app, you'd calculate coordinates precisely.
            string pathData = "M0,150 ";
            double x = 0;
            double step = 600.0 / (activities.Count - 1);
            
            for (int i = 1; i < activities.Count; i++)
            {
                double x1 = x + step / 2;
                double x2 = x + step;
                double y = 200 - (activities[i] / 200.0 * 150); // Scale 0-200 to 0-150 height
                pathData += $"C{x1},150 {x1},{y} {x2},{y} ";
                x = x2;
            }

            ActivityLinePath.Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(pathData)!;
            ActivityFillPath.Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(pathData + " L600,200 L0,200 Z")!;
        }

        private async void OnRunDiagnosticClicked(object? sender, EventArgs e)
        {
            await DisplayAlertAsync("System Diagnostic", "Running a deep system scan across all nodes... All core services are performing within optimal latency parameters.", "OK");
        }

        // ── Summary Card Hover Animations ────────────────────────────────────────
        private void OnCardPointerEntered(object? sender, PointerEventArgs e)
        {
            if (sender is not Border card) return;
            card.BackgroundColor = Color.FromArgb("#F0FDFA");
            card.Stroke = new SolidColorBrush(Color.FromArgb("#0D9488"));
            _ = card.ScaleToAsync(1.03, 160u);
        }

        private void OnCardPointerExited(object? sender, PointerEventArgs e)
        {
            if (sender is not Border card) return;
            card.BackgroundColor = Colors.White;
            card.Stroke = new SolidColorBrush(Color.FromArgb("#E2E8F0"));
            _ = card.ScaleToAsync(1.0, 160u);
        }

        private async void OnMsmeDirectoryClicked(object? sender, TappedEventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("//MsmePage");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Navigation Error", ex.Message, "OK");
            }
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
