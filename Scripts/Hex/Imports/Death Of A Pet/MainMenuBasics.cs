using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ioFileSrc;

public class MainMenuBasics : MonoBehaviour
{

    public bool isMainMenu = true;
    public Button continueButton;
    private string VAR_FILENAME = "var.flow";

    private void Start()
    {
        if (isMainMenu)
        {
            ioFile file = new ioFile(VAR_FILENAME);
            if (!file.exists() || !file.hasitem("level"))
                continueButton.interactable = false;
        }
    }

    public void Continue()
    {
        ioFile file = new ioFile(VAR_FILENAME);
        if (!file.exists())
            return;
        if (!file.hasitem("level"))
            return;
        string sceneName = "Level" + file.getitemvalue("level");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void DeleteFiles()
    {
        ioFile file = new ioFile("inventory.flow");
        file.delete();
        file = new ioFile("health.flow");
        file.delete();
        file = new ioFile("var.flow");
        file.delete();
    }

    private void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
    
    public void OpenURL(string url)
    {
        System.Diagnostics.Process.Start(url);
    }

}
