using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HexRepository
{

    /*
        Code that is prefixed with "e" is specifically for the HexEvent script. 
    */

    // Enumerations
	
	[System.Serializable]
    public enum eTriggerEnum
    {
        none,
        start,
        any,
        stay,
        enter,
        exit,
        colliderE,
        raycastE
    }
	
	[System.Serializable]
    public enum boolModifierEnum
    {
        enabled,
		disabled,
		toggle
    }
	
	[System.Serializable]
    public enum eEventModeEnum
    {
        infinite,
		once,
		disable,
		destroy
    }
	
	[System.Serializable]
    public enum playerStateEnum
    {
        play,
		pause,
		stop
    }

    // Main Classes
	
	[System.Serializable]
	public class eSceneLoader
	{
		public bool enabled = false;
		public string sceneName;
		
		public void Run()
		{
			if (!enabled)
				return;
			try
			{
				SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
			}
			catch (Exception e)
			{
				Debug.LogError("Unable to load scene " + sceneName + "!");
			}
		}
	}
	
	[System.Serializable]
	public class eObjectStateEvent
	{
		public bool enabled = false;
		public GameObject[] objects;
		public boolModifierEnum state;
		
		public void Run()
		{
			if (!enabled)
				return;
			if (objects.Length > 0)
			{
				if (state != boolModifierEnum.toggle)
				{
					bool x = (state == boolModifierEnum.enabled) ? true : false;
					foreach (GameObject g in objects)
					{
						g.SetActive(x);
					}
				}
				else
				{
					foreach (GameObject g in objects)
					{
						g.SetActive(g.activeSelf ? false : true);
					}
				}
			}
		}
	}
	
	[System.Serializable]
	public class eAudioEvent
	{
		public bool enabled = false;
		public AudioSource[] audioPlayers;
		public AudioClip[] audioClips;
		public bool pickRandom = false;
		
		public void Run()
		{
			if (!enabled)
				return;
			if (audioPlayers.Length > 0)
			{
				if (audioClips.Length > 0)
				{
					foreach (AudioSource player in audioPlayers)
					{
						int index = 0;
						if (pickRandom && audioClips.Length > 1)
						{
							System.Random rand = new System.Random();
							index = rand.Next(0, audioClips.Length);
						}
						if (player.isPlaying)
						{
							player.Stop();
						}
						player.clip = audioClips[index];
						player.Play();
					}
				}
			}
		}
	}
	
	[System.Serializable]
	public class eAnimationEvent
	{
		public bool enabled = false;
		public Animator[] animators;
		[Space(5)]
		public string[] triggers;
		public boolContainer[] boolsCustom;
		public string[] boolsInvert;
		public intContainer[] ints;
		public floatContainer[] floats;
		
		public void Run()
		{
			if (!enabled)
				return;
			if (animators.Length > 0 && (triggers.Length > 0 || boolsCustom.Length > 0 || boolsInvert.Length > 0 || ints.Length > 0 || floats.Length > 0))
			{
				foreach (Animator animator in animators)
				{
					if (triggers.Length > 0)
						foreach (string x in triggers)
							animator.SetTrigger(x);
					if (boolsCustom.Length > 0)
						foreach (boolContainer x in boolsCustom)
							animator.SetBool(x.name, x.value);
					if (boolsInvert.Length > 0)
						foreach (string x in boolsInvert)
							animator.SetBool(x, (animator.GetBool(x) ? false : true));
					if (ints.Length > 0)
						foreach (intContainer x in ints)
							animator.SetInteger(x.name, x.value);	
					if (floats.Length > 0)
						foreach (floatContainer x in floats)
							animator.SetFloat(x.name, x.value);							
				}
			}
		}
	}
	
	[System.Serializable]
	public class eParticleEvent
	{
		public bool enabled = false;
		public ParticleSystem[] particles;
		public playerStateEnum state;
		
		public void Run()
		{
			if (!enabled)
				return;
			if (particles.Length > 0)
			{
				foreach (ParticleSystem particle in particles)
				{
					switch (state)
					{
						case playerStateEnum.play:
							particle.Play();
							break;
						case playerStateEnum.pause:
							particle.Pause();
							break;
						case playerStateEnum.stop:
							particle.Stop();
							break;
					}
				}
			}
		}
	}

	[System.Serializable]
	public class eScriptEvent
	{
		public bool enabled = false;
		public GameObject[] scriptHolders;
		public scriptCall[] scriptCalls;

		public void Run()
		{
			if (!enabled)
				return;
			if (scriptHolders.Length > 0 && scriptCalls.Length > 0)
			{
				foreach (GameObject holder in scriptHolders)
				{
					for (int i = 0; i < scriptCalls.Length;)
					{
						scriptCalls[i].Run(holder);
					}
				}
			}
		}
	}

	// Sub Classes

	[System.Serializable]
	public class scriptCall
	{
		public string scriptName;
		public string[] calls;
		
		public void Run(GameObject holder)
		{
			if (holder.GetComponent(scriptName) != null)
			{
				foreach (string call in calls)
				{
					try
					{
						holder.GetComponent(scriptName).SendMessage(call);
					}
					catch (Exception e)
					{
						Debug.LogError("Could not make call " + call + " in script " + scriptName + "!");
					}
				}
			}
		}
	}
	
	// Container Classes
	
	[System.Serializable]
	public class boolContainer
	{
		public string name;
		public bool value = false;
	}
	
	[System.Serializable]
	public class intContainer
	{
		public string name;
		public int value = 0;
	}
	
	[System.Serializable]
	public class floatContainer
	{
		public string name;
		public float value = 0;
	}

	// Static Classes

	public static class presentation
    {

		public static string getProgressbar(float rawPercent, int size = 12)
        {
			// ▪▫
			//▓▒
			char cEmpty = '▫';
			char cFull = '▪';
			string progressbar = "";
			int fill = Mathf.RoundToInt(size * rawPercent);
			if (fill > 0)
            {
				for (int i = 0; i < fill; i++)
				{
					progressbar += cFull;
				}
			}
			if (size - fill > 0)
            {
				for (int i = 0; i < (size - fill); i++)
				{
					progressbar += cEmpty;
				}
			}
			return progressbar;
		}

    }

}