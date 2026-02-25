using System.ComponentModel;
using System.Runtime.CompilerServices;
using CharacterInventory.Core.Models;


namespace CharacterInventory.Wpf.ViewModels
{
    public class InventorySlotVm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public int Index { get; set; }

        private string? _name;
        public string? Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private string? _type;
        public string? Type { get => _type; set { _type = value; OnPropertyChanged(); } }

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowCount));
            }
        }

        private string? _description;
        public string? Description { get => _description; set { _description = value; OnPropertyChanged(); } }

        private string? _iconPath;
        public string? IconPath { get => _iconPath; set { _iconPath = value; OnPropertyChanged(); } }


        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected; set { _isSelected = value; OnPropertyChanged(); }
        }

        private ItemType? _itemType;
        public ItemType? ItemType
        {
            get => _itemType; set { _itemType = value; OnPropertyChanged(); }
        }
        
        private Item? _item;
        public Item? Item
        {
            get => _item;
            set
            {
                _item = value;

                Name = _item?.Name;
                Type = _item?.Type.ToString();
                Count = _item?.Count ?? 0;
                Description = _item?.Description;
                IconPath = _item?.ItemImagePath;
                ItemType = _item?.Type;


                OnPropertyChanged();
                OnPropertyChanged(nameof(HasItem));
                OnPropertyChanged(nameof(ShowCount));
            }
        }


        public bool HasItem => Item != null;

        public bool ShowCount => HasItem && Count > 1;

        public void Clear()
        {
            Item = null;
        }
    }
}
