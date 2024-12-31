//
//  ImageDownloadOperation.h
//  UnityFramework
//
//  Created by Mayank Gupta on 26/02/22.
//

#import <Foundation/Foundation.h>

typedef void (^PdfDownloadHandler)(NSData* _Nullable image, NSURL * _Nullable imageUrl, NSString  * _Nullable error);
//NSString * _Nonnull userDeafultKey = @"ImageNameUrlMapping";

NS_ASSUME_NONNULL_BEGIN

@interface PdfDownloadOperation : NSOperation

-(void) setUpOperationWithUrl: (NSURL *)url
              WithFileStorage: (BOOL) isPersistantStorage
                  WithFileMap: (NSMutableDictionary<NSString *, NSString *> *) persistantStorageMap
                   AndHandler: (PdfDownloadHandler)completion;

-(void) downloadPdf;

@end

NS_ASSUME_NONNULL_END
