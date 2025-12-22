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
    private Engine wOI;
    public currentIncident cI;
    public DateTime startTime;
    public Material[] exteriorMaterials;
    public MeshRenderer vis;

    public string displayTimer;
    // Start is called before the first frame update
    public void setTime()
    {
        
        
        
        string raw = cI.engineTimes[incidentIndex];

        // If it’s empty, null, or still the MinValue placeholder, force a reset
        if (string.IsNullOrEmpty(raw)
            || raw == DateTime.MinValue.ToShortTimeString()
            || !DateTime.TryParse(raw, out startTime))
        {
            startTime = DateTime.Now;
            cI.engineTimes[incidentIndex] = startTime.ToString();
        }
        else
        {
            Debug.Log($"Parsed engine start time: {startTime}");
        }
        
        
        
    }


    public void Start()
    {
        wOI = GetComponent<Engine>();
        cI = timeManager.instance.currentIncident;
        incidentIndex = wOI.SOindex;
        warningTimeMinutes = PlayerPrefs.GetInt("warningTimeMinutes", 10);
        alertTimeMinutes = PlayerPrefs.GetInt("alertTimeMinutes", 15);
        setTime();
    }
    
    
    // Update is called once per frame
    void Update()
    {
        TimeSpan elapsed = DateTime.Now - startTime;

        // Format the TimeSpan into HH:MM:SS (or whatever you prefer)
        string minutes = elapsed.Minutes.ToString("00");
        string seconds = elapsed.Seconds.ToString("00");
        displayTimer = $"{minutes}:{seconds}";
        if ( int.Parse(minutes) >= alertTimeMinutes)
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
        
        else if ( int.Parse(minutes) >= warningTimeMinutes)
        {
            vis.material = exteriorMaterials[0];

        }
        else
        {
            vis.material = exteriorMaterials[2];
        }
        
    }
}
