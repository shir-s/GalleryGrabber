using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public int maxHealth = 3;
        private int currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
            GameEvents.PlayerHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            GameEvents.PlayerHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                GameEvents.GameOver?.Invoke(GameOverReason.OutOfLives);
            }
        }
    }

}