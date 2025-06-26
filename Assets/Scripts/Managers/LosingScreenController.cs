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
                    break;
                case GameOverReason.OutOfLives:
                    outOfLivesPanel.SetActive(true);
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