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
    NSDictionary *info = @{@"actionType":       self.actionType,
                           @"actionLabel":      self.actionLabel,
                           @"actionTarget":     self.actionTarget,
                           @"actionKeyboard":   [NSNumber numberWithBool:self.actionKeyboard],
                           @"actionCamera":     [NSNumber numberWithBool:self.actionCamera]};
    
    return info;
}

@end
