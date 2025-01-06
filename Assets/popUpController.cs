using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
public class popUpController : MonoBehaviour
{
    public int activePopUp = -1;
    public static popUpController instance;
    public TMP_Text header;
    public GameObject[] messages;
    public string[] headerStrings;
    public companyEditor c;
    public TMP_InputField newName;
    public TMP_InputField newPersonnel;
    public TMP_InputField changedName;
    private string companyNameString;
    private int companyPersonnelNumInt;
    private string companyChangedNameString;
    public GameObject doublePopUp;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    public void openPopUp(int which)
    {
        closeDoublePopUp();
        this.gameObject.SetActive(true);
        for (int i = 0; i < messages.Length; i++)
        {
            messages[i].SetActive(i == which);
        }
        header.text = headerStrings[which];
        activePopUp = which;
    }
    
    public bool confirmNum()
    {
        if (newPersonnel.text == "") return false;
        try { companyPersonnelNumInt = int.Parse(newPersonnel.text); }
        catch (System.Exception) { return false; }
        if(companyPersonnelNumInt > 0) return true;
        return false;
    }
    public bool confirmName()
    {
        companyNameString = newName.text;
        Debug.Log(companyNameString);
        if(companyNameString != "") return true;
        return false;
    }
    public bool confirmChangedName()
    {
        companyChangedNameString = changedName.text;
        Debug.Log(companyChangedNameString);
        if(companyChangedNameString != "") return true;
        return false;
    }

    public void closePopUp()
    {
        this.gameObject.SetActive(false);
        activePopUp = -1;
    }
    public void closeDoublePopUp()
    {
        doublePopUp.SetActive(false);
    }

    public void confirm()
    {
        if (activePopUp == 0)
        {
            c.deleteCompany();
            c.EditorUI.SetActive(false);
            closePopUp();
        }
        else if (activePopUp == 1)
        {
            if (confirmName() && confirmNum())
            {
                List<string> eNames = c.eSO.engineNames.ToList();
                List<int> ePersonnel = c.eSO.enginePersonel.ToList();
                eNames.Add(companyNameString);
                ePersonnel.Add(companyPersonnelNumInt);
                c.eSO.engineNames = eNames.ToArray();
                c.eSO.enginePersonel = ePersonnel.ToArray();
                c.updateEngines();
                closePopUp();
            }
            else
            {
                Debug.Log("invalid input");
                doublePopUp.SetActive(true);
            }
        }
        else if (activePopUp == 2)
        {
            if (confirmChangedName())
            {
                c.eSO.engineNames[c.selectedIndex] = companyChangedNameString;
                c.updateEngines();
                c.updateSelectedUI();
                closePopUp();
            }
            else
            {
                Debug.Log("invalid input");
                doublePopUp.SetActive(true);
            }
        }
        
    }
}
