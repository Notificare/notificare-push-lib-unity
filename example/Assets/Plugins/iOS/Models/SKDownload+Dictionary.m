//
//  SKDownload+Dictionary.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 23-10-2015.
//
//

#import "SKDownload+Dictionary.h"

#import "SKPaymentTransaction+Dictionary.h"

@implementation SKDownload (Dictionary)

- (NSDictionary *)toDictionary {
    NSDictionary *info = @{@"contentIdentifier":        self.contentIdentifier,
                           @"contentLength":            [NSNumber numberWithLongLong:self.contentLength],
                           @"contentVersion":           self.contentVersion,
                           @"transactionIdentifier":    self.transaction.transactionIdentifier ? self.transaction.transactionIdentifier : [NSNull null],
                           @"state":                    [self getStateObject],
                           @"progress":                 [NSNumber numberWithFloat:self.progress],
                           @"error":                    self.error ? [self.error description] : [NSNull null],
                           @"contentURL":               self.contentURL.absoluteString ? self.contentURL.absoluteString : [NSNull null]};
    
    return info;
}

- (id)getStateObject {
    /*switch (self.state) {
        case SKDownloadStateWaiting:
            return @"SKDownloadStateWaiting";
            break;
            
        case SKDownloadStateActive:
            return @"SKDownloadStateActive";
            break;
            
        case SKDownloadStatePaused:
            return @"SKDownloadStatePaused";
            break;
            
        case SKDownloadStateFinished:
            return @"SKDownloadStateFinished";
            break;
            
        case SKDownloadStateFailed:
            return @"SKDownloadStateFailed";
            break;
            
        case SKDownloadStateCancelled:
            return @"SKDownloadStateCancelled";
            break;
            
        default:
            return [NSNull null];
            break;
    }*/
    
    return [NSNull null];
}

@end
