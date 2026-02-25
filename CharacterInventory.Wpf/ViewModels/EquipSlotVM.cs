using System.ComponentModel;
using System.Runtime.CompilerServices;
using CharacterInventory.Core.Models;

namespace CharacterInventory.Wpf.ViewModels
{
    public enum EquipSlotKind
    {
        Torso, Legs, Boots, Accessory1, Accessory2, Weapon
    }

    public class EquipSlotVm : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public EquipSlotKind Kind { get; }

        private Item? _item;
        public Item? Item
        {
            get => _item;
            set { _item = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(HasItem)); 
                OnPropertyChanged(nameof(IconPath)); }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool HasItem => Item != null;
        public string? IconPath => Item?.ItemImagePath;

        public EquipSlotVm(EquipSlotKind kind) => Kind = kind;
    }
}
