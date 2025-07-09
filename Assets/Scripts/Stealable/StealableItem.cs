using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sound;
using TMPro;
using UnityEngine.Rendering.Universal;
using Utils;

namespace Stealable
{
    public class StealableItem : MonoBehaviour
    {
        [SerializeField] private float stealDuration = 2f;
        [SerializeField] private int itemValue = 10;
        [SerializeField] private Canvas progressCanvas;
        [SerializeField] private Image progressCircle;
        [SerializeField] private Canvas priceCanvas;
        [SerializeField] private TextMeshProUGUI priceText;

        private float _stealProgress = 0f;
        private bool _isPlayerNearby = false;
        private bool _isBeingStolen = false;
        private Transform _playerTransform;
        private Collider2D[] _allColliders;
        private Light2D _light;

        private void Start()
        {
            if (progressCanvas != null)
                progressCanvas.gameObject.SetActive(false);
            if (priceCanvas != null)
            {
                priceCanvas.gameObject.SetActive(false);
                if (priceText != null)
                {
                    //priceText.text = itemValue.ToString("N0");
                    priceText.text = FormatNumber(itemValue);
                }
            }
            _light = GetComponentInChildren<Light2D>();
            _allColliders = GetComponentsInChildren<Collider2D>();
        }
        
        private string FormatNumber(int number)
        {
            if (number >= 1_000_000)
                return (number / 1_000_000f).ToString("0.#") + "M";
            if (number >= 1_000)
                return (number / 1_000f).ToString("0.#") + "K";
            return number.ToString();
        }


        public void TrySteal(float deltaTime)
        {
            if (!_isBeingStolen)
            {
                _isBeingStolen = true;
                progressCanvas?.gameObject.SetActive(true);
            }

            if (_stealProgress < stealDuration)
            {
                _stealProgress += deltaTime;
                UpdateProgressUI();

                if (_stealProgress >= stealDuration)
                {
                    StealSuccess();
                }
            }
        }

        public void StopSteal()
        {
            _isBeingStolen = false;
            progressCanvas?.gameObject.SetActive(false);
            UpdateProgressUI();
        }

        private void UpdateProgressUI()
        {
            if (progressCircle != null)
                progressCircle.fillAmount = _stealProgress / stealDuration;
        }

        private void StealSuccess()
        {
            _isBeingStolen = false;
            progressCanvas?.gameObject.SetActive(false);
            _light?.gameObject.SetActive(false);

            foreach (var col in _allColliders)
                col.enabled = false;

            transform.DOScale(transform.localScale * 0.7f, 0.15f).SetEase(Ease.OutBack);
            GameEvents.StoleItem?.Invoke(itemValue);

            if (_playerTransform != null)
            {
                transform
                    .DOJump(_playerTransform.position, 1.0f, 1, 0.4f)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        SpawnGhostCollider();
                        Destroy(gameObject);
                        SoundManager.Instance.PlaySound("Steal", transform);
                    });
            }
            else
            {
                SpawnGhostCollider();
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerNearby = true;
                _playerTransform = other.transform;
                if(priceCanvas!=null)
                    priceCanvas.gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerNearby = false;
                StopSteal();
                if(priceCanvas!=null)
                    priceCanvas.gameObject.SetActive(false);
            }
        }

        public bool IsBeingStolen() => _isBeingStolen;

        public bool CanBeStolen() => _isPlayerNearby;

        private void SpawnGhostCollider()
        {
            GameObject ghost = new GameObject("StolenGhost");
            ghost.transform.position = transform.position;
            ghost.transform.localScale = transform.localScale;

            Rigidbody2D rb = ghost.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.useFullKinematicContacts = true; 

            BoxCollider2D original = GetComponent<Collider2D>() as BoxCollider2D;
            BoxCollider2D box = ghost.AddComponent<BoxCollider2D>();
            box.isTrigger = true;

            if (original != null)
                box.size = original.size;
            else
                box.size = Vector2.one; 

            ghost.tag = "StolenGhost";

            SpriteRenderer renderer = ghost.AddComponent<SpriteRenderer>();
            renderer.color = new Color(1f, 1f, 1f, 0f);

        }

    }
}
