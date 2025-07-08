using UnityEngine;
using Utils;

namespace Managers
{
    public class DirtManager: MonoBehaviour
    {
        [SerializeField] private float currentCleanliness = 0f;
        private float _dirtPercent;

        private void OnEnable()
        {
            GameEvents.OnDirtCollected += HandleDirtCollected;
            GameEvents.OnDirtSpawned += HandleDirtSpawned;
        }
        private void OnDisable()
        {
            GameEvents.OnDirtCollected -= HandleDirtCollected;
            GameEvents.OnDirtSpawned -= HandleDirtSpawned;
        }
        
        private void Start()
        {
            _dirtPercent = 1f / GameManager.MaxDirt; // Assuming maxDirt is the total dirt that can be spawned
        }

        private void HandleDirtCollected(int dummy)
        {
            currentCleanliness = Mathf.Max(0f, currentCleanliness - _dirtPercent);
            GameEvents.OnCleanlinessChanged?.Invoke(currentCleanliness);
        }

        private void HandleDirtSpawned()
        {
            currentCleanliness = Mathf.Min(1f, currentCleanliness + _dirtPercent);
            GameEvents.OnCleanlinessChanged?.Invoke(currentCleanliness);
            if (currentCleanliness >= 1f)
            {
                GameEvents.GameOver?.Invoke(GameOverReason.TooMuchDirt);
            }
                
        }
    }
}