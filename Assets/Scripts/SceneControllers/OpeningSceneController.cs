using Sound;
using UnityEngine;
using Utils;

namespace SceneControllers
{
    public class OpeningSceneController : MonoBehaviour
    {
        [SerializeField] private GameObject instructionsPanel;
        [SerializeField] private GameObject openingPanel;
        private bool hasShownInstructions = false;

        private void Start()
        {
            if (instructionsPanel != null)
            {
                instructionsPanel.SetActive(false); 
            }
            if(openingPanel != null)
            {
                openingPanel.SetActive(true);
            }
            SoundManager.Instance.PlaySound("Opening", transform);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || 
                Input.GetKeyDown(KeyCode.Return))
            {
                if (!hasShownInstructions)
                {
                    if (instructionsPanel != null)
                    {
                        instructionsPanel.SetActive(true);
                        openingPanel.SetActive(false);
                    }
                    hasShownInstructions = true;
                }
                else
                {
                    GameEvents.RestartLevel?.Invoke(); 
                }
            }
        }
    }
}