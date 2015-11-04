//
//  NotificarePushLibUnity.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 14-10-2015.
//
//

#import "NotificarePushLibUnity.h"

#import "NotificarePushLib.h"

#import "NotificareNotification+Dictionary.h"
#import "NotificareProduct+Dictionary.h"
#import "SKPayment+Dictionary.h"
#import "SKPaymentTransaction+Dictionary.h"
#import "SKDownload+Dictionary.h"

typedef void (*BasicCallback)(const char *jsonUTF8String);
typedef char *(*DelegateCallback)(const char *jsonUTF8String);
typedef NSString *(^DelegateBlock)(NSString *jsonString);

#pragma mark - C -

#pragma mark - Unity

NotificarePushLibUnity *pushLibUnity;

__attribute__((constructor))
static void begin(void) {
    pushLibUnity = [NotificarePushLibUnity shared];
}

__attribute__((destructor))
static void end(void) {
    pushLibUnity = nil;
}

NSString *stringWithChar(const char *utf8String) {
    return utf8String ? [NSString stringWithUTF8String:utf8String] : nil;
}

/*char *cStringCopy(const char* cString) {
 if (cString == NULL) {
 return NULL;
 }
 
 char* buffer = (char *)malloc(strlen(cString) + 1);
 strcpy(buffer, cString);
 
 return buffer;
 }*/

NSString *jsonStringFromObject(id object) {
    if (![NSJSONSerialization isValidJSONObject:object]) {
        return nil;
    }
    
    NSError *error;
    NSData *data = [NSJSONSerialization dataWithJSONObject:object
                                                   options:0
                                                     error:&error];
    
    if (error) {
        NSLog(@"Could not convert object(%@) to JSON: %@", object, error);
        
        return nil;
    }
    
    NSString *jsonString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    
    return jsonString;
}

id objectFromJSONString(NSString *jsonString) {
    NSData *data = [NSData dataWithContentsOfFile:jsonString];
    
    NSError *error;
    id object = [NSJSONSerialization JSONObjectWithData:data
                                                options:0
                                                  error:&error];
    
    if (error) {
        NSLog(@"Could not convert JSON string\n(%@)\nto object: %@", jsonString, error);
        
        return nil;
    }
    
    return object;
}

void performBasicCallback(BasicCallback callback, id object) {
    NSString *jsonString = jsonStringFromObject(object);
    
    if (callback) {
        // char *strCopy = cStringCopy([jsonString UTF8String]);
        
        callback([jsonString UTF8String]);
        
        //free(strCopy);
    }
}

NSArray *filterDownloads(NSArray *downloadDicts) {
    NSMutableArray *filteredDownloads = [NSMutableArray array];
    
    for (NSDictionary *downloadDict in downloadDicts) {
        for (SKPaymentTransaction *transaction in [[SKPaymentQueue defaultQueue] transactions]) {
            for (SKDownload *download in transaction.downloads) {
                if ([download.contentIdentifier isEqualToString:downloadDict[@"contentIdentifier"]]) {
                    [filteredDownloads addObject:download];
                    break;
                }
            }
        }
    }
    
    return [filteredDownloads copy];
}

#pragma mark - Exported Functions

void _registerDelegateCallback(const char* delegateMethod, DelegateCallback callback) {
    NSMutableDictionary *delegateCallbacks = [[NotificarePushLibUnity shared] delegateCallbacks];
    NSString *key = stringWithChar(delegateMethod);
    
    if (key && callback) {
        DelegateBlock delegateBlock = ^NSString *(NSString *str) {
            //char *strCopy = cStringCopy([str UTF8String]);
            
            char *result = callback([str UTF8String]);
            
            //free(strCopy);
            
            return stringWithChar(result);
        };
        
        delegateCallbacks[key] = delegateBlock;
    }
}

void _unregisterDelegateCallback(const char* delegateMethod) {
    NSMutableDictionary *delegateCallbacks = [[NotificarePushLibUnity shared] delegateCallbacks];
    NSString *key = stringWithChar(delegateMethod);
    
    if (key) {
        [delegateCallbacks removeObjectForKey:key];
    }
}

void _launch() {
    [[NotificarePushLib shared] launch];
}

void _registerForNotifications() {
    [[NotificarePushLib shared] registerForNotifications];
}

void _registerUserNotifications() {
    [[NotificarePushLib shared] registerUserNotifications];
}

bool _checkRemoteNotifications() {
    return [[NotificarePushLib shared] checkRemoteNotifications] ? true : false;
}

void _handleOptions(const char *optionsJSON) {
    NSString *jsonString = stringWithChar(optionsJSON);
    NSDictionary *optionsInfo = objectFromJSONString(jsonString);
    
    [[NotificarePushLib shared] handleOptions:optionsInfo[@"options"]];
}

void _registerDevice(const void *token, uint tokenLength, BasicCallback infoCallback, BasicCallback errorCallback) {
    NSData *tokenData = [NSData dataWithBytes:token length:tokenLength];
    
    [[NotificarePushLib shared] registerDevice:tokenData completionHandler:^(NSDictionary *info) {
        NSDictionary *registrationInfo;
        
        if (info) {
            registrationInfo = @{@"registration": info};
        }
        else {
            registrationInfo = @{@"registration": [NSNull null]};
        }
        
        performBasicCallback(infoCallback, registrationInfo);
    } errorHandler:^(NSError *error) {
        NSDictionary *errorInfo;
        
        if (error) {
            errorInfo = @{@"error": error.description};
        }
        else {
            errorInfo = @{@"error": [NSNull null]};
        }
        
        performBasicCallback(errorCallback, errorInfo);
    }];
}

void _registerDeviceWithUser(const void *token, uint tokenLength, const char *userID, const char *username, BasicCallback infoCallback, BasicCallback errorCallback) {
    NSData *tokenData = [NSData dataWithBytes:token length:tokenLength];
    NSString *userIDString = userID ? [NSString stringWithUTF8String:userID] : nil;
    NSString *usernameString = username ? [NSString stringWithUTF8String:username] : nil;
    
    [[NotificarePushLib shared] registerDevice:tokenData
                                    withUserID:userIDString
                                  withUsername:usernameString
                             completionHandler:^(NSDictionary *info) {
                                 NSDictionary *registrationInfo;
                                 
                                 if (info) {
                                     registrationInfo = @{@"registration": info};
                                 }
                                 else {
                                     registrationInfo = @{@"registration": [NSNull null]};
                                 }
                                 
                                 performBasicCallback(infoCallback, registrationInfo);
                             }
                                  errorHandler:^(NSError *error) {
                                      NSDictionary *errorInfo;
                                      
                                      if (error) {
                                          errorInfo = @{@"error": error.description};
                                      }
                                      else {
                                          errorInfo = @{@"error": [NSNull null]};
                                      }
                                      
                                      performBasicCallback(errorCallback, errorInfo);
                                  }];
}

void _unregisterDevice() {
    [[NotificarePushLib shared] unregisterDevice];
}

void _updateBadge(int badge) {
    [[NotificarePushLib shared] updateBadge:[NSNumber numberWithInt:badge]];
}

void _openNotification(const char* notificationJSON) {
    NSString *jsonString = stringWithChar(notificationJSON);
    NSDictionary *notificationInfo = objectFromJSONString(jsonString);
    
    [[NotificarePushLib shared] openNotification:notificationInfo[@"notification"]];
}

void _logOpenNotification(const char* notificationJSON) {
    NSString *jsonString = stringWithChar(notificationJSON);
    NSDictionary *notificationInfo = objectFromJSONString(jsonString);
    
    [[NotificarePushLib shared] logOpenNotification:notificationInfo[@"notification"]];
}

void _getNotification(const char *notificationID, BasicCallback infoCallback, BasicCallback errorCallback) {
    NSString *notificationIDString = notificationID ? [NSString stringWithUTF8String:notificationID] : nil;
    
    [[NotificarePushLib shared] getNotification:notificationIDString
                              completionHandler:^(NSDictionary *info) {
                                  NSDictionary *notificationInfo;
                                  
                                  if (info) {
                                      notificationInfo = @{@"notification": info};
                                  }
                                  else {
                                      notificationInfo = @{@"notification": [NSNull null]};
                                  }
                                  
                                  performBasicCallback(infoCallback, notificationInfo);
                              }
                                   errorHandler:^(NSError *error) {
                                       NSDictionary *errorInfo;
                                       
                                       if (error) {
                                           errorInfo = @{@"error": error.description};
                                       }
                                       else {
                                           errorInfo = @{@"error": [NSNull null]};
                                       }
                                       
                                       performBasicCallback(errorCallback, errorInfo);
                                   }];
}

void _clearNotification(const char *notification) {
    NSString *notificationString = notification ? [NSString stringWithUTF8String:notification] : nil;
    [[NotificarePushLib shared] clearNotification:notificationString];
}

void _startLocationUpdates() {
    [[NotificarePushLib shared] startLocationUpdates];
}

bool _checkLocationUpdates() {
    return [[NotificarePushLib shared] checkLocationUpdates] ? true : false;
}

void _stopLocationUpdates() {
    [[NotificarePushLib shared] stopLocationUpdates];
}

#warning Needs confirmation that it properly overlays on top of Unity view
void _openUserPreferences() {
    [[NotificarePushLib shared] openUserPreferences];
}

int _myBadge() {
    return [[NotificarePushLib shared] myBadge];
}

void _fetchProducts(BasicCallback productsCallback, BasicCallback errorCallback) {
    [[NotificarePushLib shared] fetchProducts:^(NSArray *products) {
        NSDictionary *productsInfo;
        
        if (products) {
            NSMutableArray *dictionaryProducts = [NSMutableArray array];
            for (NotificareProduct *product in products) {
                [dictionaryProducts addObject:[product toDictionary]];
            }
            
            productsInfo = @{@"products": [dictionaryProducts copy]};
        }
        else {
            productsInfo = @{@"products": [NSNull null]};
        }
        
        performBasicCallback(productsCallback, productsInfo);
    } errorHandler:^(NSError *error) {
        NSDictionary *errorInfo;
        
        if (error) {
            errorInfo = @{@"error": error.description};
        }
        else {
            errorInfo = @{@"error": [NSNull null]};
        }
        
        performBasicCallback(errorCallback, errorInfo);
    }];
}

void _fetchPurchasedProducts(BasicCallback productsCallback, BasicCallback errorCallback) {
    [[NotificarePushLib shared] fetchPurchasedProducts:^(NSArray *products) {
        NSDictionary *productsInfo;
        
        if (products) {
            NSMutableArray *dictionaryProducts = [NSMutableArray array];
            for (NotificareProduct *product in products) {
                [dictionaryProducts addObject:[product toDictionary]];
            }
            
            productsInfo = @{@"products": [dictionaryProducts copy]};
        }
        else {
            productsInfo = @{@"products": [NSNull null]};
        }
        
        performBasicCallback(productsCallback, productsInfo);
    } errorHandler:^(NSError *error) {
        NSDictionary *errorInfo;
        
        if (error) {
            errorInfo = @{@"error": error.description};
        }
        else {
            errorInfo = @{@"error": [NSNull null]};
        }
        
        performBasicCallback(errorCallback, errorInfo);
    }];
}

void _fetchProduct(const char *productIdentifier, BasicCallback productCallback, BasicCallback errorCallback) {
    NSString *productIdentifierString = stringWithChar(productIdentifier);
    
    [[NotificarePushLib shared] fetchProduct:productIdentifierString
                           completionHandler:^(NotificareProduct *product) {
                               NSDictionary *productInfo;
                               
                               if (product) {
                                   productInfo = @{@"product": [product toDictionary]};
                               }
                               else {
                                   productInfo = @{@"product": [NSNull null]};
                               }
                               
                               performBasicCallback(productCallback, productInfo);
                           }
                                errorHandler:^(NSError *error) {
                                    NSDictionary *errorInfo;
                                    
                                    if (error) {
                                        errorInfo = @{@"error": error.description};
                                    }
                                    else {
                                        errorInfo = @{@"error": [NSNull null]};
                                    }
                                    
                                    performBasicCallback(errorCallback, errorInfo);
                                }];
}

void _buyProduct(const char *productJSON) {
    NSString *jsonString = stringWithChar(productJSON);
    NSDictionary *productInfo = objectFromJSONString(jsonString);
    NotificareProduct *notificareProduct = [[NotificareProduct alloc] initWithDictionary:productInfo[@"product"]];
    
    [[NotificarePushLib shared] buyProduct:notificareProduct];
}

void _pauseDownloads(const char* downloadsJSON) {
    NSString *jsonString = stringWithChar(downloadsJSON);
    NSDictionary *downloadsInfo = objectFromJSONString(jsonString);
    
    NSArray *downloads = filterDownloads(downloadsInfo[@"downloads"]);
    [[NotificarePushLib shared] pauseDownloads:downloads];
}

void _cancelDownloads(const char* downloadsJSON) {
    NSString *jsonString = stringWithChar(downloadsJSON);
    NSDictionary *downloadsInfo = objectFromJSONString(jsonString);
    
    NSArray *downloads = filterDownloads(downloadsInfo[@"downloads"]);
    [[NotificarePushLib shared] cancelDownloads:downloads];
}

void _resumeDownloads(const char* downloadsJSON) {
    NSString *jsonString = stringWithChar(downloadsJSON);
    NSDictionary *downloadsInfo = objectFromJSONString(jsonString);
    
    NSArray *downloads = filterDownloads(downloadsInfo[@"downloads"]);
    [[NotificarePushLib shared] resumeDownloads:downloads];
}

const char *_contentPathForProduct(const char *productIdentifier) {
    NSString *productIdentifierString = productIdentifier ? [NSString stringWithUTF8String:productIdentifier] : nil;
    NSString *contentPathString = [[NotificarePushLib shared] contentPathForProduct:productIdentifierString];
    
    return contentPathString.UTF8String;
}

const char *sdkVersion() {
    return [[[NotificarePushLib shared] sdkVersion] UTF8String];
}

#pragma mark - Objective C -

@interface NotificarePushLibUnity () <NotificarePushLibDelegate>

@property (nonatomic, strong) NotificarePushLib *notificarePushLib;

@end


@implementation NotificarePushLibUnity

#pragma mark - Lifecycle -

- (instancetype)init {
    [NSException raise:@"Singleton" format:@"Use +[NotificarePushLibUnity shared]"];
    
    return nil;
}

- (instancetype)initPrivate {
    self = [super init];
    
    if (self) {
        self.notificarePushLib = [NotificarePushLib shared];
        self.notificarePushLib.delegate = self;
        self.delegateCallbacks = [NSMutableDictionary dictionary];
    }
    
    return self;
}

+ (instancetype)shared {
    static NotificarePushLibUnity *shared = nil;
    
    if (shared == nil) {
        static dispatch_once_t oncePredicate;
        dispatch_once(&oncePredicate, ^{
            shared = [[NotificarePushLibUnity alloc] initPrivate];
            
        });
    }
    return shared;
}

#pragma mark - NotificarePushLibDelegate -

- (BOOL)notificarePushLib:(NotificarePushLib *)library shouldHandleNotification:(NSDictionary *)info {
    NSDictionary *notificationInfo;
    
    if (info) {
        notificationInfo = @{@"notification": info};
    }
    else {
        notificationInfo = @{@"notification": [NSNull null]};
    }
    
    NSString *jsonString = [self performDelegateCallback:@"shouldHandleNotification" withObject:notificationInfo];
    
    if (!jsonString) {
        return NO;
    }
    
    NSDictionary *resultInfo = objectFromJSONString(jsonString);
    
    if (!resultInfo) {
        return NO;
    }
    
    return resultInfo[@"shouldHandleNotification"] ? [resultInfo[@"shouldHandleNotification"] boolValue] : NO;
}

- (void)notificarePushLib:(NotificarePushLib *)library didUpdateBadge:(int)badge {
    NSDictionary *badgeInfo = @{@"badge": [NSNumber numberWithInt:badge]};
    [self performDelegateCallback:@"didUpdateBadge" withObject:badgeInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library willOpenNotification:(NotificareNotification *)notification {
    NSDictionary *notificationInfo;
    
    if (notification) {
        notificationInfo = @{@"notification": [notification toDictionary]};
    }
    else {
        notificationInfo = @{@"notification": [NSNull null]};
    }
    
    [self performDelegateCallback:@"willOpenNotification" withObject:notificationInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didOpenNotification:(NotificareNotification *)notification {
    NSDictionary *notificationInfo;
    
    if (notification) {
        notificationInfo = @{@"notification": [notification toDictionary]};
    }
    else {
        notificationInfo = @{@"notification": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didOpenNotification" withObject:notificationInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didCloseNotification:(NotificareNotification *)notification {
    NSDictionary *notificationInfo;
    
    if (notification) {
        notificationInfo = @{@"notification": [notification toDictionary]};
    }
    else {
        notificationInfo = @{@"notification": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didCloseNotification" withObject:notificationInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didFailToOpenNotification:(NotificareNotification *)notification {
    NSDictionary *notificationInfo;
    
    if (notification) {
        notificationInfo = @{@"notification": [notification toDictionary]};
    }
    else {
        notificationInfo = @{@"notification": [NSNull null]};
    }
    [self performDelegateCallback:@"didFailToOpenNotification" withObject:notificationInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didLoadStore:(NSArray *)products {
    NSDictionary *productsInfo;
    
    if (products) {
        NSMutableArray *productDicts = [NSMutableArray array];
        
        for (NotificareProduct *product in products) {
            [productDicts addObject:[product toDictionary]];
        }
        
        productsInfo = @{@"products": [productDicts copy]};
    }
    else {
        productsInfo = @{@"products": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didLoadStore" withObject:productsInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didFailToLoadStore:(NSError *)error {
    NSDictionary *errorInfo;
    
    if (error) {
        errorInfo = @{@"error": error.description};
    }
    else {
        errorInfo = @{@"error": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didFailToLoadStore" withObject:errorInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didCompleteProductTransaction:(SKPaymentTransaction *)transaction {
    NSDictionary *transactionInfo;
    
    if (transaction) {
        transactionInfo = @{@"transaction": [transaction toDictionary]};
    }
    else {
        transactionInfo = @{@"transaction": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didCompleteProductTransaction" withObject:transactionInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didRestoreProductTransaction:(SKPaymentTransaction *)transaction {
    NSDictionary *transactionInfo;
    
    if (transaction) {
        transactionInfo = @{@"transaction": [transaction toDictionary]};
    }
    else {
        transactionInfo = @{@"transaction": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didRestoreProductTransaction" withObject:transactionInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didFailProductTransaction:(SKPaymentTransaction *)transaction withError:(NSError *)error {
    NSDictionary *failedTransactionInfo = @{@"transaction": [NSNull null],
                                            @"error": [NSNull null]};
    
    if (transaction) {
        [failedTransactionInfo setValue:[transaction toDictionary] forKey:@"transaction"];
    }
    
    if (error) {
        [failedTransactionInfo setValue:error.description forKey:@"error"];
    }
    
    [self performDelegateCallback:@"didFailProductTransaction" withObject:failedTransactionInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didStartDownloadContent:(SKPaymentTransaction *)transaction {
    NSDictionary *transactionInfo;
    
    if (transaction) {
        transactionInfo = @{@"transaction": [transaction toDictionary]};
    }
    else {
        transactionInfo = @{@"transaction": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didStartDownloadContent" withObject:transactionInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didPauseDownloadContent:(SKDownload *)download {
    NSDictionary *downloadInfo;
    
    if (download) {
        downloadInfo = @{@"download": [download toDictionary]};
    }
    else {
        downloadInfo = @{@"download": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didPauseDownloadContent" withObject:downloadInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didCancelDownloadContent:(SKDownload *)download {
    NSDictionary *downloadInfo;
    
    if (download) {
        downloadInfo = @{@"download": [download toDictionary]};
    }
    else {
        downloadInfo = @{@"download": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didCancelDownloadContent" withObject:downloadInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didReceiveProgressDownloadContent:(SKDownload *)download {
    NSDictionary *downloadInfo;
    
    if (download) {
        downloadInfo = @{@"download": [download toDictionary]};
    }
    else {
        downloadInfo = @{@"download": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didReceiveProgressDownloadContent" withObject:downloadInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didFailDownloadContent:(SKDownload *)download {
    NSDictionary *downloadInfo;
    
    if (download) {
        downloadInfo = @{@"download": [download toDictionary]};
    }
    else {
        downloadInfo = @{@"download": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didFailDownloadContent" withObject:downloadInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library didFinishDownloadContent:(SKDownload *)download {
    NSDictionary *downloadInfo;
    
    if (download) {
        downloadInfo = @{@"download": [download toDictionary]};
    }
    else {
        downloadInfo = @{@"download": [NSNull null]};
    }
    
    [self performDelegateCallback:@"didFinishDownloadContent" withObject:downloadInfo];
}

- (void)notificarePushLib:(NotificarePushLib *)library onReady:(NSDictionary *)info {
    NSDictionary *applicationInfo;
    
    if (info) {
        applicationInfo = @{@"application": info};
    }
    else {
        applicationInfo = @{@"application": [NSNull null]};
    }
    
    [self performDelegateCallback:@"onReady" withObject:applicationInfo];
}

- (NSString *)performDelegateCallback:(NSString *)delegateMethod withObject:(id)object {
    DelegateBlock delegateBlock = self.delegateCallbacks[delegateMethod];
    NSString *jsonString = jsonStringFromObject(object);
    
    if (delegateBlock) {
        return delegateBlock(jsonString);
    }
    
    return nil;
}

@end
