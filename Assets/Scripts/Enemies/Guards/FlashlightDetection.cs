using UnityEngine;

using UnityEngine;
using Player;

public class FlashlightDetection : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && PlayerSteal.isStealing)
        {
            Debug.Log("Player caught during theft!");
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
        // SceneManager.LoadScene("GameOverScene");
    }
}
