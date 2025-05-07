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
    
    public void addInfo(string i, string IDENTIFIER = null)
    {
        if(IDENTIFIER == null){
            info.Add($"{DateTime.Now.ToLongTimeString()}: {i}\n");
        }
        else
        {
            info.Add($"{DateTime.Now.ToLongTimeString()}: ~{IDENTIFIER}~{i}\n");
        }
        if (timeManager.instance.logText != null)
        {
            timeManager.instance.logText.text = $"{DateTime.Now.ToLongTimeString()}: {i}";
        }
        
    }

    public void ClearNAGasNotes()
    {
        for (int i = info.Count - 1; i >= 0; i--)
        {
            if (info[i].Contains("~GNA~"))
            {
                info.RemoveAt(i);
            }
        }
    }

    public void ClearNAElectricalNotes()
    {
        for (int i = info.Count - 1; i >= 0; i--)
        {
            if (info[i].Contains("~ENA~"))
            {
                info.RemoveAt(i);
            }
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
