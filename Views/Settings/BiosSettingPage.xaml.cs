using AutoOS.Views.Settings.BIOS;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management;

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

        // copy scewin to localstate because of permissions
        Directory.CreateDirectory(Path.Combine(PathHelper.GetAppDataFolderPath(), "SCEWIN"));
        string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "SCEWIN");
        string destinationPath = Path.Combine(PathHelper.GetAppDataFolderPath(), "SCEWIN");

        foreach (var directory in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            string subDirPath = directory.Replace(sourcePath, destinationPath);
            Directory.CreateDirectory(subDirPath);
        }

        foreach (var file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            string destFilePath = file.Replace(sourcePath, destinationPath);
            File.Copy(file, destFilePath, true);
        }

        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        // show exporting
        SwitchPresenter.Value = "Export";

        // export nvram
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(PathHelper.GetAppDataFolderPath(), "SCEWIN", "SCEWIN_64.exe"),
                Arguments = @$"/o /s ""{nvram}""",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            }
        };

        process.Start();
        string errorOutput = await process.StandardError.ReadToEndAsync();
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        string manufacturer = "Unknown";
        string product = "Unknown";

        using (var searcher = new ManagementObjectSearcher("SELECT Manufacturer, Product FROM Win32_BaseBoard"))
        {
            foreach (ManagementObject mo in searcher.Get().Cast<ManagementObject>())
            {
                manufacturer = mo["Manufacturer"]?.ToString().ToLowerInvariant() ?? "Unknown";
                product = mo["Product"]?.ToString().ToUpperInvariant() ?? "Unknown";
            }
        }

        if (output.Contains("AMISCE is not supported on this system.", StringComparison.OrdinalIgnoreCase))
        {
            SwitchPresenter.Value = "Unsupported";
        }
        else if (errorOutput.Contains("BIOS not compatible", StringComparison.OrdinalIgnoreCase))
        {
            SwitchPresenter.Value = "Incompatible";
        }
        else if (errorOutput.Contains("WARNING: HII data does not have setup questions information", StringComparison.OrdinalIgnoreCase))
        {
            if (manufacturer.Contains("asus") || manufacturer.Contains("asustek"))
            {
                var protectedChipsets = new[] {
                    "Z790", "B760", "H770", "X870", "X670", "B650", "A620"
                };

                if (protectedChipsets.Any(c => product.Contains(c)))
                {
                    SwitchPresenter.Value = "HII Resources (Protected)";
                }
                else
                {
                    SwitchPresenter.Value = "HII Resources (Regular)";
                }
            }
            else
            {
                SwitchPresenter.Value = "HII Resources (Other)";
            }
        }
        else if (errorOutput.Contains("Script file exported successfully.", StringComparison.OrdinalIgnoreCase))
        {
            // export nvram
            using var process2 = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(PathHelper.GetAppDataFolderPath(), "SCEWIN", "SCEWIN_64.exe"),
                    Arguments = @$"/o /s ""{nvram}"" /d",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };

            process2.Start();
            errorOutput = await process2.StandardError.ReadToEndAsync();
            output = await process2.StandardOutput.ReadToEndAsync();
            await process2.WaitForExitAsync();
        }
        
        if (errorOutput.Contains("Script file exported successfully.", StringComparison.OrdinalIgnoreCase))
        {
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
            SwitchPresenter.Value = "Loaded";
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is DevWinUI.TextBox tb)
        {
            string query = tb.Text.Trim().ToLower();
            biosSettings.Clear();

            foreach (var setting in allSettings)
            {
                if (string.IsNullOrEmpty(query) || setting.SetupQuestion?.ToLower().Contains(query, StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    biosSettings.Add(setting);
                }
            }
        }
    }

    private void MergeAll_Click(object sender, RoutedEventArgs e)
    {
        BiosSettingUpdater.IsBatchUpdating = true;

        foreach (var setting in recommendedSettings)
        {
            setting.OriginalValue ??= setting.Value;
            setting.OriginalSelectedOption ??= setting.SelectedOption;

            if (setting.RecommendedOption != null)
            {
                foreach (var option in setting.Options)
                    option.IsSelected = option == setting.RecommendedOption;
            }
            else if (!string.IsNullOrEmpty(setting.RecommendedValue))
            {
                setting.Value = setting.RecommendedValue;
            }
        }

        BiosSettingUpdater.IsBatchUpdating = false;

        var modifiedSettings = recommendedSettings
            .Where(s =>
                (s.OriginalValue != null && s.Value != s.OriginalValue) ||
                (s.OriginalSelectedOption != null && s.SelectedOption != s.OriginalSelectedOption))
            .ToList();

        if (modifiedSettings.Count > 0)
        {
            BiosSettingUpdater.SaveAllSettings(modifiedSettings);
        }
    }

    private async void Import_Click(object sender, RoutedEventArgs e)
    {
        // show importing
        SwitchPresenter.Value = "Import";

        // import nvram
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Applications", "SCEWIN", "SCEWIN_64.exe"),
                Arguments = @$"/i /s ""{nvram}""",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            }
        };

        process.Start();
        string errorOutput = await process.StandardError.ReadToEndAsync();
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        string manufacturer = "Unknown";
        string product = "Unknown";

        using (var searcher = new ManagementObjectSearcher("SELECT Manufacturer, Product FROM Win32_BaseBoard"))
        {
            foreach (ManagementObject mo in searcher.Get().Cast<ManagementObject>())
            {
                manufacturer = mo["Manufacturer"]?.ToString().ToLowerInvariant() ?? "Unknown";
                product = mo["Product"]?.ToString().ToUpperInvariant() ?? "Unknown";
            }
        }

        if (errorOutput.Contains("Warning: Error in writing variable", StringComparison.OrdinalIgnoreCase))
        {
            if (manufacturer.Contains("asus") || manufacturer.Contains("asustek"))
            {
                SwitchPresenter.Value = "Write Protected (ASUS)";
            }
            else if (manufacturer.Contains("asrock"))
            {
                SwitchPresenter.Value = "Write Protected (ASRock)";
            }
            else
            {
                SwitchPresenter.Value = "Write Protected (Other)";
            }
        }
        else if (errorOutput.Contains("Script file imported successfully.", StringComparison.OrdinalIgnoreCase))
        {
            await LoadAsync();
        }
    }
}