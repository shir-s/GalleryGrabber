using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Utils;

namespace Stealable
{
    public class StealableItem : MonoBehaviour
    {
        [SerializeField] private float stealDuration = 2f; 
        private float stealProgress = 0f;

        private bool isPlayerNearby = false;
        private bool isBeingStolen = false;
        [SerializeField] private int itemValue = 10;
        private float _stealProgress = 0f;
        private Collider2D[] _allColliders; 
        private bool _isPlayerNearby = false;
        private bool _isBeingStolen = false;
        private Transform _playerTransform;
        
        public Canvas progressCanvas;
        public Image progressCircle; 
        
        private void Start()
        {
            if (progressCanvas != null)
                progressCanvas.gameObject.SetActive(false);
            _allColliders = GetComponentsInChildren<Collider2D>();
        }

        public void TrySteal(float deltaTime)
        {
            if (!_isBeingStolen)
            {
                _isBeingStolen = true;
                if (progressCanvas != null)
                    progressCanvas.gameObject.SetActive(true);
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
            if (progressCanvas != null)
                progressCanvas.gameObject.SetActive(false);
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
            if (progressCanvas != null)
                progressCanvas.gameObject.SetActive(false);
            
            foreach (var col in _allColliders)
            {
                col.enabled = false;
            }
            
            transform.DOScale(transform.localScale * 0.7f, 0.15f).SetEase(Ease.OutBack);
            if (_playerTransform != null)
            {
                transform
                    .DOJump(_playerTransform.position, 1.0f, 1, 0.4f)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
            }
            else
            {
                // fallback
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerNearby = true;
                _playerTransform = other.transform;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerNearby = false;
                StopSteal();
            }
        }

        public bool CanBeStolen()
        {
            return _isPlayerNearby;
        }
    }
}
