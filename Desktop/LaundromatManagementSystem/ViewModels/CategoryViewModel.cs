using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class CategoryViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _type;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _icon;

        [ObservableProperty]
        private string _color;

        [ObservableProperty]
        private bool _isSelected;

        [ObservableProperty]
        private Theme _theme;

        // Colors based on selection and theme
        public Color ButtonBackground => GetButtonBackground();
        public Color ButtonBorder => GetButtonBorder();
        public Color ButtonTextColor => GetButtonTextColor();
        public Color IconColor => GetIconColor();

        public IRelayCommand SelectCommand { get; }

        public CategoryViewModel(CategoryItem item, IRelayCommand selectCommand, Theme theme, bool isSelected = false)
        {
            _type = item.Type;
            _name = item.Name;
            _icon = item.Icon;
            _color = item.Color;
            _isSelected = isSelected;
            _theme = theme;
            SelectCommand = selectCommand;
        }

        partial void OnIsSelectedChanged(bool value)
        {
            OnPropertyChanged(nameof(ButtonBackground));
            OnPropertyChanged(nameof(ButtonBorder));
            OnPropertyChanged(nameof(ButtonTextColor));
            OnPropertyChanged(nameof(IconColor));
        }

        partial void OnThemeChanged(Theme value)
        {
            OnPropertyChanged(nameof(ButtonBackground));
            OnPropertyChanged(nameof(ButtonBorder));
            OnPropertyChanged(nameof(ButtonTextColor));
            OnPropertyChanged(nameof(IconColor));
        }

        private Color GetButtonBackground()
        {
            if (IsSelected)
                return Microsoft.Maui.Graphics.Color.FromArgb(Color);

            return Theme switch
            {
                Theme.Dark => Microsoft.Maui.Graphics.Color.FromArgb("#1F2937"),
                Theme.Gray => Colors.White,
                _ => Colors.White
            };
        }

        private Color GetButtonBorder()
        {
            if (IsSelected)
                return Colors.Transparent;

            return Theme switch
            {
                Theme.Dark => Microsoft.Maui.Graphics.Color.FromArgb("#374151"),
                Theme.Gray => Microsoft.Maui.Graphics.Color.FromArgb("#D1D5DB"),
                _ => Microsoft.Maui.Graphics.Color.FromArgb("#E5E7EB")
            };
        }

        private Color GetButtonTextColor()
        {
            if (IsSelected)
                return Colors.White;

            return Theme switch
            {
                Theme.Dark => Colors.White,
                Theme.Gray => Microsoft.Maui.Graphics.Color.FromArgb("#6B7280"),
                _ => Microsoft.Maui.Graphics.Color.FromArgb("#6B7280")
            };
        }

        private Color GetIconColor()
        {
            if (IsSelected)
                return Colors.White;

            return Theme switch
            {
                Theme.Dark => Colors.White,
                _ => Microsoft.Maui.Graphics.Color.FromArgb(Color)
            };
        }
    }
}