using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void BouttonJouer()
    {
        SceneManager.LoadScene("SceneFinale");
    }

    public void BouttonCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void BouttonMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void BouttonQuitter()
    {
        Application.Quit();
    }
}