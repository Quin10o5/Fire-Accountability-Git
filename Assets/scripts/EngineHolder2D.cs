using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EngineHolder2D : MonoBehaviour
{
    public SettingsSO settings;
    public engineHolder engineHolder;
    public string areaName;
    public Sprite icon;
    public TMP_Text title;
    public Image iconImage;
    public Image outline;
    public Transform unitParent;
    public List<Engine2D> units;
    private Manager2D manager;

    private Color baseOutlineColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = Manager2D.instance;
        title.text = areaName;
        iconImage.sprite = icon;
        baseOutlineColor = outline.color;
    }

    public void SetupHolder()
    {
        icon = engineHolder.icon2D;
        areaName = engineHolder.areaName;
        iconImage.sprite = icon;
        title.text = areaName;
        
        if(engineHolder.currentCommander != null) outline.color = settings.GetCommandingColor(CommandType.Area);
        else outline.color = baseOutlineColor;
    }
}
