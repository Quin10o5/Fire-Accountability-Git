using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;  

public enum PdfOptions 
{
  CreatePdf,
  DownloadPdf,
  DownloadMultiplePdfs,
  LoadPdfInsideSceneWithContent,
  LoadPdfInsideSceneWithUrl,
  LoadPdfInsideWithFileName,
  LoadPdfNewPageWithContent,
  LoadPdfNewPageWithUrl,
  LoadPdfNewPageWithFileName,
  RemovePdfFromUnityScreen,
  CloseNewScreen,
  CreateNewPage,
  WriteTextToPdf,
  EndPdf,
  SharePdf,
  writeImageToPdf,
  getSavedPdfFilePath
}

public class PdfConverter : MonoBehaviour, IPdfDownloader {
	public PdfOptions myOption;
	private string sampleContent = "JVBERi0xLjMNCiXi48/TDQoNCjEgMCBvYmoNCjw8DQovVHlwZSAvQ2F0YWxvZw0K L091dGxpbmVzIDIgMCBSDQovUGFnZXMgMyAwIFINCj4+DQplbmRvYmoNCg0KMiAw IG9iag0KPDwNCi9UeXBlIC9PdXRsaW5lcw0KL0NvdW50IDANCj4+DQplbmRvYmoN Cg0KMyAwIG9iag0KPDwNCi9UeXBlIC9QYWdlcw0KL0NvdW50IDINCi9LaWRzIFsg NCAwIFIgNiAwIFIgXSANCj4+DQplbmRvYmoNCg0KNCAwIG9iag0KPDwNCi9UeXBl IC9QYWdlDQovUGFyZW50IDMgMCBSDQovUmVzb3VyY2VzIDw8DQovRm9udCA8PA0K L0YxIDkgMCBSIA0KPj4NCi9Qcm9jU2V0IDggMCBSDQo+Pg0KL01lZGlhQm94IFsw IDAgNjEyLjAwMDAgNzkyLjAwMDBdDQovQ29udGVudHMgNSAwIFINCj4+DQplbmRv YmoNCg0KNSAwIG9iag0KPDwgL0xlbmd0aCAxMDc0ID4+DQpzdHJlYW0NCjIgSg0K QlQNCjAgMCAwIHJnDQovRjEgMDAyNyBUZg0KNTcuMzc1MCA3MjIuMjgwMCBUZA0K KCBBIFNpbXBsZSBQREYgRmlsZSApIFRqDQpFVA0KQlQNCi9GMSAwMDEwIFRmDQo2 OS4yNTAwIDY4OC42MDgwIFRkDQooIFRoaXMgaXMgYSBzbWFsbCBkZW1vbnN0cmF0 aW9uIC5wZGYgZmlsZSAtICkgVGoNCkVUDQpCVA0KL0YxIDAwMTAgVGYNCjY5LjI1 MDAgNjY0LjcwNDAgVGQNCigganVzdCBmb3IgdXNlIGluIHRoZSBWaXJ0dWFsIE1l Y2hhbmljcyB0dXRvcmlhbHMuIE1vcmUgdGV4dC4gQW5kIG1vcmUgKSBUag0KRVQN CkJUDQovRjEgMDAxMCBUZg0KNjkuMjUwMCA2NTIuNzUyMCBUZA0KKCB0ZXh0LiBB bmQgbW9yZSB0ZXh0LiBBbmQgbW9yZSB0ZXh0LiBBbmQgbW9yZSB0ZXh0LiApIFRq DQpFVA0KQlQNCi9GMSAwMDEwIFRmDQo2OS4yNTAwIDYyOC44NDgwIFRkDQooIEFu ZCBtb3JlIHRleHQuIEFuZCBtb3JlIHRleHQuIEFuZCBtb3JlIHRleHQuIEFuZCBt b3JlIHRleHQuIEFuZCBtb3JlICkgVGoNCkVUDQpCVA0KL0YxIDAwMTAgVGYNCjY5 LjI1MDAgNjE2Ljg5NjAgVGQNCiggdGV4dC4gQW5kIG1vcmUgdGV4dC4gQm9yaW5n LCB6enp6ei4gQW5kIG1vcmUgdGV4dC4gQW5kIG1vcmUgdGV4dC4gQW5kICkgVGoN CkVUDQpCVA0KL0YxIDAwMTAgVGYNCjY5LjI1MDAgNjA0Ljk0NDAgVGQNCiggbW9y ZSB0ZXh0LiBBbmQgbW9yZSB0ZXh0LiBBbmQgbW9yZSB0ZXh0LiBBbmQgbW9yZSB0 ZXh0LiBBbmQgbW9yZSB0ZXh0LiApIFRqDQpFVA0KQlQNCi9GMSAwMDEwIFRmDQo2 OS4yNTAwIDU5Mi45OTIwIFRkDQooIEFuZCBtb3JlIHRleHQuIEFuZCBtb3JlIHRl eHQuICkgVGoNCkVUDQpCVA0KL0YxIDAwMTAgVGYNCjY5LjI1MDAgNTY5LjA4ODAg VGQNCiggQW5kIG1vcmUgdGV4dC4gQW5kIG1vcmUgdGV4dC4gQW5kIG1vcmUgdGV4 dC4gQW5kIG1vcmUgdGV4dC4gQW5kIG1vcmUgKSBUag0KRVQNCkJUDQovRjEgMDAx MCBUZg0KNjkuMjUwMCA1NTcuMTM2MCBUZA0KKCB0ZXh0LiBBbmQgbW9yZSB0ZXh0 LiBBbmQgbW9yZSB0ZXh0LiBFdmVuIG1vcmUuIENvbnRpbnVlZCBvbiBwYWdlIDIg Li4uKSBUag0KRVQNCmVuZHN0cmVhbQ0KZW5kb2JqDQoNCjYgMCBvYmoNCjw8DQov VHlwZSAvUGFnZQ0KL1BhcmVudCAzIDAgUg0KL1Jlc291cmNlcyA8PA0KL0ZvbnQg PDwNCi9GMSA5IDAgUiANCj4+DQovUHJvY1NldCA4IDAgUg0KPj4NCi9NZWRpYUJv eCBbMCAwIDYxMi4wMDAwIDc5Mi4wMDAwXQ0KL0NvbnRlbnRzIDcgMCBSDQo+Pg0K ZW5kb2JqDQoNCjcgMCBvYmoNCjw8IC9MZW5ndGggNjc2ID4+DQpzdHJlYW0NCjIg Sg0KQlQNCjAgMCAwIHJnDQovRjEgMDAyNyBUZg0KNTcuMzc1MCA3MjIuMjgwMCBU ZA0KKCBTaW1wbGUgUERGIEZpbGUgMiApIFRqDQpFVA0KQlQNCi9GMSAwMDEwIFRm DQo2OS4yNTAwIDY4OC42MDgwIFRkDQooIC4uLmNvbnRpbnVlZCBmcm9tIHBhZ2Ug MS4gWWV0IG1vcmUgdGV4dC4gQW5kIG1vcmUgdGV4dC4gQW5kIG1vcmUgdGV4dC4g KSBUag0KRVQNCkJUDQovRjEgMDAxMCBUZg0KNjkuMjUwMCA2NzYuNjU2MCBUZA0K KCBBbmQgbW9yZSB0ZXh0LiBBbmQgbW9yZSB0ZXh0LiBBbmQgbW9yZSB0ZXh0LiBB bmQgbW9yZSB0ZXh0LiBBbmQgbW9yZSApIFRqDQpFVA0KQlQNCi9GMSAwMDEwIFRm DQo2OS4yNTAwIDY2NC43MDQwIFRkDQooIHRleHQuIE9oLCBob3cgYm9yaW5nIHR5 cGluZyB0aGlzIHN0dWZmLiBCdXQgbm90IGFzIGJvcmluZyBhcyB3YXRjaGluZyAp IFRqDQpFVA0KQlQNCi9GMSAwMDEwIFRmDQo2OS4yNTAwIDY1Mi43NTIwIFRkDQoo IHBhaW50IGRyeS4gQW5kIG1vcmUgdGV4dC4gQW5kIG1vcmUgdGV4dC4gQW5kIG1v cmUgdGV4dC4gQW5kIG1vcmUgdGV4dC4gKSBUag0KRVQNCkJUDQovRjEgMDAxMCBU Zg0KNjkuMjUwMCA2NDAuODAwMCBUZA0KKCBCb3JpbmcuICBNb3JlLCBhIGxpdHRs ZSBtb3JlIHRleHQuIFRoZSBlbmQsIGFuZCBqdXN0IGFzIHdlbGwuICkgVGoNCkVU DQplbmRzdHJlYW0NCmVuZG9iag0KDQo4IDAgb2JqDQpbL1BERiAvVGV4dF0NCmVu ZG9iag0KDQo5IDAgb2JqDQo8PA0KL1R5cGUgL0ZvbnQNCi9TdWJ0eXBlIC9UeXBl MQ0KL05hbWUgL0YxDQovQmFzZUZvbnQgL0hlbHZldGljYQ0KL0VuY29kaW5nIC9X aW5BbnNpRW5jb2RpbmcNCj4+DQplbmRvYmoNCg0KMTAgMCBvYmoNCjw8DQovQ3Jl YXRvciAoUmF2ZSBcKGh0dHA6Ly93d3cubmV2cm9uYS5jb20vcmF2ZVwpKQ0KL1By b2R1Y2VyIChOZXZyb25hIERlc2lnbnMpDQovQ3JlYXRpb25EYXRlIChEOjIwMDYw MzAxMDcyODI2KQ0KPj4NCmVuZG9iag0KDQp4cmVmDQowIDExDQowMDAwMDAwMDAw IDY1NTM1IGYNCjAwMDAwMDAwMTkgMDAwMDAgbg0KMDAwMDAwMDA5MyAwMDAwMCBu DQowMDAwMDAwMTQ3IDAwMDAwIG4NCjAwMDAwMDAyMjIgMDAwMDAgbg0KMDAwMDAw MDM5MCAwMDAwMCBuDQowMDAwMDAxNTIyIDAwMDAwIG4NCjAwMDAwMDE2OTAgMDAw MDAgbg0KMDAwMDAwMjQyMyAwMDAwMCBuDQowMDAwMDAyNDU2IDAwMDAwIG4NCjAw MDAwMDI1NzQgMDAwMDAgbg0KDQp0cmFpbGVyDQo8PA0KL1NpemUgMTENCi9Sb290 IDEgMCBSDQovSW5mbyAxMCAwIFINCj4+DQoNCnN0YXJ0eHJlZg0KMjcxNA0KJSVF T0YNCg==";

	private string directoryPath;
	private string fileContent = "";
	private string CallbackGameObjectName = "UnityReceiveMessage";
  private string CallbackMethodName = "DownloadedPdf";
  private string btnTitle = "Close";
  private string pageTitle = "Pdf Loader";
  private int tempCount = 0;
  public Texture2D sampleImage;
  private string imageName = "PdfImage.png";

	string [] samplePdfArray = {"https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
							      					"http://www.africau.edu/images/default/sample.pdf",
							      					"https://www.clickdimensions.com/links/TestPDFfile.pdf",
							      					"https://www.orimi.com/pdf-test.pdf",
							      					"https://www.ets.org/Media/Tests/GRE/pdf/gre_research_validity_data.pdf"};

	// Use this for initialization
	void Start () {

	}

	void OnMouseUp() {
    	StartCoroutine(BtnAnimation());
 	}

 	private IEnumerator BtnAnimation()
    {
    	Vector3 originalScale = gameObject.transform.localScale;
        gameObject.transform.localScale = 0.9f * gameObject.transform.localScale;
        yield return new WaitForSeconds(0.2f);
        gameObject.transform.localScale = originalScale;
        ButtonAction();
    }

    private void ButtonAction() {
		switch(myOption) 
		{
		    case PdfOptions.CreatePdf:
		      PdfConverterBridgeIos.createPdf("PdfGeneratorPlugin", 792, 612);
		      break;
		    case PdfOptions.CreateNewPage:
		      PdfConverterBridgeIos.createNewPage();
		      break;
		    case PdfOptions.WriteTextToPdf:
		    	tempCount++;
		    	string tempColor = "#FF0000";
		    	float fontSize = 10;
		    	if (tempCount%3 == 0) {
		    		tempColor = "#00FF00";
		    		fontSize = 7;
		    	} else if (tempCount%3 == 1) {
		    		tempColor = "#0000FF";
		    		fontSize = 13;
		    	}
		      PdfConverterBridgeIos.writeTextToPdf("Name:Mayank", 200, 200, 100, 60, tempColor, fontSize, 2);
		      tempColor = "#FF00FF";
		      fontSize = 12;
		      PdfConverterBridgeIos.writeTextToPdf("  EmpId:4839", 300, 200, 200, 60, tempColor, fontSize, 3);
		      break;
		    case PdfOptions.EndPdf:
		      PdfConverterBridgeIos.endPdf();
		      break;
		    case PdfOptions.SharePdf:
		      PdfConverterBridgeIos.sharePdfFile();
		      break;
		    case PdfOptions.writeImageToPdf:
		      string imagePathTemp = PdfConverterBridgeIos.SaveTextutreToApplicationPathAndGetPath(sampleImage, imageName);
		      PdfConverterBridgeIos.writeImageToPdf(imagePathTemp, 50, 200, 100, 100);
		      break;
		    case PdfOptions.DownloadPdf:
		      PdfConverterBridgeIos.SetCallBackMethod(CallbackGameObjectName, CallbackMethodName);
		      PdfConverterBridgeIos.downloadPdf("http://www.africau.edu/images/default/sample.pdf", this);		      
		      break;
		    case PdfOptions.DownloadMultiplePdfs:
		      PdfConverterBridgeIos.SetCallBackMethod(CallbackGameObjectName, CallbackMethodName);
		      for (int i = 0; i < samplePdfArray.Length; i++) {
		      	PdfConverterBridgeIos.downloadPdf(samplePdfArray[i], this);	
		      }
		      break;
		    case PdfOptions.LoadPdfInsideSceneWithContent:
		      PdfConverterBridgeIos.loadPdfInsideUnityWithContent(sampleContent,140,200,200,200);
		      break;
		    case PdfOptions.LoadPdfInsideSceneWithUrl:
		      System.Random rand = new System.Random();  
			  int index = rand.Next(samplePdfArray.Length);
		      PdfConverterBridgeIos.loadPdfInsideUnityWithUrl(samplePdfArray[index],140,200,200,200);
		      break;
		    case PdfOptions.LoadPdfInsideWithFileName:
		      PdfConverterBridgeIos.loadPdfInsideUnityWithFileName("PdfGeneratorPlugin.pdf",140,200,200,200);
		      break;
		    case PdfOptions.LoadPdfNewPageWithContent:
		      PdfConverterBridgeIos.loadPdfInNewPageyWithContent(sampleContent, btnTitle, pageTitle);
		      break;
		    case PdfOptions.LoadPdfNewPageWithUrl:
		      System.Random rand1 = new System.Random();  
			  int index1 = rand1.Next(samplePdfArray.Length);
		      PdfConverterBridgeIos.loadPdfInNewPageWithUrl(samplePdfArray[index1], btnTitle, pageTitle);
		      break;
		    case PdfOptions.LoadPdfNewPageWithFileName:
		      PdfConverterBridgeIos.loadPdfInNewPageWithFileName("PdfGeneratorPlugin.pdf", btnTitle, pageTitle);
		      break;
		    case PdfOptions.RemovePdfFromUnityScreen:
		      PdfConverterBridgeIos.removePdfFromUnityScreen();
		      break;
		    case PdfOptions.CloseNewScreen:
		      PdfConverterBridgeIos.removePdfFromNewScreen();
		      break;
		    case PdfOptions.getSavedPdfFilePath:
		      PdfConverterBridgeIos.SetCallBackMethod(CallbackGameObjectName, CallbackMethodName);
		      PdfConverterBridgeIos.getSavedPdfFilePath();
		      break;
		}
    }

    public void downloadComplete(string fileUrl, string fileContentTemp, string errorMessage) {
    	if (errorMessage != null || errorMessage.Length > 0) {
    		return;
    	}
    	this.fileContent = fileContentTemp;
    	PdfConverterBridgeIos.loadPdfInNewPageyWithContent(fileContentTemp, btnTitle, pageTitle);
    }
}
