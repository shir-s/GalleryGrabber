using Sound;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace Managers
{
    public class ScoreUI : MonoBehaviour
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
            if(pointsToAdd > 200) 
                SoundManager.Instance.PlaySound("Money", transform);
            _points += pointsToAdd;
            if (_points >= pointsToWin)
            {
                GameStates.LastGameOverReason = GameOverReason.PlayerWon;
                GameEvents.GameOver?.Invoke(GameOverReason.PlayerWon);
            }
            if (pointsText != null)
            {
                //pointsText.text = $"{_points:N0} / {pointsToWin:N0}";
                pointsText.text = $"{FormatNumber(_points)} / {FormatNumber(pointsToWin)}";
            }
        }
        
        
        private string FormatNumber(int number)
        {
            if (number >= 1_000_000)
                return (number / 1_000_000f).ToString("0.#") + "M";
            if (number >= 1_000)
                return (number / 1_000f).ToString("0.#") + "K";
            return number.ToString();
        }

    }
}