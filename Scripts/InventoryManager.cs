using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ioFileSrc;

[System.Serializable]
public class invItem
{

    public string id = "";
    public GameObject obj;
    public bool obtained = false;
    public bool equipable = true;
    [Space(5)]
    public bool dontSave = false;

    public void enable()
    {
        obj.SetActive(true);
    }

    public void disable()
    {
        obj.SetActive(false);
    }

    public void deduct()
    {
        obtained = false;
    }

}

public class InventoryManager : MonoBehaviour
{
    public invItem[] items;
    public int index = -1;
    [Space(10)]
    public AudioSource audioPlayer;
    [Space(10)]
    public bool DisableCollectedOnLoad = true;

    private bool halt = false;
    private string invFile = "inventory.flow";

    private void Start()
    {
        LoadInv();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            halt = halt ? false : true;
        if (!halt)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                NextItem();
            }
        }
    }

    public int GetItem(string id)
    {
        int ix = -1;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].id == id)
            {
                ix = i;
                break;
            }
        }
        return ix;
    }

    public void NextItem()
    {
        bool b = false;
        if (index != -1)
            items[index].disable();
        while (!b)
        {
            index++;
            if (index < items.Length)
            {
                if (items[index].obtained && items[index].equipable)
                    b = true;
            }
            else
            {
                index = -1;
                b = true;
            }
        }
        if (index != -1)
            items[index].enable();
        audioPlayer.Play();
    }

    public void LoadInv()
    {
        ioFile io = new ioFile(invFile);
        if (!io.exists())
        {
            //SaveInv();
            return;
        }
        for (int i = 0; i < items.Length; i++)
        {
            items[i].obtained = io.getitemvalueasbool(items[i].id);
        }
        index = io.getitemvalueasint("inv_index");
        if (index != -1)
            items[index].enable();
        if (DisableCollectedOnLoad)
        {
            InventoryItem[] allItems = FindObjectsOfType(typeof(InventoryItem)) as InventoryItem[];
            foreach (InventoryItem i in allItems)
            {
                if (items[GetItem(i.id)].obtained)
                {
                    i.gameObject.SetActive(false);
                }
            }
        }
    }

    public void SaveInv()
    {
        ioFile io = new ioFile(invFile);
        if (!io.exists())
            io.create();
        io.delete();
        io.create();
        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].dontSave)
                io.additem(items[i].id, items[i].obtained.ToString());
        }
        io.additem("inv_index", index.ToString());
    }

    public void ObtainItem(string id)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].id == id)
            {
                items[i].obtained = true;
            }
        }
    }
}
