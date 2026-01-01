using System.Collections.Generic;

namespace JustInterest.Items
{
    public static class ItemDatabase
    {
        private static Dictionary<string, Item> _items;

        static ItemDatabase()
        {
            _items = new Dictionary<string, Item>();
            InitializeItems();
        }

        private static void InitializeItems()
        {
            // Food items
            var apple = new Item("apple", "Apple", "Fresh red apple", ItemType.Food)
            {
                HungerRestore = 30f
            };
            _items.Add("apple", apple);

            var sandwich = new Item("sandwich", "Sandwich", "Delicious chicken sandwich", ItemType.Food)
            {
                HungerRestore = 50f
            };
            _items.Add("sandwich", sandwich);

            var bread = new Item("bread", "Bread", "Fresh bread", ItemType.Food)
            {
                HungerRestore = 40f
            };
            _items.Add("bread", bread);

            // Drink items
            var water = new Item("water", "Water", "Clean water bottle", ItemType.Drink)
            {
                ThirstRestore = 40f
            };
            _items.Add("water", water);

            var juice = new Item("juice", "Juice", "Orange juice", ItemType.Drink)
            {
                ThirstRestore = 30f,
                HungerRestore = 10f
            };
            _items.Add("juice", juice);

            var cola = new Item("cola", "Cola", "Cold cola", ItemType.Drink)
            {
                ThirstRestore = 35f
            };
            _items.Add("cola", cola);
        }

        public static Item GetItem(string id)
        {
            if (_items.ContainsKey(id))
            {
                // Yeni nüsxə qaytarırıq (klonlama)
                var template = _items[id];
                return new Item(template.Id, template.Name, template.Description, template.Type)
                {
                    HungerRestore = template.HungerRestore,
                    ThirstRestore = template.ThirstRestore,
                    HygieneRestore = template.HygieneRestore
                };
            }
            return null;
        }

        public static List<Item> GetAllItems()
        {
            var list = new List<Item>();
            foreach (var key in _items.Keys)
            {
                list.Add(GetItem(key));
            }
            return list;
        }
    }
}
