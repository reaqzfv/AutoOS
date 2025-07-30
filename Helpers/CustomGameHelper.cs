using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Windows.Storage;

namespace AutoOS.Helpers
{
    public static class CustomGameHelper
    {
        private static readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static readonly HttpClient httpClient = new();

        public static async Task LoadGames()
        {
            if (localSettings.Values["RyujinxLocation"] is string exePath && localSettings.Values["RyujinxDataLocation"] is string dataPath && File.Exists(exePath) && Directory.Exists(dataPath))
            {
                // download switch game catalog
                string filePath = Path.Combine(PathHelper.GetAppDataFolderPath(), "Ryujinx", "US.en.json");

                if (!File.Exists(filePath))
                {
                    try
                    {
                        var content = await httpClient.GetStringAsync("https://raw.githubusercontent.com/blawar/titledb/refs/heads/master/US.en.json");
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        await File.WriteAllTextAsync(filePath, content);
                    }
                    catch
                    {
                        
                    }
                }

                // remove previous games
                foreach (var item in GamesPage.Instance.Games.Items.OfType<Views.Settings.Games.HeaderCarousel.HeaderCarouselItem>().Where(item => item.Launcher == "Ryujinx").ToList())
                    GamesPage.Instance.Games.Items.Remove(item);

                // get game dirs
                var portableConfig = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(await File.ReadAllTextAsync(Path.Combine(localSettings.Values["RyujinxDataLocation"]?.ToString(), "Config.json")));

                var gameDirs = new List<string>();
                if (portableConfig != null && portableConfig.TryGetValue("game_dirs", out var gameDirsElement) && gameDirsElement.ValueKind == JsonValueKind.Array)
                    foreach (var dir in gameDirsElement.EnumerateArray())
                        gameDirs.Add(dir.GetString() ?? "");

                // read json database
                var jsonObject = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(await File.ReadAllTextAsync(Path.Combine(PathHelper.GetAppDataFolderPath(), "Ryujinx", "US.en.json")));

                var jsonById = new Dictionary<string, JsonElement>();
                foreach (var kvp in jsonObject)
                {
                    if (kvp.Value.TryGetProperty("id", out var idElem))
                    {
                        var key = idElem.GetString()?.ToLower();
                        if (!string.IsNullOrEmpty(key) && !jsonById.ContainsKey(key))
                        {
                            jsonById.Add(key, kvp.Value);
                        }
                    }
                }

                // get all roms in game dirs
                var candidatesPerDir = new Dictionary<string, List<string>>();
                foreach (var gameDir in gameDirs)
                {
                    if (!Directory.Exists(gameDir)) continue;

                    candidatesPerDir[gameDir] = [.. Directory.EnumerateFiles(gameDir).Where(f => f.EndsWith(".nsp", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".xci", StringComparison.OrdinalIgnoreCase))];
                }

                await Parallel.ForEachAsync(Directory.GetDirectories(Path.Combine(localSettings.Values["RyujinxDataLocation"]?.ToString(), "games")), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 }, async (folder, _) =>
                {
                    // check if game name exists in database
                    if (!jsonById.TryGetValue(Path.GetFileName(folder).Trim().ToLower(), out var entry))
                        return;

                    // get name from database
                    string name = entry.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : null;

                    // clean name for searching
                    string cleanName = Regex.Replace(name, @"[^\u0000-\u007F'’]+", "");
                    cleanName = cleanName.Replace('’', '\'');

                    var result = await SearchCovers(cleanName);
                    if (result == null) return;

                    // make name simple to find install location
                    string simpleCleanName = Regex.Replace(cleanName.ToLower(), @"[^a-z0-9]", "");

                    // find install location
                    string bestInstallLocation = null;

                    foreach (var gameDir in candidatesPerDir.Keys)
                    {
                        var candidates = candidatesPerDir[gameDir];
                        bestInstallLocation = candidates.FirstOrDefault(candidate =>
                        {
                            string simpleFileName = Regex.Replace(Path.GetFileNameWithoutExtension(candidate).ToLower(), @"[^a-z0-9]", "");
                            return simpleFileName.StartsWith(simpleCleanName);
                        });
                        if (bestInstallLocation != null)
                            break;
                    }

                    if (bestInstallLocation == null) return;

                    // get metadata for playtime
                    string metadataPath = Path.Combine(folder, "gui", "metadata.json");
                    if (!File.Exists(metadataPath)) return;

                    var metadataObj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(metadataPath));
                    string playTime = metadataObj.TryGetValue("timespan_played", out var timespanElement) &&
                        TimeSpan.TryParse(timespanElement.GetString(), out TimeSpan ts)
                            ? ((int)ts.TotalHours > 0 ? $"{(int)ts.TotalHours}h {ts.Minutes}m" : $"{ts.Minutes}m")
                            : null;

                    var data = JsonDocument.Parse(await httpClient.GetStringAsync(result["game_url"], _)).RootElement;

                    GamesPage.Instance.DispatcherQueue.TryEnqueue(() =>
                    {
                        GamesPage.Instance.Games.Items.Add(new Views.Settings.Games.HeaderCarousel.HeaderCarouselItem
                        {
                            Launcher = "Ryujinx",
                            LauncherLocation = localSettings.Values["RyujinxLocation"]?.ToString(),
                            DataLocation = localSettings.Values["RyujinxDataLocation"]?.ToString(),
                            GameLocation = bestInstallLocation,
                            InstallLocation = Path.GetDirectoryName(bestInstallLocation),
                            Title = name,
                            ImageUrl = result["cover_url"],
                            BackgroundImageUrl = entry.GetProperty("bannerUrl").GetString(),
                            Developers = result["developers"],
                            Genres = [.. data.GetProperty("genres").EnumerateArray().Select(g => g.GetProperty("name").GetString())],
                            Features = [.. data.GetProperty("game_modes").EnumerateArray().Select(m => m.GetProperty("name").GetString())],
                            Rating = Math.Round(data.GetProperty("aggregated_rating").GetDouble() / 20.0, 2),
                            PlayTime = playTime,
                            AgeRatingUrl = result["age_rating_url"],
                            AgeRatingTitle = result["age_rating_title"],
                            AgeRatingDescription = entry.GetProperty("ratingContent")[0].GetString(),
                            Description = data.GetProperty("summary").GetString(),
                            Screenshots = entry.GetProperty("screenshots").EnumerateArray().Select(x => x.GetString()).ToList(),
                            //Videos = [.. data.GetProperty("videos")
                            //                .EnumerateArray()
                            //                .Select(g => g.TryGetProperty("url", out var url) && Uri.TryCreate(url.GetString(), UriKind.Absolute, out var uri)
                            //                    ? MediaSource.CreateFromUri(uri)
                            //                    : null)
                            //                .Where(source => source is not null)],
                            Width = 240,
                            Height = 320,
                        });
                    });
                });
            }
        }

        private static async Task<Dictionary<string, string>> SearchCovers(string name)
        {
            string Clean(string input) => Regex.Replace(input.ToLower(), @"\s+", ".");
            string GetSearchBucket(string input)
            {
                string cleaned = Regex.Replace(input.ToLower().Substring(0, Math.Min(2, input.Length)), @"[^a-z\d]", "");
                return string.IsNullOrEmpty(cleaned) ? "@" : cleaned;
            }

            try
            {
                var bucketJson = await httpClient.GetStringAsync($"https://raw.githubusercontent.com/LizardByte/GameDB/gh-pages/buckets/{GetSearchBucket(Clean(name))}.json");
                var maps = JsonNode.Parse(bucketJson)?.AsObject();

                var matchingIds = new List<string>();

                foreach (var kvp in maps)
                {
                    var item = kvp.Value?.AsObject();
                    if (item == null) continue;

                    string itemName = item["name"]?.ToString() ?? "";
                    if (Clean(itemName) == Clean(name))
                        matchingIds.Add(kvp.Key);
                }

                JsonObject maxGame = null;
                int maxFields = 0;

                foreach (var id in matchingIds)
                {
                    try
                    {
                        var json = await httpClient.GetStringAsync($"https://raw.githubusercontent.com/LizardByte/GameDB/gh-pages/games/{id}.json");
                        var game = JsonNode.Parse(json)?.AsObject();
                        if (game == null) continue;

                        int count = game.Where(kv => kv.Value != null && kv.Value is not JsonObject && !string.IsNullOrWhiteSpace(kv.Value.ToString())).Count();

                        if (count > maxFields)
                        {
                            maxGame = game;
                            maxFields = count;
                        }
                    }
                    catch { }
                }

                if (maxGame != null && maxGame["cover"] is JsonObject cover && cover["url"] != null)
                {
                    string thumb = cover["url"]?.ToString() ?? "";
                    int dot = thumb.LastIndexOf('.');
                    int slash = thumb.LastIndexOf('/');

                    if (dot >= 0 && slash >= 0)
                    {
                        string slug = thumb.Substring(slash + 1, dot - slash - 1);

                        var developers = maxGame["involved_companies"]
                            ?.AsArray()
                            .Where(company => company["developer"]?.GetValue<bool>() == true)
                            .Select(company => company["company"]?["name"]?.ToString())
                            .Where(name => !string.IsNullOrWhiteSpace(name))
                            .ToList();

                        string developerNames = developers != null && developers.Any() ? string.Join(", ", developers) : "Unknown";

                        string region = RegionInfo.CurrentRegion.TwoLetterISORegionName.ToUpper();

                        string ratingKey = region switch
                        {
                            "AU" => "ACB",
                            "BR" => "DEJUS",
                            "KR" => "GRAC",
                            "DE" => "USK",
                            "US" or "CA" => "ESRB",
                            _ => "PEGI"
                        };

                        string baseUrl = ratingKey.ToLowerInvariant() switch
                        {
                            "pegI" => "https://www.igdb.com/icons/rating_icons/pegi/pegi_",
                            "esrb" => "https://www.igdb.com/icons/rating_icons/esrb/esrb_",
                            "cero" => "https://www.igdb.com/icons/rating_icons/cero/cero_",
                            "usk" => "https://www.igdb.com/icons/rating_icons/usk/usk_",
                            "grac" => "https://www.igdb.com/icons/rating_icons/grac/grac_",
                            "class_ind" => "https://www.igdb.com/icons/rating_icons/class_ind/class_ind_",
                            "acb" => "https://www.igdb.com/icons/rating_icons/acb/acb_",
                            _ => ""
                        };

                        var ratingTitles = ratingKey switch
                        {
                            "PEGI" => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                            {
                                {"3", "PEGI 3"}, {"7", "PEGI 7"}, {"12", "PEGI 12"}, {"16", "PEGI 16"}, {"18", "PEGI 18"},
                            },
                                                    "USK" => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                            {
                                {"0", "USK 0"}, {"6", "USK 6"}, {"12", "USK 12"}, {"16", "USK 16"}, {"18", "USK 18"},
                            },
                                                    "ESRB" => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                            {
                                {"ec", "Early Childhood"}, {"e", "Everyone"}, {"e10", "Everyone 10+"}, {"e10+", "Everyone 10+"},
                                {"t", "Teen"}, {"m", "Mature 17+"}, {"ao", "Adults Only 18+"},
                            },
                            _ => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        };

                        var ratingEntry = maxGame["age_ratings"]?.AsArray()
                            .FirstOrDefault(r => r?["organization"]?["name"]?.ToString().Equals(ratingKey, StringComparison.OrdinalIgnoreCase) == true);

                        string ratingCode = ratingEntry?["rating_category"]?["rating"]?.ToString();

                        if (ratingCode is null)
                            return new Dictionary<string, string>
                            {
                                { "age_rating_url", "" },
                                { "age_rating_title", "Unknown" }
                            };

                        string ratingKeyForUrl = ratingKey == "ESRB" && ratingCode.StartsWith("e10+", StringComparison.OrdinalIgnoreCase) ? "e10" : ratingCode;

                        string ratingTitle = ratingTitles.TryGetValue(ratingCode, out var title) ? title : ratingCode;

                        string ratingUrl = $"{baseUrl}{ratingKeyForUrl.ToLowerInvariant()}.png";

                        return new Dictionary<string, string>
                        {
                            { "name", maxGame["name"]?.ToString() },
                            { "game_url", $"https://raw.githubusercontent.com/LizardByte/GameDB/gh-pages/games/{maxGame["id"]}.json" },
                            { "cover_url", $"https://images.igdb.com/igdb/image/upload/t_cover_big_2x/{slug}.jpg" },
                            { "developers", developerNames },
                            { "age_rating_url", ratingUrl },
                            { "age_rating_title", ratingTitle }
                        };
                    }
                }
            }
            catch { }

            return null;
        }
    }
}