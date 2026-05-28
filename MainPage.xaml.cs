using CreatiSphere.Services;
using Microsoft.Maui.Controls.Shapes;
using System.Linq;

namespace CreatiSphere;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public MainPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    private void OnTogglePasswordClicked(object? sender, TappedEventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        // Toggle the eye icon data between open and closed state
        if (PasswordEntry.IsPassword)
        {
            // Eye Open
            EyeIconPath.Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString("M12,4.5C7,4.5,2.7,7.6,1,12c1.7,4.4,6,7.5,11,7.5s9.3-3.1,11-7.5C21.3,7.6,17,4.5,12,4.5z M12,17c-2.8,0-5-2.2-5-5s2.2-5,5-5s5,2.2,5,5S14.8,17,12,17z M12,9c-1.7,0-3,1.3-3,3s1.3,3,3,3s3-1.3,3-3S13.7,9,12,9z")!;
        }
        else
        {
            // Eye Closed
            EyeIconPath.Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString("M12,7c-2.76,0-5,2.24-5,5c0,0.65,0.13,1.26,0.36,1.82l2.92-2.92c0.41-0.23,0.89-0.4,1.41-0.4c0.11,0,0.22,0.01,0.32,0.02L12,7z M2,4.27l2.28,2.28l0.46,0.46C3.08,8.3,1.78,10.02,1,12c1.73,4.39,6,7.5,11,7.5c1.55,0,3.03-0.3,4.38-0.84l0.42,0.42L19.73,22L21,20.73L3.27,3L2,4.27z M7.53,9.8l1.55,1.55c-0.05,0.21-0.08,0.43-0.08,0.65c0,1.66,1.34,3,3,3c0.22,0,0.44-0.03,0.65-0.08l1.55,1.55C13.44,16.81,12.75,17,12,17c-2.76,0-5-2.24-5-5C7,11.25,7.19,10.56,7.53,9.8z M11.88,9.12l2.85,2.85C14.89,11.32,15,10.68,15,10c0-1.66-1.34-3-3-3C11.32,7,10.68,7.11,10.03,7.27L11.88,9.12z M23,12c-1.73-4.39-6-7.5-11-7.5c-1.27,0-2.48,0.2-3.6,0.56l2.12,2.12C11.16,7.06,11.58,7,12,7c2.76,0,5,2.24,5,5c0,0.42-0.06,0.84-0.17,1.23l3.05,3.05C21.22,15.7,22.22,13.98,23,12z")!;
        }
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            string username = UsernameEntry.Text?.Trim() ?? "";
            string password = PasswordEntry.Text ?? "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlertAsync("Error", "Please enter both email and password.", "OK");
                return;
            }

            btn.IsEnabled = false;
            var originalText = btn.Text;
            btn.Text = "Authenticating...";

            AccountInfo? account = null;
            string? loginError = null;

            try
            {
                account = await _databaseService.ValidateLogin(username, password);
            }
            catch (Exception ex)
            {
                loginError = ex.Message;
            }

            btn.IsEnabled = true;
            btn.Text = originalText;

            if (loginError != null)
            {
                await DisplayAlertAsync("Connection Error",
                    $"Could not connect to the database.\n\nDetails: {loginError}\n\nTip: Use the offline fallback credentials instead.",
                    "OK");
                return;
            }

            if (account != null)
            {
                // Populate UserSession
                UserSession.AccountID = account.AccountID;
                UserSession.Username = account.AccountName;
                UserSession.Email = account.Email;
                UserSession.RoleID = account.RoleID;
                
                // Assign tiers based on role or fetch from DB for Admin
                if (account.RoleID == 2) // Admin
                {
                    // Check local storage first for a persisted tier (survives logout)
                    string savedTier = Preferences.Get($"Tier_{account.AccountID}", "");
                    if (!string.IsNullOrEmpty(savedTier))
                    {
                        UserSession.Tier = savedTier;
                    }
                    else
                    {
                        var msmes = await _databaseService.GetMsmesAsync();
                        var userMsme = msmes.FirstOrDefault(m => m.OwnerName == account.AccountName);
                        UserSession.Tier = userMsme?.ErpTier ?? "Starter";
                    }
                }
                else if (account.RoleID == 1) // Super Admin
                {
                    UserSession.Tier = "Enterprise Plus";
                }

                // Route based on RoleID
                switch (account.RoleID)
                {
                    case 1: // Super Admin
                        await Shell.Current.GoToAsync("//DashboardPage");
                        break;
                    case 2: // Admin
                        await Shell.Current.GoToAsync("//AdminDashboard");
                        break;
                    case 3: // Customer
                        await Shell.Current.GoToAsync("//CustomerDashboard");
                        break;
                    case 4: // Creator
                        await Shell.Current.GoToAsync("//CreatorDashboardPage");
                        break;
                    case 5: // Sales Staff
                        await Shell.Current.GoToAsync("//SalesDashboardPage");
                        break;
                    case 6: // Finance
                        await Shell.Current.GoToAsync("//FinanceDashboardPage");
                        break;
                    default:
                        await Shell.Current.GoToAsync("//CustomerDashboard");
                        break;
                }
            }
            else
            {
                await DisplayAlertAsync("Login Failed",
                    "Incorrect email or password.\n\nMake sure:\n• Email matches your account exactly\n• Password is correct\n• Account is active",
                    "Try Again");
            }
        }
    }
}
