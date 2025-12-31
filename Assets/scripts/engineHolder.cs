using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class engineHolder : MonoBehaviour
{
    [Header("Incident Info")]
    public enginesSO settings;
    public string areaName = null;
    public TMP_Text areaText;
    public bool full = false;
    public GameObject currentCommander;
    
    [Header("Engine Positioning")]
    [Range(0.5f,2)] 
    public float sizeMultiplier = 1f;
    public Transform[] holderPositions;
     public GameObject[] heldEngines;
    
    [Header("Selection and Search Visualization")]
    public MeshRenderer selectionRenderer;


    public Color commandedColor;
    public float searchCompletion = 0;
    public float visUpdateTime = 1.5f;
    private float trueSearchCompletion = 0;

    [Header("Company Visualization")] 
    public int companyNum;
    public TMP_Text companyNumText;
    public GameObject[] littlePeople;
    public int[] littlePeopleOnAt;
    
    public Outline outline;
    
    // Start is called before the first frame update
    void Start()
    {
        if (areaText == null) return;
        areaText.text = areaName;
        heldEngines = new GameObject[holderPositions.Length];
}

    // Update is called once per frame
    // C#
    public void placeEngine(GameObject engine)
    {
        if (heldEngines == null || holderPositions == null) return; // Handle null issues
        updateCompanyVis();
        // Ensure arrays are consistent in size
        if (heldEngines.Length != holderPositions.Length)
        {
            Debug.LogError("Mismatch between holders and holderPositions lengths.");
            return;
        }

        for (int i = 0; i < heldEngines.Length; i++)
        {
            if (heldEngines[i] == null)
            {
                heldEngines[i] = engine;
                engine.transform.position = holderPositions[i].position;
                engine.transform.rotation = holderPositions[i].rotation;
                engine.transform.localScale = new Vector3( sizeMultiplier, sizeMultiplier, sizeMultiplier);
                engine.GetComponent<Engine>().index = i;
                
                // Exit if engine is placed, assuming one placement per call
                return;
            }
            
            if (i == heldEngines.Length - 2)
            {
                full = true;
            }
            
        }
        
        
        Debug.LogWarning("No null holders found to place the engine.");
    }

    public void RemoveEngine(Engine engine)
    {
        heldEngines[engine.index] = null;
    }


    public void updateCompanyVis()
    {
        if (companyNumText != null)
        {
            
            companyNumText.gameObject.SetActive(false);
            //Debug.Log("Company Num: " + companyNum);
            foreach (GameObject lP in littlePeople)
            {
                lP.SetActive(false);
            
            }

            if (companyNum >= littlePeopleOnAt[2])
            {
                littlePeople[2].SetActive(true);
                littlePeople[1].SetActive(true);
                littlePeople[0].SetActive(true);
                companyNumText.gameObject.SetActive(true);
            }
            else if (companyNum >= littlePeopleOnAt[1])
            {
                littlePeople[1].SetActive(true);
                littlePeople[0].SetActive(true);
                companyNumText.gameObject.SetActive(true);
            }
            else if (companyNum >= littlePeopleOnAt[0])
            {
                littlePeople[0].SetActive(true);
                companyNumText.gameObject.SetActive(true);
            }
            companyNumText.text = companyNum.ToString();
        }
    }

    public IEnumerator changeSearchVis(float newSearchCompletion)
    {
        searchCompletion = newSearchCompletion;
        float startTime = Time.time;
        float endTime = startTime + visUpdateTime;
        int outlineColorID = Shader.PropertyToID("_searchCompletion");
        while (Time.time < endTime)
        {
            float u = (Time.time - startTime) / visUpdateTime;
            trueSearchCompletion = Mathf.Lerp(trueSearchCompletion, searchCompletion, u);
            selectionRenderer.material.SetFloat(outlineColorID, (float)trueSearchCompletion);
            yield return null;
        }
        
        
    }
    
    
    public void SetCommander(GameObject commander)
    {

        currentCommander = commander;
        timeManager t = timeManager.instance;
        t.currentIncident.addInfo($"{settings.engineNames[currentCommander.GetComponent<Engine>().SOindex]} is now commanding {areaName}");
        SetOutline(commandedColor);
        
    }

    public void ClearCommander()
    {
        Debug.Log("Clearing commander");
            timeManager t = timeManager.instance;
        if(currentCommander)t.currentIncident.addInfo(
            $"{settings.engineNames[currentCommander.GetComponent<Engine>().SOindex]} is no longer commanding {areaName}");
        t.currentIncident.engineCommanderInfo[currentCommander.GetComponent<Engine>().SOindex] = CommandType.None;
        currentCommander = null;
        ClearOutline();
    }

    public void SetOutline(Color outlineColor)
    {
        if(!outline.enabled)outline.enabled = true;
        outline.OutlineColor = outlineColor;
    }

    public void ClearOutline()
    {
        outline.enabled = false;
    }
}
