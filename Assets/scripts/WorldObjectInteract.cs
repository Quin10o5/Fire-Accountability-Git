using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;
using System.Reflection;
public class WorldObjectInteract : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private bool isDragging = false;
    public int company;
    public engineHolder eH;
    private Vector3 offset = new Vector3(0, .4f, 0);
    private Plane dragPlane;
    private GameObject enginePrefab;
    private MeshRenderer lastSelected;
    private Material lastSelectedMaterial;
    private Vector3 lookPoint;
    public enginesSO enginesSO;
    public int SOindex;
    public int index;
    public Material selectedMat;
    public Material baseMat;
    public MeshRenderer visRenderer;
    public string currentArea;
    
    private void Start()
    {
        // Define the plane at the object's starting position, facing the camera
        dragPlane = new Plane(-Camera.main.transform.forward, transform.position);
        enginePrefab = dragManager.instance.enginePrefab;
        lookPoint = Camera.main.transform.position;
    }

    public void undoVis()
    {
        visRenderer.material = baseMat;
        //GetComponent<colorTimer>().timeText = null;
        dragManager.instance.undoVis -= undoVis;
    } 

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        // Calculate offset between object and plane intersection at pointer down
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset += transform.position - hitPoint;
        }
        GetComponent<BoxCollider>().enabled = false;
        eH.holders[index] = null;
        eH.full = false;
        visRenderer.material = selectedMat;
        dragManager.instance.deSelect();
        
        // You can adjust visuals for dragging here if needed (e.g., change transparency)
        dragManager.instance.selectedEngineIndex = SOindex;
        dragManager.instance.selectedEngine = this.gameObject;

    }

    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
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
                MeshRenderer mr = hit.collider.gameObject.GetComponent<MeshRenderer>();
                if (mr != null && mr != lastSelected)
                {
                    // Restore previous selection if any
                    if (lastSelected != null)
                    {
                        lastSelected.material = lastSelectedMaterial;
                    }

                    lastSelected = mr;
                    lastSelectedMaterial = mr.material;
                    lastSelected.material = dragManager.instance.hoverMaterial;
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

        // Restore material if needed
        if (lastSelected != null)
        {
            lastSelected.material = lastSelectedMaterial;
            lastSelectedMaterial = null;
            lastSelected = null;
        }

        // When mouse/finger is released, try to place the engine
        PlaceInWorld(eventData);
        GetComponent<BoxCollider>().enabled = true;
    }

    private void PlaceInWorld(PointerEventData eventData)
    {
        
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);

        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject.tag != "Untagged")
        {
            engineHolder eH2 = hit.collider.gameObject.GetComponent<engineHolder>();
            if (eH2 != null && !eH2.full)
            {
                
                // Destroy this draggable object
                Destroy(this.gameObject);

                if (enginePrefab != null)
                {
                    GameObject e = Instantiate(enginePrefab, hit.point, Quaternion.identity);
                    dragManager d = dragManager.instance;
                    
                    //eH2.placeEngine(e);
                    WorldObjectInteract eI = e.GetComponent<WorldObjectInteract>();
                    if (eH == eH2)
                    {
                        eI.baseMat = baseMat;
                        eI.selectedMat = selectedMat;
                        if (eH.currentCommander == this.gameObject)
                        {
                            eH.currentCommander = e;
                        }
                    }
                    else if (baseMat != eI.baseMat)
                    {
                        if (baseMat == d.engineInteriorCommanderMaterial)
                        {
                            eI.baseMat = d.engineInteriorCommanderMaterial;
                            eI.selectedMat = d.engineInteriorCommanderMaterial;
                            
                            Debug.Log(d.engineInteriorCommanderMaterial);
                        }
                        else if (baseMat == d.engineSafetyOfficerMaterial)
                        {
                            eI.baseMat = d.engineSafetyOfficerMaterial;
                            eI.selectedMat = d.engineSafetyOfficerMaterial;
                            
                            Debug.Log(d.engineSafetyOfficerMaterial);
                        }
                        else
                        {
                            eH.insideOutsideMaterials[0] = d.baseInteriorMaterial;
                            eH.insideOutsideVis[0].material =  eH.insideOutsideMaterials[0];
                            eI.baseMat = d.engineInteriorMaterial;;
                            eI.selectedMat = d.selectedEngineInteriorMaterial;
                            currentIncident cI2 = d.tM.currentIncident;
                            int incidentIndex2 = cI2.activeEngines.IndexOf(SOindex);
                            cI2.addInfo($"{enginesSO.engineNames[SOindex]} is no longer commanding {currentArea}");
                            cI2.engineCommanderInfo[incidentIndex2] = null;
                        }
                        
                    }
                    currentIncident cI = d.tM.currentIncident;
                    int incidentIndex = cI.activeEngines.IndexOf(SOindex);
                    
                    eI.company = company;
                    eI.eH = eH2;
                    eI.currentArea = eH2.areaName;
                    eI.enginesSO = enginesSO;
                    eI.SOindex = SOindex;
                    eI.visRenderer.material = eI.selectedMat;
                    if (eH != eH2)
                    {
                        eH.companyNum -= company;
                        eH2.companyNum += company;
                        timeManager.instance.setTime(incidentIndex);
                        Debug.Log("Reset time from WOI");
                        eH.updateCompanyVis();
                        cI.addInfo($"{enginesSO.engineNames[SOindex]} moved to {eI.currentArea}");
                    }
                    if (this.gameObject == d.interiorCommander)
                    {
                        d.interiorCommander = e;
                    }
                    if (this.gameObject == d.safetyOfficer)
                    {
                        d.safetyOfficer = e;
                    }
                    eH2.placeEngine(e);
                    
                    
                    
                    
                    cI.engineHolderPositions[incidentIndex] = eI.currentArea;
                    
                    d.selectedEngineIndex = SOindex;
                    d.selectedEngine = eI.gameObject;
                    d.updateUI();
                    e.GetComponentInChildren<TMP_Text>().text = enginesSO.engineNames[SOindex];
                    
                    //Debug.Log("Placed Engine");
                    dragManager.instance.undoVis += eI.undoVis;
                    

                }
            }
            else
            {
                
                eH.placeEngine(this.gameObject);
                dragManager d = dragManager.instance;
                d.selectedEngineIndex = SOindex;
                d.selectedEngine = this.gameObject;
                d.updateUI();
                dragManager.instance.undoVis += this.undoVis;
                //Debug.Log("No suitable engine holder found or holder is full.");
            }
        }
        else
        {
            Debug.Log("No valid hit. Not placed.");
            dragManager d = dragManager.instance;
            d.selectedEngineIndex = SOindex;
            d.selectedEngine = this.gameObject;
            d.updateUI();
            eH.placeEngine(this.gameObject);
            dragManager.instance.undoVis += this.undoVis;
            // Optionally reset position or take another action
        }
    }
    
    public void ClearCommandStatus()
    {
        // grab singletons
        var dm = dragManager.instance;
        var tm = dm.tM;
        var cI = tm.currentIncident;

        // find our index & name
        int incidentIndex = cI.activeEngines.IndexOf(SOindex);
        string name = enginesSO.engineNames[SOindex];

        // 1) Incident Commander?
        if (dm.interiorCommander == gameObject)
        {
            dm.interiorCommander = null;
            cI.addInfo($"{name} is no longer Incident Commander");
            cI.engineCommanderInfo[incidentIndex] = null;
        }

        // 2) Safety Officer?
        if (dm.safetyOfficer == gameObject)
        {
            dm.safetyOfficer = null;
            cI.addInfo($"{name} is no longer Safety Chief Officer");
            cI.engineCommanderInfo[incidentIndex] = null;
        }

        // 3) Area Commander?
        if (eH.currentCommander == gameObject)
        {
            eH.currentCommander = null;
            cI.addInfo($"{name} is no longer commanding {eH.areaName}");
            cI.engineCommanderInfo[incidentIndex] = null;

            // reset the holder's visual to its default
            if (eH.insideOutsideVis.Length > 0 && eH.insideOutsideMaterials.Length > 0)
                eH.insideOutsideVis[0].material = eH.insideOutsideMaterials[0];
        }

        // 4) reset this engine's own visual
        
        selectedMat = dm.selectedEngineInteriorMaterial;
        baseMat = dm.engineInteriorMaterial;
        visRenderer.material = selectedMat;
        // 5) refresh any UI buttons, etc.
        dm.updateUI();
    }
    
    /// <summary>
    /// Shortcut to send this engine back to the UI list.
    /// </summary>
    public void ReturnToUI()
    {
        dragManager.instance.ReturnEngineToUI(this);
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
