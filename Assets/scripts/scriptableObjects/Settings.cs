using UnityEngine;
[System.Serializable]
public enum CommandType
{
    None,
    Safety,
    Incident,
    Area,
    Search,
    Fire,
    RIT,
    Vent,
    Supervisor
};
[CreateAssetMenu(fileName = "SettingsSO", menuName = "Scriptables/Settings")]
public class SettingsSO : ScriptableObject
{
    public CommandData[] commands;
    public Color selectedColor = Color.white;
    [Header("Warnings")] public Color[] cyclingButtonColors = new Color[] { Color.white, Color.yellow, Color.red };
    public Color[] engineWarningColors = new Color[] { Color.white, Color.yellow, Color.red };
    
    public Color GetCommandingColor(CommandType commandType){
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
    
    public string GetCommandingAcronym(CommandType commandType)
    {
        for (int i = 0; i < commands.Length; i++)
        {
            if (commands[i].type == commandType)
            {
                return commands[i].acronym;
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
    public string acronym;
    public Color color;
}
