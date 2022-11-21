using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ioFileSrc;

[System.Serializable]
public class speechLine
{
	public string line = "";
	public float duration = 3;
}

[System.Serializable]
public class objectToggle
{
	public GameObject[] objects;
	public bool state = false;
	
	public void run()
	{
		foreach (GameObject g in objects)
		{
			g.SetActive(state);
		}
	}
}

[System.Serializable]
public class task
{
    public string info = "";
    public bool displayObjective = true;
    [Space(10)]
    public objectToggle[] objectToggles;
    [Space(10)]
    public FlowEvent[] events;
    public bool autoRunEvents = false;
	public bool UseEventCompletionCount = false;
    [Space(10)]
    public bool isSpeech = false;
    public speechLine[] speechLines;

	private int eventsCompleted = 0;
    private bool completed = false;
	private FlowManager flowManager;

    public void runEvents()
    {
        if (!completed)
        {
			if (events.Length > 0)
			{
				foreach (FlowEvent e in events)
				{
					e.runEvent();
				}
			}
			completed = true;
        }
    }

	public void completeEvent()
    {
		eventsCompleted++;
		if (flowManager == null)
			flowManager = Object.FindObjectOfType<FlowManager>();
    }

	public bool allEventsCompleted()
    {
		return eventsCompleted == events.Length;
    }
}

[System.Serializable]
public class indexedBool
{
	public int index;
	public bool boolean;

	public indexedBool(int i, bool b)
    {
		index = i;
		boolean = b;
    }
}

public class FlowManager : MonoBehaviour
{
	
	public string level = "";
    public bool skipSpeeches = false;
    [Space(10)]
    public PlayerController player;
    public GameObject mainCamera;
    public GameObject[] speechCameras;
	[Space(10)]
	public InventoryManager inventory;
	public HealthManager playerHealth;
    [Space(10)]
    public task[] tasks;
    public int taskIndex = -1;
	public bool displayCounter = false;
    [Space(10)]
    public Text txtTask;
    public Text txtSpeech;
	[Space(10)]
	public string var1 = "";
	[Space(10)]
	public bool saveOnStart = false;
	public bool saveOnNextTask = false;
	public bool saveSpecial = true;
	public GameObject[] anchorObjects;

	private List<indexedBool> ibs = new List<indexedBool>();
    private string VAR_FILENAME = "var.flow";
	
	private void Start()
	{
		AnchorObject[] anchors = FindObjectsOfType(typeof(AnchorObject)) as AnchorObject[];
		List<GameObject> objs = new List<GameObject>();
		foreach (AnchorObject a in anchors)
        {
			objs.Add(a.gameObject);
        }
		anchorObjects = objs.ToArray();

		clearTask();
		loadVariables();
		if (tasks.Length > 0)
		{
			if (taskIndex == -1)
			{
				taskIndex = 0;
			}
			updateTask();
		}
		if (saveOnStart)
        {
			saveProgress();
		}
	}
	
	// -------------------------
	
	public void completeTask()
	{
		tasks[taskIndex].runEvents();
		nextTask();
	}
	
	public bool nextTask()
	{
		if (taskIndex + 1 >= tasks.Length)
		{
			clearTask();
			if (saveOnNextTask)
            {
				inventory.SaveInv();
				playerHealth.saveHealth();
			}
			return false;
		}
		taskIndex++;
		updateTask();
		return true;
	}

	public void clearTask()
	{
		txtTask.text = "";
		txtSpeech.text = "";
	}
	public void clearSpeech()
	{
		txtSpeech.text = "";
	}

	public void updateTask()
	{
		task t = tasks[taskIndex];

		if (t.displayObjective)
		{
			txtTask.text = t.info.Replace("<var1>", var1);
		}
		else
		{
			txtTask.text = "";
		}

		if (t.objectToggles.Length > 0)
		{
			foreach (objectToggle objT in t.objectToggles)
			{
				objT.run();
			}
		}
		
		if (t.events.Length > 0)
		{
			if (t.autoRunEvents)
			{
				foreach (FlowEvent fe in t.events)
                {
					fe.runEvent();
                }
			}
		}
		else
		{
			if (!t.isSpeech)
			{
				completeTask();
			}
		}
		
		if (t.isSpeech)
		{
			if (!skipSpeeches)
			{
				player.force_pause = true;
				mainCamera.SetActive(false);
				foreach (GameObject g in speechCameras)
				{
					g.SetActive(true);
				}
				clearTask();
				StartCoroutine(ISpeech());
			}
			else
			{
				Debug.Log("Speech Skipped (Task: " + taskIndex + ")");
				completeTask();
			}
		}
	}
	
	private IEnumerator ISpeech()
	{
		task t = tasks[taskIndex];
		for (int i = 0; i < t.speechLines.Length; i++)
		{
			txtSpeech.text = "\"" + t.speechLines[i].line.Replace("<var1>", var1) + "\"";
            yield return new WaitForSeconds(t.speechLines[i].duration);
		}
		player.force_pause = false;
		foreach (GameObject g in speechCameras)
		{
			g.SetActive(false);
		}
		mainCamera.SetActive(true);
		clearTask();
		completeTask();
	}
	
	public void reloadScene()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }	
	
	public bool currentTaskContainsEvent(FlowEvent fe)
    {
        bool b = false;
        if (tasks[taskIndex].events.Length > 0)
        {
            foreach (FlowEvent e in tasks[taskIndex].events)
            {
                if (e.Equals(fe))
                {
                    b = true;
                    break;
                }
            }
        }
        return b;
    }

    public bool isLastEventInTask(FlowEvent fe)
    {
        bool b = false;
        if ((tasks[taskIndex].events[tasks[taskIndex].events.Length - 1]).Equals(fe))
		{
			b = true;
		}
        return b;
    }
	
	// -------------------------
	
	private void loadVariables()
    {
        ioFile file = new ioFile(VAR_FILENAME);
        if (file.exists())
        {
            if (file.hasitem("var1"))
                var1 = file.getitemvalue("var1");
            else
                var1 = "<no var1>";

			if (saveSpecial)
            {
				player.gameObject.SetActive(false);
				float x = file.getitemvalueasfloat("player_pos_x");
				float y = file.getitemvalueasfloat("player_pos_y");
				float z = file.getitemvalueasfloat("player_pos_z");
				player.transform.position = new Vector3(x, y, z);

				x = 0;
				y = file.getitemvalueasfloat("player_rot_y");
				z = 0;
				player.transform.eulerAngles = new Vector3(x, y, z);
				player.gameObject.SetActive(true);

				if (anchorObjects.Length > 0)
                {
					for (int i = 0; i < anchorObjects.Length; i++)
                    {
						string name = anchorObjects[i].name;

						anchorObjects[i].SetActive(false);
						x = file.getitemvalueasfloat(name + "_pos_x");
						y = file.getitemvalueasfloat(name + "_pos_y");
						z = file.getitemvalueasfloat(name + "_pos_z");
						anchorObjects[i].transform.position = new Vector3(x, y, z);

						x = file.getitemvalueasfloat(name + "_rot_x");
						y = file.getitemvalueasfloat(name + "_rot_y");
						z = file.getitemvalueasfloat(name + "_rot_z");
						anchorObjects[i].transform.eulerAngles = new Vector3(x, y, z);

						bool active = file.getitemvalueasbool(name + "_active");
						anchorObjects[i].SetActive(active);
					}
                }
			}				
        }
        else
        {
            var1 = "<no var1>";
        }
    }

    public void saveProgress()
    {
        ioFile file = new ioFile(VAR_FILENAME);
        if (!file.exists())
            file.create();

		if (!file.hasitem("level"))
			file.additem("level", level);
		else
			file.updateitem("level", level);

		if (saveSpecial)
        {
			string x = "";

			x = "player_pos_x";
			if (!file.hasitem(x))
				file.additem(x, player.transform.position.x.ToString());
			else
				file.updateitem(x, player.transform.position.x.ToString());
			x = "player_pos_y";
			if (!file.hasitem(x))
				file.additem(x, player.transform.position.y.ToString());
			else
				file.updateitem(x, player.transform.position.y.ToString());
			x = "player_pos_z";
			if (!file.hasitem(x))
				file.additem(x, player.transform.position.z.ToString());
			else
				file.updateitem(x, player.transform.position.z.ToString());

			x = "player_rot_y";
			if (!file.hasitem(x))
				file.additem(x, player.transform.eulerAngles.y.ToString());
			else
				file.updateitem(x, player.transform.eulerAngles.y.ToString());

			x = "task_index";

			if (anchorObjects.Length > 0)
            {
				for (int i = 0; i < anchorObjects.Length; i++)
                {
					string name = anchorObjects[i].name;
					string val = "";

					x = name + "_pos_x";
					val = anchorObjects[i].transform.position.x.ToString();
					if (!file.hasitem(x))
						file.additem(x, val);
					else
						file.updateitem(x, val);
					x = name + "_pos_y";
					val = anchorObjects[i].transform.position.y.ToString();
					if (!file.hasitem(x))
						file.additem(x, val);
					else
						file.updateitem(x, val);
					x = name + "_pos_z";
					val = anchorObjects[i].transform.position.z.ToString();
					if (!file.hasitem(x))
						file.additem(x, val);
					else
						file.updateitem(x, val);

					x = name + "_rot_x";
					val = anchorObjects[i].transform.eulerAngles.x.ToString();
					if (!file.hasitem(x))
						file.additem(x, val);
					else
						file.updateitem(x, val);
					x = name + "_rot_y";
					val = anchorObjects[i].transform.eulerAngles.y.ToString();
					if (!file.hasitem(x))
						file.additem(x, val);
					else
						file.updateitem(x, val);
					x = name + "_rot_z";
					val = anchorObjects[i].transform.eulerAngles.z.ToString();
					if (!file.hasitem(x))
						file.additem(x, val);
					else
						file.updateitem(x, val);

					x = name + "_active";
					val = anchorObjects[i].activeSelf.ToString();
					if (!file.hasitem(x))
						file.additem(x, val);
					else
						file.updateitem(x, val);

				}
            }
		}
	}

	public void setVar1(string s)
    {
        var1 = s;
        ioFile file = new ioFile(VAR_FILENAME);
        if (!file.exists())
            file.create();
        if (file.hasitem("var1"))
            file.updateitem("var1", var1);
        else
            file.additem("var1", var1);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
	
}