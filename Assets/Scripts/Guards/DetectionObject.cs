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
        [Header("Detection Settings")]
        [SerializeField] private float detectionCooldown = 3f; // Cooldown between triggers
        [SerializeField] private Light2D detectionLight;

        private float _lastDetectionTime = -Mathf.Infinity;
        private Color _originalLightColor;

        private void Awake()
        {
            if (detectionLight != null)
            {
                _originalLightColor = detectionLight.color;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryHandleDetection(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (Time.time >= _lastDetectionTime + detectionCooldown)
            {
                if (TryHandleDetection(other))
                {
                    _lastDetectionTime = Time.time;
                }
            }
        }

        private bool TryHandleDetection(Collider2D other)
        {
            if (!other.CompareTag("Player") || !PlayerSteal.isStealing)
                return false;

            if (CompareTag("Guard"))
            {
                StartCoroutine(HandleCaughtWithPause());
            }
            else if (CompareTag("Camera"))
            {
                HandleCaughtImmediate();
            }

            return true;
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

        private void TriggerFlashEffect()
        {
            if (detectionLight != null)
            {
                StartCoroutine(FlashRed());
            }
        }

        private IEnumerator FlashRed()
        {
            const float duration = 5f;
            const float blinkInterval = 0.25f;
            float timer = 0f;

            while (timer < duration)
            {
                detectionLight.color = Color.red;
                yield return new WaitForSeconds(blinkInterval);

                detectionLight.color = Color.clear;
                yield return new WaitForSeconds(blinkInterval);

                timer += blinkInterval * 2;
            }

            detectionLight.color = _originalLightColor;
        }
    }
}
