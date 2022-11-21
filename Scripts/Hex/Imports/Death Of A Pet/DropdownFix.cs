using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DropdownFix : MonoBehaviour
{

    public void fix(GameObject obj)
    {
        int x = obj.transform.childCount;
        for (int i = 0; i < x; i ++)
        {
            if (obj.transform.GetChild(i).name.ToLower() == "dropdown list")
            {
                Destroy(obj.transform.GetChild(i).gameObject);
                break;
            }
        }
    }

}
