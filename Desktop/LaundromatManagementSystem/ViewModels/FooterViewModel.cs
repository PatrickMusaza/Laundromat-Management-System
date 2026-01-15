using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LaundromatManagementSystem.ViewModels;

public class FooterViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Branch { get; set; } = "Main Branch";
    public string Username { get; set; } = "user";
    public string Role { get; set; } = "Manager";

    public string UserDisplay => $"{Username}"; //userDisplay format  => $"{Username ({Role})}"

    private string _currentTime;
    public string CurrentTime
    {
        get => _currentTime;
        set
        {
            _currentTime = value;
            OnPropertyChanged();
        }
    }

    public FooterViewModel()
    {
        UpdateTime();

        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            UpdateTime();
            return true;
        });
    }

    private void UpdateTime()
    {
        CurrentTime = DateTime.Now.ToString("HH:mm:ss");
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
