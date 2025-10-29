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
    public GameObject[] selectedEnginesButtons;
    public TMP_Text selectedEnginePersonnel;
    [Header("Engine Spawning")]
    public int[] activeEngines;
    public enginesSO eSO;
    public Transform dragDropParent;
    public GameObject enginePrefab; // Assigned in inspector
    public GameObject uiPrefab;     // Assigned in inspector 
    public engineScroller e;
    [Header("Materials")]
    public Material hoverMaterial;  // Material used to highlight engineHolder
    public Material engineInteriorMaterial;
    public Material selectedEngineInteriorMaterial;
    public Material baseInteriorMaterial;
    public Material engineInteriorCommanderMaterial;
    public Material engineSafetyOfficerMaterial;
    public Material engineAreaCommanderMaterial;
    [Header("Commanders")]
    public GameObject interiorCommander = null;
    public GameObject safetyOfficer = null;
    public float yOffset = 0.5f;    // Not used in this example, but you can adjust as needed.
    public delegate void simpleEvent();
    public event Action undoVis ;
    [Header("Time Manager")]
    public timeManager tM;

    private currentIncident cI;

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
    }

    void Start()
    {
        tM = timeManager.instance;
        cI = tM.currentIncident;
        cI.resetInfo();
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
                cI.engineCommanderInfo.Insert(0, null);
                cI.engineTimes.Insert(0, DateTime.MinValue.ToShortTimeString());
                cI.engineUVal.Insert(0, -0.1f);
            }
            else
            {
                cI.activeEngines.Add(i);
                cI.engineHolderPositions.Add(null);
                cI.engineCommanderInfo.Add(null);
                cI.engineTimes.Add(DateTime.MinValue.ToShortTimeString());
                cI.engineUVal.Add(-0.1f);
            }
            
        }
        else
        {
            cI.activeEngines[i] = i;
            cI.engineHolderPositions[i] = null;
            cI.engineCommanderInfo[i] = null;
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
        draggableUI.enginesSO = eSO;
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
        var woi = selectedEngine.GetComponent<WorldObjectInteract>();
        ReturnEngineToUI(woi);
        selectedEngine = null;
        selectedEngineIndex = -1;
        updateUI();
    }

    public void ReturnEngineToUI(WorldObjectInteract woi)
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
        if (undoVis != null)
        {
            undoVis.Invoke();
        }
    }

    public void updateUI()
    {
       
            foreach (GameObject engine in selectedEnginesButtons)
            {
                engine.SetActive(false);
            }

            if (selectedEngine != null)
            {
                WorldObjectInteract eI = selectedEngine.GetComponent<WorldObjectInteract>();
                if(eI!=null) {
                    removeUnitButton.SetActive(true);
                }
                else
                {
                    removeUnitButton.SetActive(false);
                }
                Material mat1 = eI.visRenderer.material;
                Material mat2 = selectedEngineInteriorMaterial;

                if (mat1.color == mat2.color)
                {
                    selectedEnginesButtons[0].SetActive(true);
                    selectedEnginesButtons[1].SetActive(true);
                    selectedEnginesButtons[2].SetActive(true);
                }
                else if (mat1.color == engineInteriorCommanderMaterial.color)
                {
                    selectedEnginesButtons[4].SetActive(true);
                    selectedEnginesButtons[6].SetActive(true);
                }
                else if (mat1.color == engineSafetyOfficerMaterial.color)
                {
                    selectedEnginesButtons[5].SetActive(true);
                    selectedEnginesButtons[6].SetActive(true);
                }
                else if (mat1.color != mat2.color && mat1.color != engineInteriorMaterial.color)
                {
                    selectedEnginesButtons[3].SetActive(true);
                    selectedEnginesButtons[6].SetActive(true);
                }
            }
            else
            {
                removeUnitButton.SetActive(false);
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
    
    
    public void clearCommand()
    {
        WorldObjectInteract s = selectedEngine.GetComponent<WorldObjectInteract>();

        s.ClearCommandStatus();
        
        
    }

    public void setCommander()
    {
        WorldObjectInteract s = selectedEngine.GetComponent<WorldObjectInteract>();

        if (s.eH.currentCommander != null)
        {
            WorldObjectInteract eI = s.eH.currentCommander.GetComponent<WorldObjectInteract>();
            eI.baseMat = engineInteriorMaterial;
            eI.selectedMat = selectedEngineInteriorMaterial;
            eI.visRenderer.material = eI.baseMat;
            cI.addInfo($"{eSO.engineNames[eI.SOindex]} is no longer commanding {s.currentArea}");
            int cIndex = cI.activeEngines.IndexOf(eI.SOindex);
            cI.engineCommanderInfo[cIndex] = null;
        }
        
        int cIndex2 = cI.activeEngines.IndexOf(s.SOindex);
        cI.engineCommanderInfo[cIndex2] = "Area";
        cI.addInfo($"{eSO.engineNames[s.SOindex]} was named {s.currentArea} Commander");
        s.eH.currentCommander = selectedEngine;
        s.eH.insideOutsideVis[0].material = s.eH.insideOutsideMaterials[2];
        s.baseMat = s.eH.insideOutsideMaterials[2];
        s.selectedMat = s.eH.insideOutsideMaterials[2];
        s.visRenderer.material = s.eH.insideOutsideMaterials[2];
        updateUI();
        
        
    }
    public void setInteriorCommander()
    {
        if (interiorCommander != null)
        {
            WorldObjectInteract eI = interiorCommander.GetComponent<WorldObjectInteract>();
            eI.baseMat = engineInteriorMaterial;
            eI.selectedMat = selectedEngineInteriorMaterial;
            eI.visRenderer.material = eI.baseMat;
            cI.addInfo($"{eSO.engineNames[eI.SOindex]} is no longer Incident Commander");
            int cIndex = cI.activeEngines.IndexOf(eI.SOindex);
            cI.engineCommanderInfo[cIndex] = null;
            interiorCommander = null;
        }
        

        WorldObjectInteract s = selectedEngine.GetComponent<WorldObjectInteract>();
        int cIndex2 = cI.activeEngines.IndexOf(s.SOindex);
        cI.engineCommanderInfo[cIndex2] = "Incident";
        cI.addInfo($"{eSO.engineNames[s.SOindex]} was named Incident Commander");
        s.baseMat = engineInteriorCommanderMaterial;
        s.selectedMat = engineInteriorCommanderMaterial;
        s.visRenderer.material = engineInteriorCommanderMaterial;
        interiorCommander = selectedEngine;
        updateUI();
        
    }
    public void setSafetyOfficer()
    {
        if (safetyOfficer != null)
        {
            WorldObjectInteract eI = safetyOfficer.GetComponent<WorldObjectInteract>();
            eI.baseMat = engineInteriorMaterial;
            eI.selectedMat = selectedEngineInteriorMaterial;
            eI.visRenderer.material = engineInteriorMaterial;
            
            cI.addInfo($"{eSO.engineNames[eI.SOindex]} is no longer Safety Chief Officer");
            int cIndex = cI.activeEngines.IndexOf(eI.SOindex);
            cI.engineCommanderInfo[cIndex] = null;
            safetyOfficer = null;
        }

        WorldObjectInteract s = selectedEngine.GetComponent<WorldObjectInteract>();
        int cIndex2 = cI.activeEngines.IndexOf(s.SOindex);
        cI.engineCommanderInfo[cIndex2] = "Safety";
        cI.addInfo($"{eSO.engineNames[s.SOindex]} was named Safety Chief Officer");
        s.baseMat = engineSafetyOfficerMaterial;
        s.selectedMat = engineSafetyOfficerMaterial;
        s.visRenderer.material = engineSafetyOfficerMaterial;
        
        
        safetyOfficer = selectedEngine;
        updateUI();
        
    }
}