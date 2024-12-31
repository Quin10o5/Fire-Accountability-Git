using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System;

public interface IPdfDownloader
{
    public void downloadComplete(string fileUrl, string fileContent, string errorMessage);
}

public class PdfConverterBridgeIos {


	public static PdfConverterBridgeIos sharedInstance = null;
	// private List<IImageDownloader> downloaders = new List<IImageDownloader>();

	private IDictionary<string, IPdfDownloader> downloadersMap = new Dictionary<string, IPdfDownloader>();

	public static PdfConverterBridgeIos getInstance() {
		if (sharedInstance == null) {
			sharedInstance = new PdfConverterBridgeIos();
		}
		return sharedInstance;
	}

	private PdfConverterBridgeIos() {}


	[DllImport("__Internal")]
	private static extern void _createPdf(string fileName,string pageHeight,string pageWidth);
	public static void createPdf(string fileName,float pageHeight,float pageWidth){
		fileName += ".pdf";
		_createPdf(fileName, pageHeight.ToString(), pageWidth.ToString());
	}

	[DllImport("__Internal")]
	private static extern void _createNewPage();
	public static void createNewPage(){
		_createNewPage();
	}

	[DllImport("__Internal")]
	private static extern void _writeTextToPdf(string data,string xPos,string yPos,string width,string height,string hexColor,string fontSize, string fontType);
	public static void writeTextToPdf(string data,float xPos,float yPos,float width,float height,string hexColor,float fontSize, int fontType){
		_writeTextToPdf(data, xPos.ToString(), yPos.ToString(), width.ToString(), height.ToString(), hexColor, fontSize.ToString(), fontType.ToString());
	}

	[DllImport("__Internal")]
	private static extern void _endPdf();
	public static void endPdf(){
		_endPdf();
	}

	[DllImport("__Internal")]
	private static extern void _getSavedFileFullPath();
	public static void getSavedPdfFilePath(){
		_getSavedFileFullPath();
	}

	[DllImport("__Internal")]
	private static extern void _sharePdfFile();
	public static void sharePdfFile(){
		_sharePdfFile();
	}

	[DllImport("__Internal")]
	private static extern void _writeImageToPdf(string imagePath,string xPos,string yPos,string imageWidth,string imageHeight);
	public static void writeImageToPdf(string imagePath,float xPos,float yPos, float imageWidth, float imageHeight){
		_writeImageToPdf(imagePath, xPos.ToString(), yPos.ToString(), imageWidth.ToString(), imageHeight.ToString());
	}

	public static string SaveTextutreToApplicationPathAndGetPath(Texture2D texture, string imageName) {
		string path = Application.persistentDataPath;
		string savePath = path + "/" + imageName;
		File.WriteAllBytes(savePath, texture.EncodeToPNG());
		return savePath;
	}

	[DllImport("__Internal")]
	private static extern void _loadPdfInsideUnityWithUrl(string fileUrl,string viewOriginX,string viewOriginY,string viewWidth,string viewHeight);
	public static void loadPdfInsideUnityWithUrl(string fileUrl,float viewOriginX,float viewOriginY,float viewWidth,float viewHeight){
		_loadPdfInsideUnityWithUrl(fileUrl, viewOriginX.ToString(), viewOriginY.ToString(),viewWidth.ToString(),viewHeight.ToString());
	}

	[DllImport("__Internal")]
	private static extern void _loadPdfInsideUnityWithContent(string fileContent,string viewOriginX,string viewOriginY,string viewWidth,string viewHeight);
	public static void loadPdfInsideUnityWithContent(string fileContent,float viewOriginX,float viewOriginY,float viewWidth,float viewHeight){
		_loadPdfInsideUnityWithContent(fileContent, viewOriginX.ToString(), viewOriginY.ToString(),viewWidth.ToString(),viewHeight.ToString());
	}

	[DllImport("__Internal")]
	private static extern void _loadPdfInsideUnityWithFileName(string fileName,string viewOriginX,string viewOriginY,string viewWidth,string viewHeight);
	public static void loadPdfInsideUnityWithFileName(string fileName,float viewOriginX,float viewOriginY,float viewWidth,float viewHeight){
		_loadPdfInsideUnityWithFileName(fileName, viewOriginX.ToString(), viewOriginY.ToString(),viewWidth.ToString(),viewHeight.ToString());
	}


	[DllImport("__Internal")]
	private static extern void _loadPdfInNewPageWithUrl(string fileUrl, string btnTitle, string title);
	public static void loadPdfInNewPageWithUrl(string fileUrl, string btnTitle, string title){
		_loadPdfInNewPageWithUrl(fileUrl, btnTitle, title);
	}

	[DllImport("__Internal")]
	private static extern void _loadPdfInNewPageWithContent(string fileContent, string btnTitle, string title);
	public static void loadPdfInNewPageyWithContent(string fileContent, string btnTitle, string title){
		_loadPdfInNewPageWithContent(fileContent, btnTitle, title);
	}

	[DllImport("__Internal")]
	private static extern void _loadPdfInNewPageWithFileName(string fileName, string btnTitle, string title);
	public static void loadPdfInNewPageWithFileName(string fileName, string btnTitle, string title){
		_loadPdfInNewPageWithFileName(fileName, btnTitle, title);
	}


	[DllImport("__Internal")]
	private static extern void _downloadPdf(string fileUrl);
	public static void downloadPdf(string fileUrl, IPdfDownloader handler){
		PdfConverterBridgeIos instance = PdfConverterBridgeIos.getInstance();
		bool keyExists = instance.downloadersMap.ContainsKey(fileUrl);
		if (keyExists) {
			return;
		}
		instance.downloadersMap.Add(fileUrl, handler);
		_downloadPdf(fileUrl);
	}

	[DllImport("__Internal")]
	private static extern void _removePdfFromUnityScreen();
	public static void removePdfFromUnityScreen(){
		_removePdfFromUnityScreen();
	}

	[DllImport("__Internal")]
	private static extern void _removePdfFromNewScreen();
	public static void removePdfFromNewScreen(){
		_removePdfFromNewScreen();
	}

	[DllImport("__Internal")]
	private static extern void _setCallBackMethod(string msgReceivingGameObjectName,string msgReceivingMethodName);
	public static void SetCallBackMethod(string msgReceivingGameObjectName,string msgReceivingMethodName)
	{
		if (Application.isEditor) {
			return;
		}
		#if UNITY_IPHONE
			_setCallBackMethod( msgReceivingGameObjectName, msgReceivingMethodName);
		#endif
	}

//=================================================================================================================================================

	public void DownloadCompleteNotify(string resultString)
	{
		string[] subs = resultString.Split(new string[] { "#######" }, System.StringSplitOptions.None);
		string pdfStringOrError = subs[1];
		string fileUrl = subs[2];
		bool keyExists = downloadersMap.ContainsKey(fileUrl);
		if (!keyExists) {
			return;
		}
		IPdfDownloader handlerTemp = downloadersMap[fileUrl];
		if (string.Compare(subs[0], "Success") == 0) {
			handlerTemp.downloadComplete(fileUrl, subs[1], "");
		} else {
			handlerTemp.downloadComplete(fileUrl, null, pdfStringOrError);
		}
		PdfConverterBridgeIos instance = PdfConverterBridgeIos.getInstance();
		instance.downloadersMap.Remove(fileUrl);
	}
}
