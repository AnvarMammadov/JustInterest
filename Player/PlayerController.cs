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

        /// <summary>
        /// WASD movement vector-unu qaytarır
        /// </summary>
        public Vector2 GetMovementInput()
        {
            Vector2 movement = Vector2.Zero;

            if (_currentKeyState.IsKeyDown(Keys.A))
                movement.X -= 1;
            if (_currentKeyState.IsKeyDown(Keys.D))
                movement.X += 1;

            // Normalize et ki diagonal hərəkət sürətli olmasın
            if (movement.Length() > 0)
                movement.Normalize();

            return movement;
        }

        /// <summary>
        /// W/S depth movement-i qaytarır
        /// </summary>
        public float GetDepthInput()
        {
            float depth = 0f;

            if (_currentKeyState.IsKeyDown(Keys.S))
                depth += 1; // İrəli = böyüdülür (scale artır)
            if (_currentKeyState.IsKeyDown(Keys.W))
                depth -= 1; // Geriyə = kiçildilir (scale azalır)

            return depth;
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
