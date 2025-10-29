using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class DraggableUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public bool forceToTop = false;
    private RectTransform rectTransform;
    private Canvas canvas; 
    private CanvasGroup canvasGroup; 
    private Vector2 originalPosition;
    private Vector2 pointerOffset;
    private GameObject enginePrefab;
    private MeshRenderer lastSelected;
    private Material lastSelectedMaterial;
    private Transform holderTransform;
    public enginesSO enginesSO;
    public int SOindex;
    public int company;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;

        // Convert pointer position to local space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, 
            eventData.position, 
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera, 
            out var localPointDown
        );

        // Calculate offset
        pointerOffset = rectTransform.anchoredPosition - localPointDown;

        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
        dragManager d = dragManager.instance;
        d.selectedEngineIndex = SOindex;
        d.selectedEngine = null;
        d.updateUI();
        d.deSelect();
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position + new Vector2(0, 100),
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera,
            out var localPoint
            
        );
        
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject.tag != "Untagged")
        {
            if (hit.collider.gameObject.GetComponent<engineHolder>() != null)
            {
                if (lastSelected != hit.collider.gameObject.GetComponent<MeshRenderer>())
                {
                    if (lastSelected != null)
                    {
                        lastSelected.material = lastSelectedMaterial;
                    }
                    lastSelected = hit.collider.gameObject.GetComponent<MeshRenderer>();
                    lastSelectedMaterial = lastSelected.material;
                }
                lastSelected.material = dragManager.instance.hoverMaterial;
                //Debug.Log(dragManager.instance.hoverMaterial); 
            }
        }
        else if (lastSelected != null)
        {
            lastSelected.material = lastSelectedMaterial;
            lastSelectedMaterial = null;
            lastSelected = null;
        }

        rectTransform.anchoredPosition = localPoint + pointerOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        enginePrefab = dragManager.instance.enginePrefab;
        PlaceInWorld(eventData);

        rectTransform.anchoredPosition = originalPosition;
    }

    private void PlaceInWorld(PointerEventData eventData)
    {
        if (lastSelected != null)
        {
            lastSelected.material = lastSelectedMaterial;
            lastSelectedMaterial = null;
            lastSelected = null;
        }
        
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject.tag != "Untagged")
        {
            engineHolder eH = hit.collider.gameObject.GetComponent<engineHolder>();
            if (eH != null && !eH.full)
            {
                Debug.Log("Placed");
                engineScroller.instance.removeFromArray(this.gameObject);
                Destroy(this.gameObject);

                if (enginePrefab != null)
                {
                    GameObject e = Instantiate(enginePrefab, hit.point, Quaternion.identity);
                    
                    eH.companyNum += company;
                    eH.placeEngine(e);
                    WorldObjectInteract eI = e.GetComponent<WorldObjectInteract>();
                    eI.eH = eH;
                    eI.enginesSO = enginesSO;
                    eI.SOindex = SOindex;
                    eI.company = company;
                    eI.currentArea = eH.areaName;
                    dragManager d = dragManager.instance;
                    d.selectedEngine = eI.gameObject;
                    d.updateUI();
                    d.deSelect();
                    dragManager.instance.undoVis += eI.undoVis;
                    e.GetComponentInChildren<TMP_Text>().text = enginesSO.engineNames[SOindex];
                    currentIncident cI = d.tM.currentIncident;
                    int incidentIndex = cI.activeEngines.IndexOf(SOindex);
                    cI.addInfo($"{enginesSO.engineNames[SOindex]} with {enginesSO.enginePersonel[SOindex]} personnel arrived at scene");
                    cI.addInfo($"{enginesSO.engineNames[SOindex]} was placed in {eI.currentArea}");
                    cI.engineHolderPositions[incidentIndex] = eI.currentArea;
                    timeManager.instance.setTime(incidentIndex);
                    Debug.Log("Reset time from DraggableUI");
                    

                }
            }
            else
            {
                // If no engineHolder or can't place, you can decide what to do.
                // For example, reset the UI element position or just do nothing.
            }
        }
    }


    public void StartDragging(Vector2 screenPosition)
    {
        originalPosition = rectTransform.anchoredPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out var localPointDown
        );

        pointerOffset = rectTransform.anchoredPosition - localPointDown;
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

}
