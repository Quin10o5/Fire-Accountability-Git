using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CycleButton : MonoBehaviour
{
    public UtilitiesManager utilitiesManager;
    public string type = "Null";
    public TMP_Text titleText;
    public TMP_Text cycleText;
    public int index = 0;
     public SettingsSO settings;
    [FormerlySerializedAs("engines")] public enginesSO customSettings;
    private float startTime;
    private bool buttonPressed = false;
    private Button button;
    private Image image;

    public void Start()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.AddListener(() => CycleButtonClicked());
        titleText.text = type;
        cycleText.text = "";
        startTime = Time.time;
        InvokeRepeating("UpdateButtonColor", 0, 1);
    }

    private void UpdateButtonColor()
    {
        if (buttonPressed) return;
        int time = Mathf.RoundToInt(Time.time - startTime);
        if (time > customSettings.cycleSevereWarningTime * 60)
        {
            image.color = settings.cyclingButtonColors[(time % 2) + 1];
        }
        else if (time > customSettings.cycleWarningTime * 60)
        {
            image.color = settings.cyclingButtonColors[1];
        }
        
    }

    public void CycleButtonClicked()
    {
        utilitiesManager.HandleCycleButton(this);
        buttonPressed = true;
        image.color = settings.cyclingButtonColors[0];
    }

    public void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

}
