using Sound;

namespace Managers
{
    using UnityEngine;
    using Utils;
    public class LosingScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject tooMuchDirtPanel;
        [SerializeField] private GameObject outOfLivesPanel;

        private void Start()
        {
            GameOverReason reason = GameManager.Instance.LastGameOverReason;

            tooMuchDirtPanel.SetActive(false);
            outOfLivesPanel.SetActive(false);

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