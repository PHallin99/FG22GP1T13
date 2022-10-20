using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interactables;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Tooltip("All collectibles in the level.")]
        private List<PickUp> collectibles = new();

        [SerializeField] [Range(0f, 10f)] private int startGameTimer;
        [SerializeField] [Range(1f, 20f)] private int timeLimitMinutes;
        private GameState gameState = GameState.Stopped;

        public event Action OnStartGame;


        private float secondsLeft;
        private bool timerStarted;
        [SerializeField] private int trashLeft;
        [SerializeField] private int endScene = 6;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        private void Start()
        {
            SceneManager.GetInstance().OnNewSceneLoad += StartGame;
            PlayerPrefs.SetInt("Previous Scene",  UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        private void Update()
        {
            if (!timerStarted || gameState == GameState.Stopped) return;
            secondsLeft -= Time.deltaTime;
            if (secondsLeft <= 0) EndGame(false);
        }

        public void StartGame()
        {
            timeLimitMinutes = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex switch
            {
                1 => GameConstants.Level1TimeLimitMinutes,
                2 => GameConstants.Level2TimeLimitMinutes,
                3 => GameConstants.Level3TimeLimitMinutes,
                _ => timeLimitMinutes
            };

            StartCoroutine(StartGameTimer());
            collectibles.AddRange(FindObjectsOfType<PickUp>());
            OnStartGame?.Invoke();

            IEnumerator StartGameTimer()
            {
                yield return new WaitForSeconds(startGameTimer);

                gameState = GameState.Started;
                trashLeft = collectibles.Count;
                secondsLeft = timeLimitMinutes * 60;
                timerStarted = true;
            }
        }

        public void CollectableCollect(int trash)
        {
            if (gameState == GameState.Stopped) return;
            trashLeft -= trash;
            if (trashLeft == 0) EndGame(true);
        }

        public int TrashLeft()
        {
            return trashLeft;
        }

        public float SecondsLeft()
        {
            return secondsLeft;
        }

        public void EndGame(bool wonGame)
        {
            // Win or lose logic
            Debug.Log("End Game");
            gameState = GameState.Stopped;
            ResetGame();

            switch (wonGame)
            {
                case true:
                    SceneManager.GetInstance().LoadNextScene();
                    break;
                case false:
                    SceneManager.GetInstance().LoadScene(0);
                    break;
            }        
        }

        private void ResetGame()
        {
            secondsLeft = timeLimitMinutes * 60;
            trashLeft = collectibles.Count;
        }
    }
}