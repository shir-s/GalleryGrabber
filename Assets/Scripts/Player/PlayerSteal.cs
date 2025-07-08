using Managers;
using Stealable;
using UnityEngine;
using Utils;

namespace SceneControllers
{
    public class PlayerSteal : MonoBehaviour
    {
        private StealableItem itemNearby = null;
        public static bool isStealing = false;

        void Update()
        {
            if (itemNearby != null && itemNearby.CanBeStolen() && !GameStates.isPlayerCaught)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    isStealing = true;
                    itemNearby.TrySteal(Time.deltaTime);
                }
                else if (Input.GetKeyUp(KeyCode.Space))
                {
                    isStealing = false;
                    itemNearby.StopSteal();
                }
            }
            else
            {
                isStealing = false;
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
            {
                itemNearby = null;
                isStealing = false;
            }
        }
    }

}