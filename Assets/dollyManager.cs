using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

/// <summary>
/// Manages Cinemachine dolly carts for touch-driven vertical (Y) and horizontal (X) camera movement,
/// with adjustable clamping and adaptive X-axis speed adjustment based on proximity to center.
/// </summary>
public class DollyManager : MonoBehaviour
{
    public BuildingManager buildingManager;
    public Scrollbar scrollbar;
    [Header("Cinemachine Dolly Carts")]
    [Tooltip("Dolly cart controlling vertical movement (Y axis)")]
    [SerializeField] private CinemachineDollyCart dollyY;

    [SerializeField] private CinemachineSmoothPath dollySpline;
    



    
// internal velocity tracker for SmoothDamp
    private float clampXVel = 0f;
    private float knownBuildingHeight = -1f;


    void Start()
    {
        scrollbar.value = .3f;
    }
    
    /// <summary>
    /// Update is called once per frame: process input, move dolly carts, clamp, and adapt X speed.
    /// </summary>
    private void Update()
    {
        UpdateScrollHeight();
    }

    public void ScrollBarValueChanged()
    {
        float u = scrollbar.value;
        float v = Qtils.Remap(u, 0f, 1f, 0f, .895f);
        dollyY.m_Position = v;
    }

    private void UpdateScrollHeight() 
    {
        if(knownBuildingHeight == buildingManager.buildingHeight) return;
        knownBuildingHeight = buildingManager.buildingHeight;
        if(knownBuildingHeight > 2 && !scrollbar.gameObject.activeInHierarchy) scrollbar.gameObject.SetActive(true);
        else if(knownBuildingHeight < 2 && scrollbar.gameObject.activeInHierarchy)
        {
            scrollbar.gameObject.SetActive(false);
            scrollbar.value = .3f;
        }
        dollySpline.m_Waypoints[1].position = new Vector3(0, 0, knownBuildingHeight  -1);
        scrollbar.value = dollyY.m_Position;
    }


    




}
