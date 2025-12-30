using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Degugging")]
    public bool testing = false;
    public bool verbose = false;
    public int testingFloorNum = 2;
    
    [Header("Building Prefabs")] 
    public GameObject floorPrefab;
    public GameObject atticPrefab;
    public GameObject roofPrefab;

    [Header("Settings")] 
    public Transform spawnPoint;
    public int floorNum = -1;
    public float yOffset = 0.02f;
    public float extraOffset = 0.03f;
    
    [Header("Runtime")]
    public List<engineHolder> buildingHolders = new List<engineHolder>();
    // private
    private float startY;
    private float floorAddY;
    private float currentY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(testing) SpawnBuilding(testingFloorNum);
    }

    public void SpawnBuilding(int startFloorNum)
    {
        if (startFloorNum < 1)
        {
            TryLog("Starting floor num cannot be less then 1");
            floorNum = 1;
        }
        floorNum = startFloorNum;
        startY = spawnPoint.position.y;
        currentY = startY;
        TryLog("Starting floor: " + floorNum);
        TryLog("Starting yPos: " + startY);
        SpawnFloor(floorPrefab, "Basement");
        int spawningIndex = 1;
        for (int i = spawningIndex; i < floorNum + 1; i++)
        {
            SpawnFloor(floorPrefab, $"Floor {i}");
        }

        floorAddY = currentY;
        SpawnFloor(atticPrefab, $"Attic", true);
        SpawnFloor(roofPrefab, $"Roof", true);
    }

    [ContextMenu("Add Floor")]
    public void AddFloor()
    {
        floorNum++;
        float offset = floorPrefab.GetComponent<engineHolder>().selectionRenderer.bounds.size.y + yOffset;
        floorAddY += offset;
        Vector3 spawnPos = new Vector3(spawnPoint.position.x, floorAddY, spawnPoint.position.z);
        TryLog("Adding floor: " + floorNum + " at yLevel: " + floorAddY);
        GameObject holder = Instantiate(floorPrefab, spawnPos, Quaternion.identity);
        holder.transform.parent = transform;
        engineHolder engineHolder = holder.GetComponent<engineHolder>();
        engineHolder.areaName = $"Floor {floorNum}";
        buildingHolders.Insert(buildingHolders.Count - 3, engineHolder);
        // move up roof and attic
        buildingHolders[buildingHolders.Count - 1].transform.position += Vector3.up * offset;
        buildingHolders[buildingHolders.Count - 2].transform.position += Vector3.up * offset;
    }

    void SpawnFloor(GameObject prefab, string holderName, bool useExtraOffset = false)
    {
        if(holderName != "Basement")currentY += prefab.GetComponent<engineHolder>().selectionRenderer.bounds.size.y + yOffset;
        if(useExtraOffset) currentY += extraOffset;
        if(holderName == "Roof") currentY += extraOffset * .5f;
        Vector3 spawnPos = new Vector3(spawnPoint.position.x, currentY, spawnPoint.position.z);
        TryLog("Spawning floor: " + holderName + " at yLevel: " + currentY);
        GameObject holder = Instantiate(prefab, spawnPos, Quaternion.identity);
        holder.transform.parent = transform;
        engineHolder engineHolder = holder.GetComponent<engineHolder>();
        buildingHolders.Add(engineHolder);
        engineHolder.areaName = holderName;
        
        
    }

    private void TryLog(string input)
    {
        if (verbose) Debug.Log(input);
    }
}
