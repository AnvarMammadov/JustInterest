using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JustInterest.Player
{
    /// <summary>
    /// Oyunçu karakteri - qız personaj
    /// </summary>
    public class Player
    {
        // Position və scale (depth illusion üçün)
        public Vector2 Position { get; set; }
        public float Scale { get; private set; } = 1.0f;

        // Movement boundaries - yeni scale range
        private const float MIN_SCALE = 0.6f;  // Arxada (kiçik) - uzaqda
        private const float MAX_SCALE = 1.2f;  // Öndə (böyük) - yaxında

        // Movement speed
        private const float HORIZONTAL_SPEED = 200f;
        private const float DEPTH_SPEED = 0.25f; // Scale dəyişmə sürəti

        // Texture və animation (placeholder)
        private Texture2D _texture;
        private Rectangle _bounds;

        // Stats referansı
        public PlayerStats Stats { get; private set; }

        // Cari location ID
        public string CurrentLocationId { get; set; }

        public Player(Vector2 startPosition)
        {
            Position = startPosition;
            Scale = MAX_SCALE; // Öndə başlayır (böyük)
            Stats = new PlayerStats(startingMoney: 50); // Az pulla başlayır
            CurrentLocationId = "PlayerRoom"; // Ev otağından başlayır
        }

        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
            if (_texture != null)
            {
                _bounds = new Rectangle(0, 0, _texture.Width, _texture.Height);
            }
        }

        public void Update(GameTime gameTime, Vector2 movement, float depthMovement, Rectangle movementBounds)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Horizontal hərəkət (A/D)
            if (movement != Vector2.Zero)
            {
                Vector2 newPosition = Position + movement * HORIZONTAL_SPEED * deltaTime;
                
                // Boundary check - ekranın içində qalsın
                newPosition.X = MathHelper.Clamp(newPosition.X, movementBounds.Left, movementBounds.Right);
                newPosition.Y = MathHelper.Clamp(newPosition.Y, movementBounds.Top, movementBounds.Bottom);
                
                Position = newPosition;
            }

            // Scale/Depth hərəkəti (W/S)
            float newScale = Scale + depthMovement * DEPTH_SPEED * deltaTime;
            newScale = MathHelper.Clamp(newScale, MIN_SCALE, MAX_SCALE);
            Scale = newScale;

            // Stats update
            Stats.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_texture != null)
            {
                // Sadə çəkmə - Position top-left corner-dir
                int width = (int)(_bounds.Width * Scale);
                int height = (int)(_bounds.Height * Scale);
                
                Rectangle destRect = new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    width,
                    height
                );

                spriteBatch.Draw(_texture, destRect, Color.White);
            }
        }

        public Rectangle GetBounds()
        {
            int width = (int)(_bounds.Width * Scale);
            int height = (int)(_bounds.Height * Scale);
            
            // Sadə qutu - Position top-left-dir
            return new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                width,
                height
            );
        }

        // Position setter (location dəyişəndə)
        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public void SetScale(float scale)
        {
            Scale = MathHelper.Clamp(scale, MIN_SCALE, MAX_SCALE);
        }
    }
}
