using Sound;

namespace SceneControllers
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
            GameOverReason reason = GameStates.LastGameOverReason;
            tooMuchDirtPanel.SetActive(false);
            outOfLivesPanel.SetActive(false);
            playerWonPanel.SetActive(false);
            switch (reason)
            {
                case GameOverReason.TooMuchDirt:
                    tooMuchDirtPanel.SetActive(true);
                    SoundManager.Instance.PlaySound("Fired1", transform);
                    SoundManager.Instance.PlaySound("Losing", transform);
                    break;
                case GameOverReason.OutOfLives:
                    outOfLivesPanel.SetActive(true);
                    SoundManager.Instance.PlaySound("Jail1", transform);
                    SoundManager.Instance.PlaySound("Losing", transform);
                    break;
                case GameOverReason.PlayerWon:
                    playerWonPanel.SetActive(true);
                    SoundManager.Instance.PlaySound("WinScreen", transform);
                    break;
            }
        }
        public void Update()
        {
            if( Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                GameEvents.RestartLevel?.Invoke();
            }
        }
    }
}