//
//  NotificareAction+Dictionary.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 28-10-2015.
//
//

#import "NotificareAction+Dictionary.h"

@implementation NotificareAction (Dictionary)

- (NSDictionary *)toDictionary {
    NSDictionary *info = @{@"actionType":       self.actionType     ? self.actionType   : [NSNull null],
                           @"actionLabel":      self.actionLabel    ? self.actionLabel  : [NSNull null],
                           @"actionTarget":     self.actionTarget   ? self.actionTarget : [NSNull null],
                           @"actionKeyboard":   [NSNumber numberWithBool:self.actionKeyboard],
                           @"actionCamera":     [NSNumber numberWithBool:self.actionCamera]};
    
    return info;
}

@end
