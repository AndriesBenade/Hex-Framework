using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialMover : MonoBehaviour
{
    public int index = 0;
    [Space(10)]
    public bool x = false;
    public bool y = false;
    [Space(10)]
    public float offset = 0.1f;
    public float offsetDelay = 0.05f;
    public float maxOffset = 4;

    private MeshRenderer render;
    private float lastTime = 0;

    private void Start()
    {
        if (render == null)
            render = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (Time.time - lastTime >= offsetDelay)
        {
            lastTime = Time.time;
            Vector2 vec2 = render.materials[index].GetTextureOffset("_MainTex");
            if (x)
            {
                if (vec2.x >= maxOffset)
                {
                    vec2.x -= maxOffset;
                    vec2.x += offset;
                }
                else
                {
                    vec2.x += offset;
                }
            }
            if (y)
            {
                if (vec2.y >= maxOffset)
                {
                    vec2.y -= maxOffset;
                    vec2.y += offset;
                }
                else
                {
                    vec2.y += offset;
                }
            }
            render.materials[index].SetTextureOffset("_MainTex", vec2);
        }
    }

}
