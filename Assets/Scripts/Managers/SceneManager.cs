using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace Managers
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager instance;
        public event Action OnNewSceneLoad;
        public event Action MenuSceneLoad;
        private int currentScene;
        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(this);
            print(PlayerPrefs.GetInt("Previous Scene"));
            ActiveSceneIndex();


            UnityEngine.SceneManagement.SceneManager.sceneLoaded += NewSceneLoaded;
        }
        public static SceneManager GetInstance()
        {
            return instance;
        }

        private void NewSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "Tutorial1")
                return;
            if (scene.name == "Tutorial2")
                return;
            OnNewSceneLoad?.Invoke();
        }

        public void LoadNextScene()
        {
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene + 1);
        }
        public void LoadNextLevel()
        {
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentScene+ 1);
        }

        public void LoadScene(int i)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(i);
        }
        
        public int ActiveSceneIndex()
        {
            int currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            return currentScene;
        }
    }
}

