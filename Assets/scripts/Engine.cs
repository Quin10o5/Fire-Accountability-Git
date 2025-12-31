using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;
using System.Reflection;
using NaughtyAttributes;
using Unity.VisualScripting;

public class Engine : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Scriptable Objects")]
    public SettingsSO settings;
    public enginesSO customSettings;
    
    [Header("References")]
    public Color baseColor;
    public Outline outline;
    public TMP_Text nameText;
    
    [Header("Runtime")]
    [ReadOnly]public bool isDragging = false;
    [ReadOnly]public engineHolder eH;
    
    [Header("Incident Runtime")]
    [ReadOnly]public int SOindex;
    [ReadOnly]public int company;
    [ReadOnly]public int index;
    [ReadOnly]public string currentArea;
    [ReadOnly]public CommandType commandType;
    
    //private
    private Material lastSelectedMaterial;
    private dragManager dragManager;
    private bool performedPersonalHandling = false;
    private Vector3 offset = new Vector3(0, .4f, 0);
    private Plane dragPlane;
    private MeshRenderer lastSelected;
    private Color lastSelectedColor;
    private Vector3 lookPoint;
    
    
    
    private void Start()
    {
        // Define the plane at the object's starting position, facing the camera
        //dragPlane = new Plane(-Camera.main.transform.forward, transform.position);
        dragManager = dragManager.instance;

    }

    public void Select()
    {
        if(commandType == CommandType.None)SetOutline(Color.white);
        else SetOutline(Color.Lerp(settings.GetCommandingColor(commandType), Color.white, .5f)); 
    }

    public void undoVis()
    {
        ClearOutline();
    } 

    public void OnPointerDown(PointerEventData eventData)
    {
        performedPersonalHandling = false;
        
        dragPlane = new Plane(-Camera.main.transform.forward, transform.position);
        lookPoint = Camera.main.transform.position;
        isDragging = true;
        
        // Calculate offset between object and plane intersection at pointer down
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset += transform.position - hitPoint;
        }
        GetComponent<BoxCollider>().enabled = false;
        
        dragManager.deSelect();
        if(commandType == CommandType.None) SetOutline(Color.white);
        else SetOutline(Color.Lerp(settings.GetCommandingColor(commandType), Color.white, 0.5f));
        
        // You can adjust visuals for dragging here if needed (e.g., change transparency)
        dragManager.selectedEngineIndex = SOindex;
        dragManager.selectedEngine = this.gameObject;
        dragManager.updateUI();

    }

     
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        if (!performedPersonalHandling)
        {
            eH.RemoveEngine(this);
            eH.companyNum -= company;
            eH.updateCompanyVis();
            performedPersonalHandling = true;
        }
        //dragPlane = new Plane(-Camera.main.transform.forward, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            transform.position = hitPoint + offset;
            if (Vector3.Distance(transform.position, lookPoint) > 1.5f)
            {
                transform.position = Vector3.Lerp(transform.position, lookPoint, .5f);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, lookPoint, .3f);
            }
            transform.LookAt(lookPoint, Vector3.up);
            transform.RotateAround(transform.position, Vector3.up, -90);
            transform.RotateAround(transform.position, Vector3.forward, -30);
            transform.RotateAround(transform.position, Vector3.left, -20);
            
            
        }

        // Optional: Raycast from camera to scene to highlight engineHolder
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject.tag != "Untagged")
        {
            engineHolder eH3 = hit.collider.gameObject.GetComponent<engineHolder>();
            if (eH3 != null)
            {
                MeshRenderer mr = hit.collider.gameObject.GetComponent<engineHolder>().selectionRenderer;
                if (mr != null && mr != lastSelected)
                {
                    // Restore previous selection if any
                    if (lastSelected != null)
                    {
                        lastSelected.material = lastSelectedMaterial;
                    }

                    lastSelected = mr;
                    lastSelectedMaterial = mr.material;
                    lastSelected.material = dragManager.hoverMaterial;
                }
            }
        }
        else if (lastSelected != null)
        {
            lastSelected.material = lastSelectedMaterial;
            lastSelectedMaterial = null;
            lastSelected = null;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        
        if (lastSelected != null)
        {
            lastSelected.material = lastSelectedMaterial;
            lastSelectedMaterial = null;
            lastSelected = null; 
        }
        
        // Restore material if needed
        if (lastSelected != null)
        {
            //lastSelected.material = lastSelectedMaterial;
            //lastSelectedMaterial = null;
            lastSelected = null;
        }

        dragManager.selectedEngineIndex = SOindex;
        dragManager.selectedEngine = this.gameObject;
        // When mouse/finger is released, try to place the engine
        PlaceInWorld(eventData);
        if(commandType == CommandType.None) SetOutline(Color.white);
        else SetOutline(Color.Lerp(settings.GetCommandingColor(commandType), Color.white, 0.5f));
        GetComponent<BoxCollider>().enabled = true;
    }

    private void PlaceInWorld(PointerEventData eventData)
    {
        
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        
            
        if (Physics.Raycast(ray, out RaycastHit hit) && (hit.collider.gameObject.tag != "Untagged" && hit.collider.gameObject.tag != "Engine"))
        {
            engineHolder eH2 = hit.collider.gameObject.GetComponent<engineHolder>();
            if(eH != eH2 && commandType == CommandType.Area)
            {
                ClearCommandStatus();
                dragManager.updateUI();
                dragManager.cI.addInfo($"{customSettings.engineNames[SOindex]} moved from {eH.areaName} to {eH2.areaName}");
                dragManager.cI.MoveUnitPosition(SOindex, eH2.areaName);
                dragManager.tM.resetSelectedTime();
            }
            if (eH2 != null && !eH2.full)
            {
                if (performedPersonalHandling)
                {
                    eH2.companyNum += company;
                    transform.position = hit.point;
                    eH2.placeEngine(this.gameObject);
                    
                    if(eH != eH2)
                    {
                        dragManager.cI.addInfo(
                            $"{customSettings.engineNames[SOindex]} moved from {eH.areaName} to {eH2.areaName}");
                        dragManager.cI.MoveUnitPosition(SOindex, eH2.areaName);
                        dragManager.tM.resetSelectedTime();
                    }
                }
                
                eH = eH2;
                
            }
            else
            {
                if (performedPersonalHandling)
                {
                    eH.companyNum += company;
                    eH.placeEngine(this.gameObject);
                }

                eH = eH2;
                dragManager d = dragManager;
                d.selectedEngineIndex = SOindex;
                d.selectedEngine = this.gameObject;
                d.updateUI();
                //Debug.Log("No suitable engine holder found or holder is full.");
            }
            
            
        }
        else
        {
            Debug.Log("No valid hit. Not placed.");
            dragManager d = dragManager;
            d.selectedEngineIndex = SOindex;
            d.selectedEngine = this.gameObject;
            d.updateUI();
            if (performedPersonalHandling)
            {
                eH.placeEngine(this.gameObject);
                eH.companyNum += company;
            }
            
            // Optionally reset position or take another action
        }
        eH.updateCompanyVis();
        

        
    }
    
    public void ClearCommandStatus()
    {
        
        if(commandType == CommandType.Area) eH.ClearCommander();
        commandType = CommandType.None;
        ClearOutline();
        if(dragManager.selectedEngine == this.gameObject)SetOutline(Color.white);
        UpdateName();
        
        
    }

    public void SetCommand(CommandType command)
    {
        commandType = command;
        if (command == CommandType.Area)
        {
            if(eH.currentCommander)
            {
                eH.currentCommander.GetComponent<Engine>().ClearCommandStatus();
            }
            eH.SetCommander(gameObject);
        }
        if(commandType == CommandType.None) ClearCommandStatus();
        else SetOutline(settings.GetCommandingColor(command));
        
        UpdateName();
    }

    public void SetOutline(Color outlineColor) 
    {
        if(!outline.enabled) outline.enabled = true;
        outline.OutlineColor = outlineColor;
    }

    public void ClearOutline(){
        if (commandType != CommandType.None)
        {
            SetOutline(settings.GetCommandingColor(commandType));
            return;
        }
        outline.enabled = false;
        outline.OutlineColor = baseColor;
    }
    
    /// <summary>
    /// Shortcut to send this engine back to the UI list.
    /// </summary>
    public void ReturnToUI()
    {
        dragManager.ReturnEngineToUI(this);
    }

    public void UpdateName()
    {
        string baseName = customSettings.engineNames[SOindex];
        string append = "";
        if (commandType != CommandType.None)
        {
            append = settings.GetCommandingAcronym(commandType);
            nameText.text = append + "-" + baseName ;
        }
        else
        {
            nameText.text = baseName;
        }
        
    }




    
    void CopyPublicVariables<T>(T source, T target)
    {
        if (source == null || target == null) return;

        // Get all public instance fields and properties
        foreach (FieldInfo field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            field.SetValue(target, field.GetValue(source));
        }

        foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.CanRead && property.CanWrite) // Ensure we can both read and write values
            {
                property.SetValue(target, property.GetValue(source));
            }
        }
    }
}
