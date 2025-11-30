using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoOS.Views.Settings.Scheduling.Models;

public class CpuCore : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private ObservableCollection<CpuThread> _threads = new();

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public ObservableCollection<CpuThread> Threads
    {
        get => _threads;
        set => SetProperty(ref _threads, value);
    }

    public byte CoreIndex { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
