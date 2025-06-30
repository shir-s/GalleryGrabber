using System;
using Sound;
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
        private const string LosingSceneName = "Losing Scene";
        private const string WinningSceneName = "Winning Scene";


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
                LoadGamePlay();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                //EndGame(GameOverReason.OutOfLives);
            }
        }

        private void OnEnable()
        {
            GameEvents.GameOver += EndGame;
            GameEvents.PlayerWon += HandleWin;
            GameEvents.RestartLevel += LoadGamePlay;
        }
        
        private void OnDisable()
        {
            GameEvents.GameOver -= EndGame;
            GameEvents.PlayerWon -= HandleWin;
            GameEvents.RestartLevel -= LoadGamePlay;
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

        private void EndGame(GameOverReason reason)
        {
            //GameEvents.GameOver?.Invoke();
            SceneManager.LoadScene(LosingSceneName);
        }
        
        private void HandleWin()
        {
            //GameEvents.PlayerWon?.Invoke();
            SceneManager.LoadScene(WinningSceneName);
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