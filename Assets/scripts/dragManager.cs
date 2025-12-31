using System;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class dragManager : MonoBehaviour
{
    public static dragManager instance;
    [Header("Engine Selection")]
    public GameObject selectedEngine;
    public int selectedEngineIndex;
    public TMP_Text selectedEngineName;
    public GameObject removeUnitButton;
    public GameObject[] disableBeforeStart;
    public GameObject[] showForWorld;
    public GameObject[] showForUI;
    public TMP_Text selectedEnginePersonnel;
    [Header("Engine Spawning")]
    public int[] activeEngines;
    public enginesSO eSO;
    public SettingsSO settings;
    public Transform dragDropParent;
    public GameObject enginePrefab; // Assigned in inspector
    public GameObject uiPrefab;     // Assigned in inspector 
    public engineScroller e;
    [Header("Materials")]
    public Material hoverMaterial;  // Material used to highlight engineHolder
    [Header("Commanders")]
    public CommanderData[] commanders;
    public float yOffset = 0.5f;    // Not used in this example, but you can adjust as needed.
    public TMP_Dropdown commanderDropdown;
    public event Action undoVis ;
    [Header("Time Manager")]
    public timeManager tM;

    public currentIncident cI;
    

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            instance.enabled = false;
            Destroy(instance.gameObject);
            instance = null;
        }
        instance = this;
        eSO.LoadData();
        HideUI();
    }

    public void CheckForUIAwake()
    {
        for (int i = 0; i < disableBeforeStart.Length; i++)
        {
            if(!disableBeforeStart[i].activeSelf)disableBeforeStart[i].SetActive(true);
        }
    }
    public void HideUI()
    {
        for (int i = 0; i < disableBeforeStart.Length; i++)
        {
            if(disableBeforeStart[i].activeSelf)disableBeforeStart[i].SetActive(false);
        }
    }

    void Start()
    {
        tM = timeManager.instance;
        cI = tM.currentIncident;
        int floorNum = cI.floorNum;
        cI.resetInfo();
        cI.floorNum = floorNum;
        for (int i = 0; i < eSO.engineNames.Length; i++)
        {
            spawnEngineButton(i);
        }
        engineScroller.instance.updateScroller();
        
    }
    
    IEnumerator SetSiblingAfterFrame(GameObject engine)
    {
        yield return null; // Wait for the next frame
        engine.transform.SetSiblingIndex(0); // Move it to the top
        engine.GetComponent<DraggableUI>().forceToTop = true; // Force it to the top
        Debug.Log("Engine moved to the top after frame.");
    }

    public void spawnEngineButton(int i, bool atTop = false, bool addToList = true)
    {
        if (addToList)
        {
            if (atTop)
            {
                cI.activeEngines.Insert(0, i);
                cI.engineHolderPositions.Insert(0, null);
                cI.engineCommanderInfo.Insert(0, CommandType.None);
                cI.engineTimes.Insert(0, DateTime.MinValue.ToShortTimeString());
                cI.engineUVal.Insert(0, -0.1f);
            }
            else
            {
                cI.activeEngines.Add(i);
                cI.engineHolderPositions.Add(null);
                cI.engineCommanderInfo.Add(CommandType.None);
                cI.engineTimes.Add(DateTime.MinValue.ToShortTimeString());
                cI.engineUVal.Add(-0.1f);
            }
            
        }
        else
        {
            cI.activeEngines[i] = i;
            cI.engineHolderPositions[i] = null;
            cI.engineCommanderInfo[i] = CommandType.None;
            cI.engineTimes[i] = DateTime.MinValue.ToShortTimeString();
            cI.engineUVal[i] = -0.1f;
        }
        
        
        GameObject engine = Instantiate(uiPrefab, transform);
        engine.transform.SetParent(dragDropParent);
        
        

        if (atTop)
        { 
            StartCoroutine(SetSiblingAfterFrame(engine));
        }
        
        engine.GetComponentInChildren<TMP_Text>().text = eSO.engineNames[i];
            
        DraggableUI draggableUI = engine.GetComponent<DraggableUI>();
        draggableUI.enginesSo = eSO;
        draggableUI.SOindex = i;
        draggableUI.company = eSO.enginePersonel[i];
        if(atTop) e.AddToArray(engine, true);
        else e.AddToArray(engine);
    }
    
    /// <summary>
    /// Removes the currently selected engine from the world
    /// and re-creates its UI button at the top of the list.
    /// </summary>
    public void ReturnSelectedEngineToUI()
    {
        if (selectedEngine == null)
        {
            Debug.LogWarning("No selected engine to return");
            return;
        }
        var woi = selectedEngine.GetComponent<Engine>();
        ReturnEngineToUI(woi);
        selectedEngine = null;
        selectedEngineIndex = -1;
        updateUI();
    }

    public void ReturnEngineToUI(Engine woi)
    {
        // 1) Clear any existing command roles
        woi.ClearCommandStatus();

        // 2) Update holder counts & visuals
        var holder = woi.eH;
        holder.companyNum -= woi.company;
        holder.updateCompanyVis();

        // 3) Unsubscribe its undo-vis delegate
        undoVis -= woi.undoVis;

        // 4) Log the removal
        int idx = woi.SOindex;
        var cI = tM.currentIncident;
        int incidentIdx = cI.activeEngines.IndexOf(idx);
        cI.addInfo($"{eSO.engineNames[idx]} removed from {holder.areaName}");
        cI.engineHolderPositions[incidentIdx] = null;
        cI.engineTimes[incidentIdx] = DateTime.MinValue.ToShortTimeString();

        // 5) Destroy the world object
        Destroy(woi.gameObject);

        // 6) Spawn a fresh UI button for this engine
        spawnEngineButton(idx, /* atTop: */ true, false);
        updateUI();
    }

    
    
    public void deSelect()
    {
        if(selectedEngine != null) selectedEngine.GetComponent<Engine>().undoVis();
    }

    void ShowUIUI()
    {
        for (int i = 0; i < showForWorld.Length; i++)
        {
            showForWorld[i].SetActive(false);
        }
        for (int i = 0; i < showForUI.Length; i++)
        {
            showForUI[i].SetActive(true);
        }
        // Ensure selectedEngineIndex is within bounds
        if (selectedEngineIndex >= 0 && selectedEngineIndex < eSO.engineNames.Length)
        {
            selectedEngineName.text = eSO.engineNames[selectedEngineIndex];
        }
        else
        {
            Debug.LogWarning("selectedEngineIndex is out of range! exiting updateUI() function.");
            return; // Exit the function to prevent further errors
        }

        selectedEnginePersonnel.text = eSO.enginePersonel[selectedEngineIndex].ToString();
    }

    private void ShowWorldUI()
    {
        for (int i = 0; i < showForUI.Length; i++)
        {
            showForUI[i].SetActive(false);
        }
        for (int i = 0; i < showForWorld.Length; i++)
        {
            showForWorld[i].SetActive(true);
        }
        // Ensure selectedEngineIndex is within bounds
        if (selectedEngineIndex >= 0 && selectedEngineIndex < eSO.engineNames.Length)
        {
            selectedEngineName.text = eSO.engineNames[selectedEngineIndex];
        }
        else
        {
            Debug.LogWarning("selectedEngineIndex is out of range! exiting updateUI() function.");
            return; // Exit the function to prevent further errors
        }

        selectedEnginePersonnel.text = eSO.enginePersonel[selectedEngineIndex].ToString();
        
    }

    public void updateUI()
    {

            //Debug.Log($"Updating UI for engine {eSO.engineNames[selectedEngineIndex]} - {selectedEngineIndex}");
            CheckForUIAwake();
            if (selectedEngine != null)
            {
                if(!commanderDropdown.transform.parent.gameObject.activeInHierarchy) commanderDropdown.transform.parent.gameObject.SetActive(true);
                Engine eI = selectedEngine.GetComponent<Engine>();
                if(eI!=null) {
                    ShowWorldUI();
                }
                else 
                {
                    HideUI();
                }
                //Debug.Log($"Getting value for {eI.commandType} - {GetCommandIndex(eI.commandType)} - {GetCommandType(GetCommandIndex(eI.commandType))}");
                commanderDropdown.value = GetCommandIndex(eI.commandType);

            }
            else if (selectedEngineIndex >= 0)
            {
                ShowUIUI();
            }
            else
            {
                HideUI();
            }
            
            

            
    }
    
    
    
    public void SetCommanderArea() => SetSelectedCommander(CommandType.Area);
    public void ResetCommand() => SetSelectedCommander(CommandType.None);
    public void SetSafetyOfficer() => SetSelectedCommander(CommandType.Safety);
    public void SetIncidentCommander() => SetSelectedCommander(CommandType.Incident);
    
    
    
    public void clearCommand()
    {
        Engine s = selectedEngine.GetComponent<Engine>();
        for (int i = 0; i < commanders.Length; i++)
        {
            if(commanders[i].commander == s) commanders[i].commander = null;
        }
        if(s.commandType != CommandType.None) cI.addInfo($"{eSO.engineNames[s.SOindex]} is no longer {settings.GetCommandingTitle(s.commandType)}");
        s.ClearCommandStatus();
        updateUI();
    }
    
    public void SetSelectedCommander(CommandType type)
    {
        if(type == CommandType.None) return;
        if (type == CommandType.Area)
        {
            Engine s = selectedEngine.GetComponent<Engine>();
            ReplaceCommand(selectedEngine.GetComponent<Engine>(), CommandType.Area);
            //s.eH.SetCommander(selectedEngine);
        }
        else
        {
            ReplaceCommand(selectedEngine.GetComponent<Engine>(), type);
        }
        
        
    }

    public void ReplaceCommand(Engine commander, CommandType type, bool hideLog = false)
    {

        int newIncidentIndex = -1;
        if(commander) newIncidentIndex = cI.activeEngines.IndexOf(commander.SOindex);
        int oldIncidentIndex = newIncidentIndex;
        if (type == CommandType.Area)
        {
            commander.SetCommand(type);
            cI.engineCommanderInfo[newIncidentIndex] = type;
            updateUI();
            return;
        }
        List<CommandType> replacableCommands = new List<CommandType>() { CommandType.Incident, CommandType.Safety, CommandType.Supervisor,  };
        for (int i = 0; i < commanders.Length; i++)
        {
            if (commanders[i].type == type && replacableCommands.Contains(type))
            {
                if (commanders[i].commander) oldIncidentIndex = cI.activeEngines.IndexOf(commanders[i].commander.SOindex);
                if (commanders[i].commander && commanders[i].commander != commander)
                {
                    commanders[i].commander.ClearCommandStatus();
                    cI.engineCommanderInfo[oldIncidentIndex] = CommandType.None;
                    if (!hideLog)
                    {
                        if (commanders[i].type == CommandType.Area) cI.addInfo($"{eSO.engineNames[commanders[i].commander.SOindex]} is no longer commanding {commanders[i].commander.currentArea}");
                        else cI.addInfo($"{eSO.engineNames[commanders[i].commander.SOindex]} is no longer {settings.GetCommandingTitle(commanders[i].type)}");
                    }
                   
                }
                commanders[i].commander = commander;
                
            }
        }
        if(commander)cI.engineCommanderInfo[newIncidentIndex] = type;
        if (!hideLog && commander != null)
        {
            cI.addInfo($"{eSO.engineNames[commander.SOindex]} is now {settings.GetCommandingTitle(type)}");
        }
        if(commander.commandType != CommandType.None) commander.ClearCommandStatus();
        commander.SetCommand(type);
        updateUI();
        
    }

    public void SetCommand()
    {
        int commandIndex = commanderDropdown.value;
        if(commandIndex == GetCommandIndex(selectedEngine.GetComponent<Engine>().commandType)) return;
        switch (commandIndex)
        {
            case 0:
                clearCommand();
                break;
            case 1:
                SetSelectedCommander(CommandType.Area);
                Debug.Log($"Setting command type {CommandType.Area}");
                break;
            case 2:
                SetSelectedCommander(CommandType.Incident);
                Debug.Log($"Setting command type {CommandType.Incident}");
                break;
            case 3:
                SetSelectedCommander(CommandType.Safety);
                Debug.Log($"Setting command type {CommandType.Safety}");
                break;
            case 4:
                SetSelectedCommander(CommandType.Search);
                Debug.Log($"Setting command type {CommandType.Search}");
                break;
            case 5:
                SetSelectedCommander(CommandType.Fire);
                Debug.Log($"Setting command type {CommandType.Fire}");
                break;
            case 6:
                SetSelectedCommander(CommandType.RIT);
                Debug.Log($"Setting command type {CommandType.RIT}");
                break;
            case 7:
                SetSelectedCommander(CommandType.Vent);
                Debug.Log($"Setting command type {CommandType.Vent}");
                break;
            default:
                SetSelectedCommander(CommandType.Supervisor);
                Debug.Log($"Setting command type {CommandType.Supervisor}");
                break;
        }
    }

    public CommandType GetCommandType(int index)
    {
        switch (index)
        {
            case 0:
                Debug.Log($"Getting command type {CommandType.None}");
                return (CommandType.None);
                Debug.Log($"Getting command type {CommandType.None}");
                break;
            case 1:
                Debug.Log($"Getting command type {CommandType.Area}");
                return (CommandType.Area);
                break;
            case 2:
                Debug.Log($"Getting command type {CommandType.Incident}");
                return (CommandType.Incident);
                break;
            case 3:
                Debug.Log($"Getting command type {CommandType.Safety}");
                return (CommandType.Safety);
                break;
            case 4:
                Debug.Log($"Getting command type {CommandType.Search}");
                return (CommandType.Search);
                break;
            case 5:
                Debug.Log($"Getting command type {CommandType.Fire}");
                return (CommandType.Fire);
                break;
            case 6:
                Debug.Log($"Getting command type {CommandType.RIT}");
                return (CommandType.RIT);
                break;
            case 7:
                Debug.Log($"Getting command type {CommandType.Vent}");
                return (CommandType.Vent);
                break;
            default:
                Debug.Log($"Getting command type {CommandType.Supervisor}");
                return (CommandType.Supervisor);
        }
    }

    public int GetCommandIndex(CommandType commandType)
    {
        switch (commandType)
        {
            case CommandType.None:
                return 0;
            case CommandType.Area:
                return 1;
            case CommandType.Incident:
                return 2;
            case CommandType.Safety:
                return 3;
            case CommandType.Search:
                return 4;
            case CommandType.Fire:
                return 5;
            case CommandType.RIT:
                return 6;
            case CommandType.Vent:
                return 7;
            case CommandType.Supervisor:
                return 8;
            default:
                return 0;
        }
    }
}




[System.Serializable]
public class CommanderData
{
    public CommandType type;
    public Engine commander;
}