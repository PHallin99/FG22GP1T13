using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;

    private bool isPaused;
    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void Pause()
    {
        Debug.Log("Pause");

        if (isPaused)
        {
            UnPause();
        } else
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
            isPaused = true;
        }
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    public void ReloadScene()
    {
        UnPause();
        SceneManager.GetInstance().LoadScene(SceneManager.GetInstance().ActiveSceneIndex()); //I might have done the scenemanager badly
    }
    public void LoadMainMenu()
    {
        UnPause();
        SceneManager.GetInstance().LoadScene(0);
    }


}
