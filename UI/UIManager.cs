using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using JustInterest.Interactions;

namespace JustInterest.UI
{
    public class UIManager
    {
        public UIState CurrentUI { get; private set; }
        
        private InventoryUI _inventoryUI;
        private FridgeUI _fridgeUI;
        
        private Fridge _activeFridge; // Hansı soyuducu açılmış
        private Player.Player _player;

        public UIManager(Player.Player player)
        {
            _player = player;
            CurrentUI = UIState.None;
            _inventoryUI = new InventoryUI();
            _fridgeUI = new FridgeUI();
        }

        public void OpenInventoryUI()
        {
            CurrentUI = UIState.InventoryUI;
            _inventoryUI.Open(_player.Inventory);
        }

        public void OpenFridgeUI(Fridge fridge)
        {
            CurrentUI = UIState.FridgeUI;
            _activeFridge = fridge;
            _fridgeUI.Open(fridge, _player.Inventory);
        }

        public bool CloseUI()
        {
            if (CurrentUI != UIState.None)
            {
                CurrentUI = UIState.None;
                _activeFridge = null;
                return true;
            }
            return false;
        }

        public void Update(GameTime gameTime, KeyboardState keyState, KeyboardState previousKeyState)
        {
            switch (CurrentUI)
            {
                case UIState.InventoryUI:
                    _inventoryUI.Update(keyState, previousKeyState, _player.Stats);
                    if (_inventoryUI.ShouldClose)
                    {
                        CloseUI();
                    }
                    break;

                case UIState.FridgeUI:
                    _fridgeUI.Update(keyState, previousKeyState);
                    if (_fridgeUI.ShouldClose)
                    {
                        CloseUI();
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Texture2D whitePixel, int screenWidth, int screenHeight)
        {
            switch (CurrentUI)
            {
                case UIState.InventoryUI:
                    _inventoryUI.Draw(spriteBatch, font, whitePixel, screenWidth, screenHeight);
                    break;

                case UIState.FridgeUI:
                    _fridgeUI.Draw(spriteBatch, font, whitePixel, screenWidth, screenHeight);
                    break;
            }
        }

        public bool IsUIActive => CurrentUI != UIState.None;
    }
}
