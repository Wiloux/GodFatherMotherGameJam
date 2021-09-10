using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void BouttonJouer()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MapTestWiloux");
    }

    public void BouttonControl()
    {
        SceneManager.LoadScene("Control");
    }

        public void BouttonCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void BouttonMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void BouttonQuitter()
    {
        Application.Quit();
    }
}
