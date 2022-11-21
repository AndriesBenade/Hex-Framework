using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimedSceneLoad : MonoBehaviour
{

    public float delay = 30;
    public bool loadSingleScene = true;
    public string sceneName = "";

    private float startTime = 0;
    private bool loaded = false;

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - startTime >= delay && !loaded)
        {
            if (loadSingleScene)
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            else
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            loaded = true;
        }
    }

}
