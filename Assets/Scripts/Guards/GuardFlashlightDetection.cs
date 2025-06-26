using UnityEngine;

public class GuardFlashlightDetection : MonoBehaviour
{
    // [SerializeField] private SmartGuardWithModel guardScript;
    //
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("StolenGhost"))
    //     {
    //         Debug.Log("Flashlight detected stolen object (ghost)!");
    //         guardScript.ReactToStolenItem();
    //     }
    // }
    
    [SerializeField] private SmartGuardWithModel smartGuardScript;
    [SerializeField] private PatrolGuard patrolGuardScript;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("StolenGhost"))
        {
            Debug.Log("Flashlight detected stolen object (ghost)!");

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
                Debug.LogWarning("No guard script assigned!");
            }
        }
    }

}