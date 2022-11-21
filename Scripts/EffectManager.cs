using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class eff
{
    public string id;
    public ParticleSystem effect;
}

public class EffectManager : MonoBehaviour
{

    public eff[] effects;

    public void PlayEffect(string id, Vector3 position)
    {
        foreach (eff e in effects)
        {
            if (e.id == id)
            {
                e.effect.transform.position = position;
                e.effect.Play();
                break;
            }
        }
    }

}
