using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using JustInterest.Items;

namespace JustInterest.UI
{
    public class InventoryUI
    {
        private Inventory _inventory;
        private int _selectedIndex;
        public bool ShouldClose { get; private set; }

        public void Open(Inventory inventory)
        {
            _inventory = inventory;
            _selectedIndex = 0;
            ShouldClose = false;
        }

        public void Update(KeyboardState keyState, KeyboardState previousKeyState, Player.PlayerStats stats)
        {
            // ESC - bağla
            if (keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape))
            {
                ShouldClose = true;
                return;
            }

            if (_inventory.ItemCount == 0)
                return;

            // Up/Down - seçim
            if (keyState.IsKeyDown(Keys.W) && previousKeyState.IsKeyUp(Keys.W))
            {
                _selectedIndex--;
                if (_selectedIndex < 0) _selectedIndex = _inventory.ItemCount - 1;
            }
            if (keyState.IsKeyDown(Keys.S) && previousKeyState.IsKeyUp(Keys.S))
            {
                _selectedIndex++;
                if (_selectedIndex >= _inventory.ItemCount) _selectedIndex = 0;
            }

            // E - item istifadə et
            if (keyState.IsKeyDown(Keys.E) && previousKeyState.IsKeyUp(Keys.E))
            {
                _inventory.UseItem(_selectedIndex, stats);
                if (_selectedIndex >= _inventory.ItemCount && _selectedIndex > 0)
                    _selectedIndex--;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D whitePixel, int screenWidth, int screenHeight)
        {
            // Semi-transparent overlay
            spriteBatch.Draw(whitePixel,
                new Rectangle(0, 0, screenWidth, screenHeight),
                Color.Black * 0.8f);

            // Panel
            int panelWidth = 600;
            int panelHeight = 500;
            int panelX = (screenWidth - panelWidth) / 2;
            int panelY = (screenHeight - panelHeight) / 2;

            spriteBatch.Draw(whitePixel,
                new Rectangle(panelX, panelY, panelWidth, panelHeight),
                Color.DarkSlateGray);

            // Border
            DrawBorder(spriteBatch, whitePixel, panelX, panelY, panelWidth, panelHeight, Color.White, 3);

            // Title
            if (font != null)
            {
                string title = "INVENTORY";
                spriteBatch.DrawString(font, title, new Vector2(panelX + 20, panelY + 15), Color.White);
                spriteBatch.DrawString(font, $"{_inventory.ItemCount}/{_inventory.MaxSlots}", 
                    new Vector2(panelX + panelWidth - 80, panelY + 15), Color.LightGray);
            }

            // Items list
            if (_inventory.ItemCount == 0)
            {
                if (font != null)
                    spriteBatch.DrawString(font, "Empty", new Vector2(panelX + 20, panelY + 60), Color.Gray);
            }
            else
            {
                var items = _inventory.GetAllItems();
                for (int i = 0; i < items.Count; i++)
                {
                    int itemY = panelY + 60 + i * 40;
                    Color bgColor = (i == _selectedIndex) ? Color.DarkGreen : Color.Transparent;
                    
                    spriteBatch.Draw(whitePixel,
                        new Rectangle(panelX + 15, itemY, panelWidth - 30, 35),
                        bgColor);

                    if (font != null)
                    {
                        string itemText = $"{items[i].Name}";
                        if (items[i].HungerRestore > 0)
                            itemText += $" [Hunger +{items[i].HungerRestore}]";
                        if (items[i].ThirstRestore > 0)
                            itemText += $" [Thirst +{items[i].ThirstRestore}]";

                        spriteBatch.DrawString(font, itemText, new Vector2(panelX + 25, itemY + 8), Color.White);
                    }
                }
            }

            // Controls
            if (font != null)
            {
                int controlsY = panelY + panelHeight - 40;
                spriteBatch.DrawString(font, "[W/S] Select  [E] Use  [ESC] Close", 
                    new Vector2(panelX + 20, controlsY), Color.LightGray);
            }
        }

        private void DrawBorder(SpriteBatch spriteBatch, Texture2D whitePixel, int x, int y, int width, int height, Color color, int thickness)
        {
            spriteBatch.Draw(whitePixel, new Rectangle(x, y, width, thickness), color);
            spriteBatch.Draw(whitePixel, new Rectangle(x, y + height - thickness, width, thickness), color);
            spriteBatch.Draw(whitePixel, new Rectangle(x, y, thickness, height), color);
            spriteBatch.Draw(whitePixel, new Rectangle(x + width - thickness, y, thickness, height), color);
        }
    }
}
