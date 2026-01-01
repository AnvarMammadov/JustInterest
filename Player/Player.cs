using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace JustInterest.Player
{
    /// <summary>
    /// Xarakterin baxış istiqaməti
    /// </summary>
    public enum Direction
    {
        Front,  // Ön (aşağı)
        Back,   // Arxa (yuxarı)
        Left,   // Sol
        Right   // Sağ
    }

    /// <summary>
    /// Oyunçu karakteri - qız personaj
    /// </summary>
    public class Player
    {
        // Position və scale (depth illusion üçün)
        public Vector2 Position { get; set; }
        public float Scale { get; private set; } = 1.0f;

        // Movement boundaries - PNG çox böyükdür, Scale-i çox kiçik tutmalıyıq
        private const float MIN_SCALE = 0.15f;  // Arxada (kiçik)
        private const float MAX_SCALE = 0.35f;  // Öndə (böyük) - normal ölçü

        // Movement speed
        private const float HORIZONTAL_SPEED = 200f;
        private const float VERTICAL_SPEED = 150f; // Y oxu sürəti

        // Texture və animation
        private Dictionary<Direction, Texture2D> _directionTextures;
        private Texture2D _currentTexture;
        private Rectangle _bounds;
        public Direction CurrentDirection { get; private set; } = Direction.Front;

        // Stats referansı
        public PlayerStats Stats { get; private set; }

        // Cari location ID
        public string CurrentLocationId { get; set; }

        public Player(Vector2 startPosition)
        {
            Position = startPosition;
            Scale = MAX_SCALE; // MAX scale ilə başla (ən aşağıda)
            Stats = new PlayerStats(startingMoney: 50);
            CurrentLocationId = "PlayerRoom";
            _directionTextures = new Dictionary<Direction, Texture2D>();
        }

        public void SetDirectionTextures(Dictionary<Direction, Texture2D> textures)
        {
            _directionTextures = textures;
            // Default olaraq front texture istifadə et
            if (_directionTextures.ContainsKey(Direction.Front))
            {
                _currentTexture = _directionTextures[Direction.Front];
                _bounds = new Rectangle(0, 0, _currentTexture.Width, _currentTexture.Height);
            }
        }

        public void SetTexture(Texture2D texture)
        {
            _currentTexture = texture;
            if (_currentTexture != null)
            {
                _bounds = new Rectangle(0, 0, _currentTexture.Width, _currentTexture.Height);
            }
        }

        public void Update(GameTime gameTime, Vector2 movement, Rectangle movementBounds)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Hərəkət (Həm X, Həm Y) və istiqamət təyini
            if (movement != Vector2.Zero)
            {
                // İstiqaməti təyin et
                UpdateDirection(movement);

                Vector2 velocity = new Vector2(movement.X * HORIZONTAL_SPEED, movement.Y * VERTICAL_SPEED);
                Vector2 newPosition = Position + velocity * deltaTime;
                
                // Boundary check
                newPosition.X = MathHelper.Clamp(newPosition.X, movementBounds.Left, movementBounds.Right);
                newPosition.Y = MathHelper.Clamp(newPosition.Y, movementBounds.Top, movementBounds.Bottom);
                
                Position = newPosition;
            }

            // AVTOMATIİK SCALE HESABLAMA (Position.Y-ə görə)
            float range = movementBounds.Height;
            if (range > 0)
            {
                float positionInBounds = Position.Y - movementBounds.Top;
                float percentage = positionInBounds / range; // 0..1
                
                percentage = MathHelper.Clamp(percentage, 0f, 1f);
                Scale = MathHelper.Lerp(MIN_SCALE, MAX_SCALE, percentage);
            }

            // Stats update
            Stats.Update(gameTime);
        }

        private void UpdateDirection(Vector2 movement)
        {
            // Ən gülü axis-ə görə istiqamət təyin et
            if (Math.Abs(movement.X) > Math.Abs(movement.Y))
            {
                // Horizontal hərəkət dominant
                CurrentDirection = movement.X < 0 ? Direction.Left : Direction.Right;
            }
            else
            {
                // Vertical hərəkət dominant
                CurrentDirection = movement.Y < 0 ? Direction.Back : Direction.Front;
            }

            // Texture-u dəyişdir
            if (_directionTextures.ContainsKey(CurrentDirection))
            {
                _currentTexture = _directionTextures[CurrentDirection];
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_currentTexture != null)
            {
                int width = (int)(_bounds.Width * Scale);
                int height = (int)(_bounds.Height * Scale);
                
                // DÜZƏLİŞ: Anchor Bottom-Center (Position = Feet)
                Rectangle destRect = new Rectangle(
                    (int)(Position.X - width / 2), 
                    (int)(Position.Y - height),
                    width,
                    height
                );

                spriteBatch.Draw(_currentTexture, destRect, Color.White);
            }
        }

        public Rectangle GetBounds()
        {
            int width = (int)(_bounds.Width * Scale);
            int height = (int)(_bounds.Height * Scale);
            
            // Bounds da Bottom-Center
            return new Rectangle(
                (int)(Position.X - width / 2),
                (int)(Position.Y - height),
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
