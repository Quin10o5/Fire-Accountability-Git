using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class colorTimer : MonoBehaviour
{
    public int warningTimeMinutes;
    public int alertTimeMinutes;
    public int incidentIndex;
    private WorldObjectInteract wOI;
    public currentIncident cI;
    public DateTime startTime;
    public Material[] exteriorMaterials;
    public MeshRenderer vis;

    public string displayTimer;
    // Start is called before the first frame update
    public void setTime()
    {
        cI = timeManager.instance.currentIncident;
        wOI = GetComponent<WorldObjectInteract>();
        incidentIndex = cI.activeEngines.IndexOf(wOI.SOindex);
        if (DateTime.TryParse(cI.engineTimes[incidentIndex], out startTime))
        {
            Debug.Log($"Successfully parsed to: {startTime}");
        }
        else
        {
            Debug.Log("Failed to parse the time string.");
        }
    }
    
    
    // Update is called once per frame
    void Update()
    {
        TimeSpan elapsed = DateTime.UtcNow - startTime;

        // Format the TimeSpan into HH:MM:SS (or whatever you prefer)
        string minutes = elapsed.Minutes.ToString("00");
        string seconds = elapsed.Seconds.ToString("00");
        displayTimer = $"{minutes}:{seconds}";
        if (wOI.baseMat != exteriorMaterials[1] && int.Parse(minutes) >= alertTimeMinutes)
        {
            if(int.Parse(seconds)%2==0)
            {
                vis.material = exteriorMaterials[1];
            }
            else
            {
                vis.material = exteriorMaterials[0];

            }
        }
        
        else if (wOI.baseMat != exteriorMaterials[0] && int.Parse(minutes) >= warningTimeMinutes)
        {
            vis.material = exteriorMaterials[0];

        }
        else
        {
            vis.material = exteriorMaterials[2];
        }
        
    }
}
