using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicKiller : MonoBehaviour
{
    public bool play = false;

    // Start is called before the first frame update
    void Start()
    {
        if (play)
            GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().PlayMusic();
        else
            GameObject.FindGameObjectWithTag("Music").GetComponent<MusicClass>().StopMusic();
    }
}
