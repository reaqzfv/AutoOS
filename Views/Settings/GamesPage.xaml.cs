namespace AutoOS.Views.Settings;

public sealed partial class GamesPage : Page
{
    public static GamesPage Instance { get; private set; }
    public Games.HeaderCarousel.HeaderCarousel Games => games;

    public GamesPage()
    {
        Instance = this;
        InitializeComponent();
    }
}