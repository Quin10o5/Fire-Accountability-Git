//
//  PdfCacheWrapper.m
//  UnityFramework
//
//  Created by Mayank Gupta on 26/02/22.
//

#import "PdfCacheWrapper.h"


@interface PdfCacheWrapper (){
    NSOperationQueue *operationQueue;
    NSMutableDictionary<NSURL *, NSData *> *cacheDict;
    NSMutableDictionary<NSURL *, NSOperation *> *operationDict;
    NSMutableDictionary<NSURL *, PdfDownloadHandler> *handlerDict;
    NSMutableDictionary<NSString *, NSString *> *localStorageImageMapDict;
}
@end

@implementation PdfCacheWrapper

+ (instancetype)sharedInstance
{
    static PdfCacheWrapper *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[PdfCacheWrapper alloc] init];
        [sharedInstance setUpCacheWrapper];
        [sharedInstance createOprationQueue];
        [sharedInstance getFileNameDictFromUserDefault];
    });
    return sharedInstance;
}

-(void) setUpCacheWrapper {
    cacheDict = [[NSMutableDictionary alloc] init];
    operationDict = [[NSMutableDictionary alloc] init];
    handlerDict = [[NSMutableDictionary alloc] init];
}

-(void) createOprationQueue {
    operationQueue = [[NSOperationQueue alloc] init];
    operationQueue.name = @"PdfDownloadOperationQueue";
    operationQueue.qualityOfService = NSQualityOfServiceUserInitiated;
}

-(void) getFileNameDictFromUserDefault {
    NSDictionary *retrievedDictionary = [[NSUserDefaults standardUserDefaults] dictionaryForKey: @"PdfNameUrlMapping"];
    localStorageImageMapDict = [retrievedDictionary mutableCopy];
    if (localStorageImageMapDict == nil) {
        localStorageImageMapDict = [[NSMutableDictionary alloc] init];
    }
}

-(NSString *) getFilePath: (NSString *)fileUrlString {
    if (localStorageImageMapDict == nil) {
        return @"";
    }
    NSString *pdfName = [localStorageImageMapDict objectForKey:fileUrlString];
    if (pdfName == nil || [pdfName isEqualToString:@""]) {
        return @"";
    }
    NSArray *pathForDirectoriesInDomains = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [pathForDirectoriesInDomains objectAtIndex:0];
    NSString *fileAbsolutePath = [documentsDirectory stringByAppendingPathComponent:pdfName];
    return fileAbsolutePath;
}

-(void) downloadPdfWithUrl: (NSString *) fileUrlString
           WithFileStorage: (BOOL) isPersistantStorage
            withCompletion: (PdfDownloadHandler) completion {
//    NSLog(@"CacheWrapper===%@", imageUrlString);
    
    NSURL *fileUrl = [NSURL URLWithString: fileUrlString];
    if (!(fileUrl && [fileUrl scheme] && [fileUrl host])) {
//        NSLog(@"CacheWrapper===Not Valid Url");
        completion(nil, nil, @"Not Valid Url!");
        return;
    }
    
//    NSLog(@"Cache Dict===%@", cacheDict);
    NSData *cachedFile = [cacheDict objectForKey:fileUrl];
    if (cachedFile != nil) {
        completion(cachedFile, fileUrl, @"");
        return;
    }
    
    NSOperation *currentOperation = [operationDict objectForKey:fileUrl];
    if (currentOperation != nil) {
        currentOperation.queuePriority = NSOperationQueuePriorityHigh;
        return;
    }
    
    PdfDownloadOperation *newOperation = [[PdfDownloadOperation alloc] init];
    [newOperation setUpOperationWithUrl: fileUrl
                        WithFileStorage: isPersistantStorage
                            WithFileMap: localStorageImageMapDict
                             AndHandler:^(NSData *fileData, NSURL *fileUrlTemp, NSString *error){
        dispatch_async(dispatch_get_main_queue(), ^{
            PdfDownloadHandler tempHandler = [handlerDict objectForKey:fileUrl];
//            NSLog(@"Download Handler=====%@====Error====%@",image, error);
            if (error != nil) {
                if (![error isEqualToString:@""]) {
                    tempHandler(nil, fileUrlTemp, error);
                    return;
                }
            }
            
            [cacheDict setObject:fileData forKey:fileUrlTemp];
            tempHandler(fileData, fileUrlTemp, @"");

            [handlerDict removeObjectForKey:fileUrlTemp];
            [operationDict removeObjectForKey:fileUrlTemp];
        });
    }];
    [newOperation downloadPdf];
    newOperation.queuePriority = NSOperationQueuePriorityHigh;
    [operationQueue addOperation:newOperation];
    [operationDict setObject:newOperation forKey:fileUrl];
    [handlerDict setObject:completion forKey:fileUrl];
}
@end
