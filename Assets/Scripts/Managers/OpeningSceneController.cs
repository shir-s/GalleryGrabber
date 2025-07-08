using System;
using Sound;
using UnityEngine;
using Utils;

namespace Managers
{
    public class OpeningSceneController : MonoBehaviour
    {
        [SerializeField] private GameObject instructionsPanel;

        private bool hasShownInstructions = false;

        private void Start()
        {
            if (instructionsPanel != null)
            {
                instructionsPanel.SetActive(false); 
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