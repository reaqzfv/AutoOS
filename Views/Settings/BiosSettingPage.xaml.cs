using AutoOS.Views.Settings.BIOS;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AutoOS.Views.Settings;

public sealed partial class BiosSettingPage : Page
{
    private readonly string nvram = Path.Combine(PathHelper.GetAppDataFolderPath(), "SCEWIN", "nvram.txt");
    private readonly ObservableCollection<BiosSettingModel> biosSettings = [];
    private readonly ObservableCollection<BiosSettingModel> recommendedSettings = [];
    private readonly List<BiosSettingModel> allSettings = [];
    
    public BiosSettingPage()
    {
        InitializeComponent();

        RecommendedSettingsListView.ItemsSource = recommendedSettings;
        SettingsListView.ItemsSource = biosSettings;
        
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        // export nvram
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "SCEWIN", "SCEWIN_64.exe"),
                Arguments = @$"/o /s ""{nvram}""",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            }
        };

        process.Start();
        Debug.WriteLine(await process.StandardError.ReadToEndAsync());
        await process.WaitForExitAsync();

        // add error checks

        List<BiosSettingModel> parsedList;

        using var stream = File.OpenRead(nvram);
        parsedList = await Task.Run(() =>
        {
            var settings = BiosSettingParser.ParseFromStream(stream).ToList();

            foreach (var setting in settings)
            {
                foreach (var option in setting.Options)
                    option.Parent = setting;

                setting.InitializeSelectedOption();

                var rule = BiosSettingRecommendationsList.Rules
                    .FirstOrDefault(r =>
                        string.Equals(r.SetupQuestion?.Trim(), setting.SetupQuestion?.Trim(), StringComparison.OrdinalIgnoreCase));

                if (rule != null)
                {
                    string recommendedLabel = rule.RecommendedOption?.Trim().ToLowerInvariant();
                    string selectedLabel = setting.SelectedOption?.Label?.Trim().ToLowerInvariant();

                    var recommended = setting.Options
                        .FirstOrDefault(o => o.Label?.Trim().ToLowerInvariant() == recommendedLabel);

                    if (recommended != null && selectedLabel != recommended.Label?.ToLowerInvariant())
                    {
                        setting.RecommendedOption = recommended;
                        setting.IsRecommended = true;
                    }
                    else if (setting.HasValueField)
                    {
                        string currentValue = setting.Value?.Trim().ToLowerInvariant();
                        if (!string.IsNullOrEmpty(currentValue) && currentValue != recommendedLabel)
                        {
                            setting.IsRecommended = true;
                            setting.RecommendedValue = rule.RecommendedOption;
                        }
                    }
                }

                setting.MarkLoaded();
            }

            return settings;
        });

        var ruleOrder = BiosSettingRecommendationsList.Rules
         .Select((r, i) => new { r.SetupQuestion, Index = i })
         .ToDictionary(x => x.SetupQuestion.ToLowerInvariant(), x => x.Index);

        var sortedRecommended = parsedList
            .Where(s => s.IsRecommended)
            .OrderBy(s => ruleOrder.TryGetValue(s.SetupQuestion.ToLowerInvariant(), out var index) ? index : int.MaxValue)
            .ThenBy(s => s.SetupQuestion, StringComparer.OrdinalIgnoreCase)
            .ToList();

        biosSettings.Clear();
        recommendedSettings.Clear();
        allSettings.Clear();

        foreach (var setting in parsedList)
        {
            biosSettings.Add(setting);
            allSettings.Add(setting);
        }

        foreach (var setting in sortedRecommended)
        {
            recommendedSettings.Add(setting);
        }

        // show settings
        SwitchPresenter.Value = "False";
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is DevWinUI.TextBox tb)
        {
            string query = tb.Text.Trim().ToLower();
            biosSettings.Clear();

            foreach (var setting in allSettings)
            {
                if (string.IsNullOrEmpty(query) || setting.SetupQuestion?.ToLower().Contains(query) == true)
                {
                    biosSettings.Add(setting);
                }
            }
        }
    }

    private void MergeChanges_Click(object sender, RoutedEventArgs e)
    {
        foreach (var setting in recommendedSettings)
        {
            if (setting.OriginalValue == null)
                setting.OriginalValue = setting.Value;

            if (setting.OriginalSelectedOption == null)
                setting.OriginalSelectedOption = setting.SelectedOption;

            if (setting.RecommendedOption != null)
            {
                foreach (var option in setting.Options)
                    option.IsSelected = (option == setting.RecommendedOption);
            }
            else if (!string.IsNullOrEmpty(setting.RecommendedValue))
            {
                setting.Value = setting.RecommendedValue;
            }
        }
    }

    private async void Import_Click(object sender, RoutedEventArgs e)
    {
        //// import nvram
        //using var process = new Process
        //{
        //    StartInfo = new ProcessStartInfo
        //    {
        //        FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "SCEWIN", "SCEWIN_64.exe"),
        //        Arguments = @$"/i /s ""{nvram}""",
        //        UseShellExecute = false,
        //        CreateNoWindow = true,
        //        RedirectStandardError = true
        //    }
        //};

        //process.Start();
        //Debug.WriteLine(await process.StandardError.ReadToEndAsync());
        //await process.WaitForExitAsync();

        // add error checks
    }
}
