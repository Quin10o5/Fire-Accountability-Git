using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;



[CreateAssetMenu(fileName = "EnginesData", menuName = "Scriptables/Engines Data")]
public class enginesSO : ScriptableObject
{
    
    
    [Header("Units")] public string[] engineNames;
    public int[] enginePersonel;
    public int engineWarningTime = 10;
    public int engineSevereWarningTime = 15;
    public int cycleWarningTime = 10;
    public int cycleSevereWarningTime = 15;

    public void SaveData()
    {
        // "this" refers to the current ScriptableObject instance
        ES3.Save("enginesSO", this); // leave old name for deprecation purposes
        Debug.Log("ScriptableObject saved!");
    }

    // 2) Load method
    public void LoadData()
    {
        // Load into a temporary instance
        var loadedData =
            ES3.Load<enginesSO>("enginesSO", defaultValue: this); // leave old name for deprecation purposes

        // Overwrite fields on 'this' with loadedData’s fields
        this.engineNames = loadedData.engineNames;
        this.enginePersonel = loadedData.enginePersonel;

        Debug.Log("ScriptableObject loaded!");
    }
}