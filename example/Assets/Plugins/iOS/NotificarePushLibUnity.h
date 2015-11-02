//
//  NotificarePushLibUnity.h
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 14-10-2015.
//
//

#import <Foundation/Foundation.h>

typedef void (*BasicCallback)(const char *jsonUTF8String);

typedef char *(*DelegateCallback)(const char *jsonUTF8String);
typedef NSString *(^DelegateBlock)(NSString *jsonString);


@interface NotificarePushLibUnity : NSObject

@property (nonatomic, strong) NSMutableDictionary *delegateCallbacks;

+ (instancetype)shared;

@end


