using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]

[CreateAssetMenu(fileName = "EnginesData", menuName = "Scriptables/Engines Data")]
public class enginesSO : ScriptableObject
{
    public string[] engineNames;
    public int[] enginePersonel;
 
    public void SaveData()
    {
        // "this" refers to the current ScriptableObject instance
        ES3.Save("enginesSO", this);
        Debug.Log("ScriptableObject saved!");
    }

    // 2) Load method
    public void LoadData()
    {
        // Load into a temporary instance
        var loadedData = ES3.Load<enginesSO>("enginesSO", defaultValue: this);

        // Overwrite fields on 'this' with loadedData’s fields
        this.engineNames = loadedData.engineNames;
        this.enginePersonel = loadedData.enginePersonel;

        Debug.Log("ScriptableObject loaded!");
    }
    
    
    
    
}
