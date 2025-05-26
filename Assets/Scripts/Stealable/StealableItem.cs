using UnityEngine;
using UnityEngine.UI;

namespace Stealable
{
    public class StealableItem : MonoBehaviour
    {
        [SerializeField] private float stealDuration = 2f; 
        private float stealProgress = 0f;

        private bool isPlayerNearby = false;
        private bool isBeingStolen = false;
        
        public Canvas progressCanvas;
        public Image progressCircle; 
        
        private void Start()
        {
            if (progressCanvas != null)
                progressCanvas.gameObject.SetActive(false);
        }

        public void TrySteal(float deltaTime)
        {
            if (!isBeingStolen)
            {
                isBeingStolen = true;
                if (progressCanvas != null)
                    progressCanvas.gameObject.SetActive(true);
            }

            if (stealProgress < stealDuration)
            {
                stealProgress += deltaTime;
                UpdateProgressUI();
                if (stealProgress >= stealDuration)
                {
                    StealSuccess();
                }
            }
        }

        public void StopSteal()
        {
            isBeingStolen = false;
            if (progressCanvas != null)
                progressCanvas.gameObject.SetActive(false);
            UpdateProgressUI();
        }

        private void UpdateProgressUI()
        {
            if (progressCircle != null)
                progressCircle.fillAmount = stealProgress / stealDuration;
        }

        private void StealSuccess()
        {
            //TODO: CHANGE THIS BEHAVIOR
            Destroy(gameObject); 
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                isPlayerNearby = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNearby = false;
                StopSteal();
            }
        }

        public bool CanBeStolen()
        {
            return isPlayerNearby;
        }
    }
}
