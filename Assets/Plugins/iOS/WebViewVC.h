//
//  WebViewVC.h
//  Unity-iPhone
//
//  Created by Mayank Gupta on 08/11/20.
//

#import <UIKit/UIKit.h>
#import <WebKit/WebKit.h>

@protocol WebViewVCProtocol
- (void) webViewLoadingStatus :(NSString *)message ;
@end


@interface WebViewVC : UIViewController <WKNavigationDelegate> {
    WKWebView *uiWebView;
    UIButton *myButton;
    UIActivityIndicatorView *activityIndicator;
}
+(WebViewVC *) prepareWebView : (NSString *) title : (NSString *) btnTitle;
-(void) setUpWebView : (UIViewController*) vc pdfUrl: (NSString *)urlString;
@property (nonatomic, weak) id delegate;

@end
