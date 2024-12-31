//
//  PdfConverter.mm
//  PdfConverterPlugin
//
//  Created by Mayank Gupta on 19/07/17.
//  Copyright (c) 2017 Mayank Gupta. All rights reserved.
//

#import "PdfConverter.h"
#import "PdfCacheWrapper.h"
#import "WebViewVC.h"

#define PDF_ORIGIN_X 0
#define PDF_ORIGIN_Y 0
#define PDF_DEFAULT_PAGE_NUMBER 1
#define PDF_DEFAULT_PAGE_PREFIX @""

@interface PdfConverter() {
    NSString *fileName;
    NSString *filePath;
    NSString *pageNumberPrefixString;
    bool enablePageNumber;
    NSInteger initialPageNumber;
    float pdfPageHeight;
    float pdfPageWidth;
    float pdfPageMarginFromTop;
    float pdfPageMarginFromBottom;
    float pdfPageMarginFromLeft;
    float pdfPageMarginFromRight;
    float pdfPageNumberMaxSize;
    UnityAppController *objectUnityAppController;
    WKWebView *pdfLoadWebView;
    UIViewController *presentedViewController;
    
    PdfCacheWrapper *pdfCacheWrapper;
    NSString *msgReceivingGameObjectNameGlobal;
    NSString *msgReceivingMethodtNameGlobal;
    
    CGContextRef pdfContext;
    NSMutableData *pdfData;
}
 
@end

@implementation PdfConverter
#pragma mark Unity bridge
    
int count = 1;

+ (PdfConverter *)pluginSharedInstance {
    static PdfConverter *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[PdfConverter alloc] init];
        sharedInstance->pdfCacheWrapper = [PdfCacheWrapper sharedInstance];
    });
    return sharedInstance;
}



#pragma mark Ios Methods

- (void) createPdfWithFileName:(NSString *)fileNameTemp
                 pdfPageHeight:(NSString *)pdfPageHeightTemp
                  pdfPageWidth:(NSString *)pdfPageWidthTemp {
    fileName = fileNameTemp;
    NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
    pdfPageHeight = [numberFormatter numberFromString:pdfPageHeightTemp].floatValue;
    pdfPageWidth = [numberFormatter numberFromString:pdfPageWidthTemp].floatValue;
    CGRect rct = {{0.0 , 0.0 } , {pdfPageWidth , pdfPageHeight}};
    pdfData = [[NSMutableData alloc]init];
    UIGraphicsBeginPDFContextToData(pdfData, rct, nil);
    pdfContext = nil;
    pdfContext = UIGraphicsGetCurrentContext();
}

- (void) createNewPage {
    UIGraphicsBeginPDFPage();
}

-(void) writeDataToPdfContext:(NSString *) data
          content_X_Cordinate:(NSString *) xPosition
          content_Y_Cordinate:(NSString *) yPosition
               containerWidth:(NSString *) width
              containerHeight:(NSString *) height
                        color:(NSString *) hexColor
                     fontSize:(NSString *) fontSize
                     fontType:(NSString *) fontType{
    data = @"1. Sergey Ermolov  Accept 60% \n 2 Сергей Ермилов Lose 30% \n3 Sayat Rakyl Acept  80%";
    CGContextSaveGState(pdfContext);
    NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
    float xPos = [numberFormatter numberFromString:xPosition].floatValue;
    float yPos = [numberFormatter numberFromString:yPosition].floatValue;
    CGContextConcatCTM(pdfContext, CGAffineTransformMakeTranslation(xPos, yPos));
    float rectWidth = [numberFormatter numberFromString:width].floatValue;
    float rectHeight = [numberFormatter numberFromString:height].floatValue;
    CGRect rect = CGRectMake(0, 0, rectWidth, rectHeight);
    UITextView *textView = [[UITextView alloc] initWithFrame:rect];
    textView.text = data;
    [textView setBackgroundColor:[UIColor whiteColor]];
    [textView setTextColor:[self colorFromHexString:hexColor]];
    int fontTypeTemp = [numberFormatter numberFromString:fontType].intValue;
    [textView setFont:[UIFont systemFontOfSize:[numberFormatter numberFromString:fontSize].floatValue]];
    if (fontTypeTemp == 2) {
        [textView setFont:[UIFont boldSystemFontOfSize:[numberFormatter numberFromString:fontSize].floatValue]];
    } else if (fontTypeTemp == 3) {
        [textView setFont:[UIFont italicSystemFontOfSize:[numberFormatter numberFromString:fontSize].floatValue]];
    }
    [textView.layer renderInContext:pdfContext];
    CGContextRestoreGState(pdfContext);
}

-(void) writeImageToPdfContext:(NSString *) filePath
           content_X_Cordinate:(NSString *) xPosition
           content_Y_Cordinate:(NSString *) yPosition
                containerWidth:(NSString *) width
               containerHeight:(NSString *) height{
    CGContextSaveGState(pdfContext);
    NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
    float xPos = [numberFormatter numberFromString:xPosition].floatValue;
    float yPos = [numberFormatter numberFromString:yPosition].floatValue;
    CGContextConcatCTM(pdfContext, CGAffineTransformMakeTranslation(xPos, yPos));
    float rectWidth = [numberFormatter numberFromString:width].floatValue;
    float rectHeight = [numberFormatter numberFromString:height].floatValue;
    CGRect rect = CGRectMake(0, 0, rectWidth, rectHeight);
    UIImageView *imageView = [[UIImageView alloc] initWithFrame:rect];
    imageView.image = [UIImage imageWithContentsOfFile:filePath];
    [imageView.layer renderInContext:pdfContext];
    CGContextRestoreGState(pdfContext);
}


-(void) sharePdfFile {
    objectUnityAppController = GetAppController();
    NSData *pdfData = [NSData dataWithContentsOfFile:[self getPDFFilePathWithFileName: fileName]];
    UIActivityViewController *activityViewController = [[UIActivityViewController alloc] initWithActivityItems:@[@"Share Pdf", pdfData] applicationActivities:nil];

    [objectUnityAppController.rootViewController presentViewController:activityViewController animated:YES completion:nil];
}

- (UIColor *)colorFromHexString:(NSString *)hexString {
    unsigned rgbValue = 0;
    NSScanner *scanner = [NSScanner scannerWithString:hexString];
    [scanner setScanLocation:1]; // bypass '#' character
    [scanner scanHexInt:&rgbValue];
    return [UIColor colorWithRed:((rgbValue & 0xFF0000) >> 16)/255.0 green:((rgbValue & 0xFF00) >> 8)/255.0 blue:(rgbValue & 0xFF)/255.0 alpha:1.0];
}

-(void) endPdf {
    UIGraphicsEndPDFContext();
}

-(void) writePdfToFile {
    [pdfData  writeToFile:[self getPDFFileName] atomically:YES] ;
}

- (void) getSavedPdfPath {
    const char *fullFilePath = [[self getPDFFileName] cStringUsingEncoding:NSASCIIStringEncoding];
    const char *gameObjectName = [msgReceivingGameObjectNameGlobal cStringUsingEncoding:NSASCIIStringEncoding];
    const char *methodName = [msgReceivingMethodtNameGlobal cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage(gameObjectName,methodName, fullFilePath);
}

- (NSString *) getPDFFileName {
    NSArray* documentDirectories = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask,YES);
    NSString* documentDirectory = [documentDirectories objectAtIndex:0];
    NSString *pdfFileName = [documentDirectory stringByAppendingPathComponent:fileName];
    filePath = pdfFileName;
    return pdfFileName;
}

- (NSString *) getPDFFilePathWithFileName:(NSString *)tempFilename {
    NSArray* documentDirectories = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask,YES);
    NSString* documentDirectory = [documentDirectories objectAtIndex:0];
    NSString *pdfFileName = [documentDirectory stringByAppendingPathComponent:tempFilename];
    filePath = pdfFileName;
    return pdfFileName;
}

- (void) getPDFFilePath:(NSString *)msgReceivingGameObjectName
                       :(NSString *)msgReceivingMethodtName {
    NSArray* documentDirectories = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask,YES);
    NSString* documentDirectory = [documentDirectories objectAtIndex:0];
    const char *directoryPath = [documentDirectory cStringUsingEncoding:NSASCIIStringEncoding];
    const char *gameObjectName = [msgReceivingGameObjectName cStringUsingEncoding:NSASCIIStringEncoding];
    const char *methodName = [msgReceivingMethodtName cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage(gameObjectName,methodName, directoryPath);
}

- (NSString *) getPDFFilePath {
    NSArray* documentDirectories = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask,YES);
    NSString* documentDirectory = [documentDirectories objectAtIndex:0];
    return documentDirectory;
}

-(void)getListFileAtPath:(NSString *)path
                        :(NSString *)msgReceivingGameObjectName
                        :(NSString *)msgReceivingMethodtName {
    int count;
    NSString *fileNameList = @"";
    NSArray *directoryContent = [[NSFileManager defaultManager] contentsOfDirectoryAtPath:path error:NULL];
    for (count = 0; count < (int)[directoryContent count]; count++){
         fileNameList = [fileNameList stringByAppendingString: [NSString stringWithFormat:@"%@/",  [directoryContent objectAtIndex:count]]];
    }
    const char *fileNameListString = [fileNameList cStringUsingEncoding:NSASCIIStringEncoding];
    const char *gameObjectName = [msgReceivingGameObjectName cStringUsingEncoding:NSASCIIStringEncoding];
    const char *methodName = [msgReceivingMethodtName cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage(gameObjectName,methodName, fileNameListString);
}

-(int)getFileCountAtPath:(NSString *)directoryPath {
    NSArray *directoryContent = [[NSFileManager defaultManager] contentsOfDirectoryAtPath:directoryPath error:NULL];
    return (int)directoryContent.count;
}

- (void)openPdfInSamePageWithPdfUrl :(NSString *)pdfPath
                                    :(NSString *)webViewOriginXTemp
                                    :(NSString *)webViewOriginYTemp
                                    :(NSString *)webViewWidthTemp
                                    :(NSString *)webViewHeightTemp {
    objectUnityAppController = GetAppController();
    if(objectUnityAppController.rootView == nil)
        return;
    NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
    numberFormatter.numberStyle = NSNumberFormatterDecimalStyle;
    float webViewOriginX = [numberFormatter numberFromString:webViewOriginXTemp].floatValue;
    float webViewOriginY = [numberFormatter numberFromString:webViewOriginYTemp].floatValue;
    float webViewWidth = [numberFormatter numberFromString:webViewWidthTemp].floatValue;
    float webViewHeight = [numberFormatter numberFromString:webViewHeightTemp].floatValue;
    if (pdfLoadWebView == nil) {
        pdfLoadWebView = [[WKWebView alloc] init];
        [objectUnityAppController.rootView addSubview:pdfLoadWebView];
        [objectUnityAppController.rootView bringSubviewToFront:pdfLoadWebView];
    }
    pdfLoadWebView.frame = CGRectMake(webViewOriginX, webViewOriginY, webViewWidth, webViewHeight);
    NSURL *targetURL = [NSURL fileURLWithPath:pdfPath];
    NSURLRequest *request = [NSURLRequest requestWithURL:targetURL];
    [pdfLoadWebView loadRequest:request];
}

- (void)loadPdfWithUrl:(NSString *)pdfUrl
                      :(NSString *)webViewOriginXTemp
                      :(NSString *)webViewOriginYTemp
                      :(NSString *)webViewWidthTemp
                      :(NSString *)webViewHeightTemp {
    NSString *filePath = [pdfCacheWrapper getFilePath:pdfUrl];
    if (filePath == nil || [filePath isEqualToString:@""]) {
        return;
    }
    [self openPdfInSamePageWithPdfUrl:filePath
                                     :webViewOriginXTemp
                                     :webViewOriginYTemp
                                     :webViewWidthTemp
                                     :webViewHeightTemp];
}

- (void)loadPdfWithFileName:(NSString *)pdfFileName
                           :(NSString *)webViewOriginXTemp
                           :(NSString *)webViewOriginYTemp
                           :(NSString *)webViewWidthTemp
                           :(NSString *)webViewHeightTemp {
    NSArray* documentDirectories = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask,YES);
    NSString* documentDirectory = [documentDirectories objectAtIndex:0];
    NSString *filePath = [documentDirectory stringByAppendingPathComponent:pdfFileName];
    [self openPdfInSamePageWithPdfUrl:filePath
                                     :webViewOriginXTemp
                                     :webViewOriginYTemp
                                     :webViewWidthTemp
                                     :webViewHeightTemp];
}

- (void)loadPdfWithContent:(NSString *)pdfContent
                          :(NSString *)webViewOriginXTemp
                          :(NSString *)webViewOriginYTemp
                          :(NSString *)webViewWidthTemp
                          :(NSString *)webViewHeightTemp {
    NSString *fileName = [NSString stringWithFormat:@"BrainCheckTemp_Saving.pdf"];
    NSData *data = [self decodeNsStringToNsData:pdfContent];
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [NSString stringWithFormat:@"%@/%@", documentsDirectory,fileName];
    [data writeToFile:filePath atomically:YES];
    [self openPdfInSamePageWithPdfUrl:filePath
                                     :webViewOriginXTemp
                                     :webViewOriginYTemp
                                     :webViewWidthTemp
                                     :webViewHeightTemp];
}

- (void)removePdf {
    objectUnityAppController = GetAppController();
    if(objectUnityAppController.rootView == nil || pdfLoadWebView == nil)
        return;
    pdfLoadWebView.hidden = true;
    [pdfLoadWebView removeFromSuperview];
    pdfLoadWebView = nil;
}

- (void)removeNewPage {
    if (presentedViewController != nil) {
        [presentedViewController dismissViewControllerAnimated:YES completion:nil];
    }
}


-(void) downloadFile: (NSString * )fileUrl
     withPersistance: (BOOL)isAllowedLocalStorage
      withCompletion: (PdfDownloadHandler) handler
{
    __weak PdfConverter *weakSelf = self;
    [pdfCacheWrapper downloadPdfWithUrl:fileUrl
                        WithFileStorage:isAllowedLocalStorage
                         withCompletion:^(NSData * _Nullable fileData, NSURL * _Nullable fileUrlTemp, NSString * _Nullable error) {
        if (handler != nil) {
            handler(fileData, fileUrlTemp, error);
            return;
        }
        NSString *resultString;
        if (![error isEqualToString:@""]) {
            resultString = [NSString stringWithFormat:@"Failure#######%@#######%@",error, fileUrlTemp.absoluteString];
        } else {
            NSString *fileString  = [weakSelf encodeNSDataToString:fileData];
            resultString = [NSString stringWithFormat:@"Success#######%@#######%@",fileString, fileUrlTemp.absoluteString];
        }
        NSLog(@"===image resultString==%@==", resultString);
        [weakSelf sendMessageToUnity:resultString];
    }];
}

-(void) openPdfInNewPageWithUrl: (NSString *)pdfUrl
                   backBtnTitle: (NSString *) btnTitle
                         title : (NSString *) title{
    objectUnityAppController = GetAppController();
    if(objectUnityAppController.rootView == nil)
        return;

//    [self setUpUnityReceiveMessagePath : msgReceivingGameObjectNameTemp : msgReceivingMethodtNameTemp];
    WebViewVC *vc = [WebViewVC prepareWebView:title :btnTitle];
    vc.delegate = self;
    [self removeNewPage];
    presentedViewController = vc;
    NSString *filePath = [pdfCacheWrapper getFilePath:pdfUrl];
    if (filePath == nil || [filePath isEqualToString:@""]) {
        return;
    }
    [vc setUpWebView:vc
              pdfUrl:filePath];
    [objectUnityAppController.rootViewController presentViewController:vc animated:true completion:nil];
}

-(void) openPdfInNewPageWithFileName: (NSString *)pdfFileName
                        backBtnTitle: (NSString *) btnTitle
                              title : (NSString *) title{
    objectUnityAppController = GetAppController();
    if(objectUnityAppController.rootView == nil)
        return;

    NSArray* documentDirectories = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask,YES);
    NSString* documentDirectory = [documentDirectories objectAtIndex:0];
    NSString *filePath = [documentDirectory stringByAppendingPathComponent:pdfFileName];
    WebViewVC *vc = [WebViewVC prepareWebView:title :btnTitle];
    vc.delegate = self;
    [self removeNewPage];
    presentedViewController = vc;
    if (filePath == nil || [filePath isEqualToString:@""]) {
        return;
    }
    [vc setUpWebView:vc
              pdfUrl:filePath];
    [objectUnityAppController.rootViewController presentViewController:vc animated:true completion:nil];
}

-(void) openPdfInNewPageWithContent: (NSString *) pdfContent
                       backBtnTitle: (NSString *) btnTitle
                             title : (NSString *) title{
    objectUnityAppController = GetAppController();
    if(objectUnityAppController.rootView == nil)
        return;

//    [self setUpUnityReceiveMessagePath : msgReceivingGameObjectNameTemp : msgReceivingMethodtNameTemp];
    NSString *fileName = [NSString stringWithFormat:@"BrainCheckTemp_Saving.pdf"];
    NSData *data = [self decodeNsStringToNsData:pdfContent];
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    NSString *filePath = [NSString stringWithFormat:@"%@/%@", documentsDirectory,fileName];
    [data writeToFile:filePath atomically:YES];
    WebViewVC *vc = [WebViewVC prepareWebView:title :btnTitle];
    [self removeNewPage];
    presentedViewController = vc;
    vc.delegate = self;
    if (filePath == nil || [filePath isEqualToString:@""]) {
        return;
    }
    [vc setUpWebView:vc
              pdfUrl:filePath];
    [objectUnityAppController.rootViewController presentViewController:vc animated:true completion:nil];
}

//-------------------------------------------------------------------------------------------------

-(void) sendMessageToUnity : (NSString *) msg {
    const char *msgImageSaved = [msg cStringUsingEncoding:NSASCIIStringEncoding];
    const char *gameObjectName = [msgReceivingGameObjectNameGlobal cStringUsingEncoding:NSASCIIStringEncoding];
    const char *methodName = [msgReceivingMethodtNameGlobal cStringUsingEncoding:NSASCIIStringEncoding];
    UnitySendMessage(gameObjectName,methodName, msgImageSaved);
}

- (NSString *)encodeNSDataToString:(NSData *)theData {
    return [theData base64EncodedStringWithOptions:NSDataBase64Encoding64CharacterLineLength];
}

- (NSData *)decodeNsStringToNsData:(NSString *)theData {
    NSLog(@"===image decodeNSDataToString==%@==", theData);
    NSData *nsdataFromBase64String = [[NSData alloc] initWithBase64EncodedString:theData options:NSDataBase64DecodingIgnoreUnknownCharacters];
    return nsdataFromBase64String;
}


-(void) setCallBackMethod: (NSString *)gameObject
                         : (NSString *)methodName {
    msgReceivingGameObjectNameGlobal = gameObject;
    msgReceivingMethodtNameGlobal = methodName;
}
@end

// Helper method used to convert NSStrings into C-style strings.
NSString *CreateStr(const char* string) {
    if (string) {
        return [NSString stringWithUTF8String:string];
    } else {
        return [NSString stringWithUTF8String:""];
    }
}


// Unity can only talk directly to C code so use these method calls as wrappers
// into the actual plugin logic.
extern "C" {
    void _createPdf(const char *fileName,
                    const char *pdfPageHeight,
                    const char *pdfPageWidth){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter createPdfWithFileName:CreateStr(fileName)
                                 pdfPageHeight:CreateStr(pdfPageHeight)
                                  pdfPageWidth:CreateStr(pdfPageWidth)];
    }

    void _createNewPage(){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter createNewPage];
    }

    void _writeTextToPdf(const char *data,
                         const char *xPosition,
                         const char *yPosition,
                         const char *width,
                         const char *height,
                         const char *hexColor,
                         const char *fontSize,
                         const char *fontType){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter writeDataToPdfContext:CreateStr(data)
                           content_X_Cordinate:CreateStr(xPosition)
                           content_Y_Cordinate:CreateStr(yPosition)
                                containerWidth:CreateStr(width)
                               containerHeight:CreateStr(height)
                                         color:CreateStr(hexColor)
                                      fontSize:CreateStr(fontSize)
                                      fontType:CreateStr(fontType)];
    }

    void _writeImageToPdf(const char *data,
                         const char *xPosition,
                         const char *yPosition,
                         const char *width,
                         const char *height){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter writeImageToPdfContext:CreateStr(data)
                            content_X_Cordinate:CreateStr(xPosition)
                            content_Y_Cordinate:CreateStr(yPosition)
                                 containerWidth:CreateStr(width)
                                containerHeight:CreateStr(height)];
    }

    void _endPdf(){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter endPdf];
        [objPdfConverter writePdfToFile];
    }

    void _savePdf(){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter writePdfToFile];
    }

    void _getSavedFileFullPath(){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter getSavedPdfPath];
    }
    
    void _getPDFFilePath(const char *msgReceivingGameObjectName,const char *msgReceivingMethodtName) {
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter getPDFFilePath:CreateStr(msgReceivingGameObjectName)
                                       :CreateStr(msgReceivingMethodtName)];
    }
    
    int _getFileCount(const char *directoryPath) {
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        return [objPdfConverter getFileCountAtPath:CreateStr(directoryPath)];
    }
    
    void _getListFileAtPath(const char *directoryPath,const char *msgReceivingGameObjectName,const char *msgReceivingMethodtName) {
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter getListFileAtPath:CreateStr(directoryPath)
                                          :CreateStr(msgReceivingGameObjectName)
                                          :CreateStr(msgReceivingMethodtName)];
    }

    void _loadPdf(const char *filePath,const char *viewOriginX,const char *viewOriginY,const char *viewWidth,const char *viewHeight){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter openPdfInSamePageWithPdfUrl:CreateStr(filePath)
                                                    :CreateStr(viewOriginX)
                                                    :CreateStr(viewOriginY)
                                                    :CreateStr(viewWidth)
                                                    :CreateStr(viewHeight)];
    }

    void _removePdf(){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter removePdf];
    }

    void _sharePdfFile(){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter sharePdfFile];
    }


    void _loadPdfInsideUnityWithFileName(const char *fileName,const char *viewOriginX,const char *viewOriginY,const char *viewWidth,const char *viewHeight){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter loadPdfWithFileName:CreateStr(fileName)
                                            :CreateStr(viewOriginX)
                                            :CreateStr(viewOriginY)
                                            :CreateStr(viewWidth)
                                            :CreateStr(viewHeight)];
    }

    void _loadPdfInsideUnityWithContent(const char *fileContent,const char *viewOriginX,const char *viewOriginY,const char *viewWidth,const char *viewHeight){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter loadPdfWithContent:CreateStr(fileContent)
                                           :CreateStr(viewOriginX)
                                           :CreateStr(viewOriginY)
                                           :CreateStr(viewWidth)
                                           :CreateStr(viewHeight)];
    }

    void _loadPdfInsideUnityWithUrl(const char *fileUrl,const char *viewOriginX,const char *viewOriginY,const char *viewWidth,const char *viewHeight){
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter loadPdfWithUrl:CreateStr(fileUrl)
                                       :CreateStr(viewOriginX)
                                       :CreateStr(viewOriginY)
                                       :CreateStr(viewWidth)
                                       :CreateStr(viewHeight)];
    }

    void _loadPdfInNewPageWithUrl(const char *fileUrl, const char *btnTitle, const char *pageTitle) {
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter openPdfInNewPageWithUrl:CreateStr(fileUrl)
                                    backBtnTitle:CreateStr(btnTitle)
                                           title:CreateStr(pageTitle)];
    }

    void _loadPdfInNewPageWithFileName(const char *fileName, const char *btnTitle, const char *pageTitle) {
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter openPdfInNewPageWithFileName:CreateStr(fileName)
                                         backBtnTitle:CreateStr(btnTitle)
                                                title:CreateStr(pageTitle)];
    }

    void _loadPdfInNewPageWithContent(const char *fileContent, const char *btnTitle, const char *pageTitle) {
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter openPdfInNewPageWithContent:CreateStr(fileContent)
                                        backBtnTitle:CreateStr(btnTitle)
                                               title:CreateStr(pageTitle)];
    }

    void _downloadPdf(const char *fileUrl) {
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter downloadFile:CreateStr(fileUrl)
                      withPersistance:true
                       withCompletion:nil];
    }
    
    void _removePdfFromUnityScreen() {
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter removePdf];
    }

    void _removePdfFromNewScreen() {
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter removeNewPage];
    }

    void _setCallBackMethod(const char *msgReceivingGameObjectName, const char *msgReceivingMethodName) {
        PdfConverter *objPdfConverter = [PdfConverter pluginSharedInstance];
        [objPdfConverter setCallBackMethod:CreateStr(msgReceivingGameObjectName)
                                          :CreateStr(msgReceivingMethodName)];
    }
}
