using Microsoft.Win32;

namespace AutoOS.Views.Installer
{
    public sealed partial class HomeLandingPage : Page
    {
        public HomeLandingPage()
        {
            InitializeComponent();
            this.Loaded += HomeLandingPage_Loaded;
        }

        private async void HomeLandingPage_Loaded(object sender, RoutedEventArgs e)
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            if (key == null) return;

            if (key.GetValue("InstallDate") is int unixSeconds)
            {
                var installDate = DateTimeOffset.FromUnixTimeSeconds(unixSeconds).LocalDateTime;
                if ((DateTime.Now - installDate).TotalDays > 2)
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Fresh Windows Required",
                        Content = "AutoOS currently only on fresh installations of Windows.\nPlease follow the Getting Started guide in the README on GitHub.",
                        CloseButtonText = "OK",
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = App.MainWindow.Content.XamlRoot
                    };
                    await dialog.ShowAsync();
                    Application.Current.Exit();
                    return;
                }
            }

            string buildStr = key.GetValue("CurrentBuild")?.ToString() ?? "";
            string ubrStr = key.GetValue("UBR")?.ToString() ?? "";
            if (int.TryParse(buildStr, out int build) && int.TryParse(ubrStr, out int ubr))
            {
                if (build != 22631 || (build == 22631 && ubr < 5000))
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Unsupported Windows Version",
                        Content = $"AutoOS is currently only supported on new versions of Windows 23H2. \nPlease download it from the Getting Started guide in the README on GitHub.",
                        CloseButtonText = "OK",
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = App.MainWindow.Content.XamlRoot
                    };
                    await dialog.ShowAsync();
                    Application.Current.Exit();
                }
            }
        }
    }
}