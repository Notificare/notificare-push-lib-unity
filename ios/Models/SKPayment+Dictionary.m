//
//  SKPayment+Dictionary.m
//  notificare-push-lib-unity-ios
//
//  Created by Aernout Peeters on 23-10-2015.
//
//

#import "SKPayment+Dictionary.h"

@implementation SKPayment (Dictionary)

- (NSDictionary *)toDictionary {
    NSDictionary *info = @{@"productIdentifier":    self.productIdentifier,
                           @"quantity":             [NSNumber numberWithInteger:self.quantity],
                           @"applicationUsername":  self.applicationUsername ? self.applicationUsername : [NSNull null]};
    
    return info;
}

@end
