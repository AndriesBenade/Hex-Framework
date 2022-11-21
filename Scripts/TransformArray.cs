using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformArray : MonoBehaviour
{
    public Transform[] points;

    public Transform getRandom()
    {
        return points[Random.Range(0, points.Length)];
    }
    public Transform getRandomSpecial(Transform t)
    {
        int index = Random.Range(0, points.Length);
        while (index == getIndex(t))
        {
            index = Random.Range(0, points.Length);
        }
        return points[index];
    }

    private int getIndex(Transform t)
    {
        int index = -1;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == t)
            {
                index = i;
                break;
            }
        }
        return index;
    }
}
