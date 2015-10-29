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
    NSDictionary *info = @{@"notificationID":           self.notificationID,
                           @"application":              self.application,
                           @"notificationType":         self.notificationType,
                           @"notificationTime":         self.notificationTime,
                           @"notificationMessage":      self.notificationMessage,
                           @"notificationLatitude":     self.notificationLatitude,
                           @"notificationLongitude":    self.notificationLongitude,
                           @"notificationDistance":     self.notificationDistance,
                           @"notificationContent":      [self getDictionaryContent],
                           @"notificationActions":      [self getDictionaryActions],
                           @"notificationAttachments":  [self getDictionaryAttachments],
                           @"notificationTags":         self.notificationTags,
                           @"notificationSegments":     [self getDictionarySegments],
                           @"notificationExtra":        self.notificationExtra,
                           @"notificationInfo":         self.notificationInfo,
                           @"displayMessage":           self.displayMessage};
    
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
