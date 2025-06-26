using System;
using UnityEngine;
using Utils;

namespace Managers
{
    public class OpeningSceneController : MonoBehaviour
    {
        public void Update()
        {
            if( Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                GameEvents.RestartLevel?.Invoke();
            }
        }
    }
}