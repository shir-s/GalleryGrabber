using System.Collections;
using SceneControllers;
using Sound;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Utils;

namespace Guards
{
    public class DetectionObject : MonoBehaviour
    {
        [SerializeField] private float detectionCooldown = 3f; // Cooldown in seconds
        [SerializeField] private Light2D detectionLight;
        private float lastDetectionTime = -Mathf.Infinity;
        private Color originalColor;
        
        private void Awake()
        {
            if (detectionLight != null)
                originalColor = detectionLight.color;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && PlayerSteal.isStealing)
            {
                if (CompareTag("Guard"))
                {
                    StartCoroutine(HandleCaughtWithPause());
                }
                else
                {
                    HandleCaughtImmediate();
                }
            }
        }

        private IEnumerator HandleCaughtWithPause()
        {
            SoundManager.Instance.PlaySound("GuardCatches", transform);
            TriggerFlashEffect();
            yield return new WaitForSeconds(1.5f);
            GameEvents.GameOver?.Invoke(GameOverReason.OutOfLives);
        }

        private void HandleCaughtImmediate()
        {
            GameEvents.PlayerLostLife?.Invoke();
            SoundManager.Instance.PlaySound("Camera3", transform);
            TriggerFlashEffect();
        }

        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && PlayerSteal.isStealing)
            {
                if (Time.time >= lastDetectionTime + detectionCooldown)
                {
                    GameEvents.PlayerLostLife?.Invoke();
                    SoundManager.Instance.PlaySound("Camera3", transform);
                    TriggerFlashEffect();
                    lastDetectionTime = Time.time;
                }
            }
        }


        private void TriggerFlashEffect()
        {
            if (detectionLight != null)
                StartCoroutine(FlashRed());
        }
        
        private System.Collections.IEnumerator FlashRed()
        {
            float duration = 5f;
            float blinkInterval = 0.25f;
            float timer = 0f;

            while (timer < duration)
            {
                detectionLight.color = Color.red;
                yield return new WaitForSeconds(blinkInterval);

                // detectionLight.color = originalColor; // just to ensure it returns to original color
                detectionLight.color = Color.clear;
                yield return new WaitForSeconds(blinkInterval);

                timer += blinkInterval * 2;
            }

            detectionLight.color = originalColor;
        }
    }
}