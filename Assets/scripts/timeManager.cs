using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using System.ComponentModel;

public class timeManager : MonoBehaviour, IPdfDownloader
{
    public currentIncident currentIncident;
    public enginesSO eSO;
    public float startingTime;
    public TMP_Text timeText;
    public TMP_Text engineTimeText;
    [Range(.01f, 1)]
    public float timeIncrement;
    public float currentTime;
    public static timeManager instance;
    [SerializeField] private string startTimeString = "2024-12-26T23:00Z";
    private DateTime startTime;
    private dragManager d;
    public TMP_Text logText;

    private string generatedPdfFileName = "CurrentIncidentInfo.pdf";

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        d = dragManager.instance;
        currentIncident.startTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        startTime = DateTime.Parse(currentIncident.startTime, null,
            System.Globalization.DateTimeStyles.RoundtripKind);
    }

    public void setTime(int cIndex)
    {
        currentIncident.engineTimes[cIndex] = DateTime.Now.ToLongTimeString();
    }

    public void resetSelectedTime()
    {
        colorTimer c = d.selectedEngine.GetComponent<colorTimer>();
        currentIncident.addInfo($"{eSO.engineNames[d.selectedEngine.GetComponent<WorldObjectInteract>().SOindex]}'s time was reset from {c.displayTimer}");
        timeManager.instance.setTime(c.incidentIndex);
        c.setTime();
    }

    void Update()
    {
        TimeSpan elapsed = DateTime.UtcNow - startTime;

        // Format the TimeSpan into HH:MM:SS (or whatever you prefer)
        string hours = elapsed.Hours.ToString("00");
        string minutes = elapsed.Minutes.ToString("00");
        string seconds = elapsed.Seconds.ToString("00");

        // Update the UI text
        if (timeText != null)
        {
            timeText.text = $"{hours}:{minutes}:{seconds}";
        }

        if (engineTimeText != null && d.selectedEngine != null)
        {
            if (engineTimeText.gameObject.activeSelf == false) engineTimeText.gameObject.SetActive(true);
            engineTimeText.text = d.selectedEngine.GetComponent<colorTimer>().displayTimer;
        }
        else if (engineTimeText != null && d.selectedEngine == null)
        {
            engineTimeText.gameObject.SetActive(false);
        }
    }
    

    /// <summary>
    /// Saves all the text in `currentIncident.info` to a PDF and displays it in the Unity scene.
    /// </summary>

    public void ShareTextFile(string filePath, string subject, string message)
    {
        new NativeShare()
            .AddFile(filePath)
            .SetSubject(subject)
            .SetText(message)
            .Share(); // This will bring up the native share sheet on iOS
    }

    public void savePDF()
    {
        string concatInfo = $"INCIDENT INFO ON {DateTime.Now.ToString()}:\n\n";
        if (d.interiorCommander != null)
        {
            concatInfo += $"Ending Incident Commander {eSO.engineNames[d.interiorCommander.GetComponent<WorldObjectInteract>().SOindex]}\n";
        }

        if (d.safetyOfficer != null)
        {
            concatInfo += $"Ending Saftey Officer {eSO.engineNames[d.safetyOfficer.GetComponent<WorldObjectInteract>().SOindex]}\n";
        }
        
        concatInfo += "\n----------------------------------------\n";
        concatInfo  += string.Join("\n", currentIncident.info.ToArray());
        string path = System.IO.Path.Combine(Application.persistentDataPath, "incident.txt");

        System.IO.File.WriteAllText(path, concatInfo);

        Debug.Log($"Saved file at: {path}");
        ShareTextFile(path, "Incident Info", "Download your incident info here:");
    }

    public void downloadComplete(string fileUrl, string fileContentTemp, string errorMessage)
    {
        if (errorMessage != null || errorMessage.Length > 0)
        {
            return;
        }
    }
}