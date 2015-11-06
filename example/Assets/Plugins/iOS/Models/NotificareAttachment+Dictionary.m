//
//  NotificareAttachment+Dictionary.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 28-10-2015.
//
//

#import "NotificareAttachment+Dictionary.h"

@implementation NotificareAttachment (Dictionary)

- (NSDictionary *)toDictionary {
    NSDictionary *info = @{@"attachmentURI":        self.attachmentURI      ? self.attachmentURI        : [NSNull null],
                           @"attachmentMimeType":   self.attachmentMimeType ? self.attachmentMimeType   : [NSNull  null]};
    
    return info;
}

@end
