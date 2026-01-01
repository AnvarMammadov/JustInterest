using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JustInterest.Player
{
    /// <summary>
    /// Player input-unu handle edir
    /// </summary>
    public class PlayerController
    {
        private KeyboardState _previousKeyState;
        private KeyboardState _currentKeyState;

        public PlayerController()
        {
            _previousKeyState = Keyboard.GetState();
        }

        public void Update()
        {
            _previousKeyState = _currentKeyState;
            _currentKeyState = Keyboard.GetState();
        }

        public KeyboardState GetCurrentKeyState() => _currentKeyState;
        public KeyboardState GetPreviousKeyState() => _previousKeyState;

        /// <summary>
        /// WASD movement vector-unu qaytarır (X və Y)
        /// </summary>
        public Vector2 GetMovementInput()
        {
            Vector2 movement = Vector2.Zero;

            // Horizontal
            if (_currentKeyState.IsKeyDown(Keys.A))
                movement.X -= 1;
            if (_currentKeyState.IsKeyDown(Keys.D))
                movement.X += 1;

            // Vertical (Dərinlik)
            if (_currentKeyState.IsKeyDown(Keys.W))
                movement.Y -= 1; // Yuxarı (uzağa)
            if (_currentKeyState.IsKeyDown(Keys.S))
                movement.Y += 1; // Aşağı (yaxına)

            // Normalize et ki diagonal hərəkət sürətli olmasın
            if (movement.Length() > 0)
                movement.Normalize();

            return movement;
        }



        /// <summary>
        /// Inventory key - I basanda qaytarır
        /// </summary>
        public bool IsInventoryPressed()
        {
            return _currentKeyState.IsKeyDown(Keys.I) && _previousKeyState.IsKeyUp(Keys.I);
        }

        /// <summary>
        /// Key press check (bir dəfə basıldı?)
        /// </summary>
        public bool IsKeyPressed(Keys key)
        {
            return _currentKeyState.IsKeyDown(key) && _previousKeyState.IsKeyUp(key);
        }

        /// <summary>
        /// Key hold check
        /// </summary>
        public bool IsKeyDown(Keys key)
        {
            return _currentKeyState.IsKeyDown(key);
        }

        // Interaction key (E)
        public bool IsInteractPressed()
        {
            return IsKeyPressed(Keys.E);
        }

        // Pause/Menu (Escape)
        public bool IsPausePressed()
        {
            return IsKeyPressed(Keys.Escape);
        }
    }
}
