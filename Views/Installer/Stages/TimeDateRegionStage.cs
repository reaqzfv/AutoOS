using AutoOS.Views.Installer.Actions;
using Newtonsoft.Json.Linq;
using Microsoft.UI.Xaml.Media;

namespace AutoOS.Views.Installer.Stages;

public static class TimeDateRegionStage
{
    private static string countryCode = null;
    public static async Task Run()
    {
        InstallPage.Status.Text = "Time, Date and Region...";

        string previousTitle = string.Empty;
        int stagePercentage = 2;

        using (HttpClient client = new HttpClient())
        {
            string response = client.GetStringAsync("https://get.geojs.io/v1/ip/geo.json").Result;
            JObject jsonResponse = JObject.Parse(response);

            countryCode = jsonResponse["country_code"]?.ToString();
        }

        var actions = new List<(string Title, Func<Task> Action, Func<bool> Condition)>
        {
            // set time zone
            ($"Setting time zone to {GetWindowsTimeZone(countryCode)}", async () => await ProcessActions.RunPowerShell($@"Set-TimeZone -Id '{GetWindowsTimeZone(countryCode)}'"), null),
            ($"Setting time zone to {GetWindowsTimeZone(countryCode)}", async () => await ProcessActions.Sleep(1000), null),

            // set keyboard layout
            ($"Setting keyboard layout to en-{countryCode}", async () => await ProcessActions.RunPowerShell($@"$langList=Get-WinUserLanguageList;$langList=$langList|Where-Object {{$_.LanguageTag -ne 'en-US'}};$newLang=New-WinUserLanguageList 'en-{countryCode}';$langList+=$newLang[0];Set-WinUserLanguageList $langList -Force"), null),

            // sync time
            ("Syncing time", async () => await ProcessActions.RunNsudo("CurrentUser", "net start w32time"), null),
            ("Syncing time", async () => await ProcessActions.RunNsudo("CurrentUser", "w32tm /resync"), null),
            ("Syncing time", async () => await ProcessActions.RunNsudo("CurrentUser", "net stop w32time"), null),

            // apply changes to the whole system
            ("Applying changes to the whole system", async () => await ProcessActions.RunPowerShell(@"Copy-UserInternationalSettingsToSystem -WelcomeScreen $true -NewUser $true"), null),
        };

        var filteredActions = actions.Where(a => a.Condition == null || a.Condition.Invoke()).ToList();
        int groupedTitleCount = 0;

        List<Func<Task>> currentGroup = [];

        for (int i = 0; i < filteredActions.Count; i++)
        {
            if (i == 0 || filteredActions[i].Title != filteredActions[i - 1].Title)
            {
                groupedTitleCount++;
            }
        }

        double incrementPerTitle = groupedTitleCount > 0 ? stagePercentage / (double)groupedTitleCount : 0;

        foreach (var (title, action, condition) in filteredActions)
        {
            if (previousTitle != string.Empty && previousTitle != title && currentGroup.Count > 0)
            {
                foreach (var groupedAction in currentGroup)
                {
                    try
                    {
                        await groupedAction();
                    }
                    catch (Exception ex)
                    {
                        InstallPage.Info.Title += ": " + ex.Message;
                        InstallPage.Info.Severity = InfoBarSeverity.Error;
                        InstallPage.Progress.Foreground = (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];
                        InstallPage.ProgressRingControl.Foreground = (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];
                        InstallPage.ProgressRingControl.Visibility = Visibility.Collapsed;
                        InstallPage.ResumeButton.Visibility = Visibility.Visible;

                        var tcs = new TaskCompletionSource<bool>();

                        InstallPage.ResumeButton.Click += (sender, e) =>
                        {
                            tcs.TrySetResult(true);
                            InstallPage.Info.Severity = InfoBarSeverity.Informational;
                            InstallPage.Progress.ClearValue(ProgressBar.ForegroundProperty);
                            InstallPage.ProgressRingControl.Foreground = null;
                            InstallPage.ProgressRingControl.Visibility = Visibility.Visible;
                            InstallPage.ResumeButton.Visibility = Visibility.Collapsed;
                        };

                        await tcs.Task;
                    }
                }

                InstallPage.Progress.Value += incrementPerTitle;
                await Task.Delay(150);
                currentGroup.Clear();
            }

            InstallPage.Info.Title = title + "...";
            currentGroup.Add(action);
            previousTitle = title;
        }

        if (currentGroup.Count > 0)
        {
            foreach (var groupedAction in currentGroup)
            {
                try
                {
                    await groupedAction();
                }
                catch (Exception ex)
                {
                    InstallPage.Info.Title += ": " + ex.Message;
                    InstallPage.Info.Severity = InfoBarSeverity.Error;
                    InstallPage.Progress.Foreground = (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];
                    InstallPage.ProgressRingControl.Foreground = (Brush)Application.Current.Resources["SystemFillColorCriticalBrush"];
                    InstallPage.ProgressRingControl.Visibility = Visibility.Collapsed;
                    InstallPage.ResumeButton.Visibility = Visibility.Visible;

                    var tcs = new TaskCompletionSource<bool>();

                    InstallPage.ResumeButton.Click += (sender, e) =>
                    {
                        tcs.TrySetResult(true);
                        InstallPage.Info.Severity = InfoBarSeverity.Informational;
                        InstallPage.Progress.ClearValue(ProgressBar.ForegroundProperty);
                        InstallPage.ProgressRingControl.Foreground = null;
                        InstallPage.ProgressRingControl.Visibility = Visibility.Visible;
                        InstallPage.ResumeButton.Visibility = Visibility.Collapsed;
                    };

                    await tcs.Task;
                }
            }

            InstallPage.Progress.Value += incrementPerTitle;
        }
    }

    private static string GetWindowsTimeZone(string countryCode)
    {
        return countryCode switch
        {
            "AF" => "Afghanistan Standard Time",
            "AL" => "Central European Standard Time",
            "DZ" => "Central European Standard Time",
            "AS" => "UTC-11",
            "AD" => "Central European Standard Time",
            "AO" => "West Africa Standard Time",
            "AG" => "Atlantic Standard Time",
            "AR" => "Argentina Standard Time",
            "AM" => "Armenian Standard Time",
            "AW" => "Atlantic Standard Time",
            "AU" => "AUS Eastern Standard Time",
            "AT" => "Central European Standard Time",
            "AZ" => "Azerbaijan Standard Time",
            "BS" => "Eastern Standard Time",
            "BH" => "Arabian Standard Time",
            "BD" => "Bangladesh Standard Time",
            "BB" => "Atlantic Standard Time",
            "BY" => "Eastern European Standard Time",
            "BE" => "Romance Standard Time",
            "BZ" => "Central Standard Time",
            "BJ" => "West Africa Standard Time",
            "BA" => "Central European Standard Time",
            "BW" => "Central Africa Standard Time",
            "BR" => "Brazil Standard Time",
            "BN" => "Brunei Darussalam Standard Time",
            "BG" => "Eastern European Standard Time",
            "BF" => "West Africa Standard Time",
            "BI" => "Central Africa Standard Time",
            "KH" => "Indochina Time",
            "CM" => "West Africa Standard Time",
            "CA" => "Pacific Standard Time",
            "CV" => "Cape Verde Standard Time",
            "KY" => "Eastern Standard Time",
            "CF" => "West Africa Standard Time",
            "TD" => "West Africa Standard Time",
            "CL" => "Pacific SA Standard Time",
            "CN" => "China Standard Time",
            "CO" => "SA Pacific Standard Time",
            "KM" => "Comoros Standard Time",
            "CD" => "Congo Standard Time",
            "CG" => "West Africa Standard Time",
            "CR" => "Central Standard Time",
            "HR" => "Central European Standard Time",
            "CU" => "Cuba Standard Time",
            "CY" => "Eastern European Standard Time",
            "CZ" => "Central European Standard Time",
            "DK" => "Romance Standard Time",
            "DJ" => "East Africa Standard Time",
            "DM" => "Atlantic Standard Time",
            "DO" => "Atlantic Standard Time",
            "EC" => "Ecuador Time",
            "EG" => "Egypt Standard Time",
            "SV" => "Central Standard Time",
            "GQ" => "West Africa Standard Time",
            "ER" => "East Africa Standard Time",
            "EE" => "Eastern European Standard Time",
            "SZ" => "South Africa Standard Time",
            "ET" => "East Africa Standard Time",
            "FJ" => "Fiji Standard Time",
            "FI" => "FLE Standard Time",
            "FR" => "Romance Standard Time",
            "GA" => "West Africa Standard Time",
            "GM" => "Greenwich Mean Time",
            "GE" => "Georgia Standard Time",
            "DE" => "W. Europe Standard Time",
            "GH" => "Greenwich Mean Time",
            "GR" => "Eastern European Standard Time",
            "GT" => "Central Standard Time",
            "GN" => "Greenwich Mean Time",
            "GW" => "Greenwich Mean Time",
            "GY" => "Guyana Time",
            "HT" => "Haiti Standard Time",
            "HN" => "Central Standard Time",
            "HK" => "China Standard Time",
            "HU" => "Central European Standard Time",
            "IS" => "Greenwich Mean Time",
            "IN" => "India Standard Time",
            "ID" => "W. Indonesia Time",
            "IR" => "Iran Standard Time",
            "IQ" => "Arabian Standard Time",
            "IE" => "GMT Standard Time",
            "IL" => "Israel Standard Time",
            "IT" => "W. Europe Standard Time",
            "JM" => "Jamaica Time",
            "JP" => "Tokyo Standard Time",
            "JE" => "GMT Standard Time",
            "JO" => "Arabian Standard Time",
            "KZ" => "Central Asia Standard Time",
            "KE" => "East Africa Standard Time",
            "KI" => "Gilbert Islands Time",
            "KP" => "North Korea Standard Time",
            "KR" => "Korea Standard Time",
            "KW" => "Arabian Standard Time",
            "KG" => "Kyrgyzstan Standard Time",
            "LA" => "Indochina Time",
            "LV" => "Eastern European Standard Time",
            "LB" => "Middle East Standard Time",
            "LS" => "South Africa Standard Time",
            "LR" => "Greenwich Mean Time",
            "LY" => "Eastern European Standard Time",
            "LT" => "E. Europe Standard Time",
            "LU" => "Romance Standard Time",
            "MO" => "China Standard Time",
            "MK" => "Central European Standard Time",
            "MG" => "East Africa Standard Time",
            "MW" => "Central Africa Time",
            "MY" => "Malaysia Standard Time",
            "MV" => "Maldives Standard Time",
            "ML" => "GMT Standard Time",
            "MT" => "Central European Standard Time",
            "MH" => "UTC+12",
            "MR" => "West Africa Standard Time",
            "MU" => "Mauritius Standard Time",
            "MX" => "Pacific Standard Time",
            "FM" => "UTC+10",
            "MD" => "Eastern European Standard Time",
            "MC" => "Central European Standard Time",
            "MN" => "Mongolia Standard Time",
            "ME" => "Central European Standard Time",
            "MA" => "Morocco Standard Time",
            "MZ" => "Central Africa Time",
            "MM" => "Myanmar Standard Time",
            "NA" => "Namibia Standard Time",
            "NP" => "Nepal Standard Time",
            "NL" => "W. Europe Standard Time",
            "NZ" => "New Zealand Standard Time",
            "NI" => "Central Standard Time",
            "NE" => "West Africa Standard Time",
            "NG" => "West Africa Standard Time",
            "NO" => "W. Europe Standard Time",
            "OM" => "Arabian Standard Time",
            "PK" => "Pakistan Standard Time",
            "PW" => "UTC+9",
            "PA" => "Eastern Standard Time",
            "PG" => "Papua New Guinea Standard Time",
            "PY" => "Paraguay Standard Time",
            "PE" => "Peru Standard Time",
            "PH" => "Philippine Standard Time",
            "PL" => "Central European Standard Time",
            "PT" => "Pacific Standard Time",
            "PR" => "Atlantic Standard Time",
            "QA" => "Arabian Standard Time",
            "RE" => "Reunion Standard Time",
            "RO" => "Eastern European Standard Time",
            "RU" => "Russian Standard Time",
            "RW" => "Central Africa Time",
            "SA" => "Arabian Standard Time",
            "SB" => "Solomon Islands Standard Time",
            "SC" => "Seychelles Standard Time",
            "SD" => "Central Africa Time",
            "SE" => "W. Europe Standard Time",
            "SG" => "Singapore Standard Time",
            "SI" => "Central European Standard Time",
            "SK" => "Central European Standard Time",
            "SL" => "Greenwich Mean Time",
            "SN" => "Greenwich Mean Time",
            "SO" => "East Africa Standard Time",
            "ZA" => "South Africa Standard Time",
            "SS" => "Central Africa Time",
            "ES" => "Romance Standard Time",
            "LK" => "India Standard Time",
            "CH" => "W. Europe Standard Time",
            "TZ" => "East Africa Standard Time",
            "TH" => "Indochina Time",
            "TG" => "West Africa Standard Time",
            "TK" => "UTC+13",
            "TO" => "Tonga Standard Time",
            "TT" => "Atlantic Standard Time",
            "TN" => "Central European Standard Time",
            "TR" => "Turkey Standard Time",
            "TM" => "Turkmenistan Standard Time",
            "TV" => "UTC+12",
            "UG" => "East Africa Time",
            "UA" => "FLE Standard Time",
            "AE" => "Arabian Standard Time",
            "GB" => "GMT Standard Time",
            "US" => "Pacific Standard Time",
            "UY" => "Montevideo Standard Time",
            "UZ" => "Uzbekistan Standard Time",
            "VU" => "Vanuatu Standard Time",
            "VE" => "Venezuela Standard Time",
            "VN" => "SE Asia Standard Time",
            "YE" => "Arabian Standard Time",
            "ZM" => "Central Africa Time",
            "ZW" => "Central Africa Time",
            _ => "UTC"
        };
    }
}