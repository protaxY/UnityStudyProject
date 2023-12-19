using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Play()
    {
        SceneManager.LoadScene("office");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
