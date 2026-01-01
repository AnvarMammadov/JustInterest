using JustInterest.Player;

namespace JustInterest.Items
{
    public enum ItemType
    {
        Food,
        Drink,
        Hygiene,
        Other
    }

    public class Item
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        
        // Stat effects
        public float HungerRestore { get; set; }
        public float ThirstRestore { get; set; }
        public float HygieneRestore { get; set; }

        public Item(string id, string name, string description, ItemType type)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
        }

        /// <summary>
        /// Item-i istifadə et və stat-ları dəyişdir
        /// </summary>
        public void Use(PlayerStats stats)
        {
            if (HungerRestore > 0)
                stats.IncreaseHunger(HungerRestore);
            
            if (ThirstRestore > 0)
                stats.IncreaseThirst(ThirstRestore);
            
            if (HygieneRestore > 0)
                stats.IncreaseHygiene(HygieneRestore);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
