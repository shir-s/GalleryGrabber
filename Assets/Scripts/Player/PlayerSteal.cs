using Stealable;
using UnityEngine;
namespace Player
{
    public class PlayerSteal : MonoBehaviour
    {
        private StealableItem itemNearby = null;

        void Update()
        {
            if (itemNearby != null && itemNearby.CanBeStolen())
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    itemNearby.TrySteal(Time.deltaTime);
                }
                else if (Input.GetKeyUp(KeyCode.Space))
                {
                    itemNearby.StopSteal();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            StealableItem item = other.GetComponent<StealableItem>();
            if (item != null)
                itemNearby = item;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            StealableItem item = other.GetComponent<StealableItem>();
            if (item != null && item == itemNearby)
                itemNearby = null;
        }
    }
}