using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Managers
{
    public class LifeManager : MonoBehaviour
    {
        [SerializeField] private int initialPlayerlives = 3;
        [SerializeField] private Image life1;
        [SerializeField] private Image life2;
        [SerializeField] private Image life3;
        [SerializeField] private Sprite redEye;
        
        private int currentPlayerLives;

        private void Start()
        {
            currentPlayerLives = initialPlayerlives;
        }

        private void OnEnable()
        {
            GameEvents.PlayerLostLife += LoseLife;
        }
        
        private void OnDisable()
        {
            GameEvents.PlayerLostLife -= LoseLife;
        }

        private void LoseLife()
        {
            currentPlayerLives--;
            switch (currentPlayerLives)
            {
                case 2:
                    life3.sprite = redEye;
                    break;
                case 1:
                    life2.sprite = redEye;
                    break;
                case 0:
                    life1.sprite = redEye;
                    break;
            }

            if (currentPlayerLives <= 0)
            {
                GameEvents.GameOver?.Invoke(GameOverReason.OutOfLives);
            }
        }
        
    }
}