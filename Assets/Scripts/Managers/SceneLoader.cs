using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers
{
    public class SceneLoader : MonoBehaviour
    {
        private const string GamePlaySceneName = "GamePlay";
        private const string OpeningSceneName = "Opening Scene";
        private const string EndingSceneName = "Ending Scene"; 


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnExit();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                RestartGame();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EndGame();
            }
        }

        private void OnEnable()
        {
            GameEvents.GameOver += EndGame;
        }
        
        private void OnDisable()
        {
            GameEvents.GameOver -= EndGame;
        }

        public void LoadGamePlay()
        {
            SceneManager.LoadScene(GamePlaySceneName);
        }

        public void RestartGame()
        {
            //GameEvents.RestartGame?.Invoke();
            SceneManager.LoadScene(OpeningSceneName);
        }

        private void EndGame()
        {
            //GameEvents.GameOver?.Invoke();
            SceneManager.LoadScene(EndingSceneName);
        }
        
        private void OnExit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}