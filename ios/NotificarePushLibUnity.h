//
//  NotificarePushLibUnity.h
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 14-10-2015.
//
//

#import <Foundation/Foundation.h>


@interface NotificarePushLibUnity : NSObject

@property (nonatomic, strong) NSMutableDictionary *delegateCallbacks;

+ (instancetype)shared;

@end


