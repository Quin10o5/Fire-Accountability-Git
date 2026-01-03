using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HolderScroller2D : MonoBehaviour
{
    public int holderLimit = 6;
    public static HolderScroller2D instance;

    public Transform holderParent;

    public List<GameObject> holders;

    private Scrollbar s;
    // Start is called before the first frame update
    public void updateScroller()
    {
        Manager2D m = Manager2D.instance;
        for (int i = 0; i < m.areaParent.childCount; i++)
        {
            holders.Add(m.areaParent.GetChild(i).gameObject);
        }
        // Calculate how many holders fit inside the parent Transform (Vertical Layout Group)
        var layoutGroup = holderParent.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            // Get parent height
            float parentHeight = holderParent.GetComponent<RectTransform>().rect.height;
    
            // Get holder height, including spacing
            float holderHeight = holders.Count > 0 
                ? holders[0].GetComponent<RectTransform>().rect.height + layoutGroup.spacing 
                : 0;

            // Calculate the number of holders that fit within the height
            holderLimit = holderHeight > 0 ? Mathf.FloorToInt(parentHeight / holderHeight) : holderLimit;
        }
        
        for (int i = 0; i < holders.Count; i++)
        {
            holders[i].SetActive(false);
        }
        
        // Check if there are more than 6 holders
        if (holders.Count > holderLimit)
        {
            // Loop through the overflow holders and set them inactive
            for (int i = 0; i < holderLimit; i++)
            {
                holders[i].SetActive(true);
            }

            // Set this gameObject as active
            this.gameObject.SetActive(true);
        }
        else
        {
            // Ensure all holders remain active if 6 or fewer
            foreach (var holder in holders)
            {
                holder.SetActive(true);
            }
            this.gameObject.SetActive(false);
        }
    }

    public void removeFromArray(GameObject holderButton)
    {
    // Remove the specified holder from the array
    List<GameObject> holderList = new List<GameObject>(holders); // Convert the array to a list for easier manipulation
    if (holderList.Contains(holderButton))
    {
        holderList.Remove(holderButton);
    }

    // Update the holders array
    holders = holderList;

    // Reorganize the holders in the parent
    for (int i = 0; i < holders.Count; i++)
    {
        holders[i].transform.SetSiblingIndex(i);
    }

    // Update the scroller visibility if needed
    updateScroller();
    }
    
    public void AddToArray(GameObject holderButton, bool atTop = false)
    {
        // Remove the specified holder from the array
        List<GameObject> holderList = new List<GameObject>(holders); // Convert the array to a list for easier manipulation
        if (!holderList.Contains(holderButton))
        {
            if(atTop) holderList.Insert(0, holderButton);
            else holderList.Add(holderButton);
        }

        // Update the holders array
        holders = holderList;

        // Reorganize the holders in the parent
        for (int i = 0; i < holders.Count; i++)
        {
            holders[i].transform.SetSiblingIndex(i);
        }
        updateScroller();
    }       
    

    public void OnSliderValueChanged()
{
    int unshown = holders.Count - holderLimit;
    if (unshown < 0)
    {
        unshown = 0;
    }
    // Determine the start index based on the slider value
    int startIndex = Mathf.Clamp((int)(s.value * unshown), 0, holders.Count);

    // Reorder children of holderParent based on the new visibility
    for (int i = 0; i < holders.Count; i++)
    {
        if (i >= startIndex && i < startIndex + holderLimit)
        {
            // Keep the holders in the current viewing range active
            holders[i].SetActive(true);
        }
        else
        {
            // Deactivate holders outside the viewing range
            holders[i].SetActive(false);
        }
    }
}
    
    
    void Awake()
    {
        s = GetComponent<Scrollbar>();

    }
}
