//
//  NotificareSegment+Dictionary.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 28-10-2015.
//
//

#import "NotificareSegment+Dictionary.h"

@implementation NotificareSegment (Dictionary)

- (NSDictionary *)toDictionary {
    NSDictionary *info = @{@"string":   self.segmentLabel,
                           @"string":   self.segmentId,
                           @"selected": [NSNumber numberWithBool:self.selected]};
    
    return info;
}

@end
