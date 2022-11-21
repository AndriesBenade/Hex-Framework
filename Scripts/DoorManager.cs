using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorManager : MonoBehaviour
{
    public string doorHint = "Door.";
    public string userTag = "Player";
    public DoorManager[] linkedDoors;
    [Space(5)]
    public bool requireKey = false;
    public string keyId = "key";
    public bool requireKeyEquipped = false;
    [Space(5)]
    public bool enableAi = false;
    public GameObject aiBlocker;
    [Space(5)]
    public Animator anim;
    public string trigger = "Activate";
    [Space(5)]
    public AudioSource soundPlayer;
    public AudioClip[] useSound;
    public AudioClip lockedSound;
    [Space(5)]
    public bool disableOnUse = false;
    public bool playSoundInCode = false;
    public bool lockOnStart = false;
    public bool openOnStart = false;
    [Space(5)]
    public bool isTeleporter = false;
    public Transform teleportDest;
    public GameObject teleportTarget;
    public FlowEvent[] onTeleport;

    private HintText hint;
    private bool locked = false;
    private bool open = false;
    private bool usable = false;
    private InventoryManager inv;

    public void Use()
    {
        if (locked)
        {
            if (requireKey)
            {
                if (inv.items[inv.GetItem(keyId)].obtained)
                {
                    if (requireKeyEquipped)
                    {
                        if (inv.index == inv.GetItem(keyId))
                        {
                            locked = false;
                        }
                    }
                    else
                    {
                        locked = false;
                    }
                }
                else
                {
                    PlaySound(lockedSound);
                    return;
                }
            }
            else
            {
                PlaySound(lockedSound);
                return;
            }
        }
        if (isTeleporter)
        {
            teleportTarget.transform.position = teleportDest.position;
            teleportTarget.transform.eulerAngles = teleportDest.eulerAngles;
            foreach (FlowEvent fe in onTeleport)
                fe.runEvent();
            return;
        }
        open = open ? false : true;
        if (linkedDoors.Length > 0)
        {
            foreach (DoorManager door in linkedDoors)
            {
                door.setOpen(open);
            }
        }
        anim.SetTrigger(trigger);
        if (playSoundInCode)
            PlaySound(useSound[Random.Range(0, useSound.Length)]);
        if (disableOnUse)
        {
            this.enabled = false;
            GetComponent<Collider>().enabled = false;
        }
    }

    public void Open()
    {
        if (!open)
            Use();
    }

    public void Close()
    {
        if (open)
            Use();
    }

    public void Lock()
    {
        if (locked)
            return;
        if (open)
        {
            Use();
        }
        if (enableAi)
            aiBlocker.SetActive(true);
        locked = true;
    }

    public void Unlock()
    {
        if (enableAi)
            aiBlocker.SetActive(false);
        locked = false;
    }

    private void Start()
    {
        if (requireKey)
            if (inv == null)
                inv = FindObjectOfType<InventoryManager>();
        if (hint == null)
            hint = FindObjectOfType<HintText>();
        if (lockOnStart)
            Lock();
        if (openOnStart)
            Open();
    }

    private void Update()
    {
        if (usable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (locked)
                {
                    PlaySound(lockedSound);
                }
                else
                {
                    Use();
                    PlaySound(useSound[Random.Range(0, useSound.Length)]);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(userTag))
        {
            usable = true;
            hint.set(doorHint);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(userTag))
        {
            usable = false;
            hint.clear();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (soundPlayer.isPlaying)
            soundPlayer.Stop();
        soundPlayer.clip = clip;
        soundPlayer.Play();
    }

    public void setOpen(bool isOpen)
    {
        open = isOpen;
    }

    public bool isOpen()
    {
        return open;
    }

    public bool isLocked()
    {
        return locked;
    }

}
