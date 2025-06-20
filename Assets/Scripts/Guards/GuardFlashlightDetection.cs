using UnityEngine;

public class GuardFlashlightDetection : MonoBehaviour
{
    [SerializeField] private SmartGuardWithModel guardScript;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("StolenGhost"))
        {
            Debug.Log("Flashlight detected stolen object (ghost)!");
            guardScript.ReactToStolenItem();
        }
    }
}