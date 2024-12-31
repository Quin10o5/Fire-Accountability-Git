using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]

[CreateAssetMenu(fileName = "EnginesData", menuName = "Scriptables/Engines Data")]
public class enginesSO : ScriptableObject
{
    public string[] engineNames;
    public int[] enginePersonel;
 
}
