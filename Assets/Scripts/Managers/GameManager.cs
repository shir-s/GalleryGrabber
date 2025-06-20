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
        [SerializeField] internal int maxDirt = 50;
        
        public DirtPool DirtPool => dirtPool;
        public GameOverReason LastGameOverReason { get; private set; }
        
        private void OnEnable()
        {
            GameEvents.GameOver += HandleGameOver;
            
        }
        private void OnDisable()
        {
            GameEvents.GameOver -= HandleGameOver;
        }

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
        
        private void HandleGameOver(GameOverReason reason)
        {
            //TODO: change this to a proper game over screen
            LastGameOverReason = reason;
            Debug.Log("Game Over!");
        }
    }
}