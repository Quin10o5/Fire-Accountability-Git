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
            engineTimeText.text = d.selectedEngine.GetComponent<colorTimer>().displayTimer;
        }
    }

    /// <summary>
    /// Saves all the text in `currentIncident.info` to a PDF and displays it in the Unity scene.
    /// </summary>
    public void SaveAndDisplayIncidentInfoInPdf()
    {
#if UNITY_IOS
        // Step 1: Generate the PDF
        PdfConverterBridgeIos.createPdf(generatedPdfFileName, 792, 612);
        PdfConverterBridgeIos.createNewPage();

        // Concatenate all text from `currentIncident.info` into a single string
        string concatenatedIncidentInfo = string.Join("\n", currentIncident.info.ToArray());

        // Write the text to the PDF
        PdfConverterBridgeIos.writeTextToPdf(concatenatedIncidentInfo, 200, 200, 400, 400, "#000000", 12, 1);

        // Finalize the PDF
        PdfConverterBridgeIos.endPdf();

        // Step 2: Display the generated PDF in the Unity scene
        PdfConverterBridgeIos.loadPdfInsideUnityWithFileName(generatedPdfFileName, 100, 100, 500, 500);

#else
        Debug.LogWarning("PDF creation and display is only supported on iOS.");
#endif
    }

    public void savePDF()
    {
#if UNITY_IOS
        PdfConverterBridgeIos.createPdf("testPDF", 792, 612);
        PdfConverterBridgeIos.createNewPage();
        string concatenatedString = string.Join("\n", currentIncident.info.ToArray());
        PdfConverterBridgeIos.writeTextToPdf(concatenatedString, 200, 200, 100, 600, "#000000", 12, 2);
        PdfConverterBridgeIos.endPdf();
#else
        Debug.LogWarning("PDF generation is only available on iOS.");
#endif
    }

    public void downloadComplete(string fileUrl, string fileContentTemp, string errorMessage)
    {
        if (errorMessage != null || errorMessage.Length > 0)
        {
            return;
        }
    }
}