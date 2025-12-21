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
    [FormerlySerializedAs("engines")] public enginesSO settings;
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
        if (time > settings.cycleSevereWarningTime * 60)
        {
            image.color = settings.cyclingButtonColors[(time % 2) + 1];
        }
        else if (time > settings.cycleWarningTime * 60)
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
