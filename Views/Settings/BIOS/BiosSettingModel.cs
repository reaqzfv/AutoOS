using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AutoOS.Views.Settings.BIOS;

public class BiosSettingModel : INotifyPropertyChanged
{
    private bool _isLoaded = false;
    private string _value;
    private Option _selectedOption;

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public void MarkLoaded() => _isLoaded = true;

    public int Line { get; set; }

    // ─────── State Tracking ───────
    public string OriginalValue { get; set; }
    public Option OriginalSelectedOption { get; set; }

    // ─────── Display Helpers ───────
    public string DisplayBiosDefault => $"Default: {BiosDefault}";
    public string DisplayCurrent =>
        OriginalSelectedOption?.Label ?? OriginalValue ?? SelectedOption?.Label ?? Value;

    public string DisplayRecommended =>
        RecommendedOption?.Label ?? RecommendedValue;

    // ─────── Metadata ───────
    public string SetupQuestion { get; set; }
    public string HelpString { get; set; }
    public string Token { get; set; }
    public string Offset { get; set; }
    public string Width { get; set; }
    public string BiosDefault { get; set; }

    // ─────── Recommendation Info ───────
    public bool IsRecommended { get; set; }
    public string RecommendedValue { get; set; }
    public Option RecommendedOption { get; set; }

    // ─────── Value / Options ───────
    public string Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                var oldValue = _value;
                _value = value;
                OnPropertyChanged();

                if (_isLoaded)
                    Debug.WriteLine($"[Value] '{SetupQuestion}' changed from '{oldValue}' to '{_value}'");
            }
        }
    }
    public List<Option> Options { get; set; } = [];

    public Option SelectedOption
    {
        get => _selectedOption;
        set
        {
            if (_selectedOption != value)
            {
                _selectedOption = value;
                OnPropertyChanged();

                foreach (var opt in Options)
                    opt.IsSelected = opt == value;
            }
        }
    }

    // ─────── UI Conditions ───────
    public bool HasDefault => !string.IsNullOrEmpty(BiosDefault);
    public bool HasOptions => Options.Count > 0;
    public bool HasValueField => Value != null && !HasOptions;

    public void InitializeSelectedOption()
    {
        SelectedOption = Options.Find(o => o.IsSelected);
    }
}

public class Option : INotifyPropertyChanged
{
    private bool _isSelected;

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    // ─────── Option Info ───────
    public string Index { get; set; }
    public string Label { get; set; }

    public BiosSettingModel Parent { get; set; }

    // ─────── Flags ───────
    public bool IsRecommendedOption => this == Parent?.RecommendedOption;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                var oldSelected = Parent?.SelectedOption;
                _isSelected = value;
                OnPropertyChanged();

                if (_isSelected && Parent != null)
                {
                    foreach (var opt in Parent.Options)
                    {
                        if (!ReferenceEquals(opt, this) && opt.IsSelected)
                            opt.IsSelected = false;
                    }

                    string oldLabel = oldSelected?.Label;
                    string oldIndex = oldSelected?.Index;

                    Debug.WriteLine($"[Option] '{Parent.SetupQuestion}' changed from '{oldLabel} {oldIndex}' to '{Label} {Index}'{Parent.Line}");

                    Parent.SelectedOption = this;
                }
            }
        }
    }
}



