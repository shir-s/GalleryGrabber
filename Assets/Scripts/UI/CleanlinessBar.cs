using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class CleanlinessBar : MonoBehaviour
    {
        [SerializeField] private Slider cleanlinessSlider;
        
        private void OnEnable()
        {
            GameEvents.OnCleanlinessChanged += SetCleanlinessLevel;
        }
        private void OnDisable()
        {
            GameEvents.OnCleanlinessChanged -= SetCleanlinessLevel;
        }
        public void SetCleanlinessLevel(float value)
        {
            cleanlinessSlider.value = value;
        }
    }
}