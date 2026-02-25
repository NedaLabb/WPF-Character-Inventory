namespace CharacterInventory.Core.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ItemType Type { get; set; }

        public int Count { get; set; } = 1;

        public string ItemImagePath { get; set; } = "";

        public string Description { get; set; } = "";
        public int MaxStack { get; set; } = 99;

        public int BonusHealth { get; set; }
        public int BonusStrength { get; set; }
        public int BonusStamina { get; set; }
        public int BonusMagicDefense { get; set; }

        public bool Consumable { get; set; }
        public int UseHealth { get; set; }
        public int UseStamina { get; set; }

        public string? StatsText { get; set; }

    }
}
