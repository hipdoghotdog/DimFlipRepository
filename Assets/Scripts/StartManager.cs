using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public void StartGame()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.LoadLevel(0);
            return;
        }
        
        SceneManager.LoadScene("InitScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
