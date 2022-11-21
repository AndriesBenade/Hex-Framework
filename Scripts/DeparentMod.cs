using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeparentMod : MonoBehaviour
{
    void Start()
    {
        if (transform.parent != null)
            transform.SetParent(null);
    }
}
