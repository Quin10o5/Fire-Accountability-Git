using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.UI;

public class popUpsManager : MonoBehaviour
{
    public bool fireControlled = false;
    public bool waterOnFire = false;
    public static popUpsManager instance;
    private timeManager tM;
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
    public Scrollbar scroller;
    private string companyNameString;
    
    private int companyPersonnelNumInt;
    private dragManager dM;
    
    [Header("Notes")] 
    private bool prevNotesOpen = false;
    private string noteString;
    public TMP_InputField noteText;
    public GameObject prevNotes;
    public Transform notesParent;
    public GameObject pastNotePrefab;
    public TMP_Text viewNoteText;
    [Header("Warnings")]
    public GameObject warningPopUp;
    public TMP_Text errorText;
    public TMP_Text reasonText;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        
    }
    void Start()
    {
        dM = dragManager.instance;
        tM = timeManager.instance;
    }

    // Update is called once per frame
    public void openPopUp(int pageIndex)
    {
        dM = dragManager.instance;
        if (pageIndex != currentPage)
        {
            currentPage = pageIndex;
            popUpParent.SetActive(true);
            warningPopUp.SetActive(false);
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

                    if (i == 0)
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
        ClosePrevNotes();
    }

    void openWarning(string error, string reason)
    {
        warningPopUp.SetActive(true);
        errorText.text = error;
        reasonText.text = reason;
    }
    public void closeWarning()
    {
        warningPopUp.SetActive(false);
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
            Engine w = dM.selectedEngine.GetComponent<Engine>();
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
            Engine w = dM.selectedEngine.GetComponent<Engine>();
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
        if (companyPersonnelNum.text == "") return false;
        try{companyPersonnelNumInt = int.Parse(companyPersonnelNum.text);}
        catch (System.Exception) { return false; }
        if(companyPersonnelNumInt > 0) return true;
        return false;
    }
    public bool confirmName()
    {
        companyNameString = companyName.text;
        Debug.Log(companyNameString);
        if(companyNameString != "") return true;
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
            dM.spawnEngineButton(eSO.engineNames.Length - 1, true);
            dM.updateUI();
            closePopUp();
            scroller.value = 0;
            companyName.text = null;
            companyPersonnelNum.text =null ;
        }
        else
        {
            if (!confirmName())
            {
                openWarning("This Company cannot be added", "Company name cannot be blank" );
            }
            else
            {
                openWarning("This Company cannot be added", "Company personnel cannot be less than 1" );
            }
            
        }
    }
    
    public void confirmNote()
    {
        if (prevNotesOpen) ClosePrevNotes();
        noteString = noteText.text;
        if(!string.IsNullOrEmpty(noteString))
        {
            tM.currentIncident.addInfo(noteString, "NOTE");
            closePopUp();
            noteText.text = null;
        }
        else
        {
            openWarning("This note cannot be added", "Note cannot be blank" );
        }
    }

    public void OpenPrevNotes()
    {
        if(prevNotesOpen)
        {
            ClosePrevNotes();
            return;
        }
        prevNotesOpen = true;
        prevNotes.SetActive(true);
        viewNoteText.text = "Close Notes";
        
        string[] notes = dM.cI.GetAllOfNoteType("NOTE");
        
        for(int i = 0; i < notesParent.childCount; i++) Destroy(notesParent.GetChild(i).gameObject);
        
        for (int i = 0; i < notes.Length; i++)
        {
            GameObject note = Instantiate(pastNotePrefab, notesParent);
            note.GetComponent<TMP_Text>().text = notes[i];
        }
    }

    public void ClosePrevNotes()
    {
        prevNotes.SetActive(false);
        prevNotesOpen = false;
        viewNoteText.text = "View Notes";
        
    }

    public void fireControlledUpdate(GameObject visual)
    {
        tM = timeManager.instance;
        fireControlled = !fireControlled;
        visual.SetActive(fireControlled);
        if(fireControlled)
        {
            tM.currentIncident.info.Add($"\n\n {DateTime.Now.ToLongTimeString()}: FIRE UNDER CONTROL\n\n");
            if (tM.logText != null)
            {
                tM.logText.text = $"{DateTime.Now.ToLongTimeString()}: FIRE UNDER CONTROL";
            }
        }
        else
        {
            tM.currentIncident.info.Add($"\n\n {DateTime.Now.ToLongTimeString()}: FIRE NO LONGER UNDER CONTROL\n\n");
            if (tM.logText != null)
            {
                tM.logText.text = $"{DateTime.Now.ToLongTimeString()}: FIRE NO LONGER UNDER CONTROL";
            }
        }
        
    }
    public void WaterOnFireUpdate(GameObject visual)
    {
        tM = timeManager.instance;
        waterOnFire = !waterOnFire;
        visual.SetActive(waterOnFire);
        if(waterOnFire) tM.currentIncident.addInfo("Water is on the fire");
        else tM.currentIncident.addInfo("Water is off the fire");
        
    }
    
}
