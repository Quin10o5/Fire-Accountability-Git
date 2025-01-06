using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;

public class menuManager : MonoBehaviour
{
    public GameObject[] menuParents;
    public GameObject[] buttons;

    public CinemachineVirtualCamera c;
    private CinemachineTrackedDolly dolly;
    private int currentMenu = 1;
    // Start is called before the first frame update
    void Start()
    {
        dolly = c.GetCinemachineComponent<CinemachineTrackedDolly>();
        enterMenuSelection();
    }

    // Update is called once per frame
    public void enterSceneSelection()
    {
        changeMenu(2);
    }
    public void enterMenuSelection()
    {
        changeMenu(1);
    }
    public void enterEngineEdits()
    {
        changeMenu(0);
    }

    public void tryOtherButton()
    {
        if (currentMenu == 0)
        {
            changeMenu(2);
        }
        else if (currentMenu == 2)
        {
            changeMenu(0);
        }
    }

    public void changeMenu(float dollyPosition)
    {
        for (int i = 0; i < menuParents.Length; i++)
        {
            menuParents[i].SetActive(false);
        }
        menuParents[(int)dollyPosition].SetActive(true);
        dolly.m_PathPosition = dollyPosition;
        currentMenu = (int)dollyPosition;
        updateButtons();
    }

    void updateButtons()
    {
        if (currentMenu == 1)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].SetActive(true);
            }

            if (currentMenu == 0)
            {
                buttons[0].GetComponentInChildren<TMP_Text>().text = "< Menu";
                buttons[1].GetComponentInChildren<TMP_Text>().text = "< Scenes";
            }
            else if (currentMenu == 2)
            {
                buttons[0].GetComponentInChildren<TMP_Text>().text = "Menu >";
                buttons[1].GetComponentInChildren<TMP_Text>().text = "Companies >";
            }
        }
        
    }
}
