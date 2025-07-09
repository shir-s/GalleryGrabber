using System.Collections;
using Sound;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Managers
{
    public class LifeManager : MonoBehaviour
    {
        [Header("Lives")]
        [SerializeField] private int initialPlayerLives = 3;
        [SerializeField] private Image life1;
        [SerializeField] private Image life2;
        [SerializeField] private Image life3;
        [SerializeField] private Sprite redEye;

        private int _currentPlayerLives;

        private void OnEnable()
        {
            GameEvents.PlayerLostLife += LoseLife;
        }

        private void OnDisable()
        {
            GameEvents.PlayerLostLife -= LoseLife;
        }

        private void Start()
        {
            _currentPlayerLives = initialPlayerLives;
            GameEvents.StartLevel?.Invoke();
            SoundManager.Instance.PlaySound("Background", transform);
        }

        private void LoseLife()
        {
            if (GameStates.isPlayerCaught || _currentPlayerLives <= 0)
                return;

            StartCoroutine(HandleLifeLossWithCooldown());
        }

        private IEnumerator HandleLifeLossWithCooldown()
        {
            GameStates.isPlayerCaught = true;
            _currentPlayerLives--;

            UpdateLifeUI();

            if (_currentPlayerLives <= 0)
            {
                GameEvents.GameOver?.Invoke(GameOverReason.OutOfLives);
            }

            yield return new WaitForSeconds(3f);
            GameStates.isPlayerCaught = false;
        }

        private void UpdateLifeUI()
        {
            switch (_currentPlayerLives)
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
        }
    }
}