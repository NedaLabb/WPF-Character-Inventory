using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CharacterInventory.Core.Managers;
using CharacterInventory.Core.Models;
using System;
using System.Linq;

namespace CharacterInventory.Wpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ObservableCollection<Character> Characters { get; } = new();

        private Character? _selectedCharacter;
        public Character? SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                if (_selectedCharacter == value) return;
                _selectedCharacter = value;
                OnPropertyChanged();
                LoadSlotsFromCharacter();
            }
        }

        public ObservableCollection<InventorySlotVm> Slots { get; } = new();

        public Item? DisplayItem => SelectedEquipSlot?.Item ?? SelectedSlot?.Item;
        public bool HasDisplayItem => DisplayItem != null;

        private InventorySlotVm? _selectedSlot;
        public InventorySlotVm? SelectedSlot
        {
            get => _selectedSlot;
            set
            {
                if (_selectedSlot == value) return;
                _selectedSlot = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayItem));
                OnPropertyChanged(nameof(HasDisplayItem));
            }
        }

        public void SelectSlot(InventorySlotVm? slot)
        {
            foreach (var s in Slots) s.IsSelected = false;

            SelectedSlot = slot;
            if (slot != null) slot.IsSelected = true;

            SelectEquipSlot(null);
        }

        public void SelectEquipSlot(EquipSlotVm? slot)
        {
            foreach (var e in EquipSlots) e.IsSelected = false;

            SelectedEquipSlot = slot;

            if (slot != null)
            {
                slot.IsSelected = true;

                foreach (var s in Slots) s.IsSelected = false;
                SelectedSlot = null;
            }

            OnPropertyChanged(nameof(DisplayItem));
            OnPropertyChanged(nameof(HasDisplayItem));
        }

        public ObservableCollection<EquipSlotVm> EquipLeftSlots { get; } = new();
        public ObservableCollection<EquipSlotVm> EquipRightSlots { get; } = new();

        public MainViewModel()
        {

            for (int i = 0; i < 36; i++)
            {
                Slots.Add(new InventorySlotVm { Index = i });
            }

            var torso = new EquipSlotVm(EquipSlotKind.Torso);
            var legs = new EquipSlotVm(EquipSlotKind.Legs);
            var boots = new EquipSlotVm(EquipSlotKind.Boots);
            var acc1 = new EquipSlotVm(EquipSlotKind.Accessory1);
            var acc2 = new EquipSlotVm(EquipSlotKind.Accessory2);
            var weapon = new EquipSlotVm(EquipSlotKind.Weapon);

            EquipSlots.Add(torso);
            EquipSlots.Add(legs);
            EquipSlots.Add(boots);
            EquipSlots.Add(acc1);
            EquipSlots.Add(acc2);
            EquipSlots.Add(weapon);
            EquipLeftSlots.Add(acc1);
            EquipLeftSlots.Add(acc2);
            EquipLeftSlots.Add(weapon);

            EquipRightSlots.Add(torso);
            EquipRightSlots.Add(legs);
            EquipRightSlots.Add(boots); 

            CharacterManager.LoadCharacters();
            foreach (var c in CharacterManager.Characters)
                Characters.Add(c);

            SelectedCharacter = Characters.FirstOrDefault();
        }

        public ObservableCollection<EquipSlotVm> EquipSlots { get; } = new();

        private EquipSlotVm GetEquip(EquipSlotKind kind) => EquipSlots.First(s => s.Kind == kind);

        private void LoadEquipFromCharacter()
        {
            if (SelectedCharacter == null) return;

            GetEquip(EquipSlotKind.Torso).Item = SelectedCharacter.TorsoSlot;
            GetEquip(EquipSlotKind.Legs).Item = SelectedCharacter.LegSlot;
            GetEquip(EquipSlotKind.Boots).Item = SelectedCharacter.BootSlot;
            GetEquip(EquipSlotKind.Accessory1).Item = SelectedCharacter.AccessorySlot1;
            GetEquip(EquipSlotKind.Accessory2).Item = SelectedCharacter.AccessorySlot2;
            GetEquip(EquipSlotKind.Weapon).Item = SelectedCharacter.LeftHand;
        }


        public void MoveOrStack(InventorySlotVm from, InventorySlotVm to)
        {
            if (from == to) return;
            if (from.Item == null) return;

            if (to.Item == null)
            {
                to.Item = from.Item;
                from.Item = null;
                SelectSlot(to);
                return;
            }

            if (to.Item.Id == from.Item.Id)
            {
                to.Item.Count += from.Item.Count;

                var tmp = to.Item;
                to.Item = null;
                to.Item = tmp;

                from.Item = null;
                SelectSlot(to);
                return;
            }

            (to.Item, from.Item) = (from.Item, to.Item);
            SelectSlot(to);
        }



        private void LoadSlotsFromCharacter()
        {
            foreach (var s in Slots)
                s.Clear();

            if (SelectedCharacter?.Inventory == null) return;

            var items = SelectedCharacter.Inventory;

            for (int i = 0; i < items.Count && i < Slots.Count; i++)
            {
                var it = items[i];
                Slots[i].Item = it;
            }

            SelectSlot(Slots.FirstOrDefault(s => s.HasItem));
            LoadEquipFromCharacter();
            RaiseStats();
        }

        public bool TryEquip(InventorySlotVm fromInv)
        {
            if (SelectedCharacter == null) return false;
            if (fromInv.Item == null) return false;

            var target = GetEquipSlotForItem(fromInv.Item);
            if (target == null) return false;

            var old = target.Item;
            target.Item = fromInv.Item;
            fromInv.Item = old;

            SaveEquipBackToCharacter();
            RaiseStats();
            return true;

        }

        public bool TryUnequip(EquipSlotVm fromEquip)
        {
            if (SelectedCharacter == null) return false;
            if (fromEquip.Item == null) return false;

            var empty = Slots.FirstOrDefault(s => s.Item == null);
            if (empty == null) return false;

            empty.Item = fromEquip.Item;
            fromEquip.Item = null;

            SaveEquipBackToCharacter();
            RaiseStats();
            return true;
        }

        private EquipSlotVm? GetEquipSlotForItem(Item item)
        {
            switch (item.Type)
            {
                case ItemType.Accessory:
                    var a1 = GetEquip(EquipSlotKind.Accessory1);
                    if (a1.Item == null) return a1;

                    var a2 = GetEquip(EquipSlotKind.Accessory2);
                    if (a2.Item == null) return a2;

                    return a1;

                case ItemType.Weapon: return GetEquip(EquipSlotKind.Weapon);
                case ItemType.Boots: return GetEquip(EquipSlotKind.Boots);
                case ItemType.Armor: return GetEquip(EquipSlotKind.Torso);
                case ItemType.Pants: return GetEquip(EquipSlotKind.Legs);
                default: return null;
            }
        }
        private EquipSlotVm? _selectedEquipSlot;
        public EquipSlotVm? SelectedEquipSlot
        {
            get => _selectedEquipSlot;
            set
            {
                if (_selectedEquipSlot == value) return;
                _selectedEquipSlot = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayItem));
                OnPropertyChanged(nameof(HasDisplayItem));
            }
        }

        private void SaveEquipBackToCharacter()
        {
            if (SelectedCharacter == null) return;

            SelectedCharacter.LegSlot = GetEquip(EquipSlotKind.Legs).Item;
            SelectedCharacter.TorsoSlot = GetEquip(EquipSlotKind.Torso).Item;
            SelectedCharacter.BootSlot = GetEquip(EquipSlotKind.Boots).Item;
            SelectedCharacter.AccessorySlot1 = GetEquip(EquipSlotKind.Accessory1).Item;
            SelectedCharacter.AccessorySlot2 = GetEquip(EquipSlotKind.Accessory2).Item;
            SelectedCharacter.LeftHand = GetEquip(EquipSlotKind.Weapon).Item;

        }

        public bool TryUseFood(InventorySlotVm slot)
        {
            if (slot.Item == null) return false;

            var item = slot.Item;

            if (item.Type != ItemType.Food) return false;

            _foodBonusHealth += item.BonusHealth;
            _foodBonusStrength += item.BonusStrength;
            _foodBonusStamina += item.BonusStamina;
            _foodBonusMagicDefense += item.BonusMagicDefense;

            if (item.Count > 1)
            {
                item.Count--;
                slot.Item = item; 
            }
            else
            {
                slot.Item = null; 
            }

            RaiseStats();
            return true;
        }

        public int TotalHealth => (SelectedCharacter?.Health ?? 0) + GetEquippedBonusHealth() + FoodBonusHealth;
        public int TotalStrength => (SelectedCharacter?.Strength ?? 0) + GetEquippedBonusStrength() + FoodBonusStrength;
        public int TotalStamina => (SelectedCharacter?.Stamina ?? 0) + GetEquippedBonusStamina() + FoodBonusStamina;

        public int TotalMagicDefense => (SelectedCharacter?.MagicDefense ?? 0) + GetEquippedBonusMagicDefense() + FoodBonusMagicDefense;

        private int GetEquippedBonusHealth() =>
            EquipSlots.Where(s => s.Item != null).Sum(s => s.Item!.BonusHealth);

        private int GetEquippedBonusStrength() =>
            EquipSlots.Where(s => s.Item != null).Sum(s => s.Item!.BonusStrength);

        private int GetEquippedBonusStamina() =>
            EquipSlots.Where(s => s.Item != null).Sum(s => s.Item!.BonusStamina);

        private int GetEquippedBonusMagicDefense() =>
            EquipSlots.Where(s => s.Item != null).Sum(s => s.Item!.BonusMagicDefense);

        private void RaiseStats()
        {
            OnPropertyChanged(nameof(FoodBonusHealth));
            OnPropertyChanged(nameof(FoodBonusStrength));
            OnPropertyChanged(nameof(FoodBonusStamina));
            OnPropertyChanged(nameof(FoodBonusMagicDefense));

            OnPropertyChanged(nameof(TotalHealth));
            OnPropertyChanged(nameof(TotalStrength));
            OnPropertyChanged(nameof(TotalStamina));
            OnPropertyChanged(nameof(TotalMagicDefense));
        }

        private int _foodBonusHealth;
        private int _foodBonusStrength;
        private int _foodBonusStamina;
        private int _foodBonusMagicDefense;

        public int FoodBonusHealth => _foodBonusHealth;
        public int FoodBonusStrength => _foodBonusStrength;
        public int FoodBonusStamina => _foodBonusStamina;
        public int FoodBonusMagicDefense => _foodBonusMagicDefense;
    }
}
