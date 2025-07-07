using System;
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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || 
                Input.GetKeyDown(KeyCode.Return) || 
                Input.GetKeyDown(KeyCode.Space))
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