using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class gameObjectOnOff
{

    public GameObject[] gameObjects;
    public bool state;
    public bool toggle = false;

    public void run()
    {
        foreach (GameObject g in gameObjects)
        {
            if (toggle)
                g.SetActive(g.activeSelf ? false : true);
            else
                g.SetActive(state);
        }
    }

}

[System.Serializable]
public class sceneLoad
{

    public bool enabled = false;
    public bool notSingle = false;
    public string sceneName = "";

    public void run()
    {
        if (enabled)
            SceneManager.LoadScene(sceneName, notSingle ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }

}

[System.Serializable]
public class sound
{
    public AudioSource audio;
    public AudioClip[] clip;

    public void run()
    {
        int r = Random.Range(0, clip.Length);
        if (audio.clip != clip[r])
        {
            if (audio.isPlaying)
                audio.Stop();
            audio.clip = clip[r];
        }
        audio.Play();
    }
}

[System.Serializable]
public class animation
{
    public Animator anim;
    public string setTrigger = "Activate";
    public string setBoolTrue = "";
    public string setBoolFalse = "";

    public void run()
    {
        if (setTrigger.Length > 0)
            anim.SetTrigger(setTrigger);
        if (setBoolTrue.Length > 0)
            anim.SetBool(setBoolTrue, true);
        if (setBoolFalse.Length > 0)
            anim.SetBool(setBoolFalse, false);
    }
}

[System.Serializable]
public class invCheck
{
    public string id = "";

    private InventoryManager inv;

    public bool run()
    {
        if (inv == null)
        {
            inv = Object.FindObjectOfType<InventoryManager>();
        }
        return inv.index == -1 ? false : inv.items[inv.index].id == id;
    }
}

[System.Serializable]
public class doorMod
{
    public DoorManager door;
    public bool applyOpen = false;
    public bool applyClose = false;
    public bool applyLock = false;
    public bool applyUnlock = false;

    public void run()
    {
        if (applyOpen)
            door.Open();
        if (applyClose)
            door.Close();
        if (applyLock)
            door.Lock();
        if (applyUnlock)
            door.Unlock();
    }
}

[System.Serializable]
public class lightMod
{

    public Light[] lights;
    public bool changeColor = false;
    public Color color = Color.white;
    public bool changeIntensity = false;
    public float intensity = 0;
    public bool changeRange = false;
    public float range = 0;

    public void run()
    {
        if (lights.Length == 0)
            return;
        foreach (Light l in lights)
        {
            if (changeColor)
                l.color = color;
            if (changeIntensity)
                l.intensity = intensity;
            if (changeRange)
                l.range = range;
        }
    }
}

[System.Serializable]
public class displayTextEvent
{
    [Tooltip("Use of things like \\n are supported.")]
    public string displayText = "";

    public void run()
    {
        if (Object.FindObjectOfType<DisplayText>() != null)
        {
            string s = displayText.Replace("\\n", "\n").Replace("\\t", "\t");
            Object.FindObjectOfType<DisplayText>().set(s);
        }
    }
}

[System.Serializable]
public class hint
{
    public bool enabled = false;
    public string text = "";
}

[System.Serializable]
public enum triggerMode
{
    any,
    enter,
    stay,
    exit,
    enterOrExit,
    e,
    q,
    leftclick,
    rightclick,
    none
}

public class FlowEvent : MonoBehaviour
{

    public triggerMode mode = triggerMode.any;
    public bool randomized = false;
    public int oneInX = 2;
    public string targetTag = "Player";
    public bool includeInFlow = false;
    public bool flowOrderMatters = false;
    public bool runOnStart = false;
    public bool unlimitedRuns = false;
    public bool ignoreTriggers = false;
    [Space(10)]
    [Header("--- Pre Events ---")]
    public bool disableSelf = false;
    public bool freezePlayerForEvent = false;
    public bool disableCameraForEvent = false;
    public invCheck[] inventoryChecksPRE;
    public gameObjectOnOff[] gameObjectOnOffsPRE;
    public animation[] animationPRE;
    public sound[] soundPRE;
    public lightMod[] lightsPRE;
    public doorMod[] doorPRE;
    [Space(10)]
    public float delay = 0f;
    [Space(10)]
    [Header("--- Post Events ---")]
    public sceneLoad sceneLoader;
    public invCheck[] inventoryChecksPOST;
    public gameObjectOnOff[] gameObjectOnOffsPOST;
    public animation[] animationPOST;
    public sound[] soundPOST;
    public lightMod[] lightsPOST;
    public doorMod[] doorPOST;
    [Space(10)]
    public bool enableDisplayText = false;
    public displayTextEvent displayText;
    [Space(10)]
    public bool standaloneSpeech = false;
    public speechLine[] speechLines;
    public bool speechFirst = false;
    [Space(10)]
    public hint Hint;

    private HintText hintText;
    private FlowManager flowManager;
    private bool contained = false;
    private bool started = false;
    private bool frozen = false;

    private void Start()
    {
        if (hintText == null)
            hintText = FindObjectOfType<HintText>();
        if (flowManager == null)
            flowManager = FindObjectOfType<FlowManager>();
        if (runOnStart)
            runEvent();
    }

    private void Update()
    {
        if (mode == triggerMode.e)
            if (Input.GetKeyDown(KeyCode.E) && (ignoreTriggers ? true : contained))
                runEvent();
        if (mode == triggerMode.q)
            if (Input.GetKeyDown(KeyCode.Q) && (ignoreTriggers ? true : contained))
                runEvent();
        if (mode == triggerMode.leftclick)
            if (Input.GetKeyDown(0) && (ignoreTriggers ? true : contained))
                runEvent();
        if (mode == triggerMode.rightclick)
            if (Input.GetMouseButtonDown(1) && (ignoreTriggers ? true : contained))
                runEvent();
    }

    public void runEvent()
    {
        if (!started || unlimitedRuns)
        {
            if (randomized)
            {
                if (Random.Range(0, oneInX) != 0)
                {
                    return;
                }
            }    
            StartCoroutine(IRunEvent());
        }
    }

    private IEnumerator IRunEvent()
    {
        started = true;
        if (freezePlayerForEvent && !flowManager.player.force_pause)
        {
            flowManager.player.force_pause = true;
            frozen = true;
        }
        if (disableCameraForEvent)
            flowManager.player.getCamera().SetActive(false);
        bool invContinue1 = true;
        foreach (invCheck x in inventoryChecksPRE)
        {
            if (!x.run())
            {
                invContinue1 = false;
                started = false;
                break;
            }
        }
        Debug.Log("continue 1? " + invContinue1);
        if (invContinue1)
        {
            if (standaloneSpeech && !flowManager.skipSpeeches && speechFirst)
            {
                flowManager.player.force_pause = true;
                flowManager.mainCamera.SetActive(false);
                hintText.clear();
                hintText.halt = true;
                foreach (GameObject g in flowManager.speechCameras)
                {
                    g.SetActive(true);
                }
                for (int i = 0; i < speechLines.Length; i++)
                {
                    flowManager.txtSpeech.text = "\"" + speechLines[i].line.Replace("<var1>", flowManager.var1) + "\"";
                    yield return new WaitForSeconds(speechLines[i].duration);
                }
                flowManager.player.force_pause = false;
                foreach (GameObject g in flowManager.speechCameras)
                {
                    g.SetActive(false);
                }
                hintText.halt = false;
                flowManager.mainCamera.SetActive(true);
                flowManager.clearSpeech();
            }
            foreach (gameObjectOnOff x in gameObjectOnOffsPRE)
            {
                x.run();
            }
            foreach (animation x in animationPRE)
            {
                x.run();
            }
            foreach (sound x in soundPRE)
            {
                x.run();
            }
            if (lightsPRE != null)
            {
                foreach (lightMod x in lightsPRE)
                {
                    x.run();
                }
            }
            foreach (doorMod x in doorPRE)
            {
                x.run();
            }    
            yield return new WaitForSeconds(delay);
            sceneLoader.run();
            bool invContinue2 = true;
            foreach (invCheck x in inventoryChecksPOST)
            {
                if (!x.run())
                {
                    invContinue2 = false;
                    started = false;
                    break;
                }
            }
            Debug.Log("continue 2? " + invContinue2);
            if (invContinue2)
            {
                foreach (gameObjectOnOff x in gameObjectOnOffsPOST)
                {
                    x.run();
                }
                foreach (animation x in animationPOST)
                {
                    x.run();
                }
                foreach (sound x in soundPOST)
                {
                    x.run();
                }
                if (lightsPOST != null)
                {
                    foreach (lightMod x in lightsPOST)
                    {
                        x.run();
                    }
                }
                foreach (doorMod x in doorPOST)
                {
                    x.run();
                }
                if (enableDisplayText)
                {
                    displayText.run();
                }
                if (standaloneSpeech && !flowManager.skipSpeeches && !speechFirst)
                {
                    flowManager.player.force_pause = true;
                    flowManager.mainCamera.SetActive(false);
                    hintText.clear();
                    hintText.halt = true;
                    foreach (GameObject g in flowManager.speechCameras)
                    {
                        g.SetActive(true);
                    }
                    for (int i = 0; i < speechLines.Length; i++)
                    {
                        flowManager.txtSpeech.text = "\"" + speechLines[i].line.Replace("<var1>", flowManager.var1) + "\"";
                        yield return new WaitForSeconds(speechLines[i].duration);
                    }
                    flowManager.player.force_pause = false;
                    foreach (GameObject g in flowManager.speechCameras)
                    {
                        g.SetActive(false);
                    }
                    hintText.halt = false;
                    flowManager.mainCamera.SetActive(true);
                    flowManager.clearSpeech();
                }
                if (freezePlayerForEvent && frozen)
                {
                    flowManager.player.force_pause = false;
                    frozen = false;
                }
                if (disableCameraForEvent)
                    flowManager.player.getCamera().SetActive(true);

                // ----- END OF ALL EVENTS -----
                if (includeInFlow)
                    if (flowManager.currentTaskContainsEvent(this))
                    {
                        if (flowManager.tasks[flowManager.taskIndex].UseEventCompletionCount)
                        {
                            flowManager.tasks[flowManager.taskIndex].completeEvent();
                            if (flowManager.tasks[flowManager.taskIndex].allEventsCompleted())
                            {
                                flowManager.completeTask();
                            }
                        }
                        else
                        {
                            if (flowManager.isLastEventInTask(this) || !flowOrderMatters)
                            {
                                Debug.Log("complete in flow");
                                flowManager.completeTask();
                            }
                        }
                    }
                if (disableSelf)
                    gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator ISpeech()
    {
        flowManager.player.force_pause = true;
        flowManager.mainCamera.SetActive(false);
        foreach (GameObject g in flowManager.speechCameras)
        {
            g.SetActive(true);
        }
        for (int i = 0; i < speechLines.Length; i++)
        {
            flowManager.txtSpeech.text = "\"" + speechLines[i].line.Replace("<var1>", flowManager.var1) + "\"";
            yield return new WaitForSeconds(speechLines[i].duration);
        }
        flowManager.player.force_pause = false;
        foreach (GameObject g in flowManager.speechCameras)
        {
            g.SetActive(false);
        }
        flowManager.mainCamera.SetActive(true);
        flowManager.clearSpeech();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            switch (mode)
            {
                case triggerMode.any:
                    runEvent();
                    break;
                case triggerMode.enter:
                    runEvent();
                    break;
                case triggerMode.enterOrExit:
                    runEvent();
                    break;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            switch (mode)
            {
                case triggerMode.any:
                    runEvent();
                    break;
                case triggerMode.stay:
                    runEvent();
                    break;
                case triggerMode.e:
                    contained = true;
                    break;
            }
        }
        if (Hint.enabled)
            hintText.set(Hint.text.Replace("<var1>", flowManager.var1));
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            switch (mode)
            {
                case triggerMode.any:
                    runEvent();
                    break;
                case triggerMode.exit:
                    runEvent();
                    break;
                case triggerMode.enterOrExit:
                    runEvent();
                    break;
            }
        }
        contained = false;
        if (Hint.enabled)
            hintText.clear();
    }

    private void OnDisable()
    {
        if (Hint.enabled)
            hintText.clear();
    }

}
