using AutoOS.Helpers;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using ValveKeyValue;
using Windows.Foundation;
using Windows.Storage;
using WinRT.Interop;

namespace AutoOS.Views.Settings.Games.HeaderCarousel;

[TemplatePart(Name = nameof(PART_BackDropImage), Type = typeof(AnimatedImage))]
[TemplatePart(Name = nameof(PART_ScrollViewer), Type = typeof(ScrollViewer))]
public partial class HeaderCarousel : ItemsControl
{
    private const string PART_ScrollViewer = "PART_ScrollViewer";
    private const string PART_BackDropImage = "PART_BackDropImage";
    private ScrollViewer scrollViewer;
    private AnimatedImage backDropImage;

    private TextBlock PageTitle;
    private SwitchPresenter SwitchPresenter;
    private TextBlock SwitchPresenter_TextBlock;

    private StackPanel NoGames_StackPanel;

    private Grid MetadataGrid;
    private ScrollViewer Metadata_ScrollViewer;

    private ScrollViewer Screenshots_ScrollViewer;
    //private GameGallery Screenshots_Gallery;
    //private ScrollViewer Videos_ScrollViewer;

    private Button Play;
    private Button Update;
    private Button StopProcesses;
    private Button LaunchExplorer;

    private bool isInitializingEpicGamesAccounts = true;
    private bool isInitializingSteamAccounts = true;
    private Button EpicGamesButton;
    private ComboBox EpicGamesAccounts;
    private Button AddEpicGamesAccount;
    private Button RemoveEpicGamesAccount;
   
    private Button SteamButton;
    private ComboBox SteamAccounts;
    private Button AddSteamAccount;
    private Button RemoveSteamAccount;

    private StackPanel EpicGrowl;
    private StackPanel SteamGrowl;

    private Button OpenInstallLocation;
    
    private bool isInitializingPresentationMode = true;
    private StackPanel PresentationMode;
    private ComboBox PresentationMode_ComboBox;

    private AutoSuggestBox SearchBox;
    private string currentSortKey = "Title";
    private bool ascending = true;

    private MenuFlyoutItem SortByName, SortByLauncher, SortByRating, SortByPlayTime;
    private MenuFlyoutItem SortAscending, SortDescending;

    private Button Fullscreen;
    private TextBlock FullscreenText;
    private FontIcon FullscreenIcon;

    //public event EventHandler<HeaderCarouselEventArgs> ItemClick;

    private readonly Random random = new();
    private readonly DispatcherTimer selectionTimer = new();
    private readonly DispatcherTimer deselectionTimer = new();
    private readonly List<int> numbers = [];
    private HeaderCarouselItem selectedTile;
    private int currentIndex;

    private BlurEffectManager _blurManager;

    public HeaderCarousel()
    {
        DefaultStyleKey = typeof(HeaderCarousel);    
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        selectionTimer.Interval = SelectionDuration;
        deselectionTimer.Interval = DeSelectionDuration;

        scrollViewer = GetTemplateChild(PART_ScrollViewer) as ScrollViewer;
        backDropImage = GetTemplateChild(PART_BackDropImage) as AnimatedImage;

        PageTitle = GetTemplateChild("PageTitle") as TextBlock;

        SearchBox = GetTemplateChild("SearchBox") as AutoSuggestBox;
        SearchBox.TextChanged += SearchBox_TextChanged;
        SearchBox.QuerySubmitted += SearchBox_QuerySubmitted;

        SortByName = GetTemplateChild("SortByName") as MenuFlyoutItem;
        SortByName.Click += SortByName_Click;
        SortByLauncher = GetTemplateChild("SortByLauncher") as MenuFlyoutItem;
        SortByLauncher.Click += SortByLauncher_Click;
        SortByRating = GetTemplateChild("SortByRating") as MenuFlyoutItem;
        SortByRating.Click += SortByRating_Click;
        SortByPlayTime = GetTemplateChild("SortByPlayTime") as MenuFlyoutItem;
        SortByPlayTime.Click += SortByPlayTime_Click;
        SortAscending = GetTemplateChild("SortAscending") as MenuFlyoutItem;
        SortAscending.Click += SortAscending_Click;
        SortDescending = GetTemplateChild("SortDescending") as MenuFlyoutItem;
        SortDescending.Click += SortDescending_Click;

        Fullscreen = GetTemplateChild("Fullscreen") as Button;
        FullscreenText = GetTemplateChild("FullscreenText") as TextBlock;
        FullscreenIcon = GetTemplateChild("FullscreenIcon") as FontIcon;
        Fullscreen.Click += Fullscreen_Click;

        EpicGamesButton = GetTemplateChild("EpicGamesButton") as Button;
        EpicGamesAccounts = GetTemplateChild("EpicGamesAccounts") as ComboBox;
        EpicGamesAccounts.SelectionChanged += EpicGamesAccounts_SelectionChanged;
        AddEpicGamesAccount = GetTemplateChild("AddEpicGamesAccount") as Button;
        AddEpicGamesAccount.Click += AddEpicGamesAccount_Click;
        RemoveEpicGamesAccount = GetTemplateChild("RemoveEpicGamesAccount") as Button;
        RemoveEpicGamesAccount.Click += RemoveEpicGamesAccount_Click;
        EpicGrowl = GetTemplateChild("EpicGrowl") as StackPanel;
        Growl.Register("Epic", EpicGrowl);
        LoadEpicGamesAccounts();

        SteamButton = GetTemplateChild("SteamButton") as Button;
        SteamAccounts = GetTemplateChild("SteamAccounts") as ComboBox;
        SteamAccounts.SelectionChanged += SteamAccounts_SelectionChanged;
        AddSteamAccount = GetTemplateChild("AddSteamAccount") as Button;
        AddSteamAccount.Click += AddSteamAccount_Click;
        RemoveSteamAccount = GetTemplateChild("RemoveSteamAccount") as Button;
        RemoveSteamAccount.Click += RemoveSteamAccount_Click;
        SteamGrowl = GetTemplateChild("SteamGrowl") as StackPanel;
        Growl.Register("Steam", SteamGrowl);
        LoadSteamAccounts();

        SwitchPresenter = GetTemplateChild("SwitchPresenter") as SwitchPresenter;
        SwitchPresenter_TextBlock = GetTemplateChild("SwitchPresenter_TextBlock") as TextBlock;
        NoGames_StackPanel = GetTemplateChild("NoGames_StackPanel") as StackPanel;
        MetadataGrid = GetTemplateChild("MetadataGrid") as Grid;
        Metadata_ScrollViewer = GetTemplateChild("Metadata_ScrollViewer") as ScrollViewer;

        Play = GetTemplateChild("Play") as Button;
        Play.Click += Play_Click;
        Update = GetTemplateChild("Update") as Button;
        Update.Click += Update_Click;
        StopProcesses = GetTemplateChild("StopProcesses") as Button;
        StopProcesses.Click += StopProcesses_Click;
        LaunchExplorer = GetTemplateChild("LaunchExplorer") as Button;
        LaunchExplorer.Click += LaunchExplorer_Click;

        Screenshots_ScrollViewer = GetTemplateChild("Screenshots_ScrollViewer") as ScrollViewer;
        //Screenshots_Gallery = GetTemplateChild("Screenshots_Gallery") as GameGallery;
        //Videos_ScrollViewer = GetTemplateChild("Videos_ScrollViewer") as ScrollViewer;

        OpenInstallLocation = GetTemplateChild("OpenInstallLocation") as Button;
        OpenInstallLocation.Click += OpenInstallLocation_Click;

        PresentationMode = GetTemplateChild("PresentationMode") as StackPanel;

        PresentationMode_ComboBox = GetTemplateChild("PresentationMode_ComboBox") as ComboBox;
        PresentationMode_ComboBox.SelectionChanged += PresentationMode_SelectionChanged;

        Loaded -= HeaderCarousel_Loaded;
        Loaded += HeaderCarousel_Loaded;
        Unloaded -= HeaderCarousel_Unloaded;
        Unloaded += HeaderCarousel_Unloaded;

        LoadGames();

        if (backDropImage != null)
        {
            _blurManager = new BlurEffectManager(backDropImage);

            ApplyBackdropBlur();
        }
    }

    private async void LoadGames()
    {
        // reset
        LoadSortSettings();

        // load games
        var tasks = new List<Task>();

        if (EpicGamesAccounts.SelectedItem is ComboBoxItem && ((ComboBoxItem)EpicGamesAccounts.SelectedItem).Content?.ToString() != "Not logged in" && EpicGamesButton.Visibility == Visibility.Visible)
        {
            tasks.Add(EpicGamesHelper.LoadGames());
        }

        if ((SteamAccounts.SelectedItem is string && SteamAccounts.SelectedItem.ToString() != "Not logged in") && SteamButton.Visibility == Visibility.Visible)
        {
            tasks.Add(SteamHelper.LoadGames());
        }

        tasks.Add(CustomGameHelper.LoadGames());

        await Task.WhenAll(tasks);

        // sort games
        LoadSortSettings();

        // show games status
        NoGames_StackPanel.Visibility = Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        // show games
        SwitchPresenter.Value = false;

        await Task.Delay(700);

        if (Items.Count == 0)
            return;

        if (Items[0] is HeaderCarouselItem tile)
        {
            if (selectedTile != null)
            {
                selectedTile.IsSelected = false;
                selectedTile = null;
            }

            selectedTile = tile;
            var panel = ItemsPanelRoot;
            if (panel != null)
            {
                GeneralTransform transform = selectedTile.TransformToVisual(panel);
                Point point = transform.TransformPoint(new Point(0, 0));
                scrollViewer.ChangeView(point.X - (scrollViewer.ActualWidth / 2) + (selectedTile.ActualSize.X / 2), null, null);
                SetTileVisuals();
                PageTitle.RequestedTheme = ElementTheme.Dark;
                //deselectionTimer?.Start();
            }
        }

        selectionTimer.Start();
    }

    private void ApplyBackdropBlur()
    {
        if (_blurManager == null)
            return;

        if (IsBlurEnabled)
        {
            _blurManager.BlurAmount = BlurAmount;

            _blurManager.EnableBlur();
        }
        else
        {
            _blurManager.DisableBlur();
        }
    }

    private void HeaderCarousel_Unloaded(object sender, RoutedEventArgs e)
    {
        UnsubscribeToEvents();

        ElementSoundPlayer.State = ElementSoundPlayerState.Off;
    }

    private void HeaderCarousel_Loaded(object sender, RoutedEventArgs e)
    {
        selectionTimer.Tick += SelectionTimer_Tick;
        //deselectionTimer.Tick += DeselectionTimer_Tick;

        ElementSoundPlayer.State = ElementSoundPlayerState.On;
    }

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        foreach (HeaderCarouselItem tile in Items.Cast<HeaderCarouselItem>())
        {
            tile.PointerEntered -= Tile_PointerEntered;
            tile.PointerEntered += Tile_PointerEntered;

            //tile.PointerExited -= Tile_PointerExited;
            //tile.PointerExited += Tile_PointerExited;

            tile.GotFocus -= Tile_GotFocus;
            tile.GotFocus += Tile_GotFocus;

            //tile.LostFocus -= Tile_LostFocus;
            //tile.LostFocus += Tile_LostFocus;

            //tile.Click -= Tile_Click;
            //tile.Click += Tile_Click;
        }
    }

    private void UnsubscribeToEvents()
    {
        selectionTimer.Tick -= SelectionTimer_Tick;
        //deselectionTimer.Tick -= DeselectionTimer_Tick;
        selectionTimer?.Stop();
        //deselectionTimer?.Stop();
        gameWatcherTimer?.Stop();
        
        foreach (HeaderCarouselItem tile in Items.Cast<HeaderCarouselItem>())
        {
            tile.PointerEntered -= Tile_PointerEntered;
            //tile.PointerExited -= Tile_PointerExited;
            tile.GotFocus -= Tile_GotFocus;
            //tile.LostFocus -= Tile_LostFocus;
            //tile.Click -= Tile_Click;
        }
    }

    //private void Tile_Click(object sender, RoutedEventArgs e)
    //{
    //    if (sender is HeaderCarouselItem tile)
    //    {
    //        tile.PointerExited -= Tile_PointerExited;
    //        ItemClick?.Invoke(sender, new HeaderCarouselEventArgs { HeaderCarouselItem = tile });
    //    }
    //}

    private void SelectionTimer_Tick(object sender, object e)
    {
        SelectNextTile();
    }

    private async void SelectNextTile()
    {
        if (Items.Count == 0)
        {
            return;
        }

        if (Items[GetNextUniqueRandom()] is HeaderCarouselItem tile)
        {
            if (selectedTile != null)
            {
                selectedTile.IsSelected = false;
                selectedTile = null;
            }

            selectedTile = tile;
            var panel = ItemsPanelRoot;
            if (panel != null)
            {
                GeneralTransform transform = selectedTile.TransformToVisual(panel);
                Point point = transform.TransformPoint(new Point(0, 0));
                scrollViewer.ChangeView(point.X - (scrollViewer.ActualWidth / 2) + (selectedTile.ActualSize.X / 2), null, null);
                await Task.Delay(500);
                SetTileVisuals();
                //deselectionTimer?.Start();
            }
        }
    }

    //private void DeselectionTimer_Tick(object sender, object e)
    //{
    //    if (selectedTile != null)
    //    {
    //        selectedTile.IsSelected = false;
    //        selectedTile = null;
    //    }

    //    deselectionTimer.Stop();

    //    if (IsAutoScrollEnabled)
    //        selectionTimer.Start();
    //}

    private void ResetAndShuffle()
    {
        if (Items.Count == 0)
        {
            return;
        }

        numbers.Clear();
        for (int i = 0; i <= Items.Count - 1; i++)
        {
            numbers.Add(i);
        }

        // Shuffle the list
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (numbers[j], numbers[i]) = (numbers[i], numbers[j]);
        }

        currentIndex = 0;
    }

    private int GetNextUniqueRandom()
    {
        if (currentIndex >= numbers.Count)
        {
            ResetAndShuffle();
        }

        int nextIndex = numbers[currentIndex++];

        if (selectedTile != null)
        {
            int selectedIndex = Items.IndexOf(selectedTile);
            if (nextIndex == selectedIndex)
            {
                if (currentIndex >= numbers.Count)
                    ResetAndShuffle();
                nextIndex = numbers[currentIndex++];
            }
        }

        return nextIndex;
    }

    private void SetTileVisuals()
    {
        if (selectedTile != null)
        {

            ElementSoundPlayer.Play(ElementSoundKind.Focus);

            if (selectedTile.BackgroundImageUrl != null && backDropImage.ImageUrl?.ToString() != selectedTile.BackgroundImageUrl)
                backDropImage.ImageUrl = new Uri(selectedTile.BackgroundImageUrl);

            if (MetadataGrid.Visibility == Visibility.Collapsed)
                MetadataGrid.Visibility = Visibility.Visible;

            Metadata_ScrollViewer.Focus(FocusState.Programmatic);
            Metadata_ScrollViewer.ChangeView(null, 0, null);
            
            Title = selectedTile?.Title;
            Developers = selectedTile?.Developers;

            UpdateIsAvailable = selectedTile?.UpdateIsAvailable ?? false;
            
            Rating = selectedTile?.Rating != 0.0 ? selectedTile.Rating : Rating;
            RoundedRating = Math.Round((selectedTile?.Rating ?? 0.0), 1).ToString("0.0", CultureInfo.InvariantCulture);
            PlayTime = selectedTile?.PlayTime;
            AgeRatingUrl = selectedTile?.AgeRatingUrl;
            AgeRatingTitle = selectedTile?.AgeRatingTitle;
            AgeRatingDescription = selectedTile?.AgeRatingDescription;

            Genres = selectedTile?.Genres;
            Features = selectedTile?.Features;
            Description = selectedTile?.Description;

            selectedTile.IsSelected = true;

            InstallLocation = selectedTile?.InstallLocation;

            Launcher = selectedTile?.Launcher;
            
            CatalogItemId = selectedTile?.CatalogItemId;
            CatalogNamespace = selectedTile?.CatalogNamespace;
            AppName = selectedTile?.AppName;
            LaunchExecutable = selectedTile?.LaunchExecutable;

            GameID = selectedTile?.GameID;

            LauncherLocation = selectedTile?.LauncherLocation;
            DataLocation = selectedTile?.DataLocation;
            GameLocation = selectedTile?.GameLocation;

            PresentationMode.Visibility = selectedTile?.Title == "Fortnite" ? Visibility.Visible : Visibility.Collapsed;

            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, async () =>
            {
                await Task.Delay(100);
                Screenshots = selectedTile?.Screenshots;
                Screenshots_ScrollViewer.Visibility = (Screenshots?.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
                //Screenshots_Gallery.ResetScrollPosition();

                //Videos = selectedTile?.Videos;
                //Videos_ScrollViewer.Visibility = (Videos?.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

                if (selectedTile?.Title == "Fortnite")
                {
                    await GetPresentationMode();
                }

                await CheckGameRunningAsync();
            });
        }
    }

    //private void Tile_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    //{
    //    ((HeaderCarouselItem)sender).IsSelected = false;
    //    if (IsAutoScrollEnabled)
    //        selectionTimer.Start();
    //}

    private void Tile_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var tile = (HeaderCarouselItem)sender;
        if (tile != selectedTile)
        {
            selectedTile = tile;
            SelectTile();
        }
    }

    private void SelectTile()
    {
        selectionTimer.Stop();
        //deselectionTimer.Stop();

        foreach (HeaderCarouselItem t in Items.Cast<HeaderCarouselItem>())
        {
            t.IsSelected = false;
        }

        SetTileVisuals();
    }

    private void Tile_GotFocus(object sender, RoutedEventArgs e)
    {
        selectedTile = (HeaderCarouselItem)sender;
        SelectTile();
    }

    //private void Tile_LostFocus(object sender, RoutedEventArgs e)
    //{
    //    ((HeaderCarouselItem)sender).IsSelected = false;
    //    if (IsAutoScrollEnabled)
    //        selectionTimer.Start();
    //}

    private void ApplyAutoScroll()
    {
        if (IsAutoScrollEnabled)
        {
            ResetAndShuffle();
            SelectNextTile();
        }
        else
        {
            selectionTimer?.Stop();
            //deselectionTimer?.Stop();
        }
    }

    private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            var suggestions = Items
                .OfType<HeaderCarouselItem>()
                .Where(g => !string.IsNullOrEmpty(g.Title) && g.Title.Contains(sender.Text, StringComparison.CurrentCultureIgnoreCase))
                .Select(g => g.Title)
                .Distinct()
                .ToList();

            sender.ItemsSource = suggestions;
        }
    }

    private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var query = args.QueryText?.Trim();
        if (string.IsNullOrEmpty(query))
            return;

        SelectTileByTitle(query);
    }

    private async void SelectTileByTitle(string title)
    {
        if (string.IsNullOrEmpty(title) || Items.Count == 0)
            return;

        var tile = Items
            .OfType<HeaderCarouselItem>()
            .FirstOrDefault(t => string.Equals(t.Title, title, StringComparison.CurrentCultureIgnoreCase));

        if (tile == null)
            return;

        if (selectedTile != null)
        {
            selectedTile.IsSelected = false;
            selectedTile = null;
        }

        selectedTile = tile;

        UnsubscribeToEvents();

        var panel = ItemsPanelRoot;
        if (panel != null)
        {
            GeneralTransform transform = selectedTile.TransformToVisual(panel);
            Point point = transform.TransformPoint(new Point(0, 0));
            scrollViewer.ChangeView(point.X - (scrollViewer.ActualWidth / 2) + (selectedTile.ActualSize.X / 2), null, null);
            SetTileVisuals();
            //deselectionTimer?.Start();
        }

        await Task.Delay(500);

        SubscribeToEvents();
    }

    private void SortByName_Click(object sender, RoutedEventArgs e)
    {
        currentSortKey = "Title";
        UpdateSortIcons();
        ApplySort();
        SaveSortSettings();
    }

    private void SortByLauncher_Click(object sender, RoutedEventArgs e)
    {
        currentSortKey = "Launcher";
        UpdateSortIcons();
        ApplySort();
        SaveSortSettings();
    }

    private void SortByPlayTime_Click(object sender, RoutedEventArgs e)
    {
        currentSortKey = "PlayTime";
        UpdateSortIcons();
        ApplySort();
        SaveSortSettings();
    }

    private void SortByRating_Click(object sender, RoutedEventArgs e)
    {
        currentSortKey = "Rating";
        UpdateSortIcons();
        ApplySort();
        SaveSortSettings();
    }

    private void SortAscending_Click(object sender, RoutedEventArgs e)
    {
        ascending = true;
        UpdateSortIcons();
        ApplySort();
        SaveSortSettings();
    }

    private void SortDescending_Click(object sender, RoutedEventArgs e)
    {
        ascending = false;
        UpdateSortIcons();
        ApplySort();
        SaveSortSettings();
    }

    private void ApplySort()
    {
        var items = Items.OfType<HeaderCarouselItem>().ToList();
        if (items.Count == 0) return;

        IEnumerable<HeaderCarouselItem> result = currentSortKey switch
        {
            "Title" => ascending
                ? items.OrderBy(g => g.Title ?? "", StringComparer.CurrentCultureIgnoreCase)
                : items.OrderByDescending(g => g.Title ?? "", StringComparer.CurrentCultureIgnoreCase),

            "Launcher" => ascending
                ? items.OrderBy(g => g.Launcher ?? "", StringComparer.CurrentCultureIgnoreCase)
                      .ThenBy(g => g.Title ?? "", StringComparer.CurrentCultureIgnoreCase)
                : items.OrderByDescending(g => g.Launcher ?? "", StringComparer.CurrentCultureIgnoreCase)
                      .ThenBy(g => g.Title ?? "", StringComparer.CurrentCultureIgnoreCase),

            "PlayTime" => ascending
                ? items.OrderBy(g => ParseMinutes(g.PlayTime))
                      .ThenBy(g => g.Title ?? "", StringComparer.CurrentCultureIgnoreCase)
                : items.OrderByDescending(g => ParseMinutes(g.PlayTime))
                      .ThenBy(g => g.Title ?? "", StringComparer.CurrentCultureIgnoreCase),

            "Rating" => ascending
                ? items.OrderBy(g => g.Rating)
                      .ThenBy(g => g.Title ?? "", StringComparer.CurrentCultureIgnoreCase)
                : items.OrderByDescending(g => g.Rating)
                      .ThenBy(g => g.Title ?? "", StringComparer.CurrentCultureIgnoreCase),

            _ => items
        };

        Items.Clear();
        foreach (var item in result)
        {
            Items.Add(item);
        }
    }

    private void UpdateSortIcons()
    {
        SortByName.Icon = currentSortKey == "Title" ? new FontIcon { Glyph = "\uE915" } : null;
        SortByLauncher.Icon = currentSortKey == "Launcher" ? new FontIcon { Glyph = "\uE915" } : null;
        SortByPlayTime.Icon = currentSortKey == "PlayTime" ? new FontIcon { Glyph = "\uE915" } : null;
        SortByRating.Icon = currentSortKey == "Rating" ? new FontIcon { Glyph = "\uE915" } : null;
        SortAscending.Icon = ascending ? new FontIcon { Glyph = "\uE915" } : null;
        SortDescending.Icon = !ascending ? new FontIcon { Glyph = "\uE915" } : null;
    }

    private void SaveSortSettings()
    {
        ApplicationData.Current.LocalSettings.Values["SortKey"] = currentSortKey;
        ApplicationData.Current.LocalSettings.Values["SortAscending"] = ascending;
    }

    private void LoadSortSettings()
    {
        var settings = ApplicationData.Current.LocalSettings.Values;
        currentSortKey = settings["SortKey"] as string ?? "Title";
        ascending = settings["SortAscending"] is not bool b || b;

        UpdateSortIcons();
        ApplySort();
    }

    private static int ParseMinutes(string time)
    {
        if (string.IsNullOrWhiteSpace(time))
            return 0;

        var match = Regex.Match(time, @"(?:(\d+)h)?\s*(\d+)m");
        if (match.Success)
        {
            int hours = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 0;
            int minutes = int.Parse(match.Groups[2].Value);
            return hours * 60 + minutes;
        }

        return 0;
    }

    private bool isFullscreen = false;
    private void Fullscreen_Click(object sender, RoutedEventArgs e)
    {
        App.MainWindow.ExtendsContentIntoTitleBar = true;

        IntPtr hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

        var navView = MainWindow.Instance.GetNavView();
        var titleBar = MainWindow.Instance.GetTitleBar();

        if (!isFullscreen)
        {
            UnsubscribeToEvents();

            navView.IsPaneVisible = false;
            titleBar.Visibility = Visibility.Collapsed;
            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            FullscreenText.Text = "Exit Full Screen";
            FullscreenIcon.Glyph = "\uE92C";
            isFullscreen = true;

            SubscribeToEvents();
        }
        else
        {
            navView.IsPaneVisible = true;
            titleBar.Visibility = Visibility.Visible;
            appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
            FullscreenText.Text = "Enter Full Screen";
            FullscreenIcon.Glyph = "\uE92D";
            isFullscreen = false;
        }
    }

    public void LoadEpicGamesAccounts()
    {
        if (File.Exists(EpicGamesHelper.EpicGamesPath))
        {
            // get all accounts
            var accounts = EpicGamesHelper.GetEpicGamesAccounts();

            // reset ui elements
            EpicGamesAccounts.Items.Clear();
            EpicGamesAccounts.IsEnabled = accounts.Count > 0;
            RemoveEpicGamesAccount.IsEnabled = accounts.Count > 0;

            // add accounts to combobox
            if (accounts.Count == 0)
            {
                var notLoggedIn = new ComboBoxItem { Content = "Not logged in", IsEnabled = false };
                EpicGamesAccounts.Items.Add(notLoggedIn);
                EpicGamesAccounts.SelectedItem = notLoggedIn;
                EpicGamesAccounts.IsEnabled = false;
                RemoveEpicGamesAccount.IsEnabled = false;
            }
            else if (!accounts.Any(a => a.IsActive))
            {
                var notLoggedIn = new ComboBoxItem { Content = "Not logged in", IsEnabled = false };
                EpicGamesAccounts.Items.Add(notLoggedIn);
                EpicGamesAccounts.SelectedItem = notLoggedIn;
                RemoveEpicGamesAccount.IsEnabled = false;

                foreach (var account in accounts)
                {
                    var item = new ComboBoxItem
                    {
                        Content = account.DisplayName,
                        Tag = account.AccountId
                    };

                    EpicGamesAccounts.Items.Add(item);
                }
            }
            else
            {
                foreach (var account in accounts)
                {
                    var item = new ComboBoxItem
                    {
                        Content = account.DisplayName,
                        Tag = account.AccountId
                    };

                    EpicGamesAccounts.Items.Add(item);

                    if (account.IsActive)
                        EpicGamesAccounts.SelectedItem = item;
                }
            }
        }
        else
        {
            EpicGamesButton.Visibility = Visibility.Collapsed;
        }

        isInitializingEpicGamesAccounts = false;
    }

    private async void EpicGamesAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isInitializingEpicGamesAccounts) return;

        // close epic games launcher
        EpicGamesHelper.CloseEpicGames();

        // update config before switching
        if (EpicGamesHelper.ValidateData(EpicGamesHelper.ActiveEpicGamesAccountPath))
        {
            var (oldAccountId, _, _, _) = EpicGamesHelper.GetAccountData(EpicGamesHelper.ActiveEpicGamesAccountPath);

            string accountDir = Path.Combine(EpicGamesHelper.EpicGamesAccountDir, oldAccountId);
            if (Directory.Exists(accountDir))
                File.Copy(EpicGamesHelper.ActiveEpicGamesAccountPath, Path.Combine(accountDir, "GameUserSettings.ini"), true);
        }

        // get accountId
        string accountId = (EpicGamesAccounts.SelectedItem as ComboBoxItem)?.Tag as string;

        // replace file
        File.Copy(Path.Combine(EpicGamesHelper.EpicGamesAccountDir, accountId, "GameUserSettings.ini"), EpicGamesHelper.ActiveEpicGamesAccountPath, true);

        // replace accountid
        Process.Start("regedit.exe", $@"/s ""{Path.Combine(EpicGamesHelper.EpicGamesAccountDir, accountId, "accountId.reg")}""");

        // update refresh token
        if (await EpicGamesHelper.UpdateEpicGamesToken(EpicGamesHelper.ActiveEpicGamesAccountPath) == null)
        {
            UpdateInvalidEpicGamesToken();
            return;
        }

        // close epic games launcher
        EpicGamesHelper.CloseEpicGames();

        // refresh combobox
        isInitializingEpicGamesAccounts = true;
        LoadEpicGamesAccounts();

        // refresh library
        await EpicGamesHelper.LoadGames();
        LoadSortSettings();
    }

    private async void UpdateInvalidEpicGamesToken()
    {
        // add growl
        Growl.Info(new GrowlInfo
        {
            ShowDateTime = false,
            StaysOpen = true,
            IsClosable = false,
            UseBlueColorForInfo = true,
            Title = "The refresh token is no longer valid. Please enter your password again...",
            Token = "Epic"
        });

        // close epic games launcher
        EpicGamesHelper.CloseEpicGames();

        // delay
        await Task.Delay(500);

        // launch epic games launcher
        Process.Start(EpicGamesHelper.EpicGamesPath);

        // check when logged in
        while (true)
        {
            if (File.Exists(EpicGamesHelper.ActiveEpicGamesAccountPath))
            {
                if (EpicGamesHelper.ValidateData(EpicGamesHelper.ActiveEpicGamesAccountPath))
                {
                    break;
                }
            }

            await Task.Delay(500);
        }

        // close epic games launcher
        EpicGamesHelper.CloseEpicGames();

        // disable tray and notifications
        EpicGamesHelper.DisableMinimizeToTray(EpicGamesHelper.ActiveEpicGamesAccountPath);
        EpicGamesHelper.DisableNotifications(EpicGamesHelper.ActiveEpicGamesAccountPath);

        // clear
        Growl.Clear("Epic");

        // add growl
        Growl.Success(new GrowlInfo
        {
            ShowDateTime = false,
            StaysOpen = false,
            IsClosable = false,
            Title = $"Successfully logged in as {EpicGamesHelper.GetAccountData(EpicGamesHelper.ActiveEpicGamesAccountPath).DisplayName}.",
            Token = "Epic"
        });

        // refresh combobox
        isInitializingEpicGamesAccounts = true;
        LoadEpicGamesAccounts();

        // refresh library
        await EpicGamesHelper.LoadGames();
        LoadSortSettings();
    }

    private async void AddEpicGamesAccount_Click(object sender, RoutedEventArgs e)
    {
        // add content dialog
        var contentDialog = new ContentDialog
        {
            Title = "Add Epic Games Account",
            Content = "Are you sure that you want to add an Epic Games account?",
            PrimaryButtonText = "Yes",
            CloseButtonText = "No",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot
        };

        ContentDialogResult result = await contentDialog.ShowAsync();

        // check result
        if (result == ContentDialogResult.Primary)
        {
            // add growl
            Growl.Info(new GrowlInfo
            {
                ShowDateTime = false,
                StaysOpen = true,
                IsClosable = false,
                UseBlueColorForInfo = true,
                Title = "Please log in to your Epic Games account...",
                Token = "Epic"
            });

            // close epic games launcher
            EpicGamesHelper.CloseEpicGames();

            // delete gameusersettings.ini
            if (File.Exists(EpicGamesHelper.ActiveEpicGamesAccountPath))
            {
                File.Delete(EpicGamesHelper.ActiveEpicGamesAccountPath);
            }

            // delay
            await Task.Delay(500);

            // launch epic games launcher
            Process.Start(EpicGamesHelper.EpicGamesPath);

            // check when logged in
            while (true)
            {
                if (File.Exists(EpicGamesHelper.ActiveEpicGamesAccountPath))
                {
                    if (EpicGamesHelper.ValidateData(EpicGamesHelper.ActiveEpicGamesAccountPath))
                    {
                        break;
                    }
                }

                await Task.Delay(500);
            }

            // close epic games launcher
            EpicGamesHelper.CloseEpicGames();

            // disable tray and notifications
            EpicGamesHelper.DisableMinimizeToTray(EpicGamesHelper.ActiveEpicGamesAccountPath);
            EpicGamesHelper.DisableNotifications(EpicGamesHelper.ActiveEpicGamesAccountPath);

            // clear
            Growl.Clear("Epic");

            // add growl
            Growl.Success(new GrowlInfo
            {
                ShowDateTime = false,
                StaysOpen = false,
                IsClosable = false,
                Title = $"Successfully logged in as {EpicGamesHelper.GetAccountData(EpicGamesHelper.ActiveEpicGamesAccountPath).DisplayName}.",
                Token = "Epic"
            });

            // refresh combobox
            isInitializingEpicGamesAccounts = true;
            LoadEpicGamesAccounts();

            // refresh library
            await EpicGamesHelper.LoadGames();
            LoadSortSettings();
        }
    }

    private async void RemoveEpicGamesAccount_Click(object sender, RoutedEventArgs e)
    {
        // add content dialog
        var contentDialog = new ContentDialog
        {
            Title = "Remove Epic Games Account",
            Content = $"Are you sure that you want to remove {(EpicGamesAccounts.SelectedItem as ComboBoxItem).Content}?",
            PrimaryButtonText = "Yes",
            CloseButtonText = "No",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot,
        };
        ContentDialogResult result = await contentDialog.ShowAsync();

        // check results
        if (result == ContentDialogResult.Primary)
        {
            // close epic games launcher
            EpicGamesHelper.CloseEpicGames();

            // get accountId
            string accountId = EpicGamesHelper.GetAccountData(EpicGamesHelper.ActiveEpicGamesAccountPath).AccountId;

            // remove account
            File.Delete(EpicGamesHelper.ActiveEpicGamesAccountPath);
            Directory.Delete(Path.Combine(EpicGamesHelper.EpicGamesAccountDir, accountId), true);

            // add growl
            Growl.Success(new GrowlInfo
            {
                ShowDateTime = false,
                StaysOpen = false,
                IsClosable = false,
                Title = $"Successfully removed {(EpicGamesAccounts.SelectedItem as ComboBoxItem).Content}.",
                Token = "Epic",
            });

            // refresh combobox
            isInitializingEpicGamesAccounts = true;
            LoadEpicGamesAccounts();

            // remove all epic games titles
            foreach (var item in Items.OfType<HeaderCarouselItem>().Where(item => item.Launcher == "Epic Games").ToList())
               Items.Remove(item);
        }
    }

    public void LoadSteamAccounts()
    {
        if (File.Exists(SteamHelper.SteamPath))
        {
            // get all accounts
            var accounts = SteamHelper.GetSteamAccounts();

            // reset ui elements
            SteamAccounts.Items.Clear();
            SteamAccounts.IsEnabled = true;
            RemoveSteamAccount.IsEnabled = true;

            // add accounts to combobox
            if (accounts.Count == 0)
            {
                var notLoggedIn = new ComboBoxItem { Content = "Not logged in", IsEnabled = false };
                SteamAccounts.Items.Add(notLoggedIn);
                SteamAccounts.SelectedItem = notLoggedIn;
                SteamAccounts.IsEnabled = false;
                RemoveSteamAccount.IsEnabled = false;
            }
            else if (accounts.All(a => !a.MostRecent) || accounts.All(a => !a.AllowAutoLogin))
            {
                var notLoggedIn = new ComboBoxItem { Content = "Not logged in", IsEnabled = false };
                SteamAccounts.Items.Add(notLoggedIn);
                SteamAccounts.SelectedItem = notLoggedIn;
                RemoveSteamAccount.IsEnabled = false;

                foreach (var account in accounts)
                {
                    SteamAccounts.Items.Add(account.AccountName);
                }
            }
            else
            {
                foreach (var account in accounts)
                {
                    SteamAccounts.Items.Add(account.AccountName);
                }

                int selectedIndex = accounts.FindIndex(a => a.MostRecent);
                if (selectedIndex < 0)
                    selectedIndex = accounts.FindIndex(a => a.AllowAutoLogin);

                SteamAccounts.SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
            }
        }
        else
        {
            SteamButton.Visibility = Visibility.Collapsed;
        }

        isInitializingSteamAccounts = false;
    }

    private async void SteamAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isInitializingSteamAccounts) return;

        // close steam
        SteamHelper.CloseSteam();

        // read file
        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Deserialize(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(SteamHelper.SteamLoginUsersPath))));

        // make all accounts inactive
        foreach (var user in kv.Children)
        {
            if (user["AccountName"]?.ToString() == SteamAccounts.SelectedItem.ToString())
            {
                user["MostRecent"] = "1";
                user["AllowAutoLogin"] = "1";
                user["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            }
            else
            {
                user["MostRecent"] = "0";
                user["AllowAutoLogin"] = "0";
            }
        }

        // write changes
        using var msOut = new MemoryStream();
        KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Serialize(msOut, kv);
        msOut.Position = 0;
        File.WriteAllText(SteamHelper.SteamLoginUsersPath, new StreamReader(msOut).ReadToEnd());

        // update registry key
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "reg.exe",
                Arguments = @$"add ""HKEY_CURRENT_USER\Software\Valve\Steam"" /v AutoLoginUser /t REG_SZ /d {SteamAccounts.SelectedItem} /f",
                CreateNoWindow = true,
                UseShellExecute = false
            }
        };
        process.Start();

        // refresh combobox
        isInitializingSteamAccounts = true;
        LoadSteamAccounts();

        // refresh library
        await SteamHelper.LoadGames();
        LoadSortSettings();
    }

    private async void AddSteamAccount_Click(object sender, RoutedEventArgs e)
    {
        // add content dialog
        var contentDialog = new ContentDialog
        {
            Title = "Add Steam Account",
            Content = "Are you sure that you want to add a Steam account?",
            PrimaryButtonText = "Yes",
            CloseButtonText = "No",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot
        };

        ContentDialogResult result = await contentDialog.ShowAsync();

        // check result
        if (result == ContentDialogResult.Primary)
        {
            // add growl
            Growl.Info(new GrowlInfo
            {
                ShowDateTime = false,
                StaysOpen = true,
                IsClosable = false,
                UseBlueColorForInfo = true,
                Title = "Info",
                Token = "Steam",
                Message = "Please log in to your Steam account..."
            });

            // close steam
            SteamHelper.CloseSteam();

            // read file
            var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Deserialize(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(SteamHelper.SteamLoginUsersPath))));

            // make all accounts inactive
            foreach (var user in kv.Children)
            {
                user["MostRecent"] = "0";
                user["AllowAutoLogin"] = "0";
            }

            // write changes
            using var msOut = new MemoryStream();
            KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Serialize(msOut, kv);
            msOut.Position = 0;
            File.WriteAllText(SteamHelper.SteamLoginUsersPath, new StreamReader(msOut).ReadToEnd());

            // delay
            await Task.Delay(500);

            // get initial user count
            int initialUserCount = KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Deserialize(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(SteamHelper.SteamLoginUsersPath)))).Children.Count();

            // launch steam
            Process.Start(SteamHelper.SteamPath);

            // check when logged in
            while (true)
            {
                if (KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Deserialize(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(SteamHelper.SteamLoginUsersPath)))).Children.Count() > initialUserCount)
                    break;

                await Task.Delay(500);
            }

            // close steam
            SteamHelper.CloseSteam();

            // refresh combobox
            isInitializingSteamAccounts = true;
            LoadSteamAccounts();

            // refresh library
            await SteamHelper.LoadGames();
            LoadSortSettings();

            // clear
            Growl.Clear("Steam");

            // add growl
            Growl.Success(new GrowlInfo
            {
                ShowDateTime = false,
                StaysOpen = false,
                IsClosable = false,
                Title = $"Successfully logged in as {SteamAccounts.SelectedItem}",
                Token = "Steam"
            });
        }
    }

    private async void RemoveSteamAccount_Click(object sender, RoutedEventArgs e)
    {
        // add content dialog
        var contentDialog = new ContentDialog
        {
            Title = "Remove Steam Account",
            Content = $"Are you sure that you want to remove {SteamAccounts.SelectedItem}?",
            PrimaryButtonText = "Yes",
            CloseButtonText = "No",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot,
        };
        ContentDialogResult result = await contentDialog.ShowAsync();

        // check results
        if (result == ContentDialogResult.Primary)
        {
            // close steam
            SteamHelper.CloseSteam();

            // read file
            var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text)
                                 .Deserialize(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(SteamHelper.SteamLoginUsersPath))));
            // remove selected account
            var newChildren = kv.Children.Where(c => c != kv.Children.First(child => child.Value["AccountName"]?.ToString() == SteamAccounts.SelectedItem.ToString()));
            var newRoot = new KVObject(kv.Name, newChildren);

            // write changes
            using var msOut = new MemoryStream();
            KVSerializer.Create(KVSerializationFormat.KeyValues1Text).Serialize(msOut, newRoot);
            msOut.Position = 0;
            File.WriteAllText(SteamHelper.SteamLoginUsersPath, new StreamReader(msOut).ReadToEnd());

            // add growl
            Growl.Success(new GrowlInfo
            {
                ShowDateTime = false,
                StaysOpen = false,
                IsClosable = false,
                Title = $"Successfully removed {SteamAccounts.SelectedItem}.",
                Token = "Epic",
            });

            // refresh combobox
            isInitializingSteamAccounts = true;
            LoadSteamAccounts();

            // remove all steam titles
            foreach (var item in Items.OfType<HeaderCarouselItem>().Where(item => item.Launcher == "Steam").ToList())
                Items.Remove(item);
        }
    }

    private async Task GetPresentationMode()
    {
        isInitializingPresentationMode = true;

        var selectedIndex = await Task.Run(() =>
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"System\GameConfigStore\Children");
            if (key == null) return 0;

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                using var subKey = key.OpenSubKey(subKeyName);
                if (subKey == null) continue;

                if (subKey.GetValueNames()
                          .Any(valueName => subKey.GetValue(valueName) is string str && str.Contains("Fortnite")))
                {
                    var flags = Convert.ToInt32(subKey.GetValue("Flags"));
                    return flags == 0x211 ? 1 : 0;
                }
            }

            return 0;
        });

        PresentationMode_ComboBox.SelectedIndex = selectedIndex;
        isInitializingPresentationMode = false;
    }


    private void PresentationMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isInitializingPresentationMode) return;

        using var key = Registry.CurrentUser.OpenSubKey(@"System\GameConfigStore\Children", writable: true);

        foreach (var subKeyName in key.GetSubKeyNames())
        {
            using var subKey = key.OpenSubKey(subKeyName, writable: true);

            if (subKey.GetValueNames().Any(valueName =>
                subKey.GetValue(valueName) is string strValue && strValue.Contains("Fortnite")))
            {
                if (PresentationMode_ComboBox.SelectedIndex == 0)
                {
                    using var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "reg.exe",
                            Arguments = $@"delete ""HKCU\System\GameConfigStore\Children\{subKeyName}"" /v Flags /f",
                            CreateNoWindow = true,
                        }
                    };
                    process.Start();
                    process.WaitForExit();
                }
                else if (PresentationMode_ComboBox.SelectedIndex == 1)
                {
                    using var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "reg.exe",
                            Arguments = $@"add ""HKCU\System\GameConfigStore\Children\{subKeyName}"" /v Flags /t REG_DWORD /d 0x211 /f",
                            CreateNoWindow = true,
                        }
                    };
                    process.Start();
                    process.WaitForExit();
                }
            }
        }
    }

    private void OpenInstallLocation_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(InstallLocation) && Directory.Exists(InstallLocation))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = @$"{InstallLocation}",
                UseShellExecute = true
            });
        }
    }

    private void Play_Click(object sender, RoutedEventArgs e)
    {
        if (Launcher == "Epic Games")
        {
            EpicGamesHelper.CloseEpicGames();
            Process.Start(new ProcessStartInfo($"com.epicgames.launcher://apps/{CatalogNamespace}%3A{CatalogItemId}%3A{AppName}?action=launch&silent=true") { UseShellExecute = true });
        }
        else if (Launcher == "Steam")
        {
            Process.Start(new ProcessStartInfo { FileName = @"C:\Program Files (x86)\Steam\steam.exe", Arguments = $"-applaunch {GameID} -silent" });
        }
        else if (Launcher == "Ryujinx")
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = LauncherLocation,
                Arguments = $@"-r ""{DataLocation}"" -fullscreen ""{GameLocation}""",
                CreateNoWindow = true,
            };

            Process.Start(startInfo);
        }
    }

    private async void Update_Click(object sender, RoutedEventArgs e)
    {
        if (Launcher == "Epic Games")
        {
            Process.Start(new ProcessStartInfo($"com.epicgames.launcher://apps/{CatalogNamespace}%3A{CatalogItemId}%3A{AppName}?action=update") { UseShellExecute = true });

            await Task.Delay(4000);

            Process.Start(new ProcessStartInfo($"com.epicgames.launcher://apps/{CatalogNamespace}%3A{CatalogItemId}%3A{AppName}?action=update") { UseShellExecute = true });
        }
    }

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetProcessWorkingSetSize(IntPtr process, int min, int max);

    private void StopProcesses_Click(object sender, RoutedEventArgs e)
    {
        // close dllhost processes
        foreach (var process in Process.GetProcessesByName("dllhost"))
        {
            try
            {
                using var searcher = new ManagementObjectSearcher($"SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name = 'dllhost.exe'");

                foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                {
                    string cmdLine = obj["CommandLine"]?.ToString() ?? "";
                    int pid = Convert.ToInt32(obj["ProcessId"]);

                    if (cmdLine.Contains("/PROCESSID", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var proc = Process.GetProcessById(pid);
                            proc.Kill();
                            proc.WaitForExit();
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        // close executables
        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "AutoRestartShell", 0, RegistryValueKind.DWord);

        var processNames = new[]
        {
            "ApplicationFrameHost",
            "CrashReportClient",
            "CrossDeviceResume",
            "ctfmon",
            "DataExchangeHost",
            "EasyAntiCheat_EOS",
            "EpicGamesLauncher",
            "explorer",
            //"Files",
            "FortniteClient-Win64-Shipping_EAC_EOS",
            "GameBar",
            "GameBarFTServer",
            "mobsync",
            "rundll32",
            "RuntimeBroker",
            "SearchHost",
            "secd",
            "ShellExperienceHost",
            "SpatialAudioLicenseSrv",
            "sppsvc",
            "StartMenuExperienceHost",
            "SystemSettingsBroker",
            "TrustedInstaller",
            "useroobebroker",
            "WMIADAP",
            "WmiPrvSE",
            "WUDFHost"
        };

        foreach (var name in processNames)
        {
            foreach (var process in Process.GetProcessesByName(name))
            {
                try { process.Kill(); process.WaitForExit(); } catch { }
            }

            foreach (var process in Process.GetProcessesByName(name))
            {
                try { process.Kill(); process.WaitForExit(); } catch { }
            }
        }

        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "AutoRestartShell", 1, RegistryValueKind.DWord);

        // stop services
        var serviceNames = new[]
        {
            "AudioEndpointBuilder",
            "AppXSvc",
            "Appinfo",
            "CaptureService",
            "cbdhsvc",
            "ClipSvc",
            "CryptSvc",
            "DevicesFlowUserSvc",
            //"DispBrokerDesktopSvc",
            //"Dnscache",
            "DoSvc",
            "gpsvc",
            "InstallService",
            "lfsvc",
            "msiserver",
            "netprofm",
            "nsi",
            "ProfSvc",
            "StateRepository",
            "TextInputManagementService",
            "TrustedInstaller",
            "UdkUserSvc",
            "UserManager",
            "WFDSConMgrSvc",
            "Windhawk",
            "Winmgmt"
        };

        foreach (var serviceName in serviceNames)
        {
            try
            {
                var searcher = new ManagementObjectSearcher($"SELECT ProcessId FROM Win32_Service WHERE Name LIKE '{serviceName}%'");
                foreach (ManagementObject service in searcher.Get().Cast<ManagementObject>())
                {
                    try
                    {
                        int pid = Convert.ToInt32(service["ProcessId"]);
                        var process = Process.GetProcessById(pid);
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch { }
                }
            }
            catch { }
        }

        foreach (var process in Process.GetProcesses())
        {
            try
            {
                SetProcessWorkingSetSize(process.Handle, -1, -1);
            }
            catch { }
        }
    }

    private void LaunchExplorer_Click(object sender, RoutedEventArgs e)
    {
        // start audioendpoint builder
        using var audioService = new ServiceController("AudioEndpointBuilder");
        if (audioService.Status == ServiceControllerStatus.Stopped)
        {
            audioService.Start();
        }

        // launch ctfmon
        Process.Start("ctfmon.exe");

        // launch explorer
        Process.Start("explorer.exe");

        // start windhawk service
        using var windhawkService = new ServiceController("Windhawk");
        if (windhawkService.Status == ServiceControllerStatus.Stopped)
        {
            windhawkService.Start();
        }
    }

    private DispatcherTimer gameWatcherTimer;
    private bool? previousGameState = null;
    private bool? previousExplorerState = null;
    private bool servicesState = false;

    async Task StartGameWatcherAsync(Func<Task<bool>> isGameRunningAsync)
    {
        if (gameWatcherTimer != null)
        {
            gameWatcherTimer.Stop();
            gameWatcherTimer = null;
        }

        previousGameState = null;
        previousExplorerState = null;

        servicesState = new ServiceController("Beep").Status == ServiceControllerStatus.Running;

        gameWatcherTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };

        async Task TickHandler()
        {
            bool isRunning = await isGameRunningAsync();
            bool explorerRunning = Process.GetProcessesByName("explorer").Length > 0;

            DispatcherQueue.TryEnqueue(() =>
            {
                if (Play != null)
                    Play.IsEnabled = !isRunning;

                if (StopProcesses != null && !servicesState)
                    StopProcesses.Visibility = isRunning ? Visibility.Visible : Visibility.Collapsed;

                if (LaunchExplorer != null && !servicesState && previousExplorerState != (isRunning && !explorerRunning))
                {
                    LaunchExplorer.Visibility = (isRunning && !explorerRunning) ? Visibility.Visible : Visibility.Collapsed;
                    previousExplorerState = isRunning && !explorerRunning;
                }

                if (previousGameState == true && isRunning == false && !explorerRunning)
                {
                    LaunchExplorer_Click(this, new RoutedEventArgs());
                }

                previousGameState = isRunning;
            });
        }

        gameWatcherTimer.Tick += async (s, e) => await Task.Run(TickHandler);

        await Task.Run(TickHandler);
        gameWatcherTimer.Start();
    }

    public async Task CheckGameRunningAsync()
    {
        previousGameState = null;
        previousExplorerState = null;

        if (Launcher == "Epic Games")
        {
            string offlineExecutable = Path.GetFileNameWithoutExtension(LaunchExecutable);
            string onlineExecutable = Title switch
            {
                "Fortnite" => "FortniteClient-Win64-Shipping",
                "Fall Guys" => "FallGuys_client_game.exe",
                _ => string.Empty
            };
            if (Title == "Fall Guys") offlineExecutable = "FallGuys_client.exe";

            await StartGameWatcherAsync(async () =>
                await Task.Run(() =>
                    Process.GetProcessesByName(offlineExecutable).Length > 0 ||
                    (!string.IsNullOrEmpty(onlineExecutable) &&
                     Process.GetProcessesByName(onlineExecutable).Length > 0)
                )
            );
        }
        else if (Launcher == "Steam")
        {
            string installLocation = InstallLocation;
            if (string.IsNullOrEmpty(installLocation)) return;

            var exeNames = await Task.Run(() =>
                Directory.GetFiles(installLocation, "*.exe")
                    .Select(Path.GetFileNameWithoutExtension)
                    .ToList()
            );

            if (exeNames.Count == 0) return;

            await StartGameWatcherAsync(async () =>
                await Task.Run(() =>
                    exeNames.Any(name =>
                        Process.GetProcessesByName(name).Length > 0)
                )
            );
        }
        else if (Launcher == "Ryujinx")
        {
            string launcherFileName = Path.GetFileName(LauncherLocation);
            string expectedPath = $@"-r ""{DataLocation}"" -fullscreen ""{GameLocation}""";

            await StartGameWatcherAsync(async () =>
                await Task.Run(() =>
                {
                    using var searcher = new ManagementObjectSearcher($"SELECT CommandLine FROM Win32_Process WHERE Name = '{launcherFileName}'");
                    foreach (var obj in searcher.Get().OfType<ManagementObject>())
                    {
                        string cmdLine = obj["CommandLine"]?.ToString() ?? "";
                        if (cmdLine.Contains(expectedPath))
                            return true;
                    }
                    return false;
                })
            );
        }
    }
}
