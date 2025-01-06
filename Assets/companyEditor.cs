using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class companyEditor : MonoBehaviour
{
    public static companyEditor instance;
    public enginesSO eSO;
    public GameObject selectedCompany;
    public int selectedIndex;
    public GameObject[] companyLists;
    public TMP_Text companyName;
    public TMP_Text companyPersonnel;
    public popUpController pUp;
    public GameObject EditorUI;
    public GameObject subtractButton;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        updateEngines();
    }

    public void updateEngines()
    {
        //eSO.LoadData();
        for (int i = 0; i < companyLists.Length; i++)
        {
            for (int j = 0; j < companyLists[i].transform.childCount; j++){
                companyLists[i].transform.GetChild(j).gameObject.SetActive(false);}
            companyLists[i].SetActive(false);
        }
        int activeLists = Mathf.CeilToInt(((float)eSO.engineNames.Length)/7);
        int currentEngine = 0;
        for (int i = 0; i < activeLists; i++)
        {
            companyLists[i].SetActive(true);
            for (int j = 0; j < companyLists[i].transform.childCount; j++)
            {
                GameObject c = companyLists[i].transform.GetChild(j).gameObject;
                c.SetActive(true);
                editorButton e = c.GetComponent<editorButton>();
                e.SOindex = currentEngine;
                e.eSO = eSO;
                e.companyEditor = this;
                e.setVis();
                Debug.Log(currentEngine);
                currentEngine++;
                
                if (currentEngine >= eSO.engineNames.Length)
                    break;
            }
        }
    }

    public void changePersonnel(int amt)
    {
        eSO.enginePersonel[selectedIndex] += amt;
        updateEngines();
        updateSelectedUI();
    }
    

    public void deleteCompany()
    {
        List<string> engineNamesList = eSO.engineNames.ToList();
        List<int> enginePersonelList = eSO.enginePersonel.ToList();
        engineNamesList.RemoveAt(selectedIndex);
        enginePersonelList.RemoveAt(selectedIndex);
        eSO.engineNames = engineNamesList.ToArray();
        eSO.enginePersonel = enginePersonelList.ToArray();
        updateEngines();
    }
    

    private void OnApplicationQuit()
    {
        eSO.SaveData();
    }

    public void updateSelectedUI()
    {
        if (EditorUI.activeSelf == false)
        {
            EditorUI.SetActive(true);
        }
        companyName.text = eSO.engineNames[selectedIndex];
        companyPersonnel.text = "Personnel: " + eSO.enginePersonel[selectedIndex].ToString();

        if (eSO.enginePersonel[selectedIndex] <= 1)
        {
            subtractButton.SetActive(false);
        }
        else
        {
            subtractButton.SetActive(true);
        }
    }
    

    public void selectCompany(GameObject company)
    {
        if (selectedCompany != null)
        {
            editorButton eB2 = selectedCompany.GetComponent<editorButton>();
            eB2.i.color = eB2.colors[0];
        }
        
        editorButton eB = company.GetComponent<editorButton>();
        eB.i.color = eB.colors[1];
        selectedCompany = company;
        selectedIndex = eB.SOindex;
        updateSelectedUI();
    }
}
