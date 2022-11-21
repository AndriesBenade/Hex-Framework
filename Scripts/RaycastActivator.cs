using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastActivator : MonoBehaviour
{
    public Transform look;
    public float maxDistance = 3;
    public float interval = 0.025f;
    [Space(10)]
    public bool enablePhysGrab = false;
    public Transform grip;
    public float throwPower = 5;

    private HintText hint;
    private float lastCheck = 0;
    private bool grabbing = false;
    private PhysObject grabObj;

    private void Start()
    {
        if (hint == null)
        {
            hint = FindObjectOfType<HintText>();
        }
    }

    void Update()
    {
        if (Time.time - lastCheck >= interval && !grabbing)
        {
            RaycastHit ray = new RaycastHit();
            int layerMask =~ LayerMask.GetMask("Player");
            if (Physics.Raycast(look.position, look.forward, out ray, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal))
            {
                GameObject obj = ray.collider.gameObject;
                if (obj.GetComponent<DoorManager>() != null)
                {
                    hint.set(obj.GetComponent<DoorManager>().doorHint);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        obj.GetComponent<DoorManager>().Use();
                    }
                }
                if (obj.GetComponent<InventoryItem>() != null)
                {
                    hint.set(obj.GetComponent<InventoryItem>().hint);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        obj.GetComponent<InventoryItem>().obtain();
                    }
                }
                if (obj.GetComponent<FlowEvent>() != null)
                {
                    if (obj.GetComponent<FlowEvent>().mode == triggerMode.e)
                    {
                        hint.set(obj.GetComponent<FlowEvent>().Hint.text);
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            obj.GetComponent<FlowEvent>().runEvent();
                        }
                    }
                }
                if (obj.GetComponent<PhysObject>() != null && enablePhysGrab)
                {
                    if (obj.GetComponent<PhysObject>().allowPhysGrab)
                    {
                        hint.set("Grab [G]");
                        if (Input.GetKeyDown(KeyCode.G))
                        {
                            grabObj = obj.GetComponent<PhysObject>();
                            grabObj.freeze();
                            grabbing = true;
                            return;
                        }
                    }
                }
            }
            else
            {
                if (!grabbing)
                    hint.clear();
            }
        }
        if (grabbing)
        {
            hint.set("Drop [G]\nThrow [F]");
            grabObj.transform.position = grip.position;
            if (Input.GetKeyDown(KeyCode.G))
            {
                grabObj.unfreeze();
                grabbing = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                grabObj.unfreeze();
                grabObj.shoot(look.forward * throwPower);
                grabbing = false;
                return;
            }
        }
    }
}
