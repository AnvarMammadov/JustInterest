using System;

namespace JustInterest.Core
{
    /// <summary>
    /// Oyun state-lərini idarə edən manager
    /// </summary>
    public class GameStateManager
    {
        private GameState _currentState;
        private GameState _previousState;

        public GameState CurrentState => _currentState;
        public GameState PreviousState => _previousState;

        public event Action<GameState, GameState> OnStateChanged;

        public GameStateManager()
        {
            _currentState = GameState.MainMenu;
            _previousState = GameState.MainMenu;
        }

        /// <summary>
        /// State-i dəyişdir
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (_currentState == newState)
                return;

            _previousState = _currentState;
            _currentState = newState;

            OnStateChanged?.Invoke(_previousState, _currentState);
        }

        /// <summary>
        /// Əvvəlki state-ə qayıt (məsələn pause-dan çıxmaq üçün)
        /// </summary>
        public void ReturnToPreviousState()
        {
            ChangeState(_previousState);
        }

        public bool IsPlaying() => _currentState == GameState.Playing;
        public bool IsPaused() => _currentState == GameState.Paused;
        public bool IsGameOver() => _currentState == GameState.GameOver;
    }
}
