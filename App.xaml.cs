using Microsoft.Extensions.DependencyInjection;
#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

namespace CreatiSphere;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());
        
#if WINDOWS
        window.Created += (s, e) =>
        {
            var currentApp = Microsoft.Maui.Controls.Application.Current;
            if (currentApp?.Windows != null && currentApp.Windows.Count > 0)
            {
                var platformView = currentApp.Windows[0].Handler?.PlatformView as Microsoft.UI.Xaml.Window;
                if (platformView != null)
                {
                    var handle = WinRT.Interop.WindowNative.GetWindowHandle(platformView);
                    var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                    if (appWindow?.Presenter is Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter)
                    {
                        overlappedPresenter.Maximize();
                    }
                }
            }
        };
#endif
		return window;
	}
}
