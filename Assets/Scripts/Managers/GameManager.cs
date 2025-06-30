using System;
using Dirt;
using Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameOverReason lastGameOverReason;
        [SerializeField] internal int maxDirt = 50;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // מונע שיכפול
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // שורד סצנות
        }
        
        
        /*private void OnEnable()
        {
            GameEvents.GameOver += HandleGameOver;
            
        }
        private void OnDisable()
        {
            GameEvents.GameOver -= HandleGameOver;
        }*/

        private void Start()
        {
            GameEvents.StartLevel?.Invoke();
        }
        
       /* private void HandleGameOver(GameOverReason reason)
        {
            //TODO: change this to a proper game over screen
            LastGameOverReason = reason;
            Debug.Log("Game Over!");
        }*/
        
    }
}