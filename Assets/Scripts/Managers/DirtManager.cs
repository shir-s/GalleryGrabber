using UnityEngine;
using Utils;

namespace Managers
{
    public class DirtManager : MonoBehaviour
    {
        [Header("Cleanliness (0 = clean, 1 = max dirt)")]
        [SerializeField] private float currentCleanliness = 0f;

        private float _dirtIncrement;

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        private void Start()
        {
            _dirtIncrement = 1f / GameStates.MaxDirt;
        }

        private void SubscribeEvents()
        {
            GameEvents.OnDirtCollected += HandleDirtCollected;
            GameEvents.OnDirtSpawned += HandleDirtSpawned;
        }

        private void UnsubscribeEvents()
        {
            GameEvents.OnDirtCollected -= HandleDirtCollected;
            GameEvents.OnDirtSpawned -= HandleDirtSpawned;
        }

        private void HandleDirtCollected(int _)
        {
            currentCleanliness = Mathf.Max(0f, currentCleanliness - _dirtIncrement);
            GameEvents.OnCleanlinessChanged?.Invoke(currentCleanliness);
        }

        private void HandleDirtSpawned()
        {
            currentCleanliness = Mathf.Min(1f, currentCleanliness + _dirtIncrement);
            GameEvents.OnCleanlinessChanged?.Invoke(currentCleanliness);

            if (currentCleanliness >= 0.999f)
            {
                GameEvents.GameOver?.Invoke(GameOverReason.TooMuchDirt);
            }
        }
    }
}