//
//  NotificareContent+Dictionary.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 28-10-2015.
//
//

#import "NotificareContent+Dictionary.h"

@implementation NotificareContent (Dictionary)

- (NSDictionary *)toDictionary {
    NSDictionary *info = @{@"type":             self.type,
                           @"data":             self.data,
                           @"dataDictionary:":  self.dataDictionary};
    
    return info;
}

@end
