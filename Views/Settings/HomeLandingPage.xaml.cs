using CommunityToolkit.Labs.WinUI.MarkdownTextBlock;
using Windows.Storage;

namespace AutoOS.Views.Settings
{
    public sealed partial class HomeLandingPage : Page
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static readonly HttpClient httpClient = new();
        public HomeLandingPage()
        {
            InitializeComponent();
            GetChangeLog();
        }

        public async void GetChangeLog()
        {
            string storedVersion = localSettings.Values["Version"] as string;
            string currentVersion = ProcessInfoHelper.Version;

            if (storedVersion != currentVersion)
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AutoOS");

                using var doc = System.Text.Json.JsonDocument.Parse(await httpClient.GetStringAsync($"https://api.github.com/repos/tinodin/AutoOS/releases/tags/v{currentVersion}"));

                string rawChangelog = doc.RootElement.GetProperty("body").GetString()!;
                string changelog = rawChangelog.Replace("`", "")[rawChangelog.IndexOf("- ")..];

                var contentDialog = new ContentDialog
                {
                    Title = $"What’s new in AutoOS v{currentVersion}",
                    Content = new MarkdownTextBlock
                    {
                        Text = changelog,
                        Margin = new Thickness(0, 12, 0, 0),
                        Config = new MarkdownConfig()
                    },
                    CloseButtonText = "Close",
                    XamlRoot = this.XamlRoot
                };

                contentDialog.Resources["ContentDialogMaxWidth"] = 1000;

                await contentDialog.ShowAsync();

                localSettings.Values["Version"] = currentVersion;
            }
        }
    }
}
