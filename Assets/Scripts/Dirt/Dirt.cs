using Sound;
using UnityEngine;
using Utilities;
using Utils;

namespace Dirt
{
    public class Dirt : MonoBehaviour,IPoolable
    {
        [SerializeField] private GameObject bubbleParticle;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GameEvents.OnDirtCollected?.Invoke(1);
               SoundManager.Instance.PlaySound("Dirt", transform);
               var bubble = Instantiate(bubbleParticle, transform.position, Quaternion.identity);
                Destroy(bubble, 1f);
                DirtPool.Instance.Return(this);
            }
        }

        public void Reset()
        {
            
        }
    }
}