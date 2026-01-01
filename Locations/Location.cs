using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace JustInterest.Locations
{
    /// <summary>
    /// Location transition trigger-i
    /// </summary>
    public class TransitionTrigger
    {
        public Rectangle Area { get; set; }
        public string TargetLocationId { get; set; }
        public Vector2 TargetSpawnPosition { get; set; }
        public string Label { get; set; } // UI üçün (məs: "Evə gir")

        public TransitionTrigger(Rectangle area, string targetLocationId, Vector2 targetSpawnPosition, string label = "")
        {
            Area = area;
            TargetLocationId = targetLocationId;
            TargetSpawnPosition = targetSpawnPosition;
            Label = label;
        }

        public bool IsPlayerInside(Rectangle playerBounds)
        {
            return Area.Intersects(playerBounds);
        }
    }

    /// <summary>
    /// Oyunun hər bir lokasiyası (statik background PNG)
    /// </summary>
    public class Location
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Texture2D Background { get; set; }

        // Hərəkət edə biləcəyi sahə (görünməz bounds)
        public Rectangle MovementBounds { get; set; }

        // Transition-lar (başqa location-lara keçid)
        public List<TransitionTrigger> Transitions { get; set; }

        // Spawn point (player bu location-a gələndə harada duracaq)
        public Vector2 DefaultSpawnPosition { get; set; }

        // Zaman məhdudiyyəti (məsələn məktəb gecə bağlıdır)
        public int? AvailableFromHour { get; set; } // null = hər zaman
        public int? AvailableToHour { get; set; }

        // Event-lər (genişləndirmə üçün)
        public event Action<string> OnEnter; // Player daxil olduqda
        public event Action<string> OnExit;  // Player çıxdıqda

        public Location(string id, string name)
        {
            Id = id;
            Name = name;
            Transitions = new List<TransitionTrigger>();
        }

        public void SetBackground(Texture2D background)
        {
            Background = background;
        }

        public void SetMovementBounds(Rectangle bounds)
        {
            MovementBounds = bounds;
        }

        public void AddTransition(TransitionTrigger trigger)
        {
            Transitions.Add(trigger);
        }

        public TransitionTrigger GetActiveTransition(Rectangle playerBounds)
        {
            foreach (var transition in Transitions)
            {
                if (transition.IsPlayerInside(playerBounds))
                {
                    return transition;
                }
            }
            return null;
        }

        public bool IsAvailableAtTime(int currentHour)
        {
            if (AvailableFromHour == null && AvailableToHour == null)
                return true;

            if (AvailableFromHour.HasValue && currentHour < AvailableFromHour.Value)
                return false;

            if (AvailableToHour.HasValue && currentHour >= AvailableToHour.Value)
                return false;

            return true;
        }

        public void Enter()
        {
            OnEnter?.Invoke(Id);
        }

        public void Exit()
        {
            OnExit?.Invoke(Id);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Background != null)
            {
                // Stretch background to fill screen (1920x1080 Full HD)
                Rectangle destRect = new Rectangle(0, 0, 1920, 1080);
                spriteBatch.Draw(Background, destRect, Color.White);
            }
        }
    }
}
