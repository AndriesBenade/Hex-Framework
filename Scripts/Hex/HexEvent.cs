using System.Collections;
using UnityEngine;
using HexRepository;

[AddComponentMenu("Hex Framework/Hex Event", 0)]
public class HexEvent : MonoBehaviour
{

    // Public Variables

    [Header("HEX EVENT ----------")]
    public bool enabled = true;
    public eTriggerEnum trigger;
    [Tooltip("Use <IGNORE> if you don't want a tag.")]
    public string triggerTag = "Player";
	public eEventModeEnum eventMode;
	[Space(10)]
	public bool enableDelay = false;
	public float delayDuration = 1;
	[Space(10)]
	[Header("--- PRE DELAY ------")]
	public eSceneLoader sceneLoader_PRE;
	public eObjectStateEvent[] objectState_PRE;
	public eAudioEvent[] audio_PRE;
	public eAnimationEvent[] animation_PRE;
	public eParticleEvent[] particle_PRE;
    public eScriptEvent[] script_PRE;
    [Header("--- POST DELAY -----")]
	public eSceneLoader sceneLoader_POST;
	public eObjectStateEvent[] objectState_POST;
	public eAudioEvent[] audio_POST;
	public eAnimationEvent[] animation_POST;
	public eParticleEvent[] particle_POST;
    public eScriptEvent[] script_POST;

    // Private Variables

    private bool finished;

    private void Start()
    {
        finished = false;
        if (trigger == eTriggerEnum.start)
            Run();
    }

    private void OnTriggerStay(Collider other)
    {
        switch (trigger)
        {
            case eTriggerEnum.any:
                preRun(other.tag);
                break;
            case eTriggerEnum.stay:
                preRun(other.tag);
                break;
            case eTriggerEnum.colliderE:
                if (Input.GetKeyDown(KeyCode.E))
                    preRun(other.tag);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (trigger)
        {
            case eTriggerEnum.any:
                preRun(other.tag);
                break;
            case eTriggerEnum.enter:
                preRun(other.tag);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (trigger)
        {
            case eTriggerEnum.any:
                preRun(other.tag);
                break;
            case eTriggerEnum.exit:
                preRun(other.tag);
                break;
        }
    }

    public void preRun(string stag)
    {
        if (finished)
            return;
        if (triggerTag == "<IGNORE>")
            Run();
        else
        {
            if (stag == triggerTag)
                Run();
        }
    }

    public void Run()
    {
        if (finished)
            return;
        StartCoroutine(IRun());
    }

    private IEnumerator IRun()
    {

        //  Pre Delay Events

        sceneLoader_PRE.Run();
        if (objectState_PRE.Length > 0)
            foreach (eObjectStateEvent x in objectState_PRE)
                x.Run();
        if (audio_PRE.Length > 0)
            foreach (eAudioEvent x in audio_PRE)
                x.Run();
        if (animation_PRE.Length > 0)
            foreach (eAnimationEvent x in animation_PRE)
                x.Run();
        if (particle_PRE.Length > 0)
            foreach (eParticleEvent x in particle_PRE)
                x.Run();
        if (script_PRE.Length > 0)
            foreach (eScriptEvent x in script_PRE)
                x.Run();

        // Manage Delay

        if (enableDelay)
            yield return new WaitForSeconds(delayDuration);

        // Post Delay Events

        if (enableDelay)
        {
            sceneLoader_POST.Run();
            if (objectState_POST.Length > 0)
                foreach (eObjectStateEvent x in objectState_POST)
                    x.Run();
            if (audio_POST.Length > 0)
                foreach (eAudioEvent x in audio_POST)
                    x.Run();
            if (animation_POST.Length > 0)
                foreach (eAnimationEvent x in animation_POST)
                    x.Run();
            if (particle_POST.Length > 0)
                foreach (eParticleEvent x in particle_POST)
                    x.Run();
            if (script_POST.Length > 0)
                foreach (eScriptEvent x in script_POST)
                    x.Run();
        }

        FinishRun();
        yield return null;

    }

    private void FinishRun()
    {
        switch (eventMode)
        {
            case eEventModeEnum.infinite:
                finished = false;
                break;
            case eEventModeEnum.once:
                finished = true;
                break;
            case eEventModeEnum.disable:
                gameObject.SetActive(false);
                break;
            case eEventModeEnum.destroy:
                Destroy(gameObject);
                break;
        }
    }

}