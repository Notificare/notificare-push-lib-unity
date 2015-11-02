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
                           @"transactionIdentifier":    self.transaction.transactionIdentifier,
                           @"state":                    [self getStateString],
                           @"progress":                 [NSNumber numberWithFloat:self.progress],
                           @"error":                    self.error ? [self.error description] : nil,
                           @"contentURL":               self.contentURL ? self.contentURL.absoluteString : nil};
    
    return info;
}

- (NSString *)getStateString {
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
            return nil;
            break;
    }*/
    
    return nil;
}

@end
