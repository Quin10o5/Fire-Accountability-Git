using System;
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
    [Header("Commanders")]
    public GameObject interiorCommander = null;
    public GameObject safetyOfficer = null;
    public float yOffset = 0.5f;    // Not used in this example, but you can adjust as needed.
    public delegate void simpleEvent();
    public static event Action undoVis ;
    [Header("Time Manager")]
    public timeManager tM;

    private currentIncident cI;

    void Awake()
    {
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

    public void spawnEngineButton(int i)
    {
        cI.activeEngines.Add(i);
        cI.engineHolderPositions.Add(null);
        cI.engineCommanderInfo.Add(null);
        cI.engineTimes.Add(DateTime.MinValue.ToShortTimeString());
        cI.engineUVal.Add(-0.1f);
        cI.addInfo($"{eSO.engineNames[i]} with {eSO.enginePersonel[i]} personnel arrived at scene");
        GameObject engine = Instantiate(uiPrefab, transform);
        engine.transform.SetParent(dragDropParent);
        engine.GetComponentInChildren<TMP_Text>().text = eSO.engineNames[i];
            
        DraggableUI draggableUI = engine.GetComponent<DraggableUI>();
        draggableUI.enginesSO = eSO;
        draggableUI.SOindex = i;
        draggableUI.company = eSO.enginePersonel[i];
        e.AddToArray(engine);
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
                }
                else if (mat1.color == engineSafetyOfficerMaterial.color)
                {
                    selectedEnginesButtons[5].SetActive(true);
                }
                else if (mat1.color != mat2.color && mat1.color != engineInteriorMaterial.color)
                {
                    selectedEnginesButtons[3].SetActive(true);
                }
            }

            // Ensure selectedEngineIndex is within bounds
            if (selectedEngineIndex >= 0 && selectedEngineIndex < eSO.engineNames.Length)
            {
                selectedEngineName.text = eSO.engineNames[selectedEngineIndex];
            }
            else
            {
                Debug.LogError("selectedEngineIndex is out of range!");
                return; // Exit the function to prevent further errors
            }

            selectedEnginePersonnel.text = eSO.enginePersonel[selectedEngineIndex].ToString();
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