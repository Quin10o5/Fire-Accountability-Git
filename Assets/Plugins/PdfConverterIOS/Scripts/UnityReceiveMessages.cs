using UnityEngine;
using System.Collections;
using System.IO;
public class UnityReceiveMessages : MonoBehaviour {
	public static UnityReceiveMessages Instance;
	string directoryPath;
	string fileList;
	void Awake(){
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		directoryPath = "Test";
	}

	// Update is called once per frame
	void Update () {
	
	}
	public void pdfFilePath(string pathToPdfDirectory) {
		directoryPath = pathToPdfDirectory;
	}

	public void listOfFile(string listOfFiles){
		fileList = listOfFiles;
	}

	public string FileListGetter(){
		return fileList;
	}

	public string DirectoryPathGetter(){
		return directoryPath;
	}

	public void DownloadedPdf(string result){
		PdfConverterBridgeIos downloaderObj = PdfConverterBridgeIos.getInstance();
		downloaderObj.DownloadCompleteNotify(result);
	}
}
