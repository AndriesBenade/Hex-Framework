using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public string id = "";
    public string hint = "";
    [Space(5)]
    public FlowEvent[] onCollect;
    [Space(5)]
    public bool enableTriggers = false;

    private HintText txthint;
    private InventoryManager inv;
    private bool allow = false;

    private void Start()
    {
        if (inv == null)
            inv = FindObjectOfType<InventoryManager>();
        if (txthint == null)
            txthint = FindObjectOfType<HintText>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && allow && enableTriggers)
        {
            obtain();
            txthint.clear();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            txthint.set(hint);
            allow = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            txthint.clear();
            allow = false;
        }
    }

    public void obtain()
    {
        if (onCollect.Length > 0)
        {
            foreach (FlowEvent e in onCollect)
            {
                e.runEvent();
            }
        }
        inv.ObtainItem(id);
        gameObject.SetActive(false);
    }

}
