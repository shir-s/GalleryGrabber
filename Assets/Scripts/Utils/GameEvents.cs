using System;
using UnityEngine;
namespace Utils
{
    public class GameEvents
    {
        public static Action PlayerDied;
        public static Action PlayerWon;
        public static Action RestartLevel;
        public static Action StartLevel;
        public static Action GameOver;
        public static Action<int,int> PlayerHealthChanged;
        public static Action OnDirtCollected;
        public static Action OnDirtSpawned;
        public static Action<float> OnCleanlinessChanged;
    }
}