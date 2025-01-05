using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]

[CreateAssetMenu(fileName = "currentIncident", menuName = "Scriptables/Current Incident")]
public class currentIncident : ScriptableObject
{
    public string startTime;
    public List<string> info;
    public List<int> activeEngines;
    public List<string> engineHolderPositions;
    public List<string> engineCommanderInfo;
    public List<string> engineTimes;
    public List<float> engineUVal;
    
    public void addInfo(string i)
    {
        this.info.Add($"{DateTime.Now.ToLongTimeString()}: {i}\n");
        if (timeManager.instance.logText != null)
        {
            timeManager.instance.logText.text = $"{DateTime.Now.ToLongTimeString()}: {i}";
        }
    }
    public void resetInfo()
    {
        startTime = null;
        activeEngines.Clear();
        engineHolderPositions.Clear();
        engineCommanderInfo.Clear();
        engineTimes.Clear();
        engineUVal.Clear();
        info.Clear();
    }

    
    
}
