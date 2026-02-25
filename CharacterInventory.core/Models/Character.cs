using System.Collections.Generic;

namespace CharacterInventory.Core.Models
{
    public class Character
    {
        public string? CharacterImage { get; set; }

        public string Name { get; set; } = "";
        public string Role { get; set; } = "";
        public string Description { get; set; } = "";
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Stamina { get; set; }
        public int MagicDefense { get; set; }

        public List<Item> Inventory { get; set; } = new();

        public Item? TorsoSlot { get; set; }
        public Item? LegSlot { get; set; }
        public Item? BootSlot { get; set; }
        public Item? AccessorySlot1 { get; set; }
        public Item? AccessorySlot2 { get; set; }

        public Item? LeftHand { get; set; }

    }
}
