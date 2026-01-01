using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using JustInterest.Core;
using JustInterest.Player;
using JustInterest.Locations;
using System.Collections.Generic;
using System;

namespace JustInterest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        // Core Managers
        private GameStateManager _stateManager;
        private TimeManager _timeManager;

        // Player
        private Player.Player _player;
        private PlayerController _playerController;

        // Locations
        private Dictionary<string, Location> _locations;
        private Location _currentLocation;

        // Placeholder textures
        private Texture2D _playerTexture;
        private Texture2D _backgroundTexture;
        private Texture2D _whitePixel;

        // Screen dimensions - Full HD
        private const int SCREEN_WIDTH = 1920;
        private const int SCREEN_HEIGHT = 1080;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set screen size
            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
        }

        protected override void Initialize()
        {
            // Initialize managers
            _stateManager = new GameStateManager();
            _timeManager = new TimeManager(startHour: 8, startDay: 1);

            // Initialize player controller
            _playerController = new PlayerController();

            // Initialize player
            _player = new Player.Player(new Vector2(400, 300));

            // Initialize locations
            InitializeLocations();

            // Set initial location
            _currentLocation = _locations["PlayerRoom"];
            _player.CurrentLocationId = "PlayerRoom";

            // Subscribe to events
            _timeManager.OnDayChanged += OnDayChanged;
            _player.Stats.OnDeath += OnPlayerDeath;

            // Start in playing state (skip main menu for now)
            _stateManager.ChangeState(GameState.Playing);

            base.Initialize();
        }

        private void InitializeLocations()
        {
            _locations = new Dictionary<string, Location>();

            // Player's Room - otaq (1920x1080)
            // Ekranın aşağı 1/4-ü: Y 810-1070 (260px)
            var playerRoom = new Location("PlayerRoom", "Otağın");
            playerRoom.DefaultSpawnPosition = new Vector2(960, 1070); // Mərkəz, ən aşağıda (ayaqlar 10px üstdə)
            playerRoom.SetMovementBounds(new Rectangle(300, 810, 1320, 260)); // Y: 810-1070
            _locations.Add("PlayerRoom", playerRoom);

            // Street - küçə (1920x1080)
            var street = new Location("Street", "Küçə");
            street.DefaultSpawnPosition = new Vector2(960, 1070);
            street.SetMovementBounds(new Rectangle(250, 810, 1420, 260)); // Y: 810-1070
            
            // Transition back to room (sol tərəfdə, aşağı zonada)
            street.AddTransition(new TransitionTrigger(
                new Rectangle(150, 900, 150, 160), // Y: 900-1060
                "PlayerRoom",
                new Vector2(1450, 1070),
                "Evə gir"
            ));
            _locations.Add("Street", street);

            // Add transition from room to street (sağ tərəfdə, aşağı zonada)
            playerRoom.AddTransition(new TransitionTrigger(
                new Rectangle(1620, 900, 150, 160), // Y: 900-1060
                "Street",
                new Vector2(420, 1070),
                "Çölə çıx"
            ));
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create white pixel for UI
            _whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            _whitePixel.SetData(new[] { Color.White });

            // Load actual assets
            try
            {
                // Try loading directional character sprites
                try
                {
                    var directionTextures = new Dictionary<JustInterest.Player.Direction, Texture2D>();
                    directionTextures[JustInterest.Player.Direction.Front] = Content.Load<Texture2D>("Characters/front_view");
                    directionTextures[JustInterest.Player.Direction.Back] = Content.Load<Texture2D>("Characters/back_view");
                    directionTextures[JustInterest.Player.Direction.Left] = Content.Load<Texture2D>("Characters/left_view");
                    directionTextures[JustInterest.Player.Direction.Right] = Content.Load<Texture2D>("Characters/right_view");
                    
                    _player.SetDirectionTextures(directionTextures);
                }
                catch
                {
                    // Fallback: create placeholder
                    _playerTexture = new Texture2D(GraphicsDevice, 64, 96);
                    Color[] playerData = new Color[64 * 96];
                    for (int i = 0; i < playerData.Length; i++)
                        playerData[i] = new Color(255, 150, 200);
                    _playerTexture.SetData(playerData);
                    _player.SetTexture(_playerTexture);
                }

                // Load bedroom background
                var bedroomTexture = Content.Load<Texture2D>("Backgrounds/bedroom");
                _locations["PlayerRoom"].SetBackground(bedroomTexture);
                
                // For Street, use placeholder
                _backgroundTexture = new Texture2D(GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT);
                Color[] streetBgData = new Color[SCREEN_WIDTH * SCREEN_HEIGHT];
                for (int i = 0; i < streetBgData.Length; i++)
                    streetBgData[i] = new Color(150, 180, 200);
                _backgroundTexture.SetData(streetBgData);
                _locations["Street"].SetBackground(_backgroundTexture);
            }
            catch (Exception ex)
            {
                // If assets fail to load, use placeholders
                System.Diagnostics.Debug.WriteLine($"Asset yüklənmədi: {ex.Message}");
                
                // Fallback to placeholder textures
                _playerTexture = new Texture2D(GraphicsDevice, 64, 96);
                Color[] playerData = new Color[64 * 96];
                for (int i = 0; i < playerData.Length; i++)
                    playerData[i] = new Color(255, 150, 200);
                _playerTexture.SetData(playerData);
                _player.SetTexture(_playerTexture);

                _backgroundTexture = new Texture2D(GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT);
                Color[] bgData = new Color[SCREEN_WIDTH * SCREEN_HEIGHT];
                for (int i = 0; i < bgData.Length; i++)
                    bgData[i] = new Color(180, 200, 180);
                _backgroundTexture.SetData(bgData);

                foreach (var location in _locations.Values)
                {
                    location.SetBackground(_backgroundTexture);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            // Update controller
            _playerController.Update();

            // Handle pause
            if (_playerController.IsPausePressed())
            {
                if (_stateManager.IsPlaying())
                    _stateManager.ChangeState(GameState.Paused);
                else if (_stateManager.IsPaused())
                    _stateManager.ReturnToPreviousState();
            }

            // Update based on game state
            switch (_stateManager.CurrentState)
            {
                case GameState.Playing:
                    UpdatePlaying(gameTime);
                    break;
                case GameState.Paused:
                    // Pause edilib, heç nə update olunmur
                    break;
                case GameState.GameOver:
                    // Game over screen
                    if (_playerController.IsKeyPressed(Keys.R))
                    {
                        RestartGame();
                    }
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdatePlaying(GameTime gameTime)
        {
            // Update time
            _timeManager.Update(gameTime);

            // Get input (now includes X and Y)
            Vector2 movement = _playerController.GetMovementInput();
            // float depth = _playerController.GetDepthInput(); // Removed

            // Update player
            _player.Update(gameTime, movement, _currentLocation.MovementBounds);

            // Check for location transitions
            if (_playerController.IsInteractPressed())
            {
                var transition = _currentLocation.GetActiveTransition(_player.GetBounds());
                if (transition != null && _locations.ContainsKey(transition.TargetLocationId))
                {
                    ChangeLocation(transition.TargetLocationId, transition.TargetSpawnPosition);
                }
            }
        }

        private void ChangeLocation(string locationId, Vector2 spawnPosition)
        {
            _currentLocation.Exit();
            _currentLocation = _locations[locationId];
            _player.CurrentLocationId = locationId;
            _player.SetPosition(spawnPosition);
            _currentLocation.Enter();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            switch (_stateManager.CurrentState)
            {
                case GameState.Playing:
                    DrawPlaying();
                    break;
                case GameState.Paused:
                    DrawPlaying(); // Background
                    DrawPauseOverlay();
                    break;
                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawPlaying()
        {
            // Draw location background
            _currentLocation.Draw(_spriteBatch);

            // Draw player
            _player.Draw(_spriteBatch);

            // Draw UI
            DrawUI();

            // Draw transition triggers (debug)
            DrawTransitionTriggers();
        }

        private void DrawUI()
        {
            int barWidth = 200;
            int barHeight = 20;
            int x = 20;
            int y = 20;

            // Hunger bar (qırmızı)
            DrawStatBar(x, y, barWidth, barHeight, _player.Stats.Hunger / 100f, Color.Red, "Aclıq");
            y += 30;

            // Thirst bar (mavi)
            DrawStatBar(x, y, barWidth, barHeight, _player.Stats.Thirst / 100f, Color.Blue, "Susuzluq");
            y += 30;

            // Hygiene bar (yaşıl)
            DrawStatBar(x, y, barWidth, barHeight, _player.Stats.Hygiene / 100f, Color.Green, "Gigiyena");
            y += 30;

            // Money (top right)
            DrawText($"Pul: ${_player.Stats.Money}", SCREEN_WIDTH - 150, 20, Color.Yellow);
            DrawText($"Borc: ${_player.Stats.RentDebt}", SCREEN_WIDTH - 150, 45, Color.Red);

            // Time (top center)
            DrawText($"{_timeManager.GetDayString()} - {_timeManager.GetTimeString()}", 
                     SCREEN_WIDTH / 2 - 80, 20, Color.White);

            // Location name (bottom center)
            DrawText(_currentLocation.Name, SCREEN_WIDTH / 2 - 50, SCREEN_HEIGHT - 40, Color.White);
        }

        private void DrawStatBar(int x, int y, int width, int height, float percentage, Color color, string label)
        {
            // Background (qara)
            _spriteBatch.Draw(_whitePixel, new Rectangle(x, y, width, height), Color.Black);
            
            // Border (ağ)
            DrawRectangleBorder(x, y, width, height, 2, Color.White);

            // Fill
            int fillWidth = (int)(width * percentage);
            if (fillWidth > 0)
            {
                _spriteBatch.Draw(_whitePixel, new Rectangle(x + 2, y + 2, fillWidth - 4, height - 4), color);
            }

            // Label (if font is available)
            // DrawText(label, x + width + 10, y, Color.White);
        }

        private void DrawRectangleBorder(int x, int y, int width, int height, int thickness, Color color)
        {
            // Top
            _spriteBatch.Draw(_whitePixel, new Rectangle(x, y, width, thickness), color);
            // Bottom
            _spriteBatch.Draw(_whitePixel, new Rectangle(x, y + height - thickness, width, thickness), color);
            // Left
            _spriteBatch.Draw(_whitePixel, new Rectangle(x, y, thickness, height), color);
            // Right
            _spriteBatch.Draw(_whitePixel, new Rectangle(x + width - thickness, y, thickness, height), color);
        }

        private void DrawTransitionTriggers()
        {
            // Debug: transition trigger-ləri göstər
            foreach (var trigger in _currentLocation.Transitions)
            {
                DrawRectangleBorder(
                    trigger.Area.X, 
                    trigger.Area.Y, 
                    trigger.Area.Width, 
                    trigger.Area.Height, 
                    2, 
                    Color.Yellow
                );

                // If player is inside, show interaction prompt
                if (trigger.IsPlayerInside(_player.GetBounds()))
                {
                    DrawText($"[E] {trigger.Label}", trigger.Area.X, trigger.Area.Y - 25, Color.Yellow);
                }
            }
        }

        private void DrawPauseOverlay()
        {
            // Semi-transparent black overlay
            _spriteBatch.Draw(_whitePixel, 
                new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), 
                Color.Black * 0.7f);

            // Pause text
            DrawText("PAUZA", SCREEN_WIDTH / 2 - 40, SCREEN_HEIGHT / 2 - 20, Color.White);
            DrawText("[ESC] Davam et", SCREEN_WIDTH / 2 - 80, SCREEN_HEIGHT / 2 + 20, Color.White);
        }

        private void DrawGameOver()
        {
            _spriteBatch.Draw(_whitePixel, 
                new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), 
                Color.DarkRed);

            DrawText("OYUN BİTDİ", SCREEN_WIDTH / 2 - 80, SCREEN_HEIGHT / 2 - 40, Color.White);
            DrawText("Stat-ların çox aşağı düşdü!", SCREEN_WIDTH / 2 - 120, SCREEN_HEIGHT / 2, Color.White);
            DrawText("[R] Yenidən başla", SCREEN_WIDTH / 2 - 100, SCREEN_HEIGHT / 2 + 40, Color.White);
        }

        private void DrawText(string text, int x, int y, Color color)
        {
            // Placeholder - font yükləndikdə işləyəcək
            // if (_font != null)
            //     _spriteBatch.DrawString(_font, text, new Vector2(x, y), color);
            
            // For now, just draw a small rectangle as placeholder for each character
            int charWidth = 8;
            for (int i = 0; i < text.Length; i++)
            {
                _spriteBatch.Draw(_whitePixel, 
                    new Rectangle(x + i * charWidth, y, charWidth - 2, 12), 
                    color * 0.3f);
            }
        }

        private void OnDayChanged(int newDay)
        {
            _player.Stats.OnNewDay();
        }

        private void OnPlayerDeath()
        {
            _stateManager.ChangeState(GameState.GameOver);
        }

        private void RestartGame()
        {
            // Reset everything
            _player = new Player.Player(new Vector2(400, 300));
            _player.SetTexture(_playerTexture);
            _player.Stats.OnDeath += OnPlayerDeath;
            
            _timeManager = new TimeManager(startHour: 8, startDay: 1);
            _timeManager.OnDayChanged += OnDayChanged;
            
            _currentLocation = _locations["PlayerRoom"];
            _player.CurrentLocationId = "PlayerRoom";
            
            _stateManager.ChangeState(GameState.Playing);
        }
    }
}
