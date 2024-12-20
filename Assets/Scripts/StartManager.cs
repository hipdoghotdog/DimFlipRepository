using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [HideInInspector]
    public SaveSystem saveSystem;
    
    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void LoadGame()
    {
        MenuManager.Instance.ToggleMenu();

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetSave()
    {
        GameManager.Instance.saveSystem.ResetSave();
    }
}
