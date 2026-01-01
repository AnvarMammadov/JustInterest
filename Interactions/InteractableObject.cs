using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JustInterest.Interactions
{
    /// <summary>
    /// Bütün interaktiv obyektlər üçün base class
    /// </summary>
    public abstract class InteractableObject
    {
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; set; }
        public string Label { get; set; } // "SOYUDUCU", "KRAN" və s.
        public float InteractionDistance { get; set; } = 150f; // Piksel

        protected InteractableObject(Vector2 position, Rectangle bounds, string label)
        {
            Position = position;
            Bounds = bounds;
            Label = label;
        }

        /// <summary>
        /// Oyunçu yaxındadırmı?
        /// </summary>
        public bool IsPlayerNearby(Rectangle playerBounds)
        {
            // Mərkəzdən məsafəni hesabla
            Vector2 playerCenter = new Vector2(
                playerBounds.X + playerBounds.Width / 2,
                playerBounds.Y + playerBounds.Height / 2
            );

            Vector2 objectCenter = new Vector2(
                Bounds.X + Bounds.Width / 2,
                Bounds.Y + Bounds.Height / 2
            );

            float distance = Vector2.Distance(playerCenter, objectCenter);
            return distance < InteractionDistance;
        }

        /// <summary>
        /// İnteraction - override edilməlidir
        /// </summary>
        public abstract void OnInteract(Player.Player player);

        /// <summary>
        /// Debug çəkmə
        /// </summary>
        public virtual void DrawDebug(SpriteBatch spriteBatch, Texture2D whitePixel)
        {
            // Bounds göstər (debug)
            spriteBatch.Draw(whitePixel,
                new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 2),
                Color.Green);
            spriteBatch.Draw(whitePixel,
                new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - 2, Bounds.Width, 2),
                Color.Green);
            spriteBatch.Draw(whitePixel,
                new Rectangle(Bounds.X, Bounds.Y, 2, Bounds.Height),
                Color.Green);
            spriteBatch.Draw(whitePixel,
                new Rectangle(Bounds.X + Bounds.Width - 2, Bounds.Y, 2, Bounds.Height),
                Color.Green);
        }
    }
}
