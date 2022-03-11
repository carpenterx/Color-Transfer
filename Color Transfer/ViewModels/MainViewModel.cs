using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Color_Transfer.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private string _materialPath;
    public string MaterialPath
    {
        get => _materialPath;
        set => SetField(ref _materialPath, value);
    }

    private string _swatchPath;
    public string SwatchPath
    {
        get => _swatchPath;
        set => SetField(ref _swatchPath, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public MainViewModel()
    {

    }
}
