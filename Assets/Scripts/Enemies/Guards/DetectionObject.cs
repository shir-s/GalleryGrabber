using Player;
using Stealable;
using UnityEngine;
using Utils;

namespace Enemies.Guards
{
    public class DetectionObject : MonoBehaviour
    {
        [SerializeField] private float detectionCooldown = 3f; // Cooldown in seconds
        private float lastDetectionTime = -Mathf.Infinity;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && PlayerSteal.isStealing)
            {
                if (CompareTag("Guard"))
                {
                    GameEvents.GameOver?.Invoke();
                }
                Debug.Log("Player or stealable object detected during theft (enter)!");
                GameEvents.PlayerLostLife?.Invoke();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Stealable") && other.GetComponent<StealableItem>().IsBeingStolen())
            {
                if (Time.time >= lastDetectionTime + detectionCooldown)
                {
                    if (CompareTag("Guard"))
                    {
                        GameEvents.GameOver?.Invoke();
                    }
                    Debug.Log("Player or stealable object detected during theft (stay)!");
                    GameEvents.PlayerLostLife?.Invoke();
                    lastDetectionTime = Time.time;
                }
            }
        }
    }
}