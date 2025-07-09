using System;
using UnityEngine;
namespace Utils
{
    public class GameEvents
    {
        public static Action RestartLevel;
        public static Action StartLevel;
        public static Action<GameOverReason> GameOver;
        public static Action<int,int> PlayerHealthChanged;
        public static Action <int> OnDirtCollected;
        public static Action OnDirtSpawned;
        public static Action<float> OnCleanlinessChanged;
        public static Action<int> StoleItem;
        public static Action PlayerLostLife;
    }
}