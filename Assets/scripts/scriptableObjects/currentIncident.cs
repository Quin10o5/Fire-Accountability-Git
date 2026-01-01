using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[System.Serializable]

[CreateAssetMenu(fileName = "currentIncident", menuName = "Scriptables/Current Incident")]
public class currentIncident : ScriptableObject
{
    public bool inMayday = false;
    public int floorNum;
    public string startTime;
    public List<string> info;
    public List<int> activeEngines;
    public List<string> engineHolderPositions;
    public List<CommandType> engineCommanderInfo;
    public List<string> engineTimes;
    public List<float> engineUVal;
    public List<bool> missing;
    
    public void addInfo(string i, string IDENTIFIER = null)
    {
        if(IDENTIFIER == null){
            info.Add($"{DateTime.Now.ToLongTimeString()}: {i}\n");
            Debug.Log($"{DateTime.Now.ToLongTimeString()}: {i}\n");
        }
        else
        {
            info.Add($"{DateTime.Now.ToLongTimeString()}: ~{IDENTIFIER}~ {i}\n");
        }
        if (timeManager.instance.logText != null)
        {
            timeManager.instance.logText.text = $"{DateTime.Now.ToLongTimeString()}: {i}";
        }
        
    }
    

    public void CLearNoteType(string identifier)
    {
        for (int i = info.Count - 1; i >= 0; i--)
        {
            if (info[i].Contains($"~{identifier}~"))
            {
                info.RemoveAt(i);
            }
        }
    }

    public string[] GetAllOfNoteType(string identifier)
    {
        List<string> list = new List<string>();
        for (int i = info.Count - 1; i >= 0; i--)
        {
            if (info[i].Contains($"~{identifier}~"))
            {
                list.Add(info[i]);
            }
        }
        return list.ToArray();
    }

    public void ResetEngineTime(int index)
    {
        
    }

    public void MoveUnitPosition(int index, string newLocation)
    {
        engineHolderPositions[index] = newLocation;
    }
    
    public void resetInfo()
    {
        floorNum = 1;
        startTime = null;
        activeEngines.Clear();
        engineHolderPositions.Clear();
        engineCommanderInfo.Clear();
        engineTimes.Clear();
        engineUVal.Clear();
        info.Clear();
        missing.Clear();
    }

    
    
}
