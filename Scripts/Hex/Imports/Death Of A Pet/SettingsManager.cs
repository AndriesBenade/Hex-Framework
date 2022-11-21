using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ioFileSrc;

public class SettingsManager : MonoBehaviour
{

    public Dropdown dropQuality;
    public Slider slideSensitivity;
    public bool isMainMenu = false;

    private PlayerController player;

    private string settingsFile = "settings.ini";
    private float sensitivity;

    private void Start()
    {
        if (player == null && !isMainMenu)
            player = FindObjectOfType<PlayerController>();
        LoadSettingsFile();
    }

    public void applySettings()
    {
        QualitySettings.SetQualityLevel(dropQuality.value, true);
        if (isMainMenu)
        {
            sensitivity = slideSensitivity.value;
        }
        else
        {
            player.sensitivityX = slideSensitivity.value;
            player.sensitivityY = slideSensitivity.value;
        }

        ioFile io = new ioFile(settingsFile);
        io.create();
        io.additem("quality", dropQuality.value.ToString());
        io.additem("sensitivity", slideSensitivity.value.ToString());
    }

    public void LoadSettingsUI()
    {
        // quality
        string[] names = QualitySettings.names;
        dropQuality.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < names.Length; i++)
        {
            options.Add(new Dropdown.OptionData(names[i]));
        }
        dropQuality.AddOptions(options);
        dropQuality.value = QualitySettings.GetQualityLevel();

        // look sensitivity
        if (isMainMenu)
        {
            slideSensitivity.value = sensitivity;
        }
        else
        {
            slideSensitivity.value = player.sensitivityX;
        }
    }

    private void LoadSettingsFile()
    {
        ioFile io = new ioFile(settingsFile);
        if (!io.exists())
        {
            LoadSettingsUI();
            applySettings();
        }
        else
        {
            QualitySettings.SetQualityLevel(io.getitemvalueasint("quality"), true);
            if (isMainMenu)
            {
                sensitivity = io.getitemvalueasfloat("sensitivity");
            }
            else
            {
                player.sensitivityX = io.getitemvalueasfloat("sensitivity");
                player.sensitivityY = io.getitemvalueasfloat("sensitivity");
            }
        }
    }

}
