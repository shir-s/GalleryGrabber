using System;
using Dirt;
using Utils;
using UnityEngine;
namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] internal Collider2D roomCollider;
        [SerializeField] private DirtSpawner dirtSpawner;
        [SerializeField] private DirtPool dirtPool;
        [SerializeField] private float currentCleanliness = 0f;
        [SerializeField] private float dirtPercent = 0.05f;

        private void Awake()
        {
            GameEvents.OnDirtCollected += HandleDirtCollected;
            GameEvents.OnDirtSpawned += HandleDirtSpawned;
        }
        
        public DirtPool DirtPool => dirtPool;
        
        private void Start()
        {
            if (dirtSpawner == null)
            {
                Debug.LogError("DirtSpawner is not assigned in GameManager.");
                return;
            }
            
            if (roomCollider == null)
            {
                Debug.LogError("RoomCollider is not assigned in GameManager.");
                return;
            }

            SetUpGame();
            GameEvents.StartLevel?.Invoke();
        }

        private void SetUpGame()
        {
            //dirtSpawner.SetUp(dirtPool, roomCollider);
            dirtSpawner.InitialSpawn();
        }
        
        public void HandleDirtCollected()
        {
            currentCleanliness = Mathf.Max(0f, currentCleanliness - dirtPercent);
            GameEvents.OnCleanlinessChanged?.Invoke(currentCleanliness);
        }

        public void HandleDirtSpawned()
        {
            currentCleanliness = Mathf.Min(1f, currentCleanliness + dirtPercent);
            GameEvents.OnCleanlinessChanged?.Invoke(currentCleanliness);
        }
    }
}