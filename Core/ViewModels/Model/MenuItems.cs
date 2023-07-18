using Core.Model.Enums;
using MvvmCross.ViewModels;

namespace Core.ViewModels.Model
{
    public class MenuItems : MvxNotifyPropertyChanged
    {
        public MenuItemType Type { get; set; }
        public string Description { get; set; }

        public string Icon
        {
            get => icon;
            set => SetProperty(ref icon, value);
        }
        private string icon;
        public bool Visible { get; set; }

        public MenuItems(MenuItemType type, string description, string icon, bool visible)
        {
            Type = type;
            Description = description;
            Icon = icon;
            Visible = visible;
        }
    }
}
