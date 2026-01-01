using Microsoft.Xna.Framework;
using System.Collections.Generic;
using JustInterest.Items;

namespace JustInterest.Interactions
{
    /// <summary>
    /// Soyuducu - içində yemək/içki saxlayır
    /// </summary>
    public class Fridge : InteractableObject
    {
        private List<Item> _items;

        public List<Item> Items => _items;

        public Fridge(Vector2 position, Rectangle bounds)
            : base(position, bounds, "FRIDGE")
        {
            _items = new List<Item>();
            InitializeItems();
        }

        private void InitializeItems()
        {
            // Başlanğıc item-lər
            _items.Add(ItemDatabase.GetItem("apple"));
            _items.Add(ItemDatabase.GetItem("sandwich"));
            _items.Add(ItemDatabase.GetItem("water"));
            _items.Add(ItemDatabase.GetItem("juice"));
        }

        /// <summary>
        /// İtem götür və soyuducudan sil
        /// </summary>
        public Item TakeItem(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                var item = _items[index];
                _items.RemoveAt(index);
                return item;
            }
            return null;
        }

        public override void OnInteract(Player.Player player)
        {
            // UI-ı açacağıq (Game1-də handle edəcəyik)
            // Buradan heç nə etmirik, sadəcə marker
        }
    }
}
