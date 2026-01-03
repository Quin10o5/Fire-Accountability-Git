using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Manager2D : MonoBehaviour
{
    
    [Header("References")]
    public TMP_Text toggleButtonText;
    public maydayManager maydayManager;
    public HolderScroller2D holderScroller2D;
    public Transform areaParent;
    public GameObject UIParent;
    public GameObject holderPrefab;
    
    [Header("Settings")]
    public Vector2 defualtYMinmax;
    public float maydayOpenY = 180;
    
    [Header("Runtime")]
    public bool in2D = false;
    public List<EngineHolderPair> holders;

    public static Manager2D instance;
    private RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        rectTransform = areaParent.GetComponent<RectTransform>();
        holders = new List<EngineHolderPair>();
    }

    public void LinkEngines(Engine worldEngine, Engine2D engine2D)
    {
        engine2D.engine = worldEngine;
        worldEngine.engine2D = engine2D;
    }

    public void Create2DEngine(Engine worldEngine, engineHolder holder)
    {
        
    }

    public void Create2DView()
    {
        
        for (int i = 0; i < holders.Count; i++)
        {
            GameObject holder = Instantiate(holderPrefab, areaParent.transform);
            EngineHolder2D holder2D = holder.GetComponent<EngineHolder2D>();

            holder2D.engineHolder = holders[i].engineHolder;
            holder2D.SetupHolder();
            holders[i].engineHolder2D = holder2D;
        }
        holderScroller2D.updateScroller();
    }

    void SpawnHolder()
    {
        
    }

    public void Toggle2D()
    {
        in2D = !in2D;
        UIParent.SetActive(in2D);
        if (in2D)
        {
            toggleButtonText.text = "Switch to 3D";
        }
        else
        {
            toggleButtonText.text = "Switch to 2D";
        }
    }

    public void MaydayToggled()
    {
        if(maydayManager.maydayActive) rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, maydayOpenY);
        else rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, defualtYMinmax.y);
    }
    
}

[System.Serializable]
public class EngineHolderPair
{
    public engineHolder engineHolder;
    public EngineHolder2D engineHolder2D;
}