using Sound;

namespace Managers
{
    using UnityEngine;
    using Utils;
    public class EndingScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject tooMuchDirtPanel;
        [SerializeField] private GameObject outOfLivesPanel;
        [SerializeField] private GameObject playerWonPanel;

        private void Start()
        {
            GameOverReason reason = GameManager.LastGameOverReason;
            tooMuchDirtPanel.SetActive(false);
            outOfLivesPanel.SetActive(false);
            playerWonPanel.SetActive(false);
            switch (reason)
            {
                case GameOverReason.TooMuchDirt:
                    tooMuchDirtPanel.SetActive(true);
                    SoundManager.Instance.PlaySound("Fired1", transform);
                    break;
                case GameOverReason.OutOfLives:
                    outOfLivesPanel.SetActive(true);
                    SoundManager.Instance.PlaySound("Jail1", transform);
                    break;
                case GameOverReason.PlayerWon:
                    playerWonPanel.SetActive(true);
                    SoundManager.Instance.PlaySound("WinningScreen", transform);
                    break;
            }
        }
        public void Update()
        {
            if( Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                GameEvents.RestartLevel?.Invoke();
            }
        }
    }
}