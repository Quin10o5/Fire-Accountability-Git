using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomMaydayCheckBox : MonoBehaviour
{
    public string title;
    public TMP_Text text;
    public Toggle toggle;
    private dragManager dM;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dM = dragManager.instance;
        text.text = title;
    }

    public void toggled()
    {
        string end = "off";
        if(toggle.isOn) end = "on";
        dM.cI.addInfo($"{title} was toggled to {end}");
    }
}
