using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [Header("Degugging")] public bool testing = false;
    public bool verbose = false;
    public int testingFloorNum = 2;

    [Header("Building Prefabs")] public GameObject floorPrefab;
    public GameObject atticPrefab;
    public GameObject roofPrefab;

    [Header("Settings")] public currentIncident incident;
    public Transform spawnPoint;
    public int floorNum = -1;
    public float yOffset = 0.02f;
    public float extraOffset = 0.03f;

    [Header("Runtime")]
    [ReadOnly] public List<engineHolder> buildingHolders = new List<engineHolder>();
    [ReadOnly] public float buildingHeight;
    // private
    private float startY;
    private float floorAddY;
    private float currentY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(testing) SpawnBuilding(testingFloorNum);
        else
        {
            SpawnBuilding(incident.floorNum);
        }
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
        buildingHeight = currentY;
        SpawnFloor(roofPrefab, $"Roof", true);
    }

    [ContextMenu("Add Floor")]
    public void AddFloor()
    {
        if (floorNum >= 99)
        {
            TryLog("Removing floor num cannot be greater than 99");
            return;
        }
        floorNum++;
        float offset = floorPrefab.GetComponent<engineHolder>().selectionRenderer.bounds.size.y + yOffset;
        floorAddY += offset;
        buildingHeight += offset;
        Vector3 spawnPos = new Vector3(spawnPoint.position.x, floorAddY, spawnPoint.position.z);
        TryLog("Adding floor: " + floorNum + " at yLevel: " + floorAddY);
        GameObject holder = Instantiate(floorPrefab, spawnPos, Quaternion.identity);
        holder.transform.parent = transform;
        engineHolder engineHolder = holder.GetComponent<engineHolder>();
        engineHolder.areaName = $"Floor {floorNum}"; 
        buildingHolders.Insert(buildingHolders.Count - 2, engineHolder);
        // move up roof and attic
        buildingHolders[buildingHolders.Count - 1].transform.position += Vector3.up * offset;
        buildingHolders[buildingHolders.Count - 2].transform.position += Vector3.up * offset;
        incident.floorNum = floorNum;
    }
    
    [ContextMenu("Remove Floor")]
    public void RemoveFloor()
    {
        if (floorNum <= 1)
        {
            TryLog("Removing floor num cannot be less than 1");
            return;
        }
        
        float offset = floorPrefab.GetComponent<engineHolder>().selectionRenderer.bounds.size.y + yOffset;
        floorAddY -= offset;
        buildingHeight -= offset;
        TryLog("Removing floor: " + floorNum);
        Destroy(buildingHolders[floorNum].gameObject);
        buildingHolders.RemoveAt(floorNum);
        floorNum--;
        // move down roof and attic
        buildingHolders[buildingHolders.Count - 1].transform.position += -Vector3.up * offset;
        buildingHolders[buildingHolders.Count - 2].transform.position += -Vector3.up * offset;
        incident.floorNum = floorNum;
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
