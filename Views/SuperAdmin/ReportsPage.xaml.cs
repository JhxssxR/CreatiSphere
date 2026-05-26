using System;
using Microsoft.Maui.Controls;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CreatiSphere.Services;

namespace CreatiSphere.Views.SuperAdmin
{
    public partial class ReportsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private bool _isSidebarCollapsed = false;
        private ReportArchive? _selectedReportForRename = null;

        public ReportsPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadUserData();
        }

        private void LoadUserData()
        {
            UserNameLabel.Text = UserSession.Username ?? "Super Admin";
            UserRoleLabel.Text = UserSession.IsSuperAdmin ? "SUPER ADMIN" : "ADMIN";

            // Extract user initials
            string username = UserSession.Username ?? "Super Admin";
            UserInitialsLabel.Text = (!string.IsNullOrWhiteSpace(username) && username.Length >= 1)
                ? username.Substring(0, 1).ToUpper()
                : "S";
        }

        private async void OnNotificationClicked(object? sender, TappedEventArgs e)
        {
            NotificationBadge.IsVisible = false;
            await DisplayAlertAsync("Notifications", "• 3 new MSME registration requests\n• System audit completed successfully\n• Storage capacity reaching 85% on Node 2", "CLEAR ALL");
        }

        private async void OnProfileClicked(object? sender, TappedEventArgs e)
        {
            await DisplayAlertAsync("Account Settings", $"Logged in as: {UserSession.Username}\nEmail: {UserSession.Email}\nTier: {UserSession.Tier}", "MANAGE ACCOUNT");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadReportStats();
            await LoadReportArchives();
        }

        private async Task LoadReportStats()
        {
            try
            {
                var stats = await _databaseService.GetReportStatsAsync();
                TotalRevenueLabel.Text = stats.TotalRevenue.ToString("C0", new System.Globalization.CultureInfo("en-PH"));
                NewCreatorsLabel.Text = stats.NewCreators.ToString("N0");
                ConversionRateLabel.Text = stats.ConversionRate.ToString("F1") + "%";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading report stats: " + ex.Message);
            }
        }

        private async Task LoadReportArchives()
        {
            try
            {
                var reports = await _databaseService.GetReportArchivesAsync();
                ReportArchivesCollectionView.ItemsSource = reports;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading report archives: " + ex.Message);
            }
        }

        private void OnHamburgerClicked(object? sender, EventArgs e)
        {
            _isSidebarCollapsed = !_isSidebarCollapsed;
            SidebarColumn.Width = _isSidebarCollapsed ? new GridLength(0) : new GridLength(280);
        }

        private void OnGenerateNewReportClicked(object? sender, EventArgs e)
        {
            // Reset and pre-populate modal inputs
            NewReportNameInput.Text = $"System Audit - {DateTime.Now:MMM yyyy}";
            NewReportTypePicker.SelectedIndex = 0;
            NewReportPeriodPicker.SelectedIndex = 2; // Last 30 Days
            NewReportFormatPicker.SelectedIndex = 0; // PDF Document
            GenerateModal.IsVisible = true;
        }

        private void OnCloseGenerateModalClicked(object? sender, EventArgs e)
        {
            GenerateModal.IsVisible = false;
        }

        private async void OnGenerateSubmitClicked(object? sender, EventArgs e)
        {
            string name = NewReportNameInput.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                await DisplayAlertAsync("Validation Error", "Please provide a name for the report.", "OK");
                return;
            }

            GenerateModal.IsVisible = false;
            LoadingOverlay.IsVisible = true;

            // Simulate the processing duration for premium feel
            await Task.Delay(2000);

            try
            {
                string format = NewReportFormatPicker.SelectedItem?.ToString()?.Contains("CSV") == true ? "CSV" : "PDF";
                string fileExt = format.ToLower();
                string safeName = string.Concat(name.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
                string fileName = $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}.{fileExt}";
                string cachePath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);

                // Write dummy visual structure to the file
                if (format == "CSV")
                {
                    await File.WriteAllTextAsync(cachePath, "TransactionID,CustomerName,Amount,Status\nTXN001,John Doe,150.00,Completed\nTXN002,Jane Smith,85.50,Completed");
                }
                else
                {
                    await File.WriteAllTextAsync(cachePath, "%PDF-1.4\n%EOF\nCreatiSphere System Generated PDF Audit Document.");
                }

                var newReport = new ReportArchive
                {
                    Name = name,
                    Type = NewReportTypePicker.SelectedItem?.ToString() ?? "System Growth Audit",
                    Period = NewReportPeriodPicker.SelectedItem?.ToString() ?? "Last 30 Days",
                    Size = format == "CSV" ? "1.2 KB" : "124 KB",
                    Format = format,
                    GeneratedDate = DateTime.Now,
                    FilePath = cachePath
                };

                await _databaseService.AddReportArchiveAsync(newReport);
                await LoadReportArchives();

                await DisplayAlertAsync("Success", $"Report '{name}' generated and added to archives successfully.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Generation Error", $"Failed to generate report: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
            }
        }

        private async void OnDownloadReportClicked(object? sender, EventArgs e)
        {
            var button = sender as Button;
            var report = button?.CommandParameter as ReportArchive;
            if (report == null) return;

            try
            {
                string filePath = report.FilePath;
                
                // If it is an offline/seeded default report, let's verify and synthesize it
                if (!File.Exists(filePath))
                {
                    string fileName = Path.GetFileName(filePath);
                    if (string.IsNullOrEmpty(fileName)) 
                        fileName = $"{report.Name.Replace(" ", "_")}.{report.Format.ToLower()}";
                    filePath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);
                    
                    if (report.Format.Equals("CSV", StringComparison.OrdinalIgnoreCase))
                    {
                        await File.WriteAllTextAsync(filePath, "TransactionID,CustomerName,Amount,Status\nTXN001,John Doe,150.00,Completed\nTXN002,Jane Smith,85.50,Completed");
                    }
                    else
                    {
                        await File.WriteAllTextAsync(filePath, "%PDF-1.4\n%EOF\nCreatiSphere System Seeded PDF Audit Document.");
                    }
                    
                    report.FilePath = filePath;
                }

                if (File.Exists(filePath))
                {
                    // Open the file using default viewer or platform handler
                    bool opened = await Launcher.Default.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath)
                    });
                    
                    if (!opened)
                    {
                        await DisplayAlertAsync("Report Saved", $"The file was successfully saved to your system's local temporary storage:\n\n{filePath}", "OK");
                    }
                }
                else
                {
                    await DisplayAlertAsync("Error", "Could not synthesize report file.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Could not process report: {ex.Message}", "OK");
            }
        }

        private void OnRenameReportClicked(object? sender, EventArgs e)
        {
            var button = sender as Button;
            _selectedReportForRename = button?.CommandParameter as ReportArchive;
            if (_selectedReportForRename == null) return;

            NewReportNameEntry.Text = _selectedReportForRename.Name;
            RenameModal.IsVisible = true;
        }

        private void OnCloseRenameModalClicked(object? sender, EventArgs e)
        {
            RenameModal.IsVisible = false;
            _selectedReportForRename = null;
        }

        private async void OnSaveRenameClicked(object? sender, EventArgs e)
        {
            if (_selectedReportForRename == null) return;
            string newName = NewReportNameEntry.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(newName))
            {
                await DisplayAlertAsync("Validation Error", "Report name cannot be empty.", "OK");
                return;
            }

            RenameModal.IsVisible = false;

            try
            {
                bool success = await _databaseService.UpdateReportNameAsync(_selectedReportForRename.Id, newName);
                if (success)
                {
                    await LoadReportArchives();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Could not rename report: {ex.Message}", "OK");
            }
            finally
            {
                _selectedReportForRename = null;
            }
        }

        private async void OnDeleteReportClicked(object? sender, EventArgs e)
        {
            var button = sender as Button;
            var report = button?.CommandParameter as ReportArchive;
            if (report == null) return;

            bool confirm = await DisplayAlertAsync("Delete Report", $"Are you sure you want to delete '{report.Name}'?", "Yes", "Cancel");
            if (!confirm) return;

            try
            {
                // Delete temporary file from cache
                if (!string.IsNullOrEmpty(report.FilePath) && File.Exists(report.FilePath))
                {
                    try { File.Delete(report.FilePath); } catch { }
                }

                bool success = await _databaseService.DeleteReportArchiveAsync(report.Id);
                if (success)
                {
                    await LoadReportArchives();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Could not delete report: {ex.Message}", "OK");
            }
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

        private async void OnMonitorSystemClicked(object? sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("//MonitorSystemPage");
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
