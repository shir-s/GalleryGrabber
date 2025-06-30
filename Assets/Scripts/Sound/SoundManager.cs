using System;
using System.Linq;
using UnityEngine;
using Utilities;
using Utils;

namespace Sound
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [SerializeField] private AudioSettings settings;
        private AudioSourceWrapper _backgroundMusic;
        private AudioSourceWrapper openingMusic;
        private AudioSourceWrapper endingMusic;

        private void Start()
        {
            PlayOpeningMusic();
        }

        private void OnEnable()
        {
            GameEvents.StartLevel += PlayBackgroundMusic;
            GameEvents.RestartLevel += StopOpeningMusic;
            GameEvents.RestartLevel += StopEndingMusic;
            GameEvents.GameOver += StopBackgroundMusic;
            //GameEvents.GameOver += PlayEndingMusic;
        }
    
        private void OnDisable()
        {   
            GameEvents.StartLevel -= PlayBackgroundMusic;
            GameEvents.RestartLevel -= StopOpeningMusic;
            GameEvents.RestartLevel -= StopEndingMusic;
            GameEvents.GameOver -= StopBackgroundMusic;
            //GameEvents.GameOver -= PlayEndingMusic;
        }

        public void PlaySound(string audioName, Transform spawnTransform)
        {
            var config = FindAudioConfig(audioName);
            if (config == null)
                return;
            var soundObject = SoundPool.Instance.Get();
            soundObject.transform.position = spawnTransform.position;
            soundObject.Play(config.clip, config.volume,config.loop);
        }
    
        private void PlayBackgroundMusic()
        {
            var config = FindAudioConfig("Background");
            if (config == null)
                return;
            _backgroundMusic = SoundPool.Instance.Get();
            _backgroundMusic.Play(config.clip, config.volume,config.loop);
        }
        
        private void PlayOpeningMusic()
        {
            var config = FindAudioConfig("Background");
            if (config == null)
                return;
            openingMusic = SoundPool.Instance.Get();
            openingMusic.Play(config.clip, config.volume,config.loop);
        }
        
        /*private void PlayEndingMusic(GameOverReason reason)
        {
            AudioConfig config = null;
            if (reason == GameOverReason.OutOfLives)
            {
                config = FindAudioConfig("Victor Band");
            }
            else if(reason == GameOverReason.TooMuchDirt)
            {
                config = FindAudioConfig("Game Over");
            }
            if (config == null)
                return;
            endingMusic = SoundPool.Instance.Get();
            endingMusic.Play(config.clip, config.volume,config.loop);
        }*/
    
        private void StopBackgroundMusic(GameOverReason dummy)
        {
            if (_backgroundMusic == null)
                return;
            _backgroundMusic.Reset();
        }
        
        private void StopOpeningMusic()
        {
            if (openingMusic == null)
                return;
            openingMusic.Reset();
        }
        
        private void StopEndingMusic()
        {
            if (endingMusic == null)
                return;
            endingMusic.Reset();
        }

        private AudioConfig FindAudioConfig(string audioName)
        {
            var x = settings.audioConfigs.FirstOrDefault(config => config.name == audioName);
            if(x!= null)
            {
                return x;
            }
            else
            {
                Debug.LogError($"Audio config not found for {audioName}");
                return null;
            }
        }
    }
}