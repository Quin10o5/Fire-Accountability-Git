using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class engineHolder : MonoBehaviour
{
    [Header("Incident Info")]
    public string areaName = null;
    public TMP_Text areaText;
    public bool full = false;
    public GameObject currentCommander;
    
    [Header("Engine Positioning")]
    [Range(0.5f,2)] 
    public float sizeMultiplier = 1f;
    public Transform[] holderPositions;
    public GameObject[] holders;
    
    [Header("Selection and Search Visualization")]
    public MeshRenderer[] insideOutsideVis;
    public Material[] insideOutsideMaterials;
    public int searchCompletion;

    [Header("Company Visualization")] 
    public int companyNum;
    public TMP_Text companyNumText;
    public GameObject[] littlePeople;
    public int[] littlePeopleOnAt;
    
    // Start is called before the first frame update
    void Start()
    {
        if (areaText == null) return;
        areaText.text = areaName;
    }

    // Update is called once per frame
    // C#
    public void placeEngine(GameObject engine)
    {
        if (holders == null || holderPositions == null) return; // Handle null issues
        updateCompanyVis();
        // Ensure arrays are consistent in size
        if (holders.Length != holderPositions.Length)
        {
            Debug.LogError("Mismatch between holders and holderPositions lengths.");
            return;
        }

        for (int i = 0; i < holders.Length; i++)
        {
            if (holders[i] == null)
            {
                holders[i] = engine;
                engine.transform.position = holderPositions[i].position;
                engine.transform.rotation = holderPositions[i].rotation;
                engine.transform.localScale = new Vector3( sizeMultiplier, sizeMultiplier, sizeMultiplier);
                engine.GetComponent<WorldObjectInteract>().index = i;
                
                // Exit if engine is placed, assuming one placement per call
                return;
            }
            
            if (i == holders.Length - 2)
            {
                full = true;
            }
            
        }
        
        
        Debug.LogWarning("No null holders found to place the engine.");
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
    
    public void setCommander(GameObject commander)
    {
        if (currentCommander != null)
        {
            WorldObjectInteract w = currentCommander.GetComponent<WorldObjectInteract>();
            w.visRenderer.material =
                dragManager.instance.engineInteriorMaterial;
            w.baseMat = dragManager.instance.engineInteriorMaterial;
        }
        
    }
}
