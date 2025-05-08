using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class engineScroller : MonoBehaviour
{
    public int buttonLimit = 6;
    public static engineScroller instance;

    public Transform engineParent;

    public GameObject[] engineButtons;

    private Scrollbar s;
    // Start is called before the first frame update
    public void updateScroller()
    {
        // Calculate how many buttons fit inside the parent Transform (Vertical Layout Group)
        var layoutGroup = engineParent.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            // Get parent height
            float parentHeight = engineParent.GetComponent<RectTransform>().rect.height;
    
            // Get button height, including spacing
            float buttonHeight = engineButtons.Length > 0 
                ? engineButtons[0].GetComponent<RectTransform>().rect.height + layoutGroup.spacing 
                : 0;

            // Calculate the number of buttons that fit within the height
            buttonLimit = buttonHeight > 0 ? Mathf.FloorToInt(parentHeight / buttonHeight) : buttonLimit;
        }
        
        for (int i = 0; i < engineButtons.Length; i++)
        {
            engineButtons[i].SetActive(true);
        }
        
        // Check if there are more than 6 buttons
        if (engineButtons.Length > buttonLimit)
        {
            // Loop through the overflow buttons and set them inactive
            for (int i = buttonLimit; i < engineButtons.Length; i++)
            {
                engineButtons[i].SetActive(false);
            }

            // Set this gameObject as active
            this.gameObject.SetActive(true);
        }
        else
        {
            // Ensure all buttons remain active if 6 or fewer
            foreach (var button in engineButtons)
            {
                button.SetActive(true);
            }
            this.gameObject.SetActive(false);
        }
    }

    public void removeFromArray(GameObject engineButton)
    {
    // Remove the specified button from the array
    List<GameObject> buttonList = new List<GameObject>(engineButtons); // Convert the array to a list for easier manipulation
    if (buttonList.Contains(engineButton))
    {
        buttonList.Remove(engineButton);
    }

    // Update the engineButtons array
    engineButtons = buttonList.ToArray();

    // Reorganize the buttons in the parent
    for (int i = 0; i < engineButtons.Length; i++)
    {
        engineButtons[i].transform.SetSiblingIndex(i);
    }

    // Update the scroller visibility if needed
    updateScroller();
    }
    
    public void AddToArray(GameObject engineButton)
    {
        // Remove the specified button from the array
        List<GameObject> buttonList = new List<GameObject>(engineButtons); // Convert the array to a list for easier manipulation
        if (!buttonList.Contains(engineButton))
        {
            buttonList.Add(engineButton);
        }

        // Update the engineButtons array
        engineButtons = buttonList.ToArray();

        // Reorganize the buttons in the parent
        for (int i = 0; i < engineButtons.Length; i++)
        {
            engineButtons[i].transform.SetSiblingIndex(i);
        }
        updateScroller();
    }       
    

    public void OnSliderValueChanged()
{
    // Determine the start index based on the slider value
    int startIndex = Mathf.Clamp((int)(s.value * engineButtons.Length), 0, engineButtons.Length);

    // Reorder children of engineParent based on the new visibility
    for (int i = 0; i < engineButtons.Length; i++)
    {
        if (i >= startIndex && i < startIndex + buttonLimit)
        {
            // Keep the buttons in the current viewing range active
            engineButtons[i].SetActive(true);
        }
        else
        {
            // Deactivate buttons outside the viewing range
            engineButtons[i].SetActive(false);
        }
    }
}
    
    
    void Awake()
    {
        instance = this;
        s = GetComponent<Scrollbar>();
        
        

        
        
        
    }
}
