//
//  ImageDownloadOperation.m
//  UnityFramework
//
//  Created by Mayank Gupta on 26/02/22.
//

#import "PdfDownloadOperation.h"

@interface PdfDownloadOperation (){

    NSURL *pdfUrl;
    PdfDownloadHandler downloadHandler;
    BOOL isDeviceStorageEnabled;
    NSMutableDictionary<NSString *, NSString *> *pdfMap;
}
@end


@implementation PdfDownloadOperation

-(void) setUpOperationWithUrl: (NSURL *)url
              WithFileStorage: (BOOL) isPersistantStorage
                  WithFileMap: (NSMutableDictionary<NSString *, NSString *> *) persistantStorageMap
                   AndHandler: (PdfDownloadHandler)completion {
    NSLog(@"Operation initiated %@", url.absoluteString);
    pdfUrl = url;
    downloadHandler = completion;
    isDeviceStorageEnabled = isPersistantStorage;
    pdfMap = persistantStorageMap;
}


-(void) downloadPdf {
//    NSLog(@"Download Initiated");
    NSData *cachedData = [self getFileFromLocalStorage:pdfUrl];
    if (cachedData != nil) {
        dispatch_async(dispatch_get_main_queue(), ^{
            downloadHandler(cachedData, pdfUrl, @"");
        });
        return;
    }
    NSLog(@"Download Started  ==== %@", pdfUrl);
    NSURLSession *urlSession = [NSURLSession sharedSession];
    NSURLSessionDownloadTask *downloadTask =  [urlSession downloadTaskWithURL:pdfUrl
                                                            completionHandler:^(NSURL *location, NSURLResponse *response, NSError *error){
                                                NSLog(@"Download Complete ==== %@", location);
                                                NSData *data = [NSData dataWithContentsOfURL:location];
                                                if (isDeviceStorageEnabled) {
                                                    [self storePdfToLocalWithUrl:pdfUrl
                                                                        AndImage:data];
                                                }
                                                dispatch_async(dispatch_get_main_queue(), ^{
                                                    downloadHandler(data, pdfUrl, error.localizedDescription);
                                                });
                                            }];
    [downloadTask resume];
}

-(NSData *) getFileFromLocalStorage: (NSURL *)url {
//    NSLog(@"Local Storage  ==== %@", url);
    NSString *pdfName = [pdfMap objectForKey:url.absoluteString];
    if (pdfName == nil || [pdfName isEqualToString:@""]) {
        return nil;
    }
    NSArray *pathForDirectoriesInDomains = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [pathForDirectoriesInDomains objectAtIndex:0];
    NSString *fileAbsolutePath = [documentsDirectory stringByAppendingPathComponent:pdfName];
    NSData *fileData = [NSData dataWithContentsOfFile:fileAbsolutePath];
    return fileData;
}


-(void) removeMapFromDict: (NSURL *)url {
    dispatch_async(dispatch_get_main_queue(), ^{
        [pdfMap removeObjectForKey:url.absoluteString];
        [[NSUserDefaults standardUserDefaults] setObject:pdfMap forKey: @"PdfNameUrlMapping"];
        [[NSUserDefaults standardUserDefaults] synchronize];
    });
}

-(void) storePdfToLocalWithUrl: (NSURL *)url
                      AndImage: (NSData *) data{
    dispatch_async(dispatch_get_main_queue(), ^{
        NSString *fileName = [NSString stringWithFormat:@"BrainCheckCache_%d.pdf",(int)pdfMap.count];
        
        if ([self checkIfFileExist:fileName]) {
            for (int i = 0; i < 100; i++) {
                NSString *tempName = [NSString stringWithFormat:@"BrainCheckCache_%d_%d.pdf",(int)pdfMap.count,i];
                if ([self checkIfFileExist:tempName]) {
                    continue;
                }
                fileName = tempName;
                break;
            }
        }
        NSLog(@"==Local Storage1===%@",fileName);
        [pdfMap setObject:fileName forKey:url.absoluteString];
        NSLog(@"==Local Storage2===%@",pdfMap);
        [[NSUserDefaults standardUserDefaults] setObject:pdfMap forKey: @"PdfNameUrlMapping"];
        [[NSUserDefaults standardUserDefaults] synchronize];
        dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
            NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
            NSString *documentsDirectory = [paths objectAtIndex:0];
            NSString  *filePath = [NSString stringWithFormat:@"%@/%@", documentsDirectory,fileName];
            NSLog(@"==Local Storage3===%@",filePath);
            [data writeToFile:filePath atomically:YES];
        });
    });
}

-(BOOL) checkIfFileExist: (NSString *)fileName {
    NSArray *pathForDirectoriesInDomains = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [pathForDirectoriesInDomains objectAtIndex:0];
    NSString *fileAbsolutePath = [documentsDirectory stringByAppendingPathComponent:fileName];
    NSFileManager *fileManager = [NSFileManager defaultManager];

    if ([fileManager fileExistsAtPath: fileAbsolutePath]) {
        return true;
    }
    return false;
}

@end
