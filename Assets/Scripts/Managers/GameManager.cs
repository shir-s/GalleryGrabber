using System;
using Dirt;
using Utils;
using UnityEngine;
namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        
        [SerializeField] internal int maxDirt = 50;
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
            GameEvents.StartLevel?.Invoke();
        }
        
        private void HandleGameOver(GameOverReason reason)
        {
            //TODO: change this to a proper game over screen
            LastGameOverReason = reason;
            Debug.Log("Game Over!");
        }
        
    }
}