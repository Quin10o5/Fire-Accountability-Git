//
//  WebViewVC.m
//  Unity-iPhone
//
//  Created by Mayank Gupta on 08/11/20.
//

#import "WebViewVC.h"

@interface WebViewVC ()

@end

@implementation WebViewVC

+(WebViewVC *) prepareWebView : (NSString *) title : (NSString *) btnTitle {
    WebViewVC *vc = [[WebViewVC alloc] init];
    UIView *parentView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, UIScreen.mainScreen.bounds.size.width, UIScreen.mainScreen.bounds.size.height)];
    vc.view = parentView;
    
    int heightOfHeader = 50;
    UIView *headerView = [WebViewVC addHeader:title :parentView :vc :heightOfHeader];
    [WebViewVC addBackButtonToHeader:btnTitle :headerView :vc];
    [WebViewVC createWkWebView:heightOfHeader :parentView :vc];
    return vc;
}

+(UIView *) addHeader : (NSString *) title : (UIView *) parentView : (WebViewVC *)vc :(int) heightOfHeader{
    UIView *headerView = [[UIView alloc] initWithFrame:CGRectMake(0, 0, UIScreen.mainScreen.bounds.size.width, heightOfHeader)];
    [parentView addSubview:headerView];
    [headerView setBackgroundColor:UIColor.blackColor];
    
    
    UILabel *label1 = [[UILabel alloc] init];
    [label1 setNumberOfLines:1];
    [label1 setText:title];
    CGFloat labelWidth = [label1 sizeThatFits:CGSizeMake(UIScreen.mainScreen.bounds.size.width, 20)].width;
    
    
    UILabel *label = [[UILabel alloc] initWithFrame:CGRectMake((UIScreen.mainScreen.bounds.size.width / 2 - (labelWidth/2)), (heightOfHeader/2 - 10), labelWidth, 20)];
    [headerView addSubview:label];
    [label setText:title];
    
    return headerView;
}


+(void) addBackButtonToHeader : (NSString *) btnTitle : (UIView *) parentView : (WebViewVC *)vc {
    UIButton *btnView = [[UIButton alloc] initWithFrame: CGRectMake(10, 15, 50, 20)];
    [btnView setTitle:btnTitle forState:UIControlStateNormal];
    [parentView addSubview:btnView];
    vc->myButton = btnView;
}

+(void) createWkWebView : (int) startY_Pos : (UIView *) parentView : (WebViewVC *)vc {
    CGRect frameOfWebView = CGRectMake(0, startY_Pos + 1, UIScreen.mainScreen.bounds.size.width, (UIScreen.mainScreen.bounds.size.height - startY_Pos));
    WKWebViewConfiguration *theConfiguration = [[WKWebViewConfiguration alloc] init];
    WKWebView *webView = [[WKWebView alloc] initWithFrame:frameOfWebView configuration:theConfiguration];
    [parentView addSubview:webView];
    vc->uiWebView = webView;
}

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void) setUpWebView : (UIViewController*) vc
              pdfUrl : (NSString *)filePath {
    [myButton addTarget:vc
                 action:@selector(btnClicked:)
       forControlEvents:UIControlEventTouchUpInside];
    
    [self loadPdfInWebView :filePath];
}

- (IBAction)btnClicked:(id)sender {
    [self dismissViewControllerAnimated:true completion:nil];
}

-(void)loadPdfInWebView : (NSString *)filePath {
    uiWebView.navigationDelegate = self;
    
    NSURL *targetURL = [NSURL fileURLWithPath:filePath];
    NSURLRequest *request = [NSURLRequest requestWithURL:targetURL];
    [uiWebView loadRequest:request];
}

- (void)webView:(WKWebView *)webView didStartProvisionalNavigation:(null_unspecified WKNavigation *)navigation {
//    [self.delegate webViewLoadingStatus:@"LoadingStarted"];
    [self addActivityIndicator];
}

- (void)webView:(WKWebView *)webView didFinishNavigation:(null_unspecified WKNavigation *)navigation {
//    [self.delegate webViewLoadingStatus:@"LoadingCompleted"];
    [self removeActivityIndicator];
}


- (void)webView:(WKWebView *)webView didFailNavigation:(null_unspecified WKNavigation *)navigation withError:(NSError *)error {
//    [self.delegate webViewLoadingStatus:@"LoadingFailed"];
    [self removeActivityIndicator];
}



-(void) addActivityIndicator {
    if (activityIndicator != nil) {
        [activityIndicator removeFromSuperview];
    }
    activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    activityIndicator.center=self.view.center;
    [self.view addSubview:activityIndicator];
    [activityIndicator startAnimating];
    [self.view bringSubviewToFront:activityIndicator];
}

-(void) removeActivityIndicator {
    if (activityIndicator == nil) {
        return;
    }
    [activityIndicator stopAnimating];
    [activityIndicator removeFromSuperview];
}

@end
