using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CharacterInventory.Wpf.ViewModels;

namespace CharacterInventory.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Slot_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CharacterInventory.Wpf.ViewModels.MainViewModel vm &&
                sender is FrameworkElement fe &&
                fe.DataContext is CharacterInventory.Wpf.ViewModels.InventorySlotVm slot)
            {
                vm.SelectSlot(slot);
            }
        }
        private void Slot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2) return;

            if (sender is FrameworkElement fe && fe.DataContext is InventorySlotVm slot)
            {
                if (DataContext is MainViewModel vm)
                {
                    if (slot.Item?.Type == CharacterInventory.Core.Models.ItemType.Food)
                    {
                        vm.TryUseFood(slot);
                        return;
                    }
                    vm.TryEquip(slot);
                }
            }
        }

        private void EquipSlot_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm &&
                sender is FrameworkElement fe &&
                fe.DataContext is EquipSlotVm equip)
            {
                vm.SelectEquipSlot(equip);
            }
        }


        private void EquipSlot_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button b && b.DataContext is EquipSlotVm equip)
                ((MainViewModel)DataContext).TryUnequip(equip);
        }


    }
}
