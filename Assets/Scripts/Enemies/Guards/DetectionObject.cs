using Player;
using UnityEngine;
using Utils;

namespace Enemies.Guards
{
    public class DetectionObject : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && PlayerSteal.isStealing)
            {
                Debug.Log("Player caught during theft!");
                GameEvents.PlayerLostLife?.Invoke();
            }
        }
    }
}
