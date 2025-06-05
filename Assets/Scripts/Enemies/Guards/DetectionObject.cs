using Player;
using Stealable;
using UnityEngine;
using Utils;

namespace Enemies.Guards
{
    public class DetectionObject : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((other.CompareTag("Player") && PlayerSteal.isStealing))
            {
                Debug.Log("Player or stealable object detected during theft!");
                GameEvents.PlayerLostLife?.Invoke();
            }
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if ((other.CompareTag("Stealable") && other.GetComponent<StealableItem>().IsBeingStolen()))
            {
                Debug.Log("Player or stealable object detected during theft!");
                GameEvents.PlayerLostLife?.Invoke();
            }
        }
    }
}
