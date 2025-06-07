using Managers;
using UnityEngine;
using Utilities;
using Utils;

namespace Dirt
{
    public class Dirt : MonoBehaviour,IPoolable
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GameEvents.OnDirtCollected?.Invoke(1);
                GameManager.Instance.DirtPool.Return(this);
            }
        }

        public void Reset()
        {
            
        }
    }
}