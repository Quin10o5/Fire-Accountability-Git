using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class maydayManager : MonoBehaviour
{
    public GameObject loggingText;
    public bool calledManpower = false;
    public bool maydayActive;
    public Toggle calledManpowerToggle;
    public GameObject alertParent;
    public float blinkTime = 1;
    public TMP_Text timerText;
    public GameObject overlay;
    public GameObject maydayButton;
    public DateTime startTime;
    public GameObject customCheckboxPrefab;
    public Transform customCheckboxParent;
    private currentIncident cI;
    private TimeSpan elapsed;
    private bool foundVictims;
    
    // Start is called before the first frame update
    void beginMayday()
    {
        cI = timeManager.instance.currentIncident;
        enginesSO eSO = dragManager.instance.eSO;
        maydayActive = true;
        overlay.SetActive(true);
        maydayButton.GetComponentInChildren<TextMeshProUGUI>().text = "End Mayday";
        startTime = DateTime.UtcNow;
        StartCoroutine(BlinkAlert());
        cI.addInfo("Mayday begun!");
        loggingText.SetActive(false);
        for (int i = 0; i < customCheckboxParent.childCount; i++)
        {
            Destroy(customCheckboxParent.GetChild(i).gameObject);
        }
        for (int i = 0; i < eSO.maydayToggles.Length - 1; i++)
        {
            if(string.IsNullOrEmpty(eSO.maydayToggles[i])) continue;
            
            GameObject checkbox = GameObject.Instantiate(customCheckboxPrefab, customCheckboxParent);
            checkbox.GetComponent<CustomMaydayCheckBox>().title = eSO.maydayToggles[i];
        }
    }

    public IEnumerator BlinkAlert()
    {
        while (maydayActive)
        {
            yield return new WaitForSeconds(blinkTime);
            alertParent.SetActive(true);
            if (!calledManpower)
            {
                yield return new WaitForSeconds(blinkTime);
                alertParent.SetActive(false);
            }
            
            
        }
    }
    
    void endMayday()
    {
        loggingText.SetActive(true);
        maydayActive = false;
        overlay.SetActive(false);
        maydayButton.GetComponentInChildren<TextMeshProUGUI>().text = "Mayday";
        string hours = elapsed.Hours.ToString("00");
        string minutes = elapsed.Minutes.ToString("00");
        string seconds = elapsed.Seconds.ToString("00");
        cI.addInfo($"Mayday ended with a total time of {hours}:{minutes}:{seconds}");
        if(!foundVictims) cI.addInfo("No victims were found.");
        dragManager d = dragManager.instance;
        for (int i = 0; i < d.engines.Count; i++)
        {
            d.engines[i].ClearMissing();
        }
    }

    public void tryMayday()
    {
        if(maydayActive) endMayday();
        else beginMayday();
    }

    public void foundVictim(bool value)
    {
        foundVictims = value;
        if(!value) return;
        cI.addInfo("A victim was found.");
    }


    public void ToggleCalledManpower()
    {
        calledManpower = calledManpowerToggle.isOn;
        if (calledManpower)
        {
            cI.addInfo("Called manpower.");
        }
        else
        {
            cI.addInfo("Returned manpower.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsed = DateTime.UtcNow - startTime;

        // Format the TimeSpan into HH:MM:SS (or whatever you prefer)
        string hours = elapsed.Hours.ToString("00");
        string minutes = elapsed.Minutes.ToString("00");
        string seconds = elapsed.Seconds.ToString("00");

        // Update the UI text
        if (timerText != null && maydayActive && hours != "00")
        {
            timerText.text = $"{hours}:{minutes}:{seconds}";
        }
        else if (maydayActive)
        {
            timerText.text = $"{minutes}:{seconds}";
        }
    }
}
