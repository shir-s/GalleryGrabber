using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace Managers
{
    public class PointManager : MonoBehaviour
    {
        private int _points;
        [SerializeField] private int pointsToWin = 60; // Toggle to enable or disable points display
        [SerializeField] private TextMeshProUGUI pointsText;

        private void OnEnable()
        {
            GameEvents.StoleItem += UpdatePoints;
            GameEvents.OnDirtCollected += UpdatePoints;
        }
        
        private void OnDisable()
        {
            GameEvents.StoleItem -= UpdatePoints;   
            GameEvents.OnDirtCollected -= UpdatePoints;
        }
        
        private void Start()
        {
            _points = 0;
            UpdatePoints(0);
        }
        
        private void UpdatePoints(int pointsToAdd)
        {
            _points += pointsToAdd;
            if (_points >= pointsToWin)
            {
                GameEvents.PlayerWon?.Invoke();
            }
            if (pointsText != null)
            {
                pointsText.text = $"{_points} / {pointsToWin}";
            }
        }

    }
}