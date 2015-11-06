//
//  NotificareNotification+Dictionary.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 23-10-2015.
//
//

#import "NotificareNotification+Dictionary.h"

#import "NotificareAction+Dictionary.h"
#import "NotificareAttachment+Dictionary.h"
#import "NotificareContent+Dictionary.h"
#import "NotificareSegment+Dictionary.h"


@implementation NotificareNotification (Dictionary)

- (NSDictionary *)toDictionary {
    NSDictionary *info = @{@"notificationID":           self.notificationID         ? self.notificationID       : [NSNull null],
                           @"application":              self.application            ? self.application          : [NSNull null],
                           @"notificationType":         self.notificationType       ? self.notificationType     : [NSNull null],
                           @"notificationTime":         self.notificationTime       ? self.notificationTime     : [NSNull null],
                           @"notificationMessage":      self.notificationMessage    ? self.notificationMessage  : [NSNull null],
                           @"notificationLatitude":     self.notificationLatitude   ? [NSNumber numberWithDouble:[self.notificationLatitude doubleValue]]   : @0.0,
                           @"notificationLongitude":    self.notificationLongitude  ? [NSNumber numberWithDouble:[self.notificationLongitude doubleValue]]  : @0.0,
                           @"notificationDistance":     self.notificationDistance   ? [NSNumber numberWithDouble:[self.notificationDistance doubleValue]]   : @0.0,
                           @"notificationContent":      [self getDictionaryContent],
                           @"notificationActions":      [self getDictionaryActions],
                           @"notificationAttachments":  [self getDictionaryAttachments],
                           @"notificationTags":         self.notificationTags       ? self.notificationTags     : [NSNull null],
                           @"notificationSegments":     [self getDictionarySegments],
                           @"notificationExtra":        self.notificationExtra      ? self.notificationExtra    : [NSNull null],
                           @"notificationInfo":         self.notificationInfo       ? self.notificationInfo     : [NSNull null],
                           @"displayMessage":           self.displayMessage ? [NSNumber numberWithInt:[self.displayMessage intValue]] : @0};
    
    return info;
}

- (NSArray *)getDictionaryContent {
    NSMutableArray *dictionaryContent = [NSMutableArray array];
    
    for (NotificareContent *content in self.notificationContent) {
        [dictionaryContent addObject:[content toDictionary]];
    }
    
    return [dictionaryContent copy];
}

- (NSArray *)getDictionaryActions {
    NSMutableArray *dictionaryActions = [NSMutableArray array];
    
    for (NotificareAction *action in self.notificationActions) {
        [dictionaryActions addObject:[action toDictionary]];
    }
    
    return [dictionaryActions copy];
}

- (NSArray *)getDictionaryAttachments {
    NSMutableArray *dictionaryAttachments = [NSMutableArray array];
    
    for (NotificareAttachment *attachment in self.notificationAttachments) {
        [dictionaryAttachments addObject:[attachment toDictionary]];
    }
    
    return [dictionaryAttachments copy];
}

- (NSArray *)getDictionarySegments {
    NSMutableArray *dictionarySegments = [NSMutableArray array];
    
    for (NotificareSegment *segment in self.notificationSegments) {
        [dictionarySegments addObject:[segment toDictionary]];
    }
    
    return [dictionarySegments copy];
}

@end
