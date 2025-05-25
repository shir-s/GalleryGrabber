using Managers;
using UnityEngine;
using Utils;

namespace Dirt
{
    public class Dirt : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GameEvents.OnDirtCollected?.Invoke();
                GameManager.Instance.DirtPool.ReturnDirt(gameObject);
            }
        }
    }
}