using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class MainMenu : MonoBehaviour
{
    public Rewired.Player playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>().playerController;
    }

    private void Update()
    {
        if (playerController.GetButtonDown("Jump"))
        {
            BouttonJouer();
        }

        if (playerController.GetButtonDown("Gravity"))
        {
            BouttonMenu();
        }

    }


    public void BouttonJouer()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SceneFinal");
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
