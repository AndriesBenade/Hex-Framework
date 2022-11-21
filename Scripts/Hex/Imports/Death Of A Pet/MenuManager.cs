using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public bool lockOnStart = true;
    public bool altarCursor = true;
    public GameObject[] cursorOnUI;
    [Space(10)]
    public GameObject pauseMenu;
    private bool paused = false;

    private void Start()
    {
        if (lockOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = paused ? false : true;
            pauseMenu.SetActive(paused);
        }

        bool showCursor = false;
        foreach (GameObject g in cursorOnUI)
        {
            if (g.activeSelf)
            {
                showCursor = true;
                break;
            }
        }

        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }
}
