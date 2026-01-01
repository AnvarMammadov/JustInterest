using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace JustInterest.Player
{
    /// <summary>
    /// Oyunçu statistikalarını saxlayır və idarə edir
    /// </summary>
    public class PlayerStats
    {
        // Əsas stat-lar (0-100)
        public float Hunger { get; private set; } = 100f;
        public float Thirst { get; private set; } = 100f;
        public float Hygiene { get; private set; } = 100f;

        // Pul və kira
        public int Money { get; private set; } = 0;
        public int RentDebt { get; private set; } = 500; // İlk kira borcu
        public int DailyRent { get; private set; } = 50; // Günlük kira

        // Stat azalma sürətləri (saniyədə)
        private const float HUNGER_DECAY_RATE = 0.5f;   // 200 saniyədə sıfıra enir
        private const float THIRST_DECAY_RATE = 0.7f;   // ~143 saniyədə
        private const float HYGIENE_DECAY_RATE = 0.3f;  // ~333 saniyədə

        // Critical threshold-lar
        private const float CRITICAL_THRESHOLD = 20f;

        // Event-lər (genişləndirmə üçün)
        public event Action OnHungerCritical;
        public event Action OnThirstCritical;
        public event Action OnHygieneCritical;
        public event Action OnMoneyChanged;
        public event Action OnDeath; // Stat-lar çox aşağı düşərsə

        private bool _isHungerCritical = false;
        private bool _isThirstCritical = false;
        private bool _isHygieneCritical = false;

        public PlayerStats(int startingMoney = 0)
        {
            Money = startingMoney;
        }

        public void Update(GameTime gameTime)
        {
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // TEST MODE: Stat azalmasını söndür
            // DecreaseHunger(HUNGER_DECAY_RATE * deltaSeconds);
            // DecreaseThirst(THIRST_DECAY_RATE * deltaSeconds);
            // DecreaseHygiene(HYGIENE_DECAY_RATE * deltaSeconds);

            // Critical check-lər
            CheckCriticalStates();

            // Ölüm check-i (hər hansı stat 0-a çatsa)
            if (Hunger <= 0 || Thirst <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        private void CheckCriticalStates()
        {
            // Hunger critical
            if (Hunger <= CRITICAL_THRESHOLD && !_isHungerCritical)
            {
                _isHungerCritical = true;
                OnHungerCritical?.Invoke();
            }
            else if (Hunger > CRITICAL_THRESHOLD)
            {
                _isHungerCritical = false;
            }

            // Thirst critical
            if (Thirst <= CRITICAL_THRESHOLD && !_isThirstCritical)
            {
                _isThirstCritical = true;
                OnThirstCritical?.Invoke();
            }
            else if (Thirst > CRITICAL_THRESHOLD)
            {
                _isThirstCritical = false;
            }

            // Hygiene critical
            if (Hygiene <= CRITICAL_THRESHOLD && !_isHygieneCritical)
            {
                _isHygieneCritical = true;
                OnHygieneCritical?.Invoke();
            }
            else if (Hygiene > CRITICAL_THRESHOLD)
            {
                _isHygieneCritical = false;
            }
        }

        // Stat modifier methodları
        public void IncreaseHunger(float amount) => Hunger = Math.Clamp(Hunger + amount, 0, 100);
        public void DecreaseHunger(float amount) => Hunger = Math.Clamp(Hunger - amount, 0, 100);
        
        public void IncreaseThirst(float amount) => Thirst = Math.Clamp(Thirst + amount, 0, 100);
        public void DecreaseThirst(float amount) => Thirst = Math.Clamp(Thirst - amount, 0, 100);
        
        public void IncreaseHygiene(float amount) => Hygiene = Math.Clamp(Hygiene + amount, 0, 100);
        public void DecreaseHygiene(float amount) => Hygiene = Math.Clamp(Hygiene - amount, 0, 100);

        public void AddMoney(int amount)
        {
            Money += amount;
            OnMoneyChanged?.Invoke();
        }

        public bool TrySpendMoney(int amount)
        {
            if (Money >= amount)
            {
                Money -= amount;
                OnMoneyChanged?.Invoke();
                return true;
            }
            return false;
        }

        public void AddRentDebt(int amount)
        {
            RentDebt += amount;
        }

        public bool TryPayRent(int amount)
        {
            if (Money >= amount)
            {
                Money -= amount;
                RentDebt = Math.Max(0, RentDebt - amount);
                OnMoneyChanged?.Invoke();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gündəlik event - kira borcu əlavə et
        /// </summary>
        public void OnNewDay(int day)
        {
            RentDebt += DailyRent; // Assuming DailyRent is the correct variable to use here. The instruction used DAILY_RENT which is not defined.
        }

        // Stat check-ləri
        public bool IsHungerCritical() => Hunger <= CRITICAL_THRESHOLD;
        public bool IsThirstCritical() => Thirst <= CRITICAL_THRESHOLD;
        public bool IsHygieneCritical() => Hygiene <= CRITICAL_THRESHOLD;
        public bool IsAnyCritical() => IsHungerCritical() || IsThirstCritical() || IsHygieneCritical();
    }
}
