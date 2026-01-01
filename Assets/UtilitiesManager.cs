using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UtilitiesManager : MonoBehaviour
{
    private timeManager tM;
    public string[] options = new string[2];
    
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

    }


    

    public void HandleCycleButton(CycleButton cycleButton)
    {
        cycleButton.index = (cycleButton.index + 1) % options.Length;
        if(cycleButton.index == 0) cycleButton.index = 1;
        cycleButton.cycleText.text = options[cycleButton.index];
        string acryo = options[cycleButton.index].Substring(0,1) + "NA";
        if(cycleButton.index == 1) tM.currentIncident.addInfo($"{cycleButton.type} was non applicable");
        else
        {
            
            tM.currentIncident.addInfo($"{cycleButton.type}: " + options[cycleButton.index]);
            tM.currentIncident.CLearNoteType(acryo);
        }
    }
    
}
