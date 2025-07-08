using System.Collections;
using Sound;
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
        private bool isOnCooldown = false;

        private void Start()
        {
            currentPlayerLives = initialPlayerlives;
            GameEvents.StartLevel?.Invoke();
            SoundManager.Instance.PlaySound("Background", transform);
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
            if (GameStates.isPlayerCaught || currentPlayerLives <= 0)
                return;

            StartCoroutine(LoseLifeWithCooldown());
        }

        private IEnumerator LoseLifeWithCooldown()
        {
            GameStates.isPlayerCaught = true;
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

            yield return new WaitForSeconds(3f);
            GameStates.isPlayerCaught = false;
        }
    }
}