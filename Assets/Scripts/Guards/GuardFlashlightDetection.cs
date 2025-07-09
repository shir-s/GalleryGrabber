using UnityEngine;

namespace Guards
{
    public class GuardFlashlightDetection : MonoBehaviour
    {
        [Tooltip("Assign the smart guard script if this is a smart guard.")]
        [SerializeField] private SmartGuardWithModel smartGuardScript;

        [Tooltip("Assign the patrol guard script if this is a patrol guard.")]
        [SerializeField] private PatrolGuard patrolGuardScript;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("StolenGhost"))
                return;

            Debug.Log("Flashlight detected stolen object (ghost)!");
            ReactToStolenItem();
        }

        private void ReactToStolenItem()
        {
            if (smartGuardScript != null)
            {
                smartGuardScript.ReactToStolenItem();
            }
            else if (patrolGuardScript != null)
            {
                patrolGuardScript.ReactToStolenItem();
            }
            else
            {
                Debug.LogWarning("No guard script assigned to flashlight detection!");
            }
        }
    }
}