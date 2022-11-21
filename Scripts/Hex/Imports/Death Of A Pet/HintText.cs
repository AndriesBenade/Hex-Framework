using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HintText : MonoBehaviour
{
    Text txt;
    public bool halt = false;

    void Start()
    {
        if (txt == null)
            txt = GetComponent<Text>();
        clear();
    }

    public void clear()
    {
        txt.text = "";
    }

    public void set(string newText)
    {
        if (!halt)
            txt.text = newText;
    }

}
