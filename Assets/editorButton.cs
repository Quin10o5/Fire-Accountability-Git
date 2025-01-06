using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class editorButton : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public companyEditor companyEditor;
    public Color[] colors;
    //[HideInInspector]
    public int SOindex;
    [HideInInspector]
    public enginesSO eSO;
    [HideInInspector]
    public Image i;

    public TMP_Text name;
    public TMP_Text personnel;
    // Start is called before the first frame update
    void Start()
    {
        i = GetComponent<Image>();
        i.color = colors[0];
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // This gets called when the UI element is clicked/touched
        companyEditor.selectCompany(this.gameObject);
    }

    public void setVis()
    {
        name.text = eSO.engineNames[SOindex];
        personnel.text = eSO.enginePersonel[SOindex].ToString();
    }
}
