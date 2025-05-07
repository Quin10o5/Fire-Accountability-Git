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
    public TMP_InputField warningTime;
    public TMP_InputField alertTime;
    private string companyNameString;
    private int companyPersonnelNumInt;
    private string companyChangedNameString;
    public GameObject doublePopUp;
    public TMP_Text doublePopUpText;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            instance.enabled = false;
            Destroy(instance.gameObject);
            instance = null;
        }
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
        UpdateTimerPresets();
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

    public void UpdateTimerPresets()
    {
        warningTime.text = PlayerPrefs.GetInt("warningTimeMinutes", 10).ToString();
        alertTime.text = PlayerPrefs.GetInt("alertTimeMinutes", 15).ToString();
    }

    public void ConfirmChangedTimers()
    {
        if (int.TryParse(warningTime.text, out int warningTimeInt) &&
            int.TryParse(alertTime.text, out int alertTimeInt) && alertTimeInt > warningTimeInt && warningTimeInt >= 1)
        {
            PlayerPrefs.SetInt("warningTimeMinutes", warningTimeInt);
            PlayerPrefs.SetInt("alertTimeMinutes", alertTimeInt);
            closePopUp();
        }
        else
        {
            ErrorMSG("Please input valid timers. Alert time must be greater than warning time.");
        }
    }

    public void ErrorMSG(string msg)
    {
        doublePopUp.SetActive(true);
        doublePopUpText.text = msg;
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
                ErrorMSG("Please input valid unit name and personnel.");
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
                ErrorMSG("Please input valid unit name.");
            }
        }
        else if (activePopUp == 3)
        {
            ConfirmChangedTimers();
        }
        
    }
}
