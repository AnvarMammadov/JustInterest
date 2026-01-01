using System.Collections.Generic;
using System.Linq;

namespace JustInterest.Items
{
    public class Inventory
    {
        private List<Item> _items;
        private const int MAX_SLOTS = 20;

        public int MaxSlots => MAX_SLOTS;
        public int ItemCount => _items.Count;
        public bool IsFull => _items.Count >= MAX_SLOTS;

        public Inventory()
        {
            _items = new List<Item>();
        }

        /// <summary>
        /// Item əlavə et
        /// </summary>
        public bool AddItem(Item item)
        {
            if (IsFull)
                return false;

            _items.Add(item);
            return true;
        }

        /// <summary>
        /// İndex-ə görə item götür
        /// </summary>
        public Item GetItem(int index)
        {
            if (index >= 0 && index < _items.Count)
                return _items[index];
            return null;
        }

        /// <summary>
        /// Item sil
        /// </summary>
        public bool RemoveItem(Item item)
        {
            return _items.Remove(item);
        }

        /// <summary>
        /// İndex-ə görə item sil
        /// </summary>
        public bool RemoveAt(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _items.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Item istifadə et (index-ə görə)
        /// </summary>
        public bool UseItem(int index, Player.PlayerStats stats)
        {
            var item = GetItem(index);
            if (item != null)
            {
                item.Use(stats);
                RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Bütün item-ləri qaytarır
        /// </summary>
        public List<Item> GetAllItems()
        {
            return _items.ToList();
        }

        /// <summary>
        /// Inventory-ni təmizlə
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }
    }
}
