//
//  PdfCacheWrapper.h
//  UnityFramework
//
//  Created by Mayank Gupta on 26/02/22.
//

#import <Foundation/Foundation.h>
#import <PdfDownloadOperation.h>
#import <UIKit/UIKit.h>>

NS_ASSUME_NONNULL_BEGIN

@interface PdfCacheWrapper : NSObject

+ (instancetype)sharedInstance;

-(void) downloadPdfWithUrl: (NSString *) fileUrlString
           WithFileStorage: (BOOL) isPersistantStorage
            withCompletion: (PdfDownloadHandler) completion;

-(NSString *) getFilePath: (NSString *)fileUrlString;
@end

NS_ASSUME_NONNULL_END
