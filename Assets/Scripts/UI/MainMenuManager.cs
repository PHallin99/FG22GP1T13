using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] GameObject creditsObj;

        private void LoadLevelSelectionScene()
        {
            SceneManager.GetInstance().LoadNextScene();
        }

        public void ShowCredits()
        {
            creditsObj.SetActive(!creditsObj.active);
        }

        public void Exit()
        {
            Debug.Log("Exit");
        }
    }
}

