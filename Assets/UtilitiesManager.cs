using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UtilitiesManager : MonoBehaviour
{
    private timeManager tM;
    public string[] options = new string[2];
    public int gasIndex = 0;
    public int electricityIndex = 0;
    public TMP_Text gasText;
    public TMP_Text electricityText;
    // Start is called before the first frame update
    void Start()
    {
        tM = timeManager.instance;
        string[] newOptions = new string[4];
        newOptions[0] = "";
        newOptions[1] = "N/A";
        newOptions[2] = options[0];
        newOptions[3] = options[1];
        options = newOptions;
        UpdateUtilityUI();
    }

    public void UpdateUtilityUI()
    {
        gasText.text = options[gasIndex];
        electricityText.text = options[electricityIndex];
    }

    public void gasButton()
    {
        gasIndex = (gasIndex + 1) % options.Length;
        if(gasIndex == 0) gasIndex = 1;
        UpdateUtilityUI();
        if(gasIndex == 1) tM.currentIncident.addInfo("Gas was non applicable", "GNA");
        else
        {
            
            tM.currentIncident.addInfo("Gas: " + options[gasIndex]);
            tM.currentIncident.ClearNAGasNotes();
        }
    }

    public void electricButton()
    {
        electricityIndex = (electricityIndex + 1) % options.Length;
        if(electricityIndex == 0) electricityIndex = 1;
        UpdateUtilityUI();
        if(electricityIndex == 1) tM.currentIncident.addInfo("Electricity was non applicable", "ENA");
        else
        {
            
            tM.currentIncident.addInfo("Electricity: " + options[electricityIndex]);
            tM.currentIncident.ClearNAElectricalNotes();
        }
    }
}
