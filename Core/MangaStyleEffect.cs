using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JustInterest.Core
{
    /// <summary>
    /// Manga-style shader effect wrapper
    /// Applies edge detection, halftone patterns, and posterization
    /// </summary>
    public class MangaStyleEffect
    {
        private Effect _effect;
        
        // Shader parameters - classic manga style defaults
        private float _edgeThreshold = 0.05f;
        private float _edgeThickness = 2.0f;
        private float _halftoneSize = 14.0f;
        private float _halftoneDensity = 0.7f;
        private int _posterizeLevels = 6;
        private bool _blackAndWhite = true; // Default: manga mode ON

        public MangaStyleEffect(Effect effect)
        {
            _effect = effect;
            UpdateParameters();
        }

        /// <summary>
        /// Edge detection sensitivity (0.0 - 1.0)
        /// Lower = more edges detected
        /// </summary>
        public float EdgeThreshold
        {
            get => _edgeThreshold;
            set
            {
                _edgeThreshold = MathHelper.Clamp(value, 0.0f, 1.0f);
                _effect.Parameters["EdgeThreshold"]?.SetValue(_edgeThreshold);
            }
        }

        /// <summary>
        /// Edge line thickness (0.5 - 3.0)
        /// </summary>
        public float EdgeThickness
        {
            get => _edgeThickness;
            set
            {
                _edgeThickness = MathHelper.Clamp(value, 0.5f, 3.0f);
                _effect.Parameters["EdgeThickness"]?.SetValue(_edgeThickness);
            }
        }

        /// <summary>
        /// Halftone dot size in pixels (4.0 - 20.0)
        /// </summary>
        public float HalftoneSize
        {
            get => _halftoneSize;
            set
            {
                _halftoneSize = MathHelper.Clamp(value, 4.0f, 20.0f);
                _effect.Parameters["HalftoneSize"]?.SetValue(_halftoneSize);
            }
        }

        /// <summary>
        /// Halftone density (0.0 - 1.0)
        /// Higher = darker halftone pattern
        /// </summary>
        public float HalftoneDensity
        {
            get => _halftoneDensity;
            set
            {
                _halftoneDensity = MathHelper.Clamp(value, 0.0f, 1.0f);
                _effect.Parameters["HalftoneDensity"]?.SetValue(_halftoneDensity);
            }
        }

        /// <summary>
        /// Number of color levels for posterization (2 - 8)
        /// Lower = more dramatic manga effect
        /// </summary>
        public int PosterizeLevels
        {
            get => _posterizeLevels;
            set
            {
                _posterizeLevels = (int)MathHelper.Clamp(value, 2, 8);
                _effect.Parameters["PosterizeLevels"]?.SetValue(_posterizeLevels);
            }
        }

        /// <summary>
        /// Black and white manga mode
        /// </summary>
        public bool BlackAndWhite
        {
            get => _blackAndWhite;
            set
            {
                _blackAndWhite = value;
                _effect.Parameters["BlackAndWhite"]?.SetValue(_blackAndWhite);
            }
        }

        /// <summary>
        /// Set texture size for pixel-perfect calculations
        /// </summary>
        public void SetTextureSize(int width, int height)
        {
            _effect.Parameters["TextureSize"]?.SetValue(new Vector2(width, height));
        }

        /// <summary>
        /// Get the underlying Effect for use with SpriteBatch
        /// </summary>
        public Effect Effect => _effect;

        /// <summary>
        /// Update all shader parameters
        /// </summary>
        private void UpdateParameters()
        {
            _effect.Parameters["EdgeThreshold"]?.SetValue(_edgeThreshold);
            _effect.Parameters["EdgeThickness"]?.SetValue(_edgeThickness);
            _effect.Parameters["HalftoneSize"]?.SetValue(_halftoneSize);
            _effect.Parameters["HalftoneDensity"]?.SetValue(_halftoneDensity);
            _effect.Parameters["PosterizeLevels"]?.SetValue(_posterizeLevels);
            _effect.Parameters["BlackAndWhite"]?.SetValue(_blackAndWhite);
        }

        /// <summary>
        /// Reset to default manga style parameters
        /// </summary>
        public void ResetToDefaults()
        {
            EdgeThreshold = 0.12f;
            EdgeThickness = 1.0f;
            HalftoneSize = 8.0f;
            HalftoneDensity = 0.6f;
            PosterizeLevels = 6;
            BlackAndWhite = true; // Manga mode
        }

        /// <summary>
        /// Preset: Strong manga effect
        /// </summary>
        public void ApplyStrongMangaStyle()
        {
            EdgeThreshold = 0.05f;  // More edges
            EdgeThickness = 1.5f;   // Thicker lines
            HalftoneSize = 6.0f;    // Smaller dots
            HalftoneDensity = 0.7f; // Denser pattern
            PosterizeLevels = 3;    // Fewer colors
            BlackAndWhite = false;
        }

        /// <summary>
        /// Preset: Subtle manga effect
        /// </summary>
        public void ApplySubtleMangaStyle()
        {
            EdgeThreshold = 0.15f;  // Fewer edges
            EdgeThickness = 0.8f;   // Thinner lines
            HalftoneSize = 12.0f;   // Larger dots
            HalftoneDensity = 0.3f; // Lighter pattern
            PosterizeLevels = 5;    // More colors
            BlackAndWhite = false;
        }
    }
}
