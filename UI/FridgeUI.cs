using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using JustInterest.Interactions;
using JustInterest.Items;

namespace JustInterest.UI
{
    public class FridgeUI
    {
        private Fridge _fridge;
        private Inventory _inventory;
        private int _selectedIndex;
        public bool ShouldClose { get; private set; }

        public void Open(Fridge fridge, Inventory inventory)
        {
            _fridge = fridge;
            _inventory = inventory;
            _selectedIndex = 0;
            ShouldClose = false;
        }

        public void Update(KeyboardState keyState, KeyboardState previousKeyState)
        {
            // ESC - bağla
            if (keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape))
            {
                ShouldClose = true;
                return;
            }

            if (_fridge.Items.Count == 0)
                return;

            // Up/Down - seçim
            if (keyState.IsKeyDown(Keys.W) && previousKeyState.IsKeyUp(Keys.W))
            {
                _selectedIndex--;
                if (_selectedIndex < 0) _selectedIndex = _fridge.Items.Count - 1;
            }
            if (keyState.IsKeyDown(Keys.S) && previousKeyState.IsKeyUp(Keys.S))
            {
                _selectedIndex++;
                if (_selectedIndex >= _fridge.Items.Count) _selectedIndex = 0;
            }

            // E - item götür
            if (keyState.IsKeyDown(Keys.E) && previousKeyState.IsKeyUp(Keys.E))
            {
                if (!_inventory.IsFull)
                {
                    var item = _fridge.TakeItem(_selectedIndex);
                    if (item != null)
                    {
                        _inventory.AddItem(item);
                        if (_selectedIndex >= _fridge.Items.Count && _selectedIndex > 0)
                            _selectedIndex--;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D whitePixel, int screenWidth, int screenHeight)
        {
            // Semi-transparent overlay
            spriteBatch.Draw(whitePixel,
                new Rectangle(0, 0, screenWidth, screenHeight),
                Color.Black * 0.8f);

            // Panel
            int panelWidth = 700;
            int panelHeight = 500;
            int panelX = (screenWidth - panelWidth) / 2;
            int panelY = (screenHeight - panelHeight) / 2;

            spriteBatch.Draw(whitePixel,
                new Rectangle(panelX, panelY, panelWidth, panelHeight),
                new Color(40, 50, 60));

            // Border
            DrawBorder(spriteBatch, whitePixel, panelX, panelY, panelWidth, panelHeight, Color.White, 3);

            // Title
            if (font != null)
            {
                spriteBatch.DrawString(font, "FRIDGE", new Vector2(panelX + 20, panelY + 15), Color.White);
            }

            // Left side - Fridge items
            int leftX = panelX + 20;
            int leftWidth = panelWidth / 2 - 30;
            
            if (font != null)
            {
                spriteBatch.DrawString(font, "Fridge:", new Vector2(leftX, panelY + 50), Color.LightGray);
            }

            if (_fridge.Items.Count == 0)
            {
                if (font != null)
                    spriteBatch.DrawString(font, "Empty", new Vector2(leftX, panelY + 85), Color.Gray);
            }
            else
            {
                for (int i = 0; i < _fridge.Items.Count; i++)
                {
                    int itemY = panelY + 85 + i * 40;
                    Color bgColor = (i == _selectedIndex) ? new Color(50, 100, 50) : Color.Transparent;
                    
                    spriteBatch.Draw(whitePixel,
                        new Rectangle(leftX, itemY, leftWidth, 35),
                        bgColor);

                    if (font != null)
                    {
                        var item = _fridge.Items[i];
                        string itemText = item.Name;
                        spriteBatch.DrawString(font, itemText, new Vector2(leftX + 10, itemY + 8), Color.White);
                    }
                }
            }

            // Right side - Inventory preview
            int rightX = panelX + panelWidth / 2 + 10;
            int rightWidth = panelWidth / 2 - 30;

            if (font != null)
            {
                spriteBatch.DrawString(font, $"Your Inventory ({_inventory.ItemCount}/{_inventory.MaxSlots}):", 
                    new Vector2(rightX, panelY + 50), Color.LightGray);
            }

            var invItems = _inventory.GetAllItems();
            int displayCount = invItems.Count > 8 ? 8 : invItems.Count;
            for (int i = 0; i < displayCount; i++)
            {
                int itemY = panelY + 85 + i * 30;
                if (font != null)
                {
                    string itemText = $"- {invItems[i].Name}";
                    spriteBatch.DrawString(font, itemText, new Vector2(rightX, itemY), Color.LightGray);
                }
            }

            if (invItems.Count > 8 && font != null)
            {
                spriteBatch.DrawString(font, $"... and {invItems.Count - 8} more", 
                    new Vector2(rightX, panelY + 85 + 8 * 30), Color.Gray);
            }

            // Status message
            if (font != null)
            {
                int statusY = panelY + panelHeight - 80;
                if (_inventory.IsFull)
                {
                    spriteBatch.DrawString(font, "Inventory is full!", 
                        new Vector2(panelX + 20, statusY), Color.Red);
                }
            }

            // Controls
            if (font != null)
            {
                int controlsY = panelY + panelHeight - 40;
                spriteBatch.DrawString(font, "[W/S] Select  [E] Take  [ESC] Close", 
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
