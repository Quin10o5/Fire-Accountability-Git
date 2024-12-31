using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class popUpsManager : MonoBehaviour
{
    public static popUpsManager instance;
    public enginesSO eSO;
    public GameObject popUpParent;
    public TMP_Text header;
    public string[] pageHeaders;
    public GameObject[] pages;
    private int currentPage = -1;
    
    [Header("Personnel Edit")]
    public TMP_Text personnelName;
    public TMP_Text personnelNum;
    public GameObject subtractButton;
    
    [Header("Add Company")]
    public TMP_InputField companyName;
    public TMP_InputField companyPersonnelNum;
    private string companyNameString;
    private int companyPersonnelNumInt;
    private dragManager dM;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        
    }
    void Start()
    {
        dM = dragManager.instance;
    }

    // Update is called once per frame
    public void openPopUp(int pageIndex)
    {
        dM = dragManager.instance;
        if (pageIndex != currentPage)
        {
            currentPage = pageIndex;
            popUpParent.SetActive(true);
            for (int i = 0; i < pages.Length; i++)
            {
                if (i != pageIndex)
                {
                    pages[i].SetActive(false);
                }
                else
                {
                    pages[i].SetActive(true);
                    header.text = pageHeaders[i];

                    if (i == 2)
                    {
                        refreshPersonnelEdit(null);
                    }
                    
                    
                }
            }
        }
        else closePopUp();
    }
    public void closePopUp()
    {
        currentPage = -1;
        popUpParent.SetActive(false);
    }

    public void refreshPersonnelEdit(engineHolder e)
    {
        Debug.Log("Refreshing Personnel Edit");
        
        Debug.Log(dM.selectedEngineIndex);
        Debug.Log(eSO.engineNames[dM.selectedEngineIndex]);
        if (eSO.enginePersonel[dM.selectedEngineIndex] <= 1)
        {
            subtractButton.SetActive(false);
        }
        else subtractButton.SetActive(true);
        personnelName.text = eSO.engineNames[dM.selectedEngineIndex];
        personnelNum.text = "Personnel: " + eSO.enginePersonel[dM.selectedEngineIndex].ToString();
        if (e != null)
        {
            e.updateCompanyVis();
        }
        dM.updateUI();
        
    }
    public void addPersonnel()
    { 
        eSO.enginePersonel[dM.selectedEngineIndex] += 1;
        engineHolder e = null;
        if (dM.selectedEngine != null)
        {
            WorldObjectInteract w = dM.selectedEngine.GetComponent<WorldObjectInteract>();
            w.company += 1;
            e = w.eH;
        }
        if (e != null)
        {
            e.companyNum += 1;
        }
        refreshPersonnelEdit(e);
        
    }
    public void subtractPersonnel()
    {
        eSO.enginePersonel[dM.selectedEngineIndex] -= 1;
        engineHolder e = null;
        if (dM.selectedEngine != null)
        {
            WorldObjectInteract w = dM.selectedEngine.GetComponent<WorldObjectInteract>();
            w.company -= 1;
            e = w.eH;
        }
        if (e != null)
        {
            e.companyNum -= 1;
        }
        refreshPersonnelEdit(e); 

    }


    public bool confirmNum()
    {
        companyPersonnelNumInt = int.Parse(companyPersonnelNum.text);
        if(companyPersonnelNumInt > 0) return true;
        return false;
    }
    public bool confirmName()
    {
        companyNameString = companyName.text;
        if(companyNameString != null) return true;
        return false;
    }

    public void confirmCompany()
    {
        if (confirmName() && confirmNum())
        {
            Debug.Log("confirmed company");
            List<string> eNames = eSO.engineNames.ToList();
            List<int> ePersonnel = eSO.enginePersonel.ToList();
            eNames.Add(companyNameString);
            ePersonnel.Add(companyPersonnelNumInt);
            eSO.engineNames = eNames.ToArray();
            eSO.enginePersonel = ePersonnel.ToArray();
            List<int> aEngines = dM.activeEngines.ToList();
            aEngines.Add(eSO.engineNames.Length - 1);
            dM.spawnEngineButtion(eSO.engineNames.Length - 1);
            dM.updateUI();
            closePopUp();
        }
        else
        {
            Debug.Log("Invalid input to company");
        }
    }
    
}
