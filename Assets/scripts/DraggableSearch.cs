using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class DraggableSearch : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas; 
    private CanvasGroup canvasGroup; 
    private Vector2 originalPosition;
    private Vector2 pointerOffset;
    public Color searchColor;
    public int searchCompletion;
    private MeshRenderer lastSelected;
    private Material lastSelectedMaterial;
    private Transform holderTransform;
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
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject.tag == "Building")
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
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject.tag == "Building")
        {
            engineHolder e = hit.collider.gameObject.GetComponent<engineHolder>();
            int outlineColorID = Shader.PropertyToID("_outline_Color");
            e.insideOutsideVis[1].material.SetColor(outlineColorID, searchColor);
            e.searchCompletion = searchCompletion;
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
