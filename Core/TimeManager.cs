using Microsoft.Xna.Framework;
using System;

namespace JustInterest.Core
{
    /// <summary>
    /// Oyun zamanını idarə edən manager
    /// Gün/Gecə dövrü, saat, gün sayacı
    /// </summary>
    public class TimeManager
    {
        // Oyun saatı (0-23)
        private float _currentHour;
        private int _currentDay;

        // 1 real saniyə = neçə oyun dəqiqəsi
        private float _timeScale = 60f; // default: 1 real saniyə = 1 oyun saniyəsi (60x sürətli)

        public int CurrentHour => (int)_currentHour;
        public int CurrentMinute => (int)((_currentHour - CurrentHour) * 60);
        public int CurrentDay => _currentDay;

        // Gün/gecə check-ləri (genişləndirmə üçün)
        public bool IsMorning => CurrentHour >= 6 && CurrentHour < 12;
        public bool IsAfternoon => CurrentHour >= 12 && CurrentHour < 18;
        public bool IsEvening => CurrentHour >= 18 && CurrentHour < 22;
        public bool IsNight => CurrentHour >= 22 || CurrentHour < 6;
        public bool IsDaytime => CurrentHour >= 6 && CurrentHour < 18;

        // Event-lər
        public event Action<int> OnHourChanged;
        public event Action<int> OnDayChanged;
        public event Action OnMorning;
        public event Action OnNight;

        public TimeManager(int startHour = 8, int startDay = 1)
        {
            _currentHour = startHour;
            _currentDay = startDay;
        }

        public void Update(GameTime gameTime)
        {
            float previousHour = _currentHour;
            int previousDay = _currentDay;

            // Zamanı irəli aparmaq (real time-a görə)
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float gameMinutesElapsed = deltaSeconds * _timeScale;
            _currentHour += gameMinutesElapsed / 60f;

            // 24 saatdan sonra yeni gün
            if (_currentHour >= 24f)
            {
                _currentHour -= 24f;
                _currentDay++;
                OnDayChanged?.Invoke(_currentDay);
            }

            // Saat dəyişdimi?
            if ((int)_currentHour != (int)previousHour)
            {
                OnHourChanged?.Invoke(CurrentHour);

                // Səhər oldu?
                if ((int)previousHour < 6 && CurrentHour >= 6)
                {
                    OnMorning?.Invoke();
                }
                // Gecə oldu?
                else if ((int)previousHour < 22 && CurrentHour >= 22)
                {
                    OnNight?.Invoke();
                }
            }
        }

        /// <summary>
        /// Zamanı dəyişdir (event trigger üçün)
        /// </summary>
        public void SetTime(int hour, int minute = 0)
        {
            _currentHour = hour + (minute / 60f);
        }

        /// <summary>
        /// Time scale-i dəyişdir (daha sürətli/yavaş)
        /// </summary>
        public void SetTimeScale(float scale)
        {
            _timeScale = Math.Max(0, scale);
        }

        /// <summary>
        /// Zamanı dayandır
        /// </summary>
        public void PauseTime()
        {
            _timeScale = 0;
        }

        /// <summary>
        /// Zamanı davam etdir
        /// </summary>
        public void ResumeTime(float scale = 60f)
        {
            _timeScale = scale;
        }

        public string GetTimeString()
        {
            return $"{CurrentHour:D2}:{CurrentMinute:D2}";
        }

        public string GetDayString()
        {
            return $"Gün {_currentDay}";
        }
    }
}
