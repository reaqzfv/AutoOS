using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoOS.Views.Settings.Scheduling.Models;

public class CpuThread : INotifyPropertyChanged
{
    private bool _isSelected;
    private uint _cpuId;
    private string _name = string.Empty;

    public uint CpuId
    {
        get => _cpuId;
        set => SetProperty(ref _cpuId, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public ulong BitMask { get; set; }

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

