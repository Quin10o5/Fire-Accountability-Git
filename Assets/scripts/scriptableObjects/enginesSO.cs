using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public enum CommandType
{
    None,
    Safety,
    Incident,
    Area
};
[CreateAssetMenu(fileName = "EnginesData", menuName = "Scriptables/Engines Data")]
public class enginesSO : ScriptableObject
{
    public CommandData[] commands;
    public Color selectedColor = Color.white;
    [Header("Units")]
    public string[] engineNames;
    public int[] enginePersonel;
    [Header("Warnings")]
    public Color[] cyclingButtonColors = new Color[] { Color.white, Color.yellow, Color.red};
    public Color[] engineWarningColors = new Color[] { Color.white, Color.yellow, Color.red};
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
        var loadedData = ES3.Load<enginesSO>("enginesSO", defaultValue: this); // leave old name for deprecation purposes

        // Overwrite fields on 'this' with loadedData’s fields
        this.engineNames = loadedData.engineNames;
        this.enginePersonel = loadedData.enginePersonel;

        Debug.Log("ScriptableObject loaded!");
    }

    public Color GetCommandingColor(CommandType commandType)
    {
        for (int i = 0; i < commands.Length; i++)
        {
            if (commands[i].type == commandType)
            {
                return commands[i].color;
            }
        }

        return Color.magenta;
    }
    public string GetCommandingTitle(CommandType commandType)
    {
        for (int i = 0; i < commands.Length; i++)
        {
            if (commands[i].type == commandType)
            {
                return commands[i].title;
            }
        }

        return $"{commandType} not found";
    }
    
    
    
}

[System.Serializable]
public class CommandData
{
    public CommandType type;
    public string title;
    public Color color;
}