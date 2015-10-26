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

NSString *stringWithChar(const char *cString) {
    return cString ? [NSString stringWithUTF8String:cString] : nil;
}

char *cStringCopy(const char* cString) {
    if (cString == NULL) {
        return NULL;
    }
    
    char* buffer = (char *)malloc(strlen(cString) + 1);
    strcpy(buffer, cString);
    
    return buffer;
}

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

void registerDelegateCallback(const char* delegateMethod, CStringCallback callback) {
    NSMutableDictionary *delegateCallbacks = [[NotificarePushLibUnity shared] delegateCallbacks];
    NSString *key = stringWithChar(delegateMethod);
    
    if (key && callback) {
        StringBlock stringBlock = ^NSString *(NSString *str) {
            char *strCopy = cStringCopy([str UTF8String]);
            
            char *result = callback(strCopy);
            
            free(strCopy);
            
            return stringWithChar(result);
        };
        
        // I think I have to make a copy here because the callback pointer is going to be freed by managed code
        delegateCallbacks[key] = [stringBlock copy];
    }
}

void unregisterDelegateCallback(const char* delegateMethod) {
    NSMutableDictionary *delegateCallbacks = [[NotificarePushLibUnity shared] delegateCallbacks];
    NSString *key = stringWithChar(delegateMethod);
    
    if (key) {
        [delegateCallbacks removeObjectForKey:key];
    }
}

NSString *performCallback(CStringCallback callback, id object) {
    NSString *jsonString = jsonStringFromObject(object);
    
    if (callback) {
        char *strCopy = cStringCopy([jsonString UTF8String]);
        
        char *result = callback(strCopy);
        
        free(strCopy);
        
        return stringWithChar(result);
    }
    
    return nil;
}

#pragma mark - Notificare Forwarders

void launch() {
    [[NotificarePushLib shared] launch];
}

void registerForNotifications() {
    [[NotificarePushLib shared] registerForNotifications];
}

void registerUserNotifications() {
    [[NotificarePushLib shared] registerUserNotifications];
}

bool checkRemoteNotifications() {
    return [[NotificarePushLib shared] checkRemoteNotifications] ? true : false;
}

#warning Unity can't pass NSDictionary objects. Should probably only be used by appDelegate anyway.
void handleOptions(NSDictionary *options) {
    [[NotificarePushLib shared] handleOptions:options];
}

void registerDevice(const char *token, CStringCallback infoCallback, CStringCallback errorCallback) {
    NSData *tokenData = [NSData dataWithBytes:token length:strlen(token)];
    
    [[NotificarePushLib shared] registerDevice:tokenData completionHandler:^(NSDictionary *info) {
        performCallback(infoCallback, info);
    } errorHandler:^(NSError *error) {
#warning Have to verify that NSError is a valid JSON object
        performCallback(errorCallback, error);
    }];
}

void registerDeviceWithUser(const char *token, const char *userID, const char *username, CStringCallback infoCallback, CStringCallback errorCallback) {
    NSData *tokenData = [NSData dataWithBytes:token length:strlen(token)];
    NSString *userIDString = userID ? [NSString stringWithUTF8String:userID] : nil;
    NSString *usernameString = username ? [NSString stringWithUTF8String:username] : nil;
    
    [[NotificarePushLib shared] registerDevice:tokenData
                                    withUserID:userIDString
                                  withUsername:usernameString
                             completionHandler:^(NSDictionary *info) {
                                 performCallback(infoCallback, info);
                             }
                                  errorHandler:^(NSError *error) {
#warning Have to verify that NSError is a valid JSON object
                                      performCallback(errorCallback, error);
                                  }];
}

void unregisterDevice() {
    [[NotificarePushLib shared] unregisterDevice];
}

void updateBadge(int badge) {
    [[NotificarePushLib shared] updateBadge:[NSNumber numberWithInt:badge]];
}

void openNotification(const char* notificationJSON) {
    NSString *jsonString = stringWithChar(notificationJSON);
    NSDictionary *notificationInfo = objectFromJSONString(jsonString);
    
    [[NotificarePushLib shared] openNotification:notificationInfo];
}

#warning Unity can't pass NSDictionary objects
void logOpenNotification(const char* notificationJSON) {
    NSString *jsonString = stringWithChar(notificationJSON);
    NSDictionary *notificationInfo = objectFromJSONString(jsonString);
    
    [[NotificarePushLib shared] logOpenNotification:notificationInfo];
}

void getNotification(const char *notificationID, CStringCallback infoCallback, CStringCallback errorCallback) {
    NSString *notificationIDString = notificationID ? [NSString stringWithUTF8String:notificationID] : nil;
    
    [[NotificarePushLib shared] getNotification:notificationIDString
                              completionHandler:^(NSDictionary *info) {
                                  performCallback(infoCallback, info);
                              }
                                   errorHandler:^(NSError *error) {
                                       const char *errorCString = [[error description] UTF8String];
                                       errorCallback(errorCString);
                                   }];
}

void clearNotification(const char *notification) {
    NSString *notificationString = notification ? [NSString stringWithUTF8String:notification] : nil;
    [[NotificarePushLib shared] clearNotification:notificationString];
}

void startLocationUpdates() {
    [[NotificarePushLib shared] startLocationUpdates];
}

bool checkLocationUpdates() {
    return [[NotificarePushLib shared] checkLocationUpdates] ? true : false;
}

void stopLocationUpdates() {
    [[NotificarePushLib shared] stopLocationUpdates];
}

#warning Needs confirmation that it properly overlays on top of Unity view
void openUserPreferences() {
    [[NotificarePushLib shared] openUserPreferences];
}

int myBadge() {
    return [[NotificarePushLib shared] myBadge];
}

void fetchProducts(CStringCallback productsCallback, CStringCallback errorCallback) {
    [[NotificarePushLib shared] fetchProducts:^(NSArray *products) {
        NSMutableArray *dictionaryProducts = [NSMutableArray array];
        for (NotificareProduct *product in products) {
            [dictionaryProducts addObject:[product toDictionary]];
        }
#warning Have to verify that array consists of NotificareProduct objects
        performCallback(productsCallback, dictionaryProducts);
    } errorHandler:^(NSError *error) {
#warning Have to verify that NSError is a valid JSON object
        performCallback(errorCallback, error);
    }];
}

void fetchPurchasedProducts(CStringCallback productsCallback, CStringCallback errorCallback) {
    [[NotificarePushLib shared] fetchPurchasedProducts:^(NSArray *products) {
        NSMutableArray *dictionaryProducts = [NSMutableArray array];
        for (NotificareProduct *product in products) {
            [dictionaryProducts addObject:[product toDictionary]];
        }
#warning Have to verify that array consists of NotificareProduct objects
        performCallback(productsCallback, dictionaryProducts);
    } errorHandler:^(NSError *error) {
#warning Have to verify that NSError is a valid JSON object
        performCallback(errorCallback, error);
    }];
}

void fetchProduct(const char *productIdentifier, CStringCallback productCallback, CStringCallback errorCallback) {
    NSString *productIdentifierString = stringWithChar(productIdentifier);
    
    [[NotificarePushLib shared] fetchProduct:productIdentifierString
                           completionHandler:^(NotificareProduct *product) {
                               performCallback(productCallback, [product toDictionary]);
                           }
                                errorHandler:^(NSError *error) {
#warning Have to verify that NSError is a valid JSON object
                                    performCallback(errorCallback, error);
                                }];
}

void buyProduct(const char *productJSON) {
    NSString *jsonString = stringWithChar(productJSON);
    NSDictionary *info = objectFromJSONString(jsonString);
    NotificareProduct *notificareProduct = [[NotificareProduct alloc] initWithDictionary:info];
    
    [[NotificarePushLib shared] buyProduct:notificareProduct];
}

void pauseDownloads(const char* downloadsJSON) {
    NSString *jsonString = stringWithChar(downloadsJSON);
    NSArray *dictionaryDownloads = objectFromJSONString(jsonString);
    
    // Have to loop through pending transactions and downloads to match
    [[NotificarePushLib shared] pauseDownloads:downloads];
}

void cancelDownloads(const char* downloadsJSON) {
    NSString *jsonString = stringWithChar(downloadsJSON);
    NSArray *dictionaryDownloads = objectFromJSONString(jsonString);
    
    // Have to loop through pending transactions and downloads to match
    [[NotificarePushLib shared] cancelDownloads:downloads];
}

void resumeDownloads(const char* downloadsJSON) {
    NSString *jsonString = stringWithChar(downloadsJSON);
    NSArray *dictionaryDownloads = objectFromJSONString(jsonString);
    
    // Have to loop through pending transactions and downloads to match
    [[NotificarePushLib shared] resumeDownloads:downloads];
}

const char *contentPathForProduct(const char *productIdentifier) {
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
@property (nonatomic, assign) char *unityDelegate;

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
    NSString *jsonString = [self performDelegateCallback:@"shouldHandleNotification" withObject:info];
    
    if (!jsonString) {
        return NO;
    }
    
    NSDictionary *resultInfo = objectFromJSONString(jsonString);
    
    if (!info) {
        return NO;
    }
    
#warning Need to decide on a format for return value. [resultInfo[@"shouldHandleNotification"] boolValue]?
    return NO;
}

- (void)notificarePushLib:(NotificarePushLib *)library didUpdateBadge:(int)badge {
#warning Have to verify if NSNumber a valid JSON object
    [self performDelegateCallback:@"shouldHandleNotification" withObject:[NSNumber numberWithInt:badge]];
}

- (void)notificarePushLib:(NotificarePushLib *)library willOpenNotification:(NotificareNotification *)notification {
    NSDictionary *info = [notification toDictionary];
    [self performDelegateCallback:@"willOpenNotification" withObject:info];
}

- (void)notificarePushLib:(NotificarePushLib *)library didOpenNotification:(NotificareNotification *)notification {
    NSDictionary *info = [notification toDictionary];
    [self performDelegateCallback:@"didOpenNotification" withObject:info];
}

- (void)notificarePushLib:(NotificarePushLib *)library didCloseNotification:(NotificareNotification *)notification {
    NSDictionary *info = [notification toDictionary];
    [self performDelegateCallback:@"didCloseNotification" withObject:info];
}

- (void)notificarePushLib:(NotificarePushLib *)library didFailToOpenNotification:(NotificareNotification *)notification {
    NSDictionary *info = [notification toDictionary];
    [self performDelegateCallback:@"didFailToOpenNotification" withObject:info];
}

- (void)notificarePushLib:(NotificarePushLib *)library didLoadStore:(NSArray *)products {
    NSMutableArray *dictionaryProducts = [NSMutableArray array];
    
    for (NotificareProduct *product in products) {
        [dictionaryProducts addObject:[product toDictionary]];
    }
    
    [self performDelegateCallback:@"didLoadStore" withObject:dictionaryProducts];
}

- (void)notificarePushLib:(NotificarePushLib *)library didFailToLoadStore:(NSError *)error {
#warning have to verify if NSError is a valid JSON object
    [self performDelegateCallback:@"didFailToLoadStore" withObject:error];
}

- (void)notificarePushLib:(NotificarePushLib *)library didCompleteProductTransaction:(SKPaymentTransaction *)transaction {
#warning have to verify if SKPaymentTransaction is a valid JSON object
    [self performDelegateCallback:@"didCompleteProductTransaction" withObject:transaction];
}

- (void)notificarePushLib:(NotificarePushLib *)library didRestoreProductTransaction:(SKPaymentTransaction *)transaction {
#warning have to verify if SKPaymentTransaction is a valid JSON object
    [self performDelegateCallback:@"didRestoreProductTransaction" withObject:transaction];
}

- (void)notificarePushLib:(NotificarePushLib *)library didFailProductTransaction:(SKPaymentTransaction *)transaction withError:(NSError *)error {
#warning have to verify if SKPaymentTransaction is a valid JSON object
#warning delegate callbacks currently accept only one object
    [self performDelegateCallback:@"didFailProductTransaction" withObject:transaction];
}

- (void)notificarePushLib:(NotificarePushLib *)library didStartDownloadContent:(SKPaymentTransaction *)transaction {
#warning have to verify if SKPaymentTransaction is a valid JSON object
    [self performDelegateCallback:@"didStartDownloadContent" withObject:transaction];
}

- (void)notificarePushLib:(NotificarePushLib *)library didPauseDownloadContent:(SKDownload *)download {
#warning have to verify if SKDownload is a valid JSON object
    [self performDelegateCallback:@"didPauseDownloadContent" withObject:download];
}

- (void)notificarePushLib:(NotificarePushLib *)library didCancelDownloadContent:(SKDownload *)download {
#warning have to verify if SKDownload is a valid JSON object
    [self performDelegateCallback:@"didCancelDownloadContent" withObject:download];
}

- (void)notificarePushLib:(NotificarePushLib *)library didReceiveProgressDownloadContent:(SKDownload *)download {
#warning have to verify if SKDownload is a valid JSON object
    [self performDelegateCallback:@"didReceiveProgressDownloadContent" withObject:download];
}

- (void)notificarePushLib:(NotificarePushLib *)library didFailDownloadContent:(SKDownload *)download {
#warning have to verify if SKDownload is a valid JSON object
    [self performDelegateCallback:@"didFailDownloadContent" withObject:download];
}

- (void)notificarePushLib:(NotificarePushLib *)library didFinishDownloadContent:(SKDownload *)download {
#warning have to verify if SKDownload is a valid JSON object
    [self performDelegateCallback:@" didFinishDownloadContent" withObject:download];
}

- (void)notificarePushLib:(NotificarePushLib *)library onReady:(NSDictionary *)info {
    [self performDelegateCallback:@"onReady" withObject:info];
}

- (NSString *)performDelegateCallback:(NSString *)delegateMethod withObject:(id)object {
    StringBlock stringBlock = self.delegateCallbacks[delegateMethod];
    NSString *jsonString = jsonStringFromObject(object);
    
    if (stringBlock) {
        return stringBlock(jsonString);
    }
    
    return nil;
}

@end


